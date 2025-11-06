
using UnityEngine;

public class CharacterE : Character {
    // 每攻击2次，给当前攻击的敌方单位施加 debuff
    public int skillCount;

    private void Start() {
        skillCount = 0;
    }
    protected override int ProcessSingleRound() {
        Vector3Int? targetId = FindTargetLogic();
        if (targetId != null) {
            AttackLogic(targetId);
            skillCount++;
            if (skillCount == 2) {
                Character defender = GridManager.instance.FindCharacter(targetId.Value);
                defender.ModifyTakenDamage(0.5f, 2);
            }
            return 1;
        }
        else {
            MoveLogic();
            return 0;
        }
    }
}
