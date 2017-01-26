using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {

	public IntVector2 size;
    public float scale;

	public MazeCell cellPrefab;

	public float generationStepDelay;

	public MazePassage passagePrefab;
	public MazeWall wallPrefab;
    public GameObject player;
    public GameObject goal;
    public GameObject jammer;

    private MazeCell[,] cells;

    private bool flag;
    private bool nogoal;
    public int jammercount;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        goal = GameObject.FindGameObjectWithTag("Finish");
        flag = false;
        nogoal = true;
    }

    public IntVector2 RandomCoordinates {
		get {
			return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
		}
	}

	public bool ContainsCoordinates (IntVector2 coordinate) {
		return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
	}

	public MazeCell GetCell (IntVector2 coordinates) {
		return cells[coordinates.x, coordinates.z];
	}

	public IEnumerator Generate () {
		WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
		cells = new MazeCell[size.x, size.z];
		List<MazeCell> activeCells = new List<MazeCell>();
		DoFirstGenerationStep(activeCells);
		while (activeCells.Count > 0) {
			yield return delay;
			DoNextGenerationStep(activeCells);
		}

	}

	private void DoFirstGenerationStep (List<MazeCell> activeCells) {
		activeCells.Add(CreateCell(RandomCoordinates));
	}

	private void DoNextGenerationStep (List<MazeCell> activeCells) {
		int currentIndex = activeCells.Count - 1;
		MazeCell currentCell = activeCells[currentIndex];
		if (currentCell.IsFullyInitialized) {
			activeCells.RemoveAt(currentIndex);
			return;
		}
		MazeDirection direction = currentCell.RandomUninitializedDirection;
        IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();// * new IntVector2((int)scale, (int)scale);
		if (ContainsCoordinates(coordinates)) {
			MazeCell neighbor = GetCell(coordinates);
			if (neighbor == null) {
				neighbor = CreateCell(coordinates);
				CreatePassage(currentCell, neighbor, direction);
				activeCells.Add(neighbor);
			}
			else {
				CreateWall(currentCell, neighbor, direction);
			}
		}
		else {
			CreateWall(currentCell, null, direction);
		}
	}

	private MazeCell CreateCell (IntVector2 coordinates) {
		MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
		cells[coordinates.x, coordinates.z] = newCell;
		newCell.coordinates = coordinates;
		newCell.name = "Maze Cell " + coordinates.x / scale + ", " + coordinates.z / scale;
        if (coordinates.x == 0 && !flag)
        {
            flag = true;
            player.transform.parent = transform;
            player.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f * scale + 0.5f, -0.08f, coordinates.z - size.z * 0.5f * scale + 0.5f);
        }
        else if (coordinates.x == size.x - 1 && nogoal)
        {
            nogoal = false;
            goal.transform.parent = transform;
            goal.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f * scale + 0.5f, 1f, coordinates.z - size.z * 0.5f * scale + 0.5f);
        }
        else if (Random.Range(0, 99) < 10 && jammercount > 0)
        {
            jammercount--;
            GameObject jam = Instantiate(jammer);
            jam.transform.parent = transform;
            jam.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f * scale + 0.5f, 1f, coordinates.z - size.z * 0.5f * scale + 0.5f);
        }
		newCell.transform.parent = transform;
		newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f * scale + 0.5f, 0f, coordinates.z - size.z * 0.5f * scale + 0.5f);
		return newCell;
	}

	private void CreatePassage (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazePassage passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(otherCell, cell, direction.GetOpposite());
	}

	private void CreateWall (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazeWall wall = Instantiate(wallPrefab) as MazeWall;
		wall.Initialize(cell, otherCell, direction);
		if (otherCell != null) {
			wall = Instantiate(wallPrefab) as MazeWall;
			wall.Initialize(otherCell, cell, direction.GetOpposite());
		}
	}

    private void OnDestroy()
    {
        cells  = null;
        player = null;
        goal   = null;
        jammer = null;
        flag   = false;
        nogoal = true;
        jammercount = 0;
    }
}