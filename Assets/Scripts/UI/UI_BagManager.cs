using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BagManager : MonoBehaviour {
    public static UI_BagManager instance { get; private set; }
    private void Awake() 
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public GameObject bag;
    public Transform bagSlotsParent;
    public UI_BagSlot[] bagSlots;

    private void Start() {
        bag = GameObject.FindWithTag("Bag");
        bagSlotsParent = GameObject.FindWithTag("BagSlotsParent").transform;
        bagSlots = bagSlotsParent.GetComponentsInChildren<UI_BagSlot>();
        UpdateSlotUI();
    }

    public void UpdateSlotUI() {
        for (int i = 0; i < bagSlots.Length; i++)
            bagSlots[i].CleanUpSlot();
        List<Character> members = BagManager.instance.members;
        for (int i = 0; i < members.Count; i++)
            bagSlots[i].UpdateSlot(members[i]);
    }

    public void CleanUp() {
        bag.SetActive(false);
    }
}
