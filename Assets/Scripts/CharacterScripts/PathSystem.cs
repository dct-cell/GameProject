using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSystem : MonoBehaviour
{
	public static PathSystem instance { get; private set; }
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

	private static readonly Vector3Int[] directionsArray = new Vector3Int[]
	{
		new Vector3Int(1, -1, 0),
		new Vector3Int(0, 1, -1),
		new Vector3Int(-1, 0, 1),
		new Vector3Int(-1, 1, 0),
		new Vector3Int(0, -1, 1),
		new Vector3Int(1, 0, -1)
	};


	// 公共 API：计算最佳下一步位置（一步，包括原地不动）
	// 选择一个到最近敌人的最短路径距离最短的位置。
	// 如果多个位置相同，则随机选择一个。如果没有可到达的位置，返回原始位置。
	// 近战角色逻辑
	public Vector3Int MeleeLogic(Character c)
	{
		GridManager grid = GridManager.instance;

		List<Character> enemies = BattleManager.instance.GetAliveTeamMember(c.teamId ^ 1);
		if (enemies == null || enemies.Count == 0)
		{
			Debug.LogError("no enemies found in MeleeLogic");
			return c.position;
		}

		HashSet<Vector3Int> enemyPositions = new HashSet<Vector3Int>();
		foreach (var e in enemies)
			enemyPositions.Add(e.position);

		// 可能移动到的位置
		List<Vector3Int> candidates = new List<Vector3Int>();
		candidates.Add(c.position);
		foreach (var dir in directionsArray)
		{
			Vector3Int np = c.position + dir;
			if (!grid.CheckPosition(np)) continue;
			if (grid.HasCharacter(np)) continue;
			candidates.Add(np);
		}

		// 构建用于距离计算的可通行节点。
		// 可通行：所有未被任何角色占据的有效单元格，加上角色 c 的当前位置。
		// 敌方单元格不可通行，但在距离计算中作为源/目标。
		HashSet<Vector3Int> passable = new HashSet<Vector3Int>();
		foreach (var pos in grid.allPositions)
		{
			if (!grid.CheckPosition(pos)) continue;
			bool occupied = grid.HasCharacter(pos);
			if (!occupied || pos == c.position)
				passable.Add(pos);
		}

		// BFS 从敌人位置出发，计算到所有可通行单元格的最短距离
		Dictionary<Vector3Int, int> distToEnemy = ComputeDistancesToNearestTargets(passable, enemyPositions);

		// 找到最佳位置
		int bestDist = int.MaxValue;
		List<Vector3Int> bestPositions = new List<Vector3Int>();
		foreach (var pos in candidates)
		{
			int d;
			if (!distToEnemy.TryGetValue(pos, out d)) d = int.MaxValue;
			if (d < bestDist)
			{
				bestDist = d;
				bestPositions.Clear();
				bestPositions.Add(pos);
			}
			else if (d == bestDist)
			{
				bestPositions.Add(pos);
			}
		}

		if (bestPositions.Count == 0 || bestDist == int.MaxValue)
			return c.position;

		int idx = Random.Range(0, bestPositions.Count);
		return bestPositions[idx];
	}

	// 模块化距离计算：从目标（敌方）单元格到可通行单元格的 BFS
	// 这样可以避免穿过已占领的单元格，同时还能测量到敌方单元格的距离。
	private Dictionary<Vector3Int, int> ComputeDistancesToNearestTargets(HashSet<Vector3Int> passable, HashSet<Vector3Int> targets)
	{
		Dictionary<Vector3Int, int> dist = new Dictionary<Vector3Int, int>();
		Queue<Vector3Int> q = new Queue<Vector3Int>();
		HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

		foreach (var t in targets)
		{
			q.Enqueue(t);
			visited.Add(t);
		}

		while (q.Count > 0)
		{
			var cur = q.Dequeue();
			bool curIsPassable = passable.Contains(cur);
			int curDist = 0;
			if (curIsPassable)
			{
				curDist = dist[cur];
			}
			for (int i = 0; i < directionsArray.Length; i++)
			{
				var nb = cur + directionsArray[i];
				//if (!GridManager.instance.CheckPosition(nb)) continue;
				if (visited.Contains(nb)) continue;

				if (passable.Contains(nb))
				{
					visited.Add(nb);
					dist[nb] = curDist + 1;
					q.Enqueue(nb);
				}
			}
		}

		return dist;
	}
}
