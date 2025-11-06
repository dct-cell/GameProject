
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CharacterG : Character {
    // 亡语：死亡后爆炸，对相邻敌人造成等同于自身生命值50%的伤害，并造成1回合眩晕

    public override void ActionsWhenDie() {
        currentHealth = 1;
        List<Vector3Int> neighborPosition = GridManager.instance.FindNeighbourhood(position, 2);
        foreach (var neighbor in neighborPosition) {
            Character neighborCharacter = GridManager.instance.FindCharacter(neighbor);
            if (neighborCharacter != null && neighborCharacter.teamId != teamId) {
                BattleManager.instance.DamageCharacter(neighborCharacter.uid, DamageCalculator.instance.CalculateDamage(uid, neighborCharacter.uid, 0.5f, 1));
                neighborCharacter.nextRoundTime += neighborCharacter.speed;
            }
        }
        currentHealth = -1;
        BattleManager.instance.TriggerCharacterDied(this);
    }
}
