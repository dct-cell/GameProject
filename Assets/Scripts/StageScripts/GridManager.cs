using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
	Vector3Int[] directionsArray = new Vector3Int[]{
		new Vector3Int(1, -1, 0),
		new Vector3Int(0, 1, -1),
		new Vector3Int(-1, 0, 1),
		new Vector3Int(-1, 1, 0),
		new Vector3Int(0, -1, 1),
		new Vector3Int(1, 0, -1)
	};
	public static GridManager instance { get; private set; }
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

	[SerializeField] private GameObject gridPrefab;
    private int size = 5;
	public List<Vector3Int> allPositions;

    public void GridInit()
    {
		// TODO: ’œ∞≠
		for (int x = -size; x <= size; x++)
			for (int y = -size; y <= size; y++) {
				Vector3Int coordinate = new Vector3Int(x, y, -x - y);
                if (CheckPosition(coordinate)) {
					GameObject newGrid = Instantiate(gridPrefab, transform.position + ComputeOffset(coordinate), gridPrefab.transform.rotation, transform);
					newGrid.GetComponent<GridScript>().coordinate = coordinate;	
					allPositions.Add(coordinate);
                }
			}
	}
	public bool CheckPosition(Vector3Int coordinate) 
    {
        int x = coordinate.x, y = coordinate.y, z = coordinate.z;
        if (x + y + z != 0) return false;
		if (y > 3 || y < -3) return false;
		if (x - z > 7 || x - z < -7) return false;
        return true;
    }

    public Vector3 ComputeOffset(Vector3Int coordinate) 
    {
		Debug.Assert(CheckPosition(coordinate), $"Invalid position, coordinate: {coordinate.x}, {coordinate.y}, {coordinate.z}");

		int x = coordinate.x, y = coordinate.y, z = coordinate.z;
        float xOffset = (x - z) * Mathf.Sqrt(3) / 4f;
        float yOffset = y * .75f;
        return new Vector3(xOffset, yOffset);
    }

    public bool HasCharacter(Vector3Int pos)
    {
        foreach (var character in BattleManager.instance.GetAliveTeamMember(0))
            if (character.position == pos && character.isAlive)
                return true;
        foreach (var character in BattleManager.instance.GetAliveTeamMember(1))
            if (character.position == pos && character.isAlive)
                return true;
        return false;
    }

	public Character FindCharacter(Vector3Int pos) {
		foreach (var character in BattleManager.instance.GetAliveTeamMember(0))
			if (character.position == pos && character.isAlive)
				return character;
		foreach (var character in BattleManager.instance.GetAliveTeamMember(1))
			if (character.position == pos && character.isAlive)
				return character;
		return null;
    }

	public List<Vector3Int> FindNeighbourhood(Vector3Int position, int distance) {
		List<Vector3Int> neighbourhood = new List<Vector3Int>();
		foreach (var coordinate in allPositions) {
			if (Distance(coordinate, position) < distance)
				neighbourhood.Add(coordinate);
		}
		return neighbourhood;
	}

    public int Distance(Vector3Int a, Vector3Int b) 
    {
		return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
	}

	public void CleanUp() {
		gameObject.SetActive(false);
	}
}
