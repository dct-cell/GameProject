using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.TextCore.Text;
public class CharacterCreater : MonoBehaviour
{
	public static CharacterCreater instance { get; private set; }
	[SerializeField] private List<Character> characterPrefabs = new List<Character>();
	[SerializeField] private Dictionary<string, Character> prefabDict = new Dictionary<string, Character>();

	private int characterNumber = 0;
	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);

		foreach (Character prefab in characterPrefabs)
			if (prefab != null && !prefabDict.ContainsKey(prefab.characterName))
			{
				Debug.Log($"Register Character Prefab: {prefab.characterName}");
				prefabDict.Add(prefab.characterName, prefab);
			}
	}

	private void InitBattleCharacter(Character character, int teamId, Vector3Int pos)
	{
		//character.uid = System.Guid.NewGuid().ToString();
		character.uid = (++characterNumber).ToString();
		Debug.Log($"Create Battle Character {character.characterName} with UID {character.uid}");
		character.teamId = teamId;
		character.position = pos;
		character.characterBattleAnimator.EnableBattleAnimation();
	}

	public Character CreateBattleCharacter(string characterName, int teamId = 0, Vector3Int pos = new Vector3Int())
	{
		if (prefabDict.TryGetValue(characterName, out Character character))
		{
			Character newCharacter = Instantiate(character, transform);
			InitBattleCharacter(newCharacter, teamId, pos);
			return newCharacter;
		}
		else
		{
			Debug.LogError($"Character {characterName} not found!");
			return null;
		}
	}

	private void InitCharacter(Character character)
	{
		//character.uid = System.Guid.NewGuid().ToString();
		character.uid = (++characterNumber).ToString();
		Debug.Log($"Create Character {character.characterName} with UID {character.uid}");
		character.characterBattleAnimator.DisableBattleAnimation();
	}

	public Character CreateCharacter(string characterName)
	{
		if (prefabDict.TryGetValue(characterName, out Character character))
		{
			Character newCharacter = Instantiate(character, transform);
			InitCharacter(newCharacter);
			return newCharacter;
		}
		else
		{
			Debug.LogError($"Character {characterName} not found!");
			return null;
		}
	}
}