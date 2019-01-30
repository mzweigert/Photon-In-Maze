using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>
public class MazeController : MonoBehaviour {

	public bool FullRandom = false;
	public int RandomSeed = 12345;
	public GameObject Floor = null;
	public GameObject Wall = null;
	//public GameObject Pillar = null;
	public int Rows = 5;
	public int Columns = 5;
    public float LenghtSide { get { return 4f; } }
	public bool AddGaps = true;
    public List<GameObject> PathsToGoal { get; private set; }

	private BasicMazeGenerator mMazeGenerator = null;

	void Start () {
        PathsToGoal = new List<GameObject>();
		if (!FullRandom) {
            UnityEngine.Random.InitState(RandomSeed);
		}
        mMazeGenerator = new RecursiveMazeGenerator(Rows, Columns);
        mMazeGenerator.GenerateMaze();
		for (int row = 0; row < Rows; row++) {
			for(int column = 0; column < Columns; column++){
				float x = column*(LenghtSide + (AddGaps?.2f:0));
				float z = row*(LenghtSide + (AddGaps?.2f:0));
				MazeCell cell = mMazeGenerator.GetMazeCell(row,column);
				GameObject tmp;
				tmp = Instantiate(Floor,new Vector3(x,0,z), Quaternion.Euler(0,0,0)) as GameObject;
				tmp.transform.parent = transform;
                if(cell.IsPathToGoal || cell.IsGoal) {
                    tmp.GetComponent<Renderer>().material.color = Values.COLOR.Navy;
                    PathsToGoal.Add(tmp);
                }
				if(cell.WallRight){
					tmp = Instantiate(Wall,new Vector3(x+ LenghtSide / 2,0,z)+Wall.transform.position,Quaternion.Euler(0,90,0)) as GameObject;// right
					tmp.transform.parent = transform;
				}
				if(cell.WallFront){
					tmp = Instantiate(Wall,new Vector3(x,0,z+ LenghtSide / 2)+Wall.transform.position,Quaternion.Euler(0,0,0)) as GameObject;// front
					tmp.transform.parent = transform;
				}
				if(cell.WallLeft){
					tmp = Instantiate(Wall,new Vector3(x- LenghtSide / 2,0,z)+Wall.transform.position,Quaternion.Euler(0,270,0)) as GameObject;// left
					tmp.transform.parent = transform;
				}
				if(cell.WallBack){
					tmp = Instantiate(Wall,new Vector3(x,0,z- LenghtSide / 2)+Wall.transform.position,Quaternion.Euler(0,180,0)) as GameObject;// back
					tmp.transform.parent = transform;
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

    void Update() {
        Utils.CheckIfGameRunningAndCallUpdate(() => {

            foreach(GameObject cell in PathsToGoal) {
                Material material = cell.GetComponent<Renderer>().material;
                material.color = Color.Lerp(material.color, Values.COLOR.Ocean, 0.05f);
            }
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
