
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

    public MazeCell(int row, int column) {
        this.Row = row;
        this.Column = column;
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

    public bool IsVisited = false;
    public bool WallRight = false;
    public bool WallFront = false;
    public bool WallLeft = false;
    public bool WallBack = false;
    public bool IsPathToGoal = false;
    public bool IsGoal = false;
    public bool IsTrap = false;

    public int Column { get; }

    public int Row { get; }
    public static MazeCell StartCell { get {
            return new MazeCell(0, 0);
        }
    }
}
