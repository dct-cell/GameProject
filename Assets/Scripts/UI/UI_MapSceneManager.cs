using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MapSceneManager : MonoBehaviour {
    public static UI_MapSceneManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public int mapWidth;
    public int mapHeight;
    public int[] height;

    public GameObject mapSlotPrefab;
    public GameObject pathLinePrefab;
    public Transform mapParent;
    public List<MapSlot[]> mapSlots = new List<MapSlot[]>();

    public List<Sprite> icon = new List<Sprite>();
    public Dictionary<MapSlotType, Sprite> icons = new Dictionary<MapSlotType, Sprite>();

    public void DrawMap() {
        icons[MapSlotType.Battle] = icon[0];
        icons[MapSlotType.Event] = icon[1];
        icons[MapSlotType.Shop] = icon[2];
        mapWidth = MapManager.instance.mapWidth;
        mapHeight = MapManager.instance.mapHeight;  
        height = MapManager.instance.height;
        for (int i = 0; i < mapWidth; i++) {
            mapSlots.Add(new MapSlot[height[i]]);
            float xOffset = i * 300 + 100;
            for (int j = 0; j < height[i]; j++) {
                float yOffset = j * 150 - height[i] * 75 + 75;
                GameObject newSlot = Instantiate(mapSlotPrefab);
                newSlot.transform.SetParent(mapParent, false);
                newSlot.transform.localPosition = new Vector3(xOffset, yOffset, 0);
                newSlot.GetComponent<MapSlot>().Setup(MapManager.instance.types[i][j], i, j);
                newSlot.GetComponent<Image>().sprite = icons[MapManager.instance.types[i][j]];
                mapSlots[i][j] = newSlot.GetComponent<MapSlot>();
                if (i == GameManager.instance.playerDepth && (i == 0 || MapManager.instance.edges[i - 1][GameManager.instance.playerPosition, j])) {
                    newSlot.GetComponent<MapSlot>().image.color = Color.yellow;
                }
                else {
                    newSlot.GetComponent<MapSlot>().image.color = Color.black;
                }
            }
        }
        for (int i = 1; i < mapWidth; i++) {
            for (int j = 0; j < height[i - 1]; j++) {
                for (int k = 0; k < height[i]; k++) {
                    if (MapManager.instance.edges[i - 1][j, k] == true) {
                        Vector3 startPos = mapSlots[i - 1][j].GetComponent<RectTransform>().anchoredPosition3D + new Vector3(100, 0, 0);
                        Vector3 endPos = mapSlots[i][k].GetComponent<RectTransform>().anchoredPosition3D;
                        GameObject newLine = Instantiate(pathLinePrefab, mapParent);
                        LineRenderer lr = newLine.GetComponent<LineRenderer>();
                        lr.SetPosition(0, startPos);
                        lr.SetPosition(1, endPos);
                    }
                }
            }
        }
    }
}
