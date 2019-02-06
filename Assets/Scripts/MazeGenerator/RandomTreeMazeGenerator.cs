using UnityEngine;
using System.Collections.Generic;

//<summary>
//Basic class for Tree generation logic.
//Subclasses moust override GetCellInRange to implement selecting strategy.
//</summary>
public class RandomTreeMazeGenerator : BasicMazeGenerator {

    CellsToVisit cellsToVisit;

    public RandomTreeMazeGenerator(int rows, int columns, float cellLengthSide) : base(rows, columns, cellLengthSide) {
        cellsToVisit = new CellsToVisit();
    }

    protected override void GenerateMaze() {
        bool isInRange = false;
        List<Direction> movesAvailable;
        CellToVisit ctv = new CellToVisit(Random.Range(0, RowCount), Random.Range(0, ColumnCount), Direction.Start);
        CellToVisit next;
        cellsToVisit.Add(ctv);

        while(cellsToVisit.IsNotEmpty()) {
            movesAvailable = new List<Direction>(4);
            ctv = cellsToVisit.FindRandom();

            //check move right
            isInRange = ctv.Column + 1 < ColumnCount;
            next = new CellToVisit(ctv.Row, ctv.Column + 1, Direction.Right);
            if(isInRange && !GetMazeCell(ctv.Row, ctv.Column + 1).IsVisited && !cellsToVisit.Contains(next)) {
                movesAvailable.Add(Direction.Right);
            } else if(!GetMazeCell(ctv.Row, ctv.Column).IsVisited && ctv.MoveMade != Direction.Left) {
                GetMazeCell(ctv.Row, ctv.Column).WallRight = true;
                if(isInRange) {
                    GetMazeCell(ctv.Row, ctv.Column + 1).WallLeft = true;
                }
            }
            //check move forward
            isInRange = ctv.Row + 1 < RowCount;
            bool isExitCell = ctv.Column + 1 == ColumnCount && ctv.Row + 1 == RowCount;
            next = new CellToVisit(ctv.Row + 1, ctv.Column, Direction.Front);
            if(isInRange && !GetMazeCell(ctv.Row + 1, ctv.Column).IsVisited && !cellsToVisit.Contains(next)) {
                movesAvailable.Add(Direction.Front);
            } else if(!GetMazeCell(ctv.Row, ctv.Column).IsVisited && ctv.MoveMade != Direction.Back) {
                GetMazeCell(ctv.Row, ctv.Column).WallFront = true;
                if(isInRange) {
                    GetMazeCell(ctv.Row + 1, ctv.Column).WallBack = true;
                }
            }

            //check move left
            isInRange = ctv.Column - 1 >= 0;
            next = new CellToVisit(ctv.Row, ctv.Column - 1, Direction.Left);
            if(isInRange && !GetMazeCell(ctv.Row, ctv.Column - 1).IsVisited && !cellsToVisit.Contains(next)) {
                movesAvailable.Add(Direction.Left);
            } else if(!GetMazeCell(ctv.Row, ctv.Column).IsVisited && ctv.MoveMade != Direction.Right) {
                GetMazeCell(ctv.Row, ctv.Column).WallLeft = true;
                if(isInRange) {
                    GetMazeCell(ctv.Row, ctv.Column - 1).WallRight = true;
                }
            }
            //check move backward
            isInRange = ctv.Row - 1 >= 0;
            next = new CellToVisit(ctv.Row - 1, ctv.Column, Direction.Back);
            if(isInRange && !GetMazeCell(ctv.Row - 1, ctv.Column).IsVisited && !cellsToVisit.Contains(next)) {
                movesAvailable.Add(Direction.Back);
            } else if(!GetMazeCell(ctv.Row, ctv.Column).IsVisited && ctv.MoveMade != Direction.Front) {
                GetMazeCell(ctv.Row, ctv.Column).WallBack = true;
                if(isInRange) {
                    GetMazeCell(ctv.Row - 1, ctv.Column).WallFront = true;
                }
            }

            GetMazeCell(ctv.Row, ctv.Column).IsVisited = true;

            if(movesAvailable.Count > 0) {
                finder.FindNextToVisit(movesAvailable, ctv.Row, ctv.Column)
                    .IfPresent((nextCTV) => cellsToVisit.Add(nextCTV));
            } else {
                cellsToVisit.Remove(ctv);
            }
        }
    }

   
}
