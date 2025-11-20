using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterF : Character {
    // 守卫：战斗开始时，给自己和相邻友方单位等同于生命值上限25%的护盾
    public override void ActionsWhenStart() {
        base.ActionsWhenStart();
        List<Vector3Int> neighborPosition = GridManager.instance.FindNeighbourhood(position, 2);
        foreach (var neighbor in neighborPosition) {
            Character neighborCharacter = GridManager.instance.FindCharacter(neighbor);
            if (neighborCharacter != null && neighborCharacter.teamId == teamId) {
                neighborCharacter.shield = neighborCharacter.maxHealth / 4;
                neighborCharacter.healthBarUI.UpdateHealthUI();
            }
        }
    }
}
