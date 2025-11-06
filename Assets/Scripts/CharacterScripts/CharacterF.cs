using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterF : Character {
    public override void ActionsAtStart() {
        List<Vector3Int> neighborPosition = GridManager.instance.FindNeighbourhood(position, 2);
        foreach (var neighbor in neighborPosition) {
            Character neighborCharacter = GridManager.instance.FindCharacter(neighbor);
            if (neighborCharacter != null && neighborCharacter.teamId == 0) {
                neighborCharacter.shield = neighborCharacter.maxHealth / 4;
                neighborCharacter.healthBarUI.UpdateHealthUI();
            }
        }
    }
}
