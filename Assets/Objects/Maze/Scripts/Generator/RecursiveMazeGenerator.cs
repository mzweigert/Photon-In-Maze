using PhotonInMaze.Common;
using PhotonInMaze.Common.Model;
using System.Collections.Generic;
using UnityEngine;

namespace PhotonInMaze.Maze.Generator {
    //<summary>
    //Pure recursive maze generation.
    //Use carefully for large mazes.
    //</summary>
    internal class RecursiveMazeGenerator : BasicMazeGenerator {

        private bool isInRange = false;

        public RecursiveMazeGenerator(int rows, int columns, float cellLengthSide) : base(rows, columns, cellLengthSide) {

        }

        public override void GenerateMaze() {
            Random.InitState(Random.Range(0, RowCount * ColumnCount));
            int row = Random.Range(0, RowCount);
            Random.InitState(Random.Range(0, RowCount * ColumnCount));
            int column = Random.Range(0, ColumnCount);
            IMazeCell randomCell = manager.GetMazeCell(row, column);
            VisitCell(randomCell, Direction.Start);
        }

        private void VisitCell(IMazeCell currentCell, Direction moveMade) {
            LinkedList<MazeCell> pathToGoal = new LinkedList<MazeCell>();
            HashSet<Direction> movesAvailable;
            HashSet<MazeCell> visitedCells = new HashSet<MazeCell>();

            movesAvailable = new HashSet<Direction>();

            do {
                MazeCell current = currentCell as MazeCell;
                //check move backward
                isInRange = current.Row - 1 >= 0;
                if(isInRange && !visited.Contains(manager.GetMazeCell(current.Row - 1, current.Column))) {
                    movesAvailable.Add(Direction.Back);
                } else if(!visited.Contains(current) && moveMade != Direction.Front) {
                    current.Walls.Add(Direction.Back);
                    if(isInRange) {
                        manager.GetMazeCell(current.Row - 1, current.Column).Walls.Add(Direction.Front);
                    }
                }

                //check move forward
                isInRange = current.Row + 1 < RowCount;
                if(isInRange && !visited.Contains(manager.GetMazeCell(current.Row + 1, current.Column))) {
                    movesAvailable.Add(Direction.Front);
                } else if(!visited.Contains(current) && moveMade != Direction.Back) {
                    current.Walls.Add(Direction.Front);
                    if(isInRange) {
                        manager.GetMazeCell(current.Row + 1, current.Column).Walls.Add(Direction.Back);
                    }
                }

                //check move left
                isInRange = current.Column - 1 >= 0;
                if(isInRange && !visited.Contains(manager.GetMazeCell(current.Row, current.Column - 1))) {
                    movesAvailable.Add(Direction.Left);
                } else if(!visited.Contains(current) && moveMade != Direction.Right) {
                    current.Walls.Add(Direction.Left);
                    if(isInRange) {
                        manager.GetMazeCell(current.Row, current.Column - 1).Walls.Add(Direction.Right);
                    }
                }

                //check move right
                isInRange = current.Column + 1 < ColumnCount;
                if(isInRange && !visited.Contains(manager.GetMazeCell(current.Row, current.Column + 1))) {
                    movesAvailable.Add(Direction.Right);
                } else if(!visited.Contains(current) && moveMade != Direction.Left) {
                    current.Walls.Add(Direction.Right);
                    if(isInRange) { manager.GetMazeCell(current.Row, current.Column + 1).Walls.Add(Direction.Left); }
                }

                visited.Add(current);
                if(movesAvailable.Count > 0) {
                    finder.FindNextToVisit(movesAvailable, current.Row, current.Column).IfPresent((ctv) => {
                        IMazeCell next = manager.GetMazeCell(ctv.Row, ctv.Column);
                        VisitCell(next, ctv.MoveMade);
                        movesAvailable.Remove(ctv.MoveMade);
                    });
                }

            } while(movesAvailable.Count > 0);
        }

    }
}