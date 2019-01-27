using System.Collections.Generic;

//<summary>
//Pure recursive maze generation.
//Use carefully for large mazes.
//</summary>
public class RecursiveMazeGenerator : BasicMazeGenerator {

    private bool isInRange = false;

    public RecursiveMazeGenerator(int rows, int columns) : base(rows, columns) {

    }

    public override void GenerateMaze() {
        VisitCell(GetMazeCell(0, 0), Direction.Start);
    }

    private void VisitCell(MazeCell current, Direction moveMade) {
        List<Direction> movesAvailable;
        List<MazeCell> visitedCells = new List<MazeCell>();

        do {
            movesAvailable = new List<Direction>(4);
            //check move right
            isInRange = current.Column + 1 < ColumnCount;
            if(isInRange && !GetMazeCell(current.Row, current.Column + 1).IsVisited) {
                movesAvailable.Add(Direction.Right);
            } else if(!current.IsVisited && moveMade != Direction.Left) {
                current.WallRight = true;
                if(isInRange) { GetMazeCell(current.Row, current.Column + 1).WallLeft = true; }
            }

            //check move forward
            isInRange = current.Row + 1 < RowCount;
            bool isExitCell = current.Column + 1 == ColumnCount && current.Row + 1 == RowCount;
            if(isInRange && !GetMazeCell(current.Row + 1, current.Column).IsVisited) {
                movesAvailable.Add(Direction.Front);
            } else if(!current.IsVisited && moveMade != Direction.Back && !isExitCell) {
                current.WallFront = true;
                if(isInRange) {
                    GetMazeCell(current.Row + 1, current.Column).WallBack = true;
                }
            } else if(isExitCell) {
                current.IsGoal = true;
            }

            //check move left
            isInRange = current.Column - 1 >= 0;
            if(isInRange && !GetMazeCell(current.Row, current.Column - 1).IsVisited) {
                movesAvailable.Add(Direction.Left);
            } else if(!current.IsVisited && moveMade != Direction.Right) {
                current.WallLeft = true;
                if(isInRange) {
                    GetMazeCell(current.Row, current.Column - 1).WallRight = true;
                }
            }

            //check move backward
            isInRange = current.Row - 1 >= 0;
            if(isInRange && !GetMazeCell(current.Row - 1, current.Column).IsVisited) {
                movesAvailable.Add(Direction.Back);
            } else if(!current.IsVisited && moveMade != Direction.Front) {
                current.WallBack = true;
                if(isInRange) {
                    GetMazeCell(current.Row - 1, current.Column).WallFront = true;
                }
            }

            current.IsVisited = true;
            if(movesAvailable.Count > 0) {
                MakeMove(movesAvailable, current)
                    .ForValuePresented(next => visitedCells.Add(next));
            } else if(!isExitCell && IsATrap(visitedCells, current)) {
                current.IsTrap = true;
            } else if(!isExitCell && visitedCells.Exists(cell => cell.IsPathToGoal || cell.IsGoal)) {
                current.IsPathToGoal = true;
            }


        } while(movesAvailable.Count > 0);
    }

    private Optional<MazeCell> MakeMove(List<Direction> movesAvailable, MazeCell current) {
        int randomCell = UnityEngine.Random.Range(0, movesAvailable.Count);
        MazeCell next = null;
        switch(movesAvailable[randomCell]) {
            case Direction.Start:
                break;
            case Direction.Right:
                next = GetMazeCell(current.Row, current.Column + 1);
                VisitCell(next, Direction.Right);
                break;
            case Direction.Front:
                next = GetMazeCell(current.Row + 1, current.Column);
                VisitCell(next, Direction.Front);
                break;
            case Direction.Left:
                next = GetMazeCell(current.Row, current.Column - 1);
                VisitCell(next, Direction.Left);
                break;
            case Direction.Back:
                next = GetMazeCell(current.Row - 1, current.Column);
                VisitCell(next, Direction.Back);
                break;
        }
        return Optional<MazeCell>.OfNullable(next);
    }

    private bool IsATrap(List<MazeCell> visitedCells, MazeCell currentCell) {
        if(currentCell.Row == 0 && currentCell.Column == 0) {
            return false;
        }
        if(visitedCells.Count == 0) {
            return true;
        }

        return visitedCells.TrueForAll(cell => cell.IsTrap);
    }
}
