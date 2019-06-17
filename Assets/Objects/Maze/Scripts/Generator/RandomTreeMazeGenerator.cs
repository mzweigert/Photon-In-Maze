using UnityEngine;
using System.Collections.Generic;
using PhotonInMaze.Common;

namespace PhotonInMaze.Maze.Generator {
    //<summary>
    //Basic class for Tree generation logic.
    //Subclasses moust override GetCellInRange to implement selecting strategy.
    //</summary>
    internal class RandomTreeMazeGenerator : BasicMazeGenerator {

        CellsToVisit cellsToVisit = new CellsToVisit();

        public RandomTreeMazeGenerator(int rows, int columns, float cellLengthSide) : base(rows, columns, cellLengthSide) {

        }

        public override void GenerateMaze() {
            bool isInRange = false;
            HashSet<Direction> movesAvailable;
            Random.InitState(Random.Range(0, RowCount * ColumnCount));
            int row = Random.Range(0, RowCount);
            Random.InitState(Random.Range(0, RowCount * ColumnCount));
            int column = Random.Range(0, ColumnCount);

            CellToVisit ctv = new CellToVisit(row, column, Direction.Start);
            CellToVisit next;
            cellsToVisit.Add(ctv);

            while(cellsToVisit.IsNotEmpty()) {
                movesAvailable = new HashSet<Direction>();
                ctv = cellsToVisit.FindRandom(RowCount, ColumnCount);

                //check move backward
                isInRange = ctv.Row - 1 >= 0;
                next = new CellToVisit(ctv.Row - 1, ctv.Column, Direction.Back);
                if(isInRange && !visited.Contains(manager.GetMazeCell(ctv.Row - 1, ctv.Column)) && !cellsToVisit.Contains(next)) {
                    movesAvailable.Add(Direction.Back);
                } else if(!visited.Contains(manager.GetMazeCell(ctv.Row, ctv.Column)) && ctv.MoveMade != Direction.Front) {
                    manager.GetMazeCell(ctv.Row, ctv.Column).Walls.Add(Direction.Back);
                    if(isInRange) {
                        manager.GetMazeCell(ctv.Row - 1, ctv.Column).Walls.Add(Direction.Front);
                    }
                }
                //check move forward
                isInRange = ctv.Row + 1 < RowCount;
                bool isExitCell = ctv.Column + 1 == ColumnCount && ctv.Row + 1 == RowCount;
                next = new CellToVisit(ctv.Row + 1, ctv.Column, Direction.Front);
                if(isInRange && !visited.Contains(manager.GetMazeCell(ctv.Row + 1, ctv.Column)) && !cellsToVisit.Contains(next)) {
                    movesAvailable.Add(Direction.Front);
                } else if(!visited.Contains(manager.GetMazeCell(ctv.Row, ctv.Column)) && ctv.MoveMade != Direction.Back) {
                    manager.GetMazeCell(ctv.Row, ctv.Column).Walls.Add(Direction.Front);
                    if(isInRange) {
                        manager.GetMazeCell(ctv.Row + 1, ctv.Column).Walls.Add(Direction.Back);
                    }
                }

                //check move left
                isInRange = ctv.Column - 1 >= 0;
                next = new CellToVisit(ctv.Row, ctv.Column - 1, Direction.Left);
                if(isInRange && !visited.Contains(manager.GetMazeCell(ctv.Row, ctv.Column - 1)) && !cellsToVisit.Contains(next)) {
                    movesAvailable.Add(Direction.Left);
                } else if(!visited.Contains(manager.GetMazeCell(ctv.Row, ctv.Column)) && ctv.MoveMade != Direction.Right) {
                    manager.GetMazeCell(ctv.Row, ctv.Column).Walls.Add(Direction.Left);
                    if(isInRange) {
                        manager.GetMazeCell(ctv.Row, ctv.Column - 1).Walls.Add(Direction.Right);
                    }
                }

                //check move right
                isInRange = ctv.Column + 1 < ColumnCount;
                next = new CellToVisit(ctv.Row, ctv.Column + 1, Direction.Right);
                if(isInRange && !visited.Contains(manager.GetMazeCell(ctv.Row, ctv.Column + 1)) && !cellsToVisit.Contains(next)) {
                    movesAvailable.Add(Direction.Right);
                } else if(!visited.Contains(manager.GetMazeCell(ctv.Row, ctv.Column)) && ctv.MoveMade != Direction.Left) {
                    manager.GetMazeCell(ctv.Row, ctv.Column).Walls.Add(Direction.Right);
                    if(isInRange) {
                        manager.GetMazeCell(ctv.Row, ctv.Column + 1).Walls.Add(Direction.Left);
                    }
                }
   
                visited.Add(manager.GetMazeCell(ctv.Row, ctv.Column));

                if(movesAvailable.Count > 0) {
                    finder.FindNextToVisit(movesAvailable, ctv.Row, ctv.Column)
                        .IfPresent((nextCTV) => cellsToVisit.Add(nextCTV));
                } else {
                    cellsToVisit.Remove(ctv);
                }
            }
        }

    }
}
