
using UnityEngine;

public class CharacterI : Character {
    // 攻击后伤害提升20%（可叠加），移动后清除该效果
    public int skillCount;

    public override void ActionsWhenStart() {
        base.ActionsWhenStart();
        skillCount = 0;
    }
    protected override int ProcessSingleRound() {
        Vector3Int? targetId = FindTargetLogic();
        if (targetId != null) {
            AttackLogic(targetId);
            ModifyDamage(0.2f, 10000);
            ++skillCount;
            return 1;
        }
        else {
            MoveLogic();
            for (int i = 0; i < skillCount; i++) {
                damageModifier.Remove(0.2f);
                effectToRemove.Remove((10000 - i, 2, 0.2f));
            }
            ReCalculate();
            skillCount = 0;
            return 0;
        }
    }
}