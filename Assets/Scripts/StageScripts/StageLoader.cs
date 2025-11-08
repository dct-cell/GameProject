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

	public void StageInit()
	{
		BattleManager.instance.AddMember(1, CharacterCreater.instance.CreateBattleCharacter("E", 1, new Vector3Int(2, 2, -4)));
		BattleManager.instance.AddMember(1, CharacterCreater.instance.CreateBattleCharacter("F", 1, new Vector3Int(3, 1, -4)));
        BattleManager.instance.AddMember(1, CharacterCreater.instance.CreateBattleCharacter("G", 1, new Vector3Int(4, -1, -3)));
		StageManager.instance.maxCharacterCount = 5;
    }

}
