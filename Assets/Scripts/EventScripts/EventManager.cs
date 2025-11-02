using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventManager : MonoBehaviour {
    public static EventManager instance { get; private set; }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public TextMeshProUGUI eventText;

    public async void GenerateEvent() {
        eventText.text = await EventGenerator.instance.GetEvent();
    }

    public void ExitEvent() {
        GameManager.instance.NextLevel();
    }
}
