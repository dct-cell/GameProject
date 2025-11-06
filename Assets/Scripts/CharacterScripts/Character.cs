using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class Character : MonoBehaviour // Base class for all characters in the game
{
	[Header("Attributes")]
	public int price;
	public int level;
	public int maxHealth;
	public int attack;
	public int attackDistance; // 攻击距离
	public int attackRange; // 攻击范围
	public int speed;
	public string skillDescription;
	public string characterName;

	public int health;
    public int shield;

	public string uid;
	public int teamId;
	public Vector3Int position;

	public CharacterBattleAnimator characterBattleAnimator;
	public UI_HealthBar healthBarUI;

	public int nextRoundTime;

	public bool isAlive => health > 0;

	protected virtual void Awake()
	{
		characterBattleAnimator = GetComponent<CharacterBattleAnimator>();
		if (characterBattleAnimator == null)
			characterBattleAnimator = gameObject.AddComponent<CharacterBattleAnimator>();
		healthBarUI = GetComponentInChildren<UI_HealthBar>();
        health = maxHealth;
    }

	public virtual void IsDamagedBy(int damage)
	{
        int shieldDecrease = Math.Min(shield, damage);
        shield -= shieldDecrease;
        damage -= shieldDecrease;
		health -= damage;
		characterBattleAnimator.PlayDamageEffect();
		healthBarUI.UpdateHealthUI();
	}

    public virtual void ActionsAtStart() {

    }

	public virtual int SingleRound() 
	{
		Debug.Assert(isAlive, $"{characterName} is dead and cannot battle.");
		return ProcessSingleRound();
	}

	protected virtual int ProcessSingleRound() {
        Vector3Int? targetId = FindTargetLogic();
        if (targetId != null) {
            AttackLogic(targetId);
            return 1;
        }
        else {
            MoveLogic();
            return 0;
        }
    }

    public virtual void ActionsAtEnd() {
        shield = 0;
    }

    public void MoveAnimation() {
        Vector3 targetWorldPos = GridManager.instance.ComputeOffset(position);
        characterBattleAnimator.StartMoveTo(targetWorldPos);
    }

    #region Action Logic
    // 返回攻击坐标，默认随机找一个距离不超过 attackDistance 的敌人（的位置）
    protected virtual Vector3Int? FindTargetLogic() {
        List<Character> enemies = BattleManager.instance.GetAliveTeamMember(teamId ^ 1);
		Extensions.Shuffle(enemies);
        foreach (Character enemy in enemies)
            if (GridManager.instance.Distance(position, enemy.position) <= attackRange)
                return enemy.position;
        return null;
    }

	// 攻击以 targetPosition 为中心，范围 attackRange 的敌人
    public virtual void AttackLogic(Vector3Int? _targetPosition, float ratio = 1.0f) {
		if (_targetPosition == null) return;
		Vector3Int attackPosition = _targetPosition.Value;
        List<Character> enemies = BattleManager.instance.GetAliveTeamMember(teamId ^ 1);
        foreach (Character enemy in enemies)
            if (GridManager.instance.Distance(attackPosition, enemy.position) < attackRange) {
                string targetId = enemy.uid;
                BattleManager.instance.DamageCharacter(targetId, DamageCalculator.instance.CalculateDamage(uid, targetId, ratio));
            }
    }
	
	// 移动一步，使得距离最近的敌人距离最小
    public void MoveLogic() {
        //List<Character> targets = BattleManager.instance.GetAliveTeamMember(teamId ^ 1);
        //List<Vector3Int> validPositions = GridManager.instance.ValidPositions(position);

        //int minDist = int.MaxValue;
        //foreach (var pos in validPositions)
        //    minDist = Mathf.Min(minDist, GridManager.instance.GetMinDistInTargets(pos, targets));
        //foreach (var pos in validPositions)
        //    if (GridManager.instance.GetMinDistInTargets(pos, targets) == minDist) {
        //        Debug.Log($"{uid} moves from {position} to {pos}.");
        //        position = pos;
        //        break;
        //    }
        position = PathSystem.instance.MeleeLogic(this);

	}
    #endregion
    #region buff

    // 假定效果只包括 攻击力提升，伤害提升，受到伤害提高

    public List<float> attackModifier = new List<float>();
    public List<float> damageModifier = new List<float>();
    public List<float> takeDamageModifier = new List<float>();
    public List<(int, int, float)> effectToRemove = new List<(int, int, float)> ();

    public void ModifyAttack(float _value, int duration) {
        attackModifier.Add(_value);
        effectToRemove.Add((duration, 1, _value));
    }
    public void ModifyDamage(float _value, int duration) {
        attackModifier.Add(_value);
        effectToRemove.Add((duration, 2, _value));
    }
    public void ModifyTakenDamage(float _value, int duration) {
        attackModifier.Add(_value);
        effectToRemove.Add((duration, 3, _value));
    }

    public float getAttackModifier() {
        float result = 1;
        foreach (var modifier in attackModifier)
            result *= 1 + modifier;
        return result;
    }

    public float getDamageModifier() {
        float result = 1;
        foreach (var modifier in damageModifier)
            result *= 1 + modifier;
        return result;
    }

    public float getTakeDamageModifier() {
        float result = 1;
        foreach (var modifier in takeDamageModifier)
            result *= 1 + modifier;
        return result;
    }

    public void RemoveEffect() {
        List<(int, int, float)> _effect = new List<(int, int, float)>();
        foreach (var effect in effectToRemove) {
            if (effect.Item1 == 1) {
                switch (effect.Item2) {
                    case 1: attackModifier.Remove(effect.Item3); break;
                    case 2: damageModifier.Remove(effect.Item3); break;
                    case 3: takeDamageModifier.Remove(effect.Item3); break;
                    default: break;
                }
            }
            else {
                _effect.Add((effect.Item1 - 1, effect.Item2, effect.Item3));
            }
        }
        effectToRemove = _effect;
    }
    #endregion

}