using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//<summary>
//Pure recursive maze generation.
//Use carefully for large mazes.
//</summary>
public class RecursiveMazeGenerator : BasicMazeGenerator {

    private int exitCol;

    public RecursiveMazeGenerator(int rows, int columns) : base(rows, columns) {
        this.exitCol = UnityEngine.Random.Range(0, columns);
    }

    public override void GenerateMaze() {
        VisitCell(0, 0, Direction.Start);
    }

    private void VisitCell(int row, int column, Direction moveMade) {
        Direction[] movesAvailable = new Direction[4];
        int movesAvailableCount = 0;
        List<MazeCell> visitedCells = new List<MazeCell>();
        do {
            movesAvailableCount = 0;

            //check move right
            if(column + 1 < ColumnCount && !GetMazeCell(row, column + 1).IsVisited) {
                movesAvailable[movesAvailableCount] = Direction.Right;
                movesAvailableCount++;
            } else if(!GetMazeCell(row, column).IsVisited && moveMade != Direction.Left) {
                GetMazeCell(row, column).WallRight = true;
            }

            bool isExitCell = column + 1 == ColumnCount && row + 1 == RowCount;
            //check move forward
            if(row + 1 < RowCount && !GetMazeCell(row + 1, column).IsVisited) {
                movesAvailable[movesAvailableCount] = Direction.Front;
                movesAvailableCount++;
            } else if(!GetMazeCell(row, column).IsVisited && moveMade != Direction.Back && !isExitCell) {
                GetMazeCell(row, column).WallFront = true;
            } else if(isExitCell) {
                GetMazeCell(row, column).IsGoal = true;
            }
            //check move left
            if(column > 0 && column - 1 >= 0 && !GetMazeCell(row, column - 1).IsVisited) {
                movesAvailable[movesAvailableCount] = Direction.Left;
                movesAvailableCount++;
            } else if(!GetMazeCell(row, column).IsVisited && moveMade != Direction.Right) {
                GetMazeCell(row, column).WallLeft = true;
            }
            //check move backward
            if(row > 0 && row - 1 >= 0 && !GetMazeCell(row - 1, column).IsVisited) {
                movesAvailable[movesAvailableCount] = Direction.Back;
                movesAvailableCount++;
            } else if(!GetMazeCell(row, column).IsVisited && moveMade != Direction.Front) {
                GetMazeCell(row, column).WallBack = true;
            }

            GetMazeCell(row, column).IsVisited = true;

            if(movesAvailableCount > 0) {
                switch(movesAvailable[UnityEngine.Random.Range(0, movesAvailableCount)]) {
                    case Direction.Start:
                        break;
                    case Direction.Right:
                        visitedCells.Add(GetMazeCell(row, column + 1));
                        VisitCell(row, column + 1, Direction.Right);
                        break;
                    case Direction.Front:
                        visitedCells.Add(GetMazeCell(row + 1, column));
                        VisitCell(row + 1, column, Direction.Front);
                        break;
                    case Direction.Left:
                        visitedCells.Add(GetMazeCell(row, column - 1));
                        VisitCell(row, column - 1, Direction.Left);
                        break;
                    case Direction.Back:
                        visitedCells.Add(GetMazeCell(row - 1, column));
                        VisitCell(row - 1, column, Direction.Back);
                        break;
                }
            } else if(!isExitCell && IsATrap(visitedCells, row, column)) {
                GetMazeCell(row, column).IsTrap = true;
            } else if(!isExitCell && HasNeighbourWithPathToGoal(visitedCells, row, column)) {
                GetMazeCell(row, column).IsPathToGoal = true;
            }

        } while(movesAvailableCount > 0);
    }

    private bool IsATrap(List<MazeCell> visitedCells, int row, int column) {
        if(row == 0 && column == 0) {
            return false;
        }
        if(visitedCells.Count == 0) {
            return true;
        }
        bool allVisitedIstrap = true;
        foreach(MazeCell cell in visitedCells) {
            allVisitedIstrap = allVisitedIstrap && cell.IsTrap;
        }

        return allVisitedIstrap;
    }

    private bool HasNeighbourWithPathToGoal(List<MazeCell> visitedCells, int row, int column) {
        bool hasNeighbourWithPathToGoal = false;
        foreach(MazeCell cell in visitedCells) {
            hasNeighbourWithPathToGoal = hasNeighbourWithPathToGoal || cell.IsPathToGoal || cell.IsGoal;
        }

        return hasNeighbourWithPathToGoal;
    }
}
