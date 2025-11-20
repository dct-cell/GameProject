using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class UI_BagSlot : MonoBehaviour, IPointerClickHandler {
    public Character characterScript;
    public Image image;

    public void Start() {
        image = GetComponent<Image>();
    }

    public void UpdateSlot(Character _character) {
        characterScript = _character;
        SpriteRenderer sr = characterScript.GetComponent<SpriteRenderer>();
        image.sprite = sr.sprite;
        image.color = sr.color;
    }

    public void CleanUpSlot() {
        characterScript = null;
        image.sprite = null;
        image.color = Color.clear;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            if (GameManager.instance.inBattle) {
                StageInputHandler.instance.HandleSlotClick(characterScript);
            }
            else if(GameManager.instance.inShop) {
                ShopManager.instance.HandleBagSlotClick(characterScript);
            }
        }
    }
}
