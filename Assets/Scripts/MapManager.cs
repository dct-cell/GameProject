using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MapManager : MonoBehaviour {
    public static MapManager instance { get; private set; }
    private void Awake() // appear in all scenes
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int mapWidth;
    public int mapHeight;
    public int[] height;
    public List<int[]> vertices = new List<int[]>();
    public List<MapSlotType[]> types = new List<MapSlotType[]>();
    public List<bool[,]> edges = new List<bool[,]>(); 
    public List<MapSlot> mapSlots = new List<MapSlot>();

    public void GenerateMap(int _mapWidth, int _mapHeight) {
        mapWidth = _mapWidth;
        mapHeight = _mapHeight;
        height = new int[mapWidth];
        for (int i = 0; i < mapWidth; i++) {
            height[i] = Random.Range(1, mapHeight + 1);
            if (i == mapWidth - 1)
                height[i] = 1;
            // 最后一层一定只有一格
            vertices.Add(new int[height[i]]);
            types.Add(new MapSlotType[height[i]]);
            for (int j = 0; j < height[i]; j++) {
                int r = Random.Range(0, 100);

				if (r < 33)
                    types[i][j] = MapSlotType.Battle;
                else if (r < 66)
					types[i][j] = MapSlotType.Shop;
                else
                    types[i][j] = MapSlotType.Event;
				if (i == 0)
                    types[i][j] = MapSlotType.Shop;
                if (i == mapWidth - 1)
                    types[i][j] = MapSlotType.Battle;
                // 第一层一定全部是商店
                // 最后一层一定是战斗
            }
        }
        for (int i = 1; i < mapWidth; i++) {
            edges.Add(new bool[height[i - 1], height[i]]);
            int pos1 = 0, pos2 = 0;
            while (true) {
                edges[i - 1][pos1, pos2] = true; 
                if (pos1 == height[i - 1] - 1 && pos2 == height[i] - 1)
                    break;
                // Debug.Log(i - 1 + "," + pos1 + "->" + i + "," + pos2);
                if (pos1 < height[i - 1] - 1)
                    if (Random.Range(0, 100) < 50)
                        pos1++;
                if (pos2 < height[i] - 1)
                    if (Random.Range(0, 100) < 50)
                        pos2++;
            }
        }
    }

    public void ClearMap() {
        height = new int[0];
        vertices = new List<int[]>();
        types = new List<MapSlotType[]>();
        edges = new List<bool[,]>();
        mapSlots = new List<MapSlot>();
    }
}
