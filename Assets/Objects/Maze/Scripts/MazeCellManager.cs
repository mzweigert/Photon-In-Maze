using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Common.Model;
using PhotonInMaze.Maze.Generator;
using PhotonInMaze.Provider;
using System.Collections.Generic;
using UnityEngine;

namespace PhotonInMaze.Maze {
    public class MazeCellManager : FlowUpdateBehaviour, IMazeCellManager {

        [SerializeField]
        private MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;

        private IMazeCell[,] maze;
        private int rows, columns;
        float cellLengthSide;
        private BasicMazeGenerator generator = null;

        public override void OnInit() {
            IMazeController mazeController = MazeObjectsProvider.Instance.GetMazeController();
            rows = mazeController.Rows;
            columns = mazeController.Columns;
            cellLengthSide = mazeController.LenghtOfCellSide;

            maze = new MazeCell[rows, columns];
            for(int row = 0; row < rows; row++) {
                for(int column = 0; column < columns; column++) {
                    MazeCell cell = new MazeCell(row, column, cellLengthSide);
                    maze[row, column] = cell;
                }
            }
            generator = InitGenerator(rows, columns);
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                .When(State.GenerateMaze)
                .Then(() => {
                    generator.GenerateMaze();
                    GameFlowManager.Instance.Flow.NextState();
                })
                .Build();
        }

        public override int GetInitOrder() {
            return InitOrder.MazeCellManager;
        }

        private BasicMazeGenerator InitGenerator(int rows, int columns) {
            switch(Algorithm) {
                case MazeGenerationAlgorithm.RandomTree:
                    return new RandomTreeMazeGenerator(rows, columns, cellLengthSide);
                case MazeGenerationAlgorithm.Division:
                    return new DivisionMazeGenerator(rows, columns, cellLengthSide);
                case MazeGenerationAlgorithm.PureRecursive:
                default:
                    return new RecursiveMazeGenerator(rows, columns, cellLengthSide);
            }
        }

        public bool IsATrap(HashSet<IMazeCell> visitedCells, IMazeCell currentCell) {
            if(currentCell.Row == 0 && currentCell.Column == 0) {
                return false;
            }
            if(visitedCells.Count == 0) {
                return true;
            }
            bool allVisitedIsTrap = true;
            HashSet<IMazeCell>.Enumerator enumerator = visitedCells.GetEnumerator();
            while(enumerator.MoveNext()) {
                allVisitedIsTrap = allVisitedIsTrap && enumerator.Current.IsTrap;
            }
            return allVisitedIsTrap;
        }

        public bool IsPathToGoalVisited(HashSet<IMazeCell> visitedCells) {
            HashSet<IMazeCell>.Enumerator enumerator = visitedCells.GetEnumerator();
            while(enumerator.MoveNext()) {
                if(enumerator.Current.IsProperPathToGoal || enumerator.Current.IsGoal) {
                    return true;
                }
            }
            return false;
        }

        public IMazeCell GetExitCell() {
            return GetMazeCell(rows - 1, columns - 1);
        }

        public IMazeCell GetStartCell() {
            return GetMazeCell(0, 0);
        }

        public IMazeCell GetMazeCell(int row, int column) {
            if(row >= 0 && column >= 0 && row < rows && column < columns) {
                return maze[row, column];
            } else {
                Debug.Log(row + " " + column);
                throw new System.ArgumentOutOfRangeException();
            }
        }
      
    }

}