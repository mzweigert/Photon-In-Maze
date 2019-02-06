using System;
using System.Collections.Generic;
using UnityEngine;

//<summary>
//Pure recursive maze generation.
//Use carefully for large mazes.
//</summary>
public class RecursiveMazeGenerator : BasicMazeGenerator {

    private bool isInRange = false;

    public RecursiveMazeGenerator(int rows, int columns, float cellLengthSide) : base(rows, columns, cellLengthSide) {

    }

    protected override void GenerateMaze() {
        VisitCell(GetMazeCell(0, 0), Direction.Start);
    }

    private void VisitCell(MazeCell current, Direction moveMade) {
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
            } else if(!current.IsVisited && moveMade != Direction.Back) {
                current.WallFront = true;
                if(isInRange) {
                    GetMazeCell(current.Row + 1, current.Column).WallBack = true;
                }
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
                finder.FindNextToVisit(movesAvailable, current.Row, current.Column).IfPresent((ctv) => {
                    MazeCell next = GetMazeCell(ctv.Row, ctv.Column);
                    VisitCell(next, ctv.MoveMade);
                });
            } 

        } while(movesAvailable.Count > 0);
    }

}
