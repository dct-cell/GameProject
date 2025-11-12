using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLoader : MonoBehaviour
{
	public static StageLoader instance { get; private set; }
	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
	}
	private void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}

	public int levelId;

	public void StageInit() {
		int playerDepth = GameManager.instance.playerDepth;
		string levelId;
		if (Random.Range(0, 100) < 50)
			levelId = "Test_01";
		else
			levelId = "Test_02";
		TextAsset jsonAsset = Resources.Load<TextAsset>("Levels/" + levelId);
        string jsonText = jsonAsset.text;
        LevelConfig data = JsonUtility.FromJson<LevelConfig>(jsonText);

        StageManager.instance.maxCharacterCount = data.maxCharacterCount;
		Debug.Log(data.enemyList);
		foreach (var enemy in data.enemyList) {
			BattleManager.instance.AddMember(1, CharacterCreater.instance.CreateBattleCharacter(enemy.name, 1, new Vector3Int(enemy.xPos, enemy.yPos, enemy.zPos)));
		}
    }

}
