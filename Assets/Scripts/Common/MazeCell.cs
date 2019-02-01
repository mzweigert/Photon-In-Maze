
using System;
using UnityEngine;

public enum Direction {
    Start,
    Right,
    Front,
    Left,
    Back,
};
//<summary>
//Class for representing concrete maze cell.
//</summary>
public class MazeCell {

    public MazeCell(int row, int column, Func<MazeCell, GameObject> createRealObjFunction) {
        this.Row = row;
        this.Column = column;
        this.RealObject = createRealObjFunction.Invoke(this);
    }

    public void SetWall(Direction direction) {
        switch(direction) {
            case Direction.Right:
                WallRight = true;
                break;
            case Direction.Front:
                WallFront = true;
                break;
            case Direction.Left:
                WallLeft = true;
                break;
            case Direction.Back:
                WallBack = true;
                break;
        }
    }


    public bool IsVisited { get; set; } = false;
    public bool WallRight { get; set; } = false;
    public bool WallFront { get; set; } = false;
    public bool WallLeft { get; set; } = false;
    public bool WallBack { get; set; } = false;
    public bool IsPathToGoal { get; set; } = false;
    public bool IsGoal { get; set; } = false;
    public bool IsTrap { get; set; } = false;

    public GameObject RealObject { get; private set; }
    public Vector3 RealObjectPosition { get { return RealObject != null ? RealObject.transform.position : Vector3.zero; } }
    public int Column { get; }
    public int Row { get; }
}
