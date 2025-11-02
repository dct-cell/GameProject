using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
		ReplyGenerator.instance.InitEvent(eventText.text);
	}

    public void ExitEvent() {
        GameManager.instance.NextLevel();
    }

    public TMP_InputField userInputField;
    public TextMeshProUGUI LLMReturnText;
    public Button nextLevelButton;
    public Button endInputButton;

    public async void HandleInputCompletion() {
        LLMReturnText.text = await ReplyGenerator.instance.GenerateReply(userInputField.text);
        endInputButton.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(true);
    }
}
