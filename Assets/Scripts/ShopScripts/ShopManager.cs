using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class ShopManager : MonoBehaviour
{
	public static ShopManager instance { get; private set; }
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
		foreach (var character in shopCharacter)
			Destroy(character.gameObject);

		if (instance == this)
			instance = null;
	}

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return; // UI 已拦截
			HandleOtherClick();
        }
    }

    public int reloadCharacterCost;
	public List<Character> shopCharacter = new List<Character>();

	void AddCharacter(string name)
	{
		shopCharacter.Add(CharacterCreater.instance.CreateCharacter(name));
	}
	public void ReloadCharacter()
	{
		if (BagManager.instance.coin < reloadCharacterCost)
		{
			Debug.Log("Not enough coin to reload shop!");
            UI_ShopManager.instance.ShowInfo("没有足够的金币来刷新");
            return;
		}
		BagManager.instance.coin -= reloadCharacterCost;
        UI_ShopManager.instance.UpdateCoinText();
        foreach (var character in shopCharacter)
			Destroy(character.gameObject);
		shopCharacter.Clear();
        UI_ShopManager.instance.ShowInfo("刷新商店成功");
        shopCharacterInit();
	}

	public void shopCharacterInit()
	{
		int sellCount = Random.Range(5, 11);
		List<Character> possibleCharacters = GameManager.instance.shopCharacters;
		for (int i = 0; i < sellCount; i++)
			AddCharacter(possibleCharacters[Random.Range(0, possibleCharacters.Count)].characterName);
		UI_ShopManager.instance.UpdateSlotUI();
	}
	public void ShopInit()
	{
		reloadCharacterCost = 2;
		shopCharacterInit();
        UI_ShopManager.instance.UpdateCoinText();
    }

	public Character selectedCharacter;

	public void HandleBagSlotClick(Character _character) {
		selectedCharacter = null;
		if (_character == null)
			UI_StatsPanel.instance.Clear();
		else
			UI_StatsPanel.instance.ShowStats(_character);
	}

	public void HandleShopSlotClick(Character _character) {
		if (selectedCharacter == _character) {
			selectedCharacter = null;
			UI_StatsPanel.instance.Clear();
		}
		else {
			selectedCharacter = _character;
			UI_StatsPanel.instance.ShowStats(selectedCharacter);
		}
    }

	public void HandleOtherClick() {
		selectedCharacter = null;
		UI_StatsPanel.instance.Clear();
	}

	public void BuyCharacter()
	{
		if (selectedCharacter == null)
			return;
		if(BagManager.instance.coin < selectedCharacter.price)
		{
			Debug.Log("Not enough coin!");
			UI_ShopManager.instance.ShowInfo("没有足够的金币来购买");
			return;
		} 
		else
		{
			BagManager.instance.coin -= selectedCharacter.price;
			UI_ShopManager.instance.UpdateCoinText();
			BagManager.instance.AddMember(selectedCharacter);
			shopCharacter.Remove(selectedCharacter);
            UI_ShopManager.instance.ShowInfo($"购买 {selectedCharacter.characterName} 成功");
            selectedCharacter = null;
			UI_StatsPanel.instance.Clear();
			UI_ShopManager.instance.UpdateSlotUI();
		}
	}

	public void ExitShop() {
		GameManager.instance.NextLevel();
	}
}
