using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DamageCalculator : MonoBehaviour
{
	public static DamageCalculator instance { get; private set; }
	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	// damageType = 0 表示攻击力倍率，damageType = 1 表示生命值倍率
	public int CalculateDamage(string attackerId, string defenderId, float ratio = 1.0f, int damageType = 0)
	{
		Character attacker = BattleManager.instance.FindCharacter(attackerId);
		Character defender = BattleManager.instance.FindCharacter(defenderId);
		float damage = ratio;
		if (damageType == 0)
			damage *= attacker.currentAttack;
		else
			damage *= attacker.currentMaxHealth;
		damage *= attacker.getDamageModifier() * defender.getTakeDamageModifier();
		int finalDamage = Mathf.FloorToInt(damage);
        Debug.Log($"{attackerId} [{attacker.characterName}] attacks {defenderId} [{defender.characterName}] for {finalDamage} damage.");
        return finalDamage;
	}
}
