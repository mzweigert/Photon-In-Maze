
using System;
using System.Collections.Generic;
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

    public MazeCell(int row, int column, float cellLengthSide) {
        this.Row = row;
        this.Column = column;
        this.X = column * cellLengthSide;
        this.Y = row * cellLengthSide;
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

    public override bool Equals(object obj) {
        var cell = obj as MazeCell;
        return cell != null &&
               Column == cell.Column &&
               Row == cell.Row;
    }

    public override int GetHashCode() {
        var hashCode = 656739706;
        hashCode = hashCode * -1521134295 + Column.GetHashCode();
        hashCode = hashCode * -1521134295 + Row.GetHashCode();
        return hashCode;
    }

    public bool IsVisited { get; set; } = false;
    public bool WallRight { get; set; } = false;
    public bool WallFront { get; set; } = false;
    public bool WallLeft { get; set; } = false;
    public bool WallBack { get; set; } = false;
    public bool IsPathToGoal { get; set; } = false;
    public bool IsGoal { get; set; } = false;
    public bool IsTrap { get; set; } = false;

    public int Column { get; }
    public int Row { get; }
    public float X { get; }
    public float Y { get; }

    internal List<Direction> GetPossibleMoveDirection() {
        List<Direction> movesAvailable = new List<Direction>(4);
        if(!WallRight) {
            movesAvailable.Add(Direction.Right);
        }
        if(!WallFront) {
            movesAvailable.Add(Direction.Front);
        }
        if(!WallLeft) {
            movesAvailable.Add(Direction.Left);
        }
        if(!WallBack) {
            movesAvailable.Add(Direction.Back);
        }
        return movesAvailable;
    }

    internal bool IsStartCell() {
        return Column == 0 && Row == 0;
    }

    public override string ToString() {
        return string.Format("[MazeCell {0} {1}]", Row, Column);
    }

    public string ToStringAsName() {
        return string.Format("MazeCell_{0}_{1}", Row, Column);
    }

}
