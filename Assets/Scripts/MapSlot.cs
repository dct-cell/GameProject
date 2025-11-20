using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MapSlotType {
    Battle,
    Event,
    Shop
}

public class MapSlot : MonoBehaviour, IPointerClickHandler {
    public MapSlotType slotType;
    public int depth;
    public int position;
    public Image image;

    public void Setup(MapSlotType _type, int _depth, int _position) {
        slotType = _type;
        depth = _depth;
        position = _position;
        image = GetComponent<Image>();
    }
    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            Debug.Log("Map Slot Clicked");
            GameManager.instance.HandleClickOnMapSlot(this);
        }
    }
}
