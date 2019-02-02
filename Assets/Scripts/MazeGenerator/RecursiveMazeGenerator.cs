using System;
using System.Collections.Generic;
using UnityEngine;

//<summary>
//Pure recursive maze generation.
//Use carefully for large mazes.
//</summary>
public class RecursiveMazeGenerator : BasicMazeGenerator {

    private bool isInRange = false;

    public RecursiveMazeGenerator(int rows, int columns, Func<MazeCell, GameObject> createRealObjFunction) : base(rows, columns, createRealObjFunction) {

    }

    public override LinkedList<MazeCell> GenerateMaze() {
        LinkedList<MazeCell> path = VisitCell(GetMazeCell(0, 0), Direction.Start);
        return path;
    }

    private LinkedList<MazeCell> VisitCell(MazeCell current, Direction moveMade) {
        LinkedList<MazeCell> pathToGoal = new LinkedList<MazeCell>();
        List<Direction> movesAvailable;
        HashSet<MazeCell> visitedCells = new HashSet<MazeCell>();

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
            if(isInRange && !GetMazeCell(current.Row + 1, current.Column).IsVisited) {
                movesAvailable.Add(Direction.Front);
            } else if(!current.IsVisited && moveMade != Direction.Back && current.IsNotExitCell(RowCount, ColumnCount)) {
                current.WallFront = true;
                if(isInRange) {
                    GetMazeCell(current.Row + 1, current.Column).WallBack = true;
                }
            } else if(current.IsExitCell(RowCount, ColumnCount)) {
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
                FindNextToVisit(movesAvailable, current.Row, current.Column).IfPresent((ctv) => {
                    MazeCell next = GetMazeCell(ctv.Row, ctv.Column);
                    visitedCells.Add(next);
                    LinkedList<MazeCell> pathInNext = VisitCell(next, ctv.MoveMade);
                    if(pathInNext.Count > 0) {
                        pathToGoal = pathInNext;
                    }
                });
            } else if(current.IsNotExitCell(RowCount, ColumnCount) && IsATrap(visitedCells, current)) {
                current.IsTrap = true;
            } else if(current.IsExitCell(RowCount, ColumnCount) || IsPathToGoalVisited(visitedCells)) {
                pathToGoal.AddFirst(current);
                current.IsPathToGoal = true;
            }

        } while(movesAvailable.Count > 0);

        return pathToGoal;
    }

}
