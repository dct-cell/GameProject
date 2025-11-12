using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance { get; private set; }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool isGameStarted = false;
    public bool inBattle = false;
    public bool inShop = false;
    public bool inEvent = false;
    public bool inMap = false;
    public int mapWidth;
    public int mapHeight;
    public int playerDepth;
    public int playerPosition;

    #region Scene Controller
    public void StartGame() {
        if (isGameStarted) {
            Debug.Log("Game Started!");
            return;
        }
        isGameStarted = true;
        // BagManager.instance.coin = 20;
        playerDepth = 0;
        inBattle = false;
        inShop = false;
        inEvent = false;
        inMap = true;
        // AddInitialCharacters();
        Debug.Log("I want to start game!");
        MapManager.instance.GenerateMap(mapWidth, mapHeight);
        GameScene.instance.LoadMapScene();
    }

    public void HandleClickOnMapSlot(MapSlot mapSlot) {
        if (!isGameStarted || inBattle || inShop || inEvent || !inMap) {
            Debug.Log("You are not in map. Too strange!");
            return;
        }
        if (mapSlot == null) {
            Debug.Log("Clicked on null map slot. This should not happen.");
            return;
        }
        if (mapSlot.depth != playerDepth) {
            Debug.Log("Cannot access this slot yet. Incorrect depth.");
            return;
        }
        if (playerDepth != 0 && !MapManager.instance.edges[playerDepth - 1][playerPosition, mapSlot.position]) {
            Debug.Log("Cannot access this slot. No edge.");
            return;
        }
        playerPosition = mapSlot.position;
        switch (mapSlot.slotType) {
            case MapSlotType.Battle:
                inBattle = true;
                inShop = false;
                inEvent = false;
                inMap = false;
                GameScene.instance.LoadBattleScene();
                break;
            case MapSlotType.Shop:
                inBattle = false;
                inShop = true;
                inEvent = false;
                inMap = false;
                GameScene.instance.LoadShopScene();
                break;
            case MapSlotType.Event:
                inBattle = false;
                inShop = false;
                inEvent = true;
                inMap = false;
                GameScene.instance.LoadEventScene();
                break;
            default:
                Debug.Log("Unknown map slot type. This should not happen.");
                break;
        }
    }

    public void NextLevel() {
        playerDepth += 1;
        if (playerDepth >= mapWidth) {
            Debug.Log("Congratulations! You have completed the game!");
            GameOver();
            return;
        }
        inBattle = false;
        inShop = false;
        inEvent = false;
        inMap = true;
        GameScene.instance.LoadMapScene();
    }

    public void GameOver() {
        playerDepth = 0;
        isGameStarted = false;
        inBattle = false;
        inShop = false;
        inEvent = false;
        inMap = false;
        BagManager.instance.ClearBag();
        MapManager.instance.ClearMap();
        GameScene.instance.LoadMainScene();
    }
    #endregion

    public List<Character> shopCharacters = new List<Character>();
}
