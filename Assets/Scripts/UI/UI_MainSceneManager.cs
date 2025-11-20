using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainSceneManager : MonoBehaviour {
    public Button startButton;

    private void Start() {
        if (startButton != null) {
            Debug.Log("Start Button Name: " + startButton.gameObject.name);
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(GameManager.instance.StartGame);
            Debug.Log("Add onClick listener to Start Button");
        }
    }
}
