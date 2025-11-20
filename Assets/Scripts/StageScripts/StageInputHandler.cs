using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageInputHandler : MonoBehaviour // Handles input specific to the battle scene
{
	public static StageInputHandler instance { get; private set; }

	public bool selected;
	public bool selectedInBag;
	public Character selectedCharacter;
	[SerializeField] LayerMask gridLayerMask;

    public int currentCharacterCount;

    private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
		}
		instance = this;
	}
	public void BattleInit()
	{

	}

	private void Update()
	{
		if (!StageManager.instance.isStarted) {
            // if (Input.GetKeyDown(KeyCode.K))
            //     StartBattle();
            // already add button to start battle
            if (Input.GetMouseButtonDown(0)) {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return; // UI 已拦截，主要是背包槽
                if (selected)
                    ClickWhenSelected(); // 放置或与场景交互
                else
                    ClickWhenUnselected(); // 点击选择场景物体等（未选中时的操作）
            }
        }
		else if (!StageManager.instance.isFinished) {
            // Battle in progress
        }
        // else if (StageManager.instance.isFinished) {
        // 	if (Input.GetKeyDown(KeyCode.N)) {
        //         EndBattle();
        //     }
        // }
        // already add button to end battle
    }

    public void EndBattle() {
        if (BattleManager.instance.GetWinner() == 0) {
            GameManager.instance.NextLevel();
        }
        else {
            Debug.Log("Game Over. You lost the battle.");
            GameManager.instance.GameOver();
        }
    }

    public void StartBattle()
	{
		if (StageManager.instance.isStarted)
			return;	
        Debug.Log("Battle Start!");
		StageManager.instance.StartBattle();
	}

    public void HandleSlotClick(Character _character) {
        // 如果在背包里点击了某个角色槽
        // 如果已经选中了一个角色
        if (selectedCharacter != null) {
            // 点击了空槽，放回背包
            if (_character == null) {
                BagManager.instance.AddMember(selectedCharacter);
                RemoveMember();
                DeSelectCharacter();
            }
            // 再次点击同一角色，取消选择
            else if (_character == selectedCharacter) {
                DeSelectCharacter();
            }
            // 点击了不同角色，选中该角色
            else {
                SelectCharacter(_character, true);
            }
        }
        // 如果没有选中角色，选中该角色
        else if (_character != null) {
            SelectCharacter(_character, true);
        }
    }

    public void SelectCharacter(Character _character, bool inBag) {
		selectedCharacter = _character;
        selected = true;
		selectedInBag = inBag;
        UI_StatsPanel.instance.ShowStats(_character);
    }

    public void RemoveMember() {
		if (!selected)
			return;
        // 如果是背包里的角色，直接从背包移除
        if (selectedInBag) {
			BagManager.instance.RemoveMember(selectedCharacter);
		}
        // 如果是场上角色，从战斗队伍中移除
        else {
			int teamID = BattleManager.instance.GetTeamID(selectedCharacter);
			Debug.Assert(teamID == 0, "Only player team members can be removed.");
            BattleManager.instance.RemoveMember(selectedCharacter);
			selectedCharacter.gameObject.SetActive(false);
            --currentCharacterCount;
            UI_BattleSceneManager.instance.UpdateCharacterCountText();
        }
    }

    public void DeSelectCharacter() {
		selectedCharacter = null;
        selected = false;
		selectedInBag = false;
		UI_StatsPanel.instance.Clear();
    }

    // 在没有选中角色的情况下点击（但不是背包槽）
	public void ClickWhenUnselected() {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
		RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, gridLayerMask);
        // 如果点击到某个格子
        if (hit.collider != null) {
            GameObject hitObject = hit.collider.gameObject;
            GridScript gridScript = hitObject.GetComponent<GridScript>();
            if (gridScript != null) {
                Vector3Int coordinate = gridScript.coordinate;
                if (GridManager.instance.HasCharacter(coordinate)) {
                    Character characterInGrid = GridManager.instance.FindCharacter(coordinate);
                    // 如果格子里有角色，是玩家阵营则选中，是敌方阵营则显示信息
                    int teamID = BattleManager.instance.GetTeamID(characterInGrid);
                    if (teamID == 0) {
                        SelectCharacter(characterInGrid, false);
                    }
                    else {
                        UI_StatsPanel.instance.ShowStats(characterInGrid);
                    }
                }
            }
		    // 格子里没有棋子，取消选择
            else {
				DeSelectCharacter();
            }
        }
        // 没点击到格子，取消选择
        else {
            DeSelectCharacter();
        }
    }	
    
    // 在选中角色的情况下点击（不应该是背包槽）
    public void ClickWhenSelected() {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
		RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, gridLayerMask);
	    // 如果点击了某个格子
		if (hit.collider != null) {
			GameObject hitObject = hit.collider.gameObject;
			GridScript gridScript = hitObject.GetComponent<GridScript>();
			if (gridScript != null) {
				Vector3Int coordinate = gridScript.coordinate;
                // 如果格子里没有角色，放置选中的角色
                if (!GridManager.instance.HasCharacter(coordinate)) {
                    if (!(selectedInBag && currentCharacterCount == StageManager.instance.maxCharacterCount)) {
                        RemoveMember();
                        selectedCharacter.gameObject.SetActive(true);
                        selectedCharacter.position = coordinate;
                        BattleManager.instance.AddMember(0, selectedCharacter);
                        ++currentCharacterCount;
                        UI_BattleSceneManager.instance.UpdateCharacterCountText();
                        DeSelectCharacter();
                    }
                    else {
                        Debug.Log("已达上限！");
                        UI_BattleSceneManager.instance.StartFlashText();
                    }
				}
                // 如果格子里有角色，是玩家阵营则选中，是敌方阵营则显示信息
                else {
					Character characterHere = GridManager.instance.FindCharacter(coordinate);
                    int teamID = BattleManager.instance.GetTeamID(characterHere);
					if (teamID == 0) {
						SelectCharacter(characterHere, false);
                    }
                    else {
						DeSelectCharacter();
                        UI_StatsPanel.instance.ShowStats(characterHere);
                    }
                }
			}
			// 点击到格子但没有点击到格子，神人
			else {
                Debug.Assert(false, "Why are you here?");
			}
		} 
		// 没点击到格子，取消选择
        else {
			DeSelectCharacter();
		}
	}
}
