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

	public int health;

	public string characterName;
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
		health -= damage;
		characterBattleAnimator.PlayDamageEffect();
		healthBarUI.UpdateHealthUI();
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

    public void MoveAnimation() {
        Vector3 targetWorldPos = GridManager.instance.ComputeOffset(position);
        characterBattleAnimator.StartMoveTo(targetWorldPos);
    }

    // 返回攻击坐标，默认随机找一个距离不超过 attackDistance 的敌人
    protected virtual Vector3Int? FindTargetLogic() {
        List<Character> enemies = BattleManager.instance.GetAliveTeamMember(teamId ^ 1);
		Extensions.Shuffle(enemies);
        foreach (Character enemy in enemies)
            if (GridManager.instance.Distance(position, enemy.position) <= attackRange)
                return enemy.position;
        return null;
    }

	// 攻击以 targetPosition 为中心，范围 attackRange 的敌人
    public virtual void AttackLogic(Vector3Int? _targetPosition) {
		if (_targetPosition == null) return;
		Vector3Int attackPosition = _targetPosition.Value;
        List<Character> enemies = BattleManager.instance.GetAliveTeamMember(teamId ^ 1);
        foreach (Character enemy in enemies)
            if (GridManager.instance.Distance(attackPosition, enemy.position) < attackRange) {
                string targetId = enemy.uid;
                Debug.Log($"{uid} attacks {targetId} for {attack} damage.");
                BattleManager.instance.DamageCharacter(targetId, DamageCalculator.instance.CalculateDamage(uid, targetId));
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
}