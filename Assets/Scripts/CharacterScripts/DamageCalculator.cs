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

	public int CalculateDamage(string attackerId, string defenderId, float ratio)
	{
		Character attacker = BattleManager.instance.FindCharacter(attackerId);
		Character defender = BattleManager.instance.FindCharacter(defenderId);
		int damage = Mathf.FloorToInt(attacker.attack * attacker.getAttackModifier() * ratio * attacker.getDamageModifier() * defender.getTakeDamageModifier());
        Debug.Log($"{attackerId} [{attacker.characterName}] attacks {defenderId} [{defender.characterName}] for {damage} damage.");
        return damage;
	}
}
