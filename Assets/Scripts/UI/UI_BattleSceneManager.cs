using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BattleSceneManager : MonoBehaviour {
    public static UI_BattleSceneManager instance;
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public Button startButton;
    public Button endButton;

    public void Start() {
        if (startButton != null) {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StageInputHandler.instance.StartBattle);
        }
        if (endButton != null) {
            endButton.onClick.RemoveAllListeners();
            endButton.onClick.AddListener(StageInputHandler.instance.EndBattle);
        }
        UpdateCharacterCountText();
    }

    public void ChangeButton() {
        startButton.gameObject.SetActive(false);
        endButton.gameObject.SetActive(true);
    }

    public TextMeshProUGUI characterCountText;
    private Coroutine flashCoroutine;

    public void UpdateCharacterCountText() {
        characterCountText.text = "上场角色数：" + StageInputHandler.instance.currentCharacterCount.ToString() + "/" + StageManager.instance.maxCharacterCount.ToString();
    }

    public void StartFlashText() {
        if (flashCoroutine != null) {
            StopCoroutine(flashCoroutine);
            characterCountText.color = Color.white;
        }
        flashCoroutine = StartCoroutine(FlashText());
    }

    private IEnumerator FlashText(float duration = 1f, float interval = .1f) {
        float startTime = Time.time;
        Color originalColor = Color.white;
        Color flashColor = Color.red;
        characterCountText.gameObject.SetActive(true);
        characterCountText.color = originalColor;
        while (Time.time < startTime + duration) {
            characterCountText.color = flashColor;
            yield return new WaitForSeconds(interval);
            characterCountText.color = originalColor;
            yield return new WaitForSeconds(interval);
        }
        characterCountText.color = originalColor;
        flashCoroutine = null;
    }
}
