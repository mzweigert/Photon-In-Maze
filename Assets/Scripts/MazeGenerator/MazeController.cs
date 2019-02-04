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
    private bool pathColorChanged = false;

    private BasicMazeGenerator mMazeGenerator = null;

	void Start () {
		if (!FullRandom) {
            UnityEngine.Random.InitState(RandomSeed);
		}
        Func<MazeCell, GameObject> createRealObject = (MazeCell cell) => {
            GameObject floor = Instantiate(
            Floor,
            new Vector3(cell.Column * LenghtSide, 0, cell.Row * LenghtSide),
            Quaternion.Euler(0, 0, 0));
            floor.name = "Floor";
            return floor;
        };
        mMazeGenerator = GetGenerator(Rows, Columns, createRealObject);
        PathsToGoal = mMazeGenerator.GenerateMazeAndFindPathToGoal();
		for (int row = 0; row < Rows; row++) {
			for(int column = 0; column < Columns; column++){
				float x = column * LenghtSide;
				float z = row * LenghtSide;
				MazeCell cell = mMazeGenerator.GetMazeCell(row,column);
				GameObject tmp = cell.RealObject;
                GameObject cellGameObject = new GameObject();
                cellGameObject.name = cell.ToStringAsName();
                cellGameObject.transform.parent = transform;
                tmp.transform.parent = cellGameObject.transform;
                if(cell.IsPathToGoal || cell.IsGoal) {
                    tmp.GetComponent<Renderer>().material.color = Colors.Navy;
                }
				if(cell.WallRight){
					tmp = Instantiate(Wall,new Vector3(x+ LenghtSide / 2,0,z)+Wall.transform.position,Quaternion.Euler(0,90,0)) as GameObject;// right
                    tmp.name = "WallRight";
                    tmp.transform.parent = cellGameObject.transform;
                }
				if(cell.WallFront){
					tmp = Instantiate(Wall,new Vector3(x,0,z+ LenghtSide / 2)+Wall.transform.position,Quaternion.Euler(0,0,0)) as GameObject;// front
                    tmp.name = "WallFront";
                    tmp.transform.parent = cellGameObject.transform;
                }
				if(cell.WallLeft){
					tmp = Instantiate(Wall,new Vector3(x- LenghtSide / 2,0,z)+Wall.transform.position,Quaternion.Euler(0,270,0)) as GameObject;// left
                    tmp.name = "WallLeft";
                    tmp.transform.parent = cellGameObject.transform;
                }
				if(cell.WallBack){
					tmp = Instantiate(Wall,new Vector3(x,0,z- LenghtSide / 2)+Wall.transform.position,Quaternion.Euler(0,180,0)) as GameObject;// back
                    tmp.name = "WallBack";
                    tmp.transform.parent = cellGameObject.transform;
                }
                
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

    private BasicMazeGenerator GetGenerator(int rows, int columns, Func<MazeCell, GameObject> createRealObject) {
        switch(Algorithm) {
            case MazeGenerationAlgorithm.RandomTree:
                return new RandomTreeMazeGenerator(Rows, Columns, createRealObject);
            case MazeGenerationAlgorithm.Division:
                return new DivisionMazeGenerator(Rows, Columns, createRealObject);
            case MazeGenerationAlgorithm.PureRecursive:
            default:
                return new RecursiveMazeGenerator(Rows, Columns, createRealObject);
        }
    }

    void Update() {
        GameEvent.Instance.CallUpdateWhenGameIsRunning(() => {
            if(pathColorChanged) {
                return;
            }

            foreach(MazeCell cell in PathsToGoal) {
                Material material = cell.RealObject.GetComponent<Renderer>().material;
                material.color = Colors.Ocean;
            }
            pathColorChanged = true;
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
