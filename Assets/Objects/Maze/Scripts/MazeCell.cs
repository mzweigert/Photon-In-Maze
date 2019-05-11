using System;
using System.Collections.Generic;
using System.Linq;
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

    public HashSet<Direction> Walls { get; internal set; } = new HashSet<Direction>(); 
    public bool IsVisited { get; internal set; } = false;
    public bool IsPathToGoal { get; internal set; } = false;
    public bool IsGoal { get; internal set; } = false;
    public bool IsTrap { get; internal set; } = false;

    public int Column { get; }
    public int Row { get; }
    public float X { get; }
    public float Y { get; }

    public MazeCell(int row, int column, float cellLengthSide) {
        this.Row = row;
        this.Column = column;
        this.X = column * cellLengthSide;
        this.Y = row * cellLengthSide;
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

    internal Direction GetDirectionTo(MazeCell next) {
        if(next == null) {
            return Direction.Start;
        } else if(Row < next.Row) {
            return Direction.Right;
        } else if(Row > next.Row) {
            return Direction.Left;
        } else if(Column < next.Column) {
            return Direction.Back;
        } else if(Column > next.Column) {
            return Direction.Front;
        }
        return Direction.Start;
    }

    public Vector2 ToVector2() {
        return new Vector2(X, Y);
    }

    internal HashSet<Direction> GetPossibleMoveDirection() {
        HashSet<Direction> availableMoves = new HashSet<Direction>();
        System.Array allDirections = System.Enum.GetValues(typeof(Direction));
        foreach(Direction direction in allDirections){
            if(!Walls.Contains(direction) && direction != Direction.Start){
                availableMoves.Add(direction);
            }
        }
        return availableMoves;
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
