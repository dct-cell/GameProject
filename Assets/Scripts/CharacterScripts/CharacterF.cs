using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterF : Character {
    protected override void Awake() {
        base.Awake();
        shield = maxHealth / 2;
        Debug.Log($"Character F, shield = {shield}");
    }
}
