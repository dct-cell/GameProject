using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TeamManager
{
	public List<Character> members = new List<Character>();
	public int memberCount => members.Count;


	public void shuffle()
	{
		int n = members.Count;
		System.Random rnd = new System.Random();
		for (int i = 1; i < n; i++)
		{
			int j = rnd.Next(0, i + 1);
			Character temp = members[i];
			members[i] = members[j];
			members[j] = temp;
		}
	}
	public int MinTime()
	{
		int minTime = int.MaxValue;
		foreach (Character character in members)
			if (character.isAlive && character.nextRoundTime < minTime)
				minTime = character.nextRoundTime;
		return minTime;
	}
	public void AddMember(Character character)
	{
		members.Add(character);
		character.nextRoundTime = character.speed;
	}
	public void RemoveMember(Character character)
	{
		foreach (Character _character in members)
		{
			if (_character.uid == character.uid)
			{
				members.Remove(_character);
				return;
			}
		}
	}
	public int MemberCount()
	{
		return members.Count;
	}	

    public List<Character> GetAliveMembers()
    {
		List<Character> characters = new List<Character>();
		foreach (Character character in members)
			if(character.isAlive)
				characters.Add(character);
		return characters;
	}

    public Character GetMember(int index)
    {
        return members[index];
	}
	public bool AllDead()
	{
		foreach (Character character in members)
			if (character.isAlive)
				return false;
		return true;
	}
}
