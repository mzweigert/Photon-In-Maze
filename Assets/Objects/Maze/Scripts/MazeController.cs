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

    [Range(1, 50)]
    public int Rows = 5;
    [Range(1, 50)]
    public int Columns = 5;


    public float LenghtOfCellSide { get; } = 4f;
    public float ScaleOfCellSide { get { return LenghtOfCellSide / 4f; } }

    public LinkedList<MazeCell> PathsToGoal { get; private set; }
    private HashSet<GameObject> pathToGoalsGameObjects = new HashSet<GameObject>();
    private bool pathDestroyed = false;
    private GameObject wallPrototype, floorPrototype;
    private BasicMazeGenerator mMazeGenerator = null;

    void Awake() {
        if(!FullRandom) {
            UnityEngine.Random.InitState(RandomSeed);
        }
        wallPrototype = ObjectsManager.Instance.GetWall();
        Vector3 wallScale = wallPrototype.transform.localScale;
        wallScale.x = ScaleOfCellSide;
        wallPrototype.transform.localScale = wallScale;

        floorPrototype = ObjectsManager.Instance.GetFloor();
        Vector3 floorScale = floorPrototype.transform.localScale;
        floorScale.x = LenghtOfCellSide;
        floorScale.z = LenghtOfCellSide;
        floorPrototype.transform.localScale = floorScale;

        mMazeGenerator = GetGenerator(Rows, Columns);
        PathsToGoal = mMazeGenerator.GenerateMazeAndFindPathToGoal();

        CreateMazeCells();
    }

    private void CreateMazeCells() {
        GameObject area = CreateArea();
        byte i = 1;
        for(int row = 0; row < Rows; row++) {
            for(int column = 0; column < Columns; column++) {

                MazeCell cell = mMazeGenerator.GetMazeCell(row, column);
                GameObject cellGameObject = new GameObject() { name = cell.ToStringAsName() };
                cellGameObject.transform.parent = transform;
                cellGameObject.transform.position = new Vector3(cell.X, area.transform.position.y, cell.Y);
                GenerateWalls(cell, cellGameObject);
                GenerateBlackHoles(cell, cellGameObject, ref i);

            }
        }
    }

    private void GenerateBlackHoles(MazeCell cell, GameObject cellGameObject, ref byte counter) {
        if(cell.Walls.Count < 3) {
            return;
        }
        counter++;
        if(!cell.IsStartCell() && !cell.IsGoal && counter == 10) { 
            GameObject blackHolePrototype = ObjectsManager.Instance.GetBlackHole();
            GameObject blackHole = Instantiate(blackHolePrototype, cellGameObject.transform);
            blackHole.name = string.Format("BlackHole_{0}_{1}", cell.Row, cell.Column);
            counter = 1;
        }
    }

    private GameObject CreateArea() {
        GameObject area = new GameObject { name = "Area" };
        area.transform.parent = transform;
        float rowBound = (Rows * LenghtOfCellSide / 2) - LenghtOfCellSide / 2;
        float columnBound = (Columns * LenghtOfCellSide / 2) - LenghtOfCellSide / 2;
        GameObject floor = Instantiate(floorPrototype, new Vector3(columnBound, 0, rowBound), Quaternion.Euler(0, 0, 0));
        floor.transform.localScale = new Vector3(Columns * LenghtOfCellSide, 0.25f, Rows * LenghtOfCellSide);
        floor.name = "Floor";
        floor.transform.parent = area.transform;
        Vector3 wallPos; Quaternion wallQuat; float xLength, offset;
        for(int i = 0; i < 4; i++) {
            bool column = i % 2 == 0;

            if(column) {
                offset = i > 1 ? LenghtOfCellSide : -LenghtOfCellSide;
                wallPos = new Vector3(i * columnBound + offset / 2, area.transform.position.y, rowBound);
                wallQuat = Quaternion.Euler(0, (i - 1) * 90, 0);
                xLength = Rows * ScaleOfCellSide;

            } else {
                offset = i > 2 ? LenghtOfCellSide : -LenghtOfCellSide;
                wallPos = new Vector3(columnBound, area.transform.position.y, (i - 1) * rowBound + offset / 2);
                wallQuat = Quaternion.Euler(0, (i + 1) * 90, 0);
                xLength = Columns * ScaleOfCellSide;
            }

            GameObject wall = Instantiate(wallPrototype, wallPos, wallQuat);
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
            GameObject cellInPath = Instantiate(floorPrototype, pos, Quaternion.Euler(0, 0, 0));
            cellInPath.GetComponent<Renderer>().material.color = Colors.Navy;
            cellInPath.transform.parent = cellGameObject.transform;
            cellInPath.name = "CellInPath";
            pathToGoalsGameObjects.Add(cellInPath);
        }
        if(cell.Walls.Contains(Direction.Right) && cell.Column + 1 < Columns) {
            wall = Instantiate(wallPrototype, new Vector3(cell.X + LenghtOfCellSide / 2, 0, cell.Y) + wallPrototype.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;// right
            wall.name = "WallRight";
            wall.transform.parent = cellGameObject.transform;
        }
        if(cell.Walls.Contains(Direction.Front) && cell.Row + 1 < Rows) {
            wall = Instantiate(wallPrototype, new Vector3(cell.X, 0, cell.Y + LenghtOfCellSide / 2) + wallPrototype.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;// front
            wall.name = "WallFront";
            wall.transform.parent = cellGameObject.transform;
        }
        if(cell.Walls.Contains(Direction.Left) && cell.Column > 0) {
            wall = Instantiate(wallPrototype, new Vector3(cell.X - LenghtOfCellSide / 2, 0, cell.Y) + wallPrototype.transform.position, Quaternion.Euler(0, 270, 0)) as GameObject;// left
            wall.name = "WallLeft";
            wall.transform.parent = cellGameObject.transform;
        }
        if(cell.Walls.Contains(Direction.Back) && cell.Row > 0) {
            wall = Instantiate(wallPrototype, new Vector3(cell.X, 0, cell.Y - LenghtOfCellSide / 2) + wallPrototype.transform.position, Quaternion.Euler(0, 180, 0)) as GameObject;// back
            wall.name = "WallBack";
            wall.transform.parent = cellGameObject.transform;
        }

    }

    private BasicMazeGenerator GetGenerator(int rows, int columns) {
        switch(Algorithm) {
            case MazeGenerationAlgorithm.RandomTree:
                return new RandomTreeMazeGenerator(Rows, Columns, LenghtOfCellSide);
            case MazeGenerationAlgorithm.Division:
                return new DivisionMazeGenerator(Rows, Columns, LenghtOfCellSide);
            case MazeGenerationAlgorithm.PureRecursive:
            default:
                return new RecursiveMazeGenerator(Rows, Columns, LenghtOfCellSide);
        }
    }

    void Update() {
        GameFlow.Instance.CallUpdateWhenGameIsRunning(() => {
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
