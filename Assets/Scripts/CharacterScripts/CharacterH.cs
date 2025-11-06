
using System;

public class CharacterH : Character {
    // 场上每有一个单位死亡时，本场战斗内最大生命值和攻击力永久提升10%
    public override void ActionsWhenStart() {
        base.ActionsWhenStart();
        BattleManager.instance.OnCharacterDied += Skill;
    }

    public void Skill(Character _character) {
        ModifyAttack(0.1f, 10000);
        ModifyHealth(0.1f, 10000);
    }

    public override void ActionsWhenDie() {
        BattleManager.instance.OnCharacterDied -= Skill;
        base.ActionsWhenDie();
    }

    public override void ActionsWhenEnd() {
        BattleManager.instance.OnCharacterDied -= Skill;
        base.ActionsWhenEnd();
    }
}
