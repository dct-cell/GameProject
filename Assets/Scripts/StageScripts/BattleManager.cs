using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
	public static BattleManager instance { get; private set; }
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

	public TeamManager[] team = new TeamManager[2]; // team[0]: player, team[1]: enemy

	private bool isMoving;
	private bool isAttacking;
	
	[Header("Battle Timing")]
	public float waitMoveTime;
	public float waitAttackTime;
	private float waitTimer;

	private int preMinTime;
	public int[] order = new int[2] { 0, 1 };
	private Character currentCharacter;


	public void BattleInit()
	{
		team[0] = new TeamManager();
		team[1] = new TeamManager();
		preMinTime = -1;
	}

	public void AddMember(int teamId, Character character)
	{
		//Debug.Log($"Add member {character.uid} to team {teamId}");
		team[teamId].AddMember(character);
		character.characterBattleAnimator.EnableBattleAnimation();
		character.healthBarUI.UpdateHealthUI();
	}

	public void RemoveMember(int teamId, Character character)
	{
		//Debug.Log($"Remove member {character.uid} from team {teamId}");
		team[teamId].RemoveMember(character);
		character.characterBattleAnimator.DisableBattleAnimation();
	}

	public List<Character> GetAliveTeamMember(int teamId)
	{
		return team[teamId].GetAliveMembers();
	}

	public List<Character> GetAllTeamMember(int teamId)
	{
		return team[teamId].members;
	}

	public Character FindCharacter(string uid)
	{
		for (int i = 0; i <= 1; i++)
			foreach (var member in team[i].members)
				if (member.uid == uid)
					return member;

		Debug.LogError($"Character with UID {uid} not found!");
		return null;
	}

	public int GetTeamID (Character _character) {
		for (int i = 0; i <= 1; i++)
			foreach (var member in team[i].members)
				if (member.uid == _character.uid)
					return i;
		Debug.LogError($"Character with UID {_character.uid} not found!");
		return -1;	
    }

	public void RemoveMember(Character character)
	{
		int teamId = GetTeamID(character);
		RemoveMember(teamId, character);
	}

    public void DamageCharacter(string uid, int damage)
	{
		Character target = FindCharacter(uid);
		target.IsDamagedBy(damage);
	}

	void NextMember()
	{
		foreach (int id in order)
			team[id].shuffle();

		int minTime = Mathf.Min(team[0].MinTime(), team[1].MinTime());

		if(minTime > preMinTime)
		{
			preMinTime = minTime;
			order[0] = 0;
			order[1] = 1;
		}

		foreach (int id in order)
		{
			bool f = false;
			foreach (var member in team[id].members)
				if (member.isAlive && member.nextRoundTime == minTime)
				{
					currentCharacter = member;
					f = true;
					break;
				}
			if(f)
				break;
		}
		Debug.Log($"Current character: {currentCharacter.uid}, time {currentCharacter.nextRoundTime}(Team {GetTeamID(currentCharacter)})");
		currentCharacter.nextRoundTime += currentCharacter.speed;
		order[0] ^= 1;
		order[1] ^= 1;
	}

	#region Timer
	void WaitSomeTime(float t)
	{
		waitTimer = t;
	}
	bool StillWaiting()
	{
		return waitTimer > 0f;
	}
	void DealTimer()
	{
		waitTimer -= Time.deltaTime;
	}
	#endregion
	public bool Battle() // bool -> is finished
	{
		DealTimer();
		if (isMoving)
		{
			if(currentCharacter.characterBattleAnimator.IsMovementComplete())
			{
				isMoving = false;
				WaitSomeTime(waitMoveTime);
			}
		}
		else if (isAttacking)
		{
			isAttacking = false;
			WaitSomeTime(waitAttackTime);
		}
		else
		{
			if(StillWaiting())
			{
				return false;
			}

			if (team[0].AllDead()|| team[1].AllDead())
			{
				return true;
			}

			NextMember();

			int state = currentCharacter.SingleRound();
			if (state == 0)
			{
				isMoving = true;
				currentCharacter.MoveAnimation();
			}
			else if (state == 1)
			{
				isAttacking = true;
			}
			else
			{
				Debug.LogError("Invalid State!");
			}
		}
		return false;
	}

	public int GetWinner()
	{
		if (team[0].AllDead())
			return 1;
		else if(team[1].AllDead())
			return 0;
		else
		{
			Debug.LogError("Battle not finished yet!");
			return -1;
		}
	}

    public event Action<Character> OnCharacterDied;

    public void TriggerCharacterDied(Character diedCharacter) {
        // 确保事件有订阅者，避免空引用错误
        OnCharacterDied?.Invoke(diedCharacter);
        Debug.Log($"EVENT: Unit {diedCharacter.name} has died.");
    }
}