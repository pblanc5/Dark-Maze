using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public Maze mazePrefab;
	private Maze mazeInstance;

    public enum level
    {
        SMALL,
        MEDIUM,
        LARGE
    }


    private void Start () {
        Database db = gameObject.AddComponent<Database>();
		BeginGame();
        db.initDB();
    }
	
	private void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			RestartGame();
		}
	}

	private void BeginGame () {
		mazeInstance = Instantiate(mazePrefab) as Maze;
		StartCoroutine(mazeInstance.Generate());
	}

	private void RestartGame () {
		StopAllCoroutines();
		Destroy(mazeInstance.gameObject);
		BeginGame();
	}
}