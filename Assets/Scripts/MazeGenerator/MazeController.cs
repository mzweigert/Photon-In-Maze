using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>

public enum MazeGenerationAlgorithm {
    PureRecursive,
    RandomTree,
    Division
}

public class MazeController : MonoBehaviour {

    [SerializeField]
    private MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
    [SerializeField]
    private bool FullRandom = false;
    [SerializeField]
    private int RandomSeed = 12345;
    [SerializeField]
    private GameObject Floor = null;
    [SerializeField]
    private GameObject Wall = null;
	//public GameObject Pillar = null;
    [Range(1, 50)]
    public int Rows = 5;
    [Range(1, 50)]
    public int Columns = 5;
    public float LenghtSide { get { return 4f; } }
    public LinkedList<MazeCell> PathsToGoal { get; private set; }
    private HashSet<GameObject> pathToGoalsGameObjects = new HashSet<GameObject>();
    private bool pathDestroyed = false;

    private BasicMazeGenerator mMazeGenerator = null;

	void Awake () {
		if (!FullRandom) {
            UnityEngine.Random.InitState(RandomSeed);
		}

        mMazeGenerator = GetGenerator(Rows, Columns);
        PathsToGoal = mMazeGenerator.GenerateMazeAndFindPathToGoal();
        GameObject area = GenerateArea();

        for (int row = 0; row < Rows; row++) {
			for(int column = 0; column < Columns; column++) {

                MazeCell cell = mMazeGenerator.GetMazeCell(row, column);

                GameObject cellGameObject = new GameObject() { name = cell.ToStringAsName() };
                cellGameObject.transform.parent = transform;
                cellGameObject.transform.position = new Vector3(cell.X, area.transform.position.y, cell.Y);
                GenerateWalls(cell, cellGameObject);

            }
        }
        /*if(Pillar != null){
			for (int row = 0; row < Rows+1; row++) {
				for (int column = 0; column < Columns+1; column++) {
					float x = column*(CellWidth+(AddGaps?.2f:0));
					float z = row*(CellHeight+(AddGaps?.2f:0));
					GameObject tmp = Instantiate(Pillar,new Vector3(x-CellWidth/2,0,z-CellHeight/2),Quaternion.identity) as GameObject;
					tmp.transform.parent = transform;
				}
			}
		}*/
	}

    private GameObject GenerateArea() {
        GameObject area = new GameObject { name = "Area" };
        area.transform.parent = transform;
        float rowBound = (Rows * LenghtSide / 2) - LenghtSide / 2;
        float columnBound = (Columns * LenghtSide / 2) - LenghtSide / 2;
        GameObject floor = Instantiate(Floor, new Vector3(columnBound, 0, rowBound), Quaternion.Euler(0, 0, 0));
        floor.transform.localScale = new Vector3(Columns * LenghtSide, 0.25f, Rows * LenghtSide);
        floor.name = "Floor";
        floor.transform.parent = area.transform;
        Vector3 wallPos; Quaternion wallQuat; float xLength, offset;
        for(int i = 0; i < 4; i++) {
            bool column = i % 2 == 0;

            if(column) {
                offset = i > 1 ? LenghtSide : -LenghtSide;
                wallPos = new Vector3(i * columnBound + offset / 2, area.transform.position.y, rowBound);
                wallQuat = Quaternion.Euler(0, (i - 1) * 90, 0);
                xLength = Rows;
               
            } else {
                offset = i > 2 ? LenghtSide : -LenghtSide;
                wallPos = new Vector3(columnBound, area.transform.position.y, (i - 1) * rowBound + offset / 2);
                wallQuat = Quaternion.Euler(0, (i + 1) * 90, 0);
                xLength = Columns;
            }

            GameObject wall = Instantiate(Wall, wallPos, wallQuat);
            wall.name = "Wall";
            wall.transform.localScale = new Vector3(xLength, wall.transform.localScale.y, wall.transform.localScale.z);
            wall.transform.parent = area.transform;
        }

        return area;
    }

    private void GenerateWalls(MazeCell cell, GameObject cellGameObject) {
        GameObject wall;
        if(cell.IsPathToGoal || cell.IsGoal) {
            Vector3 pos = cellGameObject.transform.position;
            pos.y = cellGameObject.transform.position.y + 0.25f;
            GameObject cellInPath = Instantiate(Floor, pos, Quaternion.Euler(0, 0, 0));
            cellInPath.GetComponent<Renderer>().material.color = Colors.Navy;
            cellInPath.transform.parent = cellGameObject.transform;
            cellInPath.name = "CellInPath";
            pathToGoalsGameObjects.Add(cellInPath);
        }
        if(cell.WallRight && cell.Column + 1 < Columns) {
            wall = Instantiate(Wall, new Vector3(cell.X + LenghtSide / 2, 0, cell.Y) + Wall.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;// right
            wall.name = "WallRight";
            wall.transform.parent = cellGameObject.transform;
        }
        if(cell.WallFront && cell.Row + 1 < Rows) {
            wall = Instantiate(Wall, new Vector3(cell.X, 0, cell.Y + LenghtSide / 2) + Wall.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;// front
            wall.name = "WallFront";
            wall.transform.parent = cellGameObject.transform;
        }
        if(cell.WallLeft && cell.Column > 0) {
            wall = Instantiate(Wall, new Vector3(cell.X - LenghtSide / 2, 0, cell.Y) + Wall.transform.position, Quaternion.Euler(0, 270, 0)) as GameObject;// left
            wall.name = "WallLeft";
            wall.transform.parent = cellGameObject.transform;
        }
        if(cell.WallBack && cell.Row > 0) {
            wall = Instantiate(Wall, new Vector3(cell.X, 0, cell.Y - LenghtSide / 2) + Wall.transform.position, Quaternion.Euler(0, 180, 0)) as GameObject;// back
            wall.name = "WallBack";
            wall.transform.parent = cellGameObject.transform;
        }

    }

    private BasicMazeGenerator GetGenerator(int rows, int columns) {
        switch(Algorithm) {
            case MazeGenerationAlgorithm.RandomTree:
                return new RandomTreeMazeGenerator(Rows, Columns, LenghtSide);
            case MazeGenerationAlgorithm.Division:
                return new DivisionMazeGenerator(Rows, Columns, LenghtSide);
            case MazeGenerationAlgorithm.PureRecursive:
            default:
                return new RecursiveMazeGenerator(Rows, Columns, LenghtSide);
        }
    }

    void Update() {
        GameEvent.Instance.CallUpdateWhenGameIsRunning(() => {
            if(pathDestroyed) {
                return;
            }

           foreach(GameObject cell in pathToGoalsGameObjects) {
                Destroy(cell);
           }

            pathDestroyed = true;
        });
    }

    public Optional<MazeCell> GetMazeCell(int row, int column) {
        try {
            MazeCell cell = mMazeGenerator.GetMazeCell(row, column);
            return Optional<MazeCell>.Of(cell);
        } catch(ArgumentOutOfRangeException) {
            return Optional<MazeCell>.Empty();
        }
    }

}
