using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ShopSlot : MonoBehaviour, IPointerClickHandler {
    public Character characterScript;
    public TextMeshProUGUI characterName;
    public Image characterImage;
    public TextMeshProUGUI characterPrice;

    public void UpdateSlot(Character _character) {
        characterScript = _character;
        characterName.text = characterScript.characterName;
        SpriteRenderer sr = characterScript.GetComponent<SpriteRenderer>();
        characterImage.sprite = sr.sprite;
        characterImage.color = sr.color;
        characterPrice.text = _character.price.ToString();
    }

    public void CleanUpSlot() {
        characterScript = null;
        characterName.text = "";
        characterImage.sprite = null;
        characterImage.color = Color.clear;
        characterPrice.text = "";
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            ShopManager.instance.HandleShopSlotClick(characterScript);
        }
    }
}
