using System;
using System.Collections.Generic;
using PhotonInMaze.Common;
using PhotonInMaze.Common.Flow;

namespace PhotonInMaze.Game.Maze {
    public partial class MazeController : FlowUpdateBehaviour {

        public OrderedSet<MazeCell> PathsToGoal { get; private set; }
        protected MazeCellFinder finder;

        public Optional<MazeCell> FindMazeCell(int row, int column) {
            try {
                MazeCell cell = mazeGenerator.GetMazeCell(row, column);
                return Optional<MazeCell>.Of(cell);
            } catch(ArgumentOutOfRangeException) {
                return Optional<MazeCell>.Empty();
            }
        }

        public LinkedListNode<MazeCell> FindCellFromPathToGoalNear(MazeCell cell, Direction moveMade = Direction.Start) {
            HashSet<Direction> availableMoves = cell.GetPossibleMovesDirection();
            if(moveMade != Direction.Start) {
                availableMoves.Remove(GetOposedMove(moveMade));
            } else if(cell.IsGoal) {
                availableMoves.Remove(Direction.Front);
            }
            while(availableMoves.Count > 0) {
                Optional<CellToVisit> possibleNext = finder.FindNextToVisit(availableMoves, cell.Row, cell.Column);
                if(possibleNext.HasNotValue) {
                    break;
                }
                CellToVisit next = possibleNext.Get();
                availableMoves.Remove(next.MoveMade);
                MazeCell nextMazeCell = FindMazeCell(next.Row, next.Column).Get();
                if(PathsToGoal.Contains(nextMazeCell)) {
                    LinkedListNode<MazeCell> cellInPathToGoal = PathsToGoal.Find(nextMazeCell);
                    return cellInPathToGoal;
                }
                LinkedListNode<MazeCell> possibleCellInPathToGoal = FindCellFromPathToGoalNear(nextMazeCell, next.MoveMade);
                if(possibleCellInPathToGoal != null) {
                    return possibleCellInPathToGoal;
                }
            }
            return null;
        }

        protected OrderedSet<MazeCell> FindPathToGoal() {
            MazeCell exitCell = FindMazeCell(Rows - 1, Columns - 1).Get();
            exitCell.IsGoal = true;
            exitCell.Walls.Remove(Direction.Front);
            OrderedSet<MazeCell> path = FindInitialPathToGoal(exitCell, Direction.Start);
            return path;
        }

        private OrderedSet<MazeCell> FindInitialPathToGoal(MazeCell current, Direction moveMade) {
            OrderedSet<MazeCell> pathToGoal = new OrderedSet<MazeCell>();
            HashSet<Direction> movesAvailable = current.GetPossibleMovesDirection();
            if(moveMade != Direction.Start) {
                movesAvailable.Remove(GetOposedMove(moveMade));
            } else if(current.IsGoal) {
                movesAvailable.Remove(Direction.Front);
            }
            HashSet<MazeCell> visitedCells = new HashSet<MazeCell>();
            while(movesAvailable.Count > 0) {
                finder.FindNextToVisit(movesAvailable, current.Row, current.Column).IfPresent((ctv) => {
                    MazeCell next = FindMazeCell(ctv.Row, ctv.Column).Get();
                    next.IsPathToGoal = true;
                    visitedCells.Add(next);
                    movesAvailable.Remove(ctv.MoveMade);
                    OrderedSet<MazeCell> pathInNext = FindInitialPathToGoal(next, ctv.MoveMade);
                    if(pathInNext.Count > 0) {
                        pathToGoal = pathInNext;
                    }
                });
            }

            if(current.IsGoal || current.IsStartCell() || finder.IsPathToGoalVisited(visitedCells)) {
                pathToGoal.Add(current);
            } else if(!current.IsGoal && finder.IsATrap(visitedCells, current)) {
                current.IsTrap = true;
                current.IsPathToGoal = false;
            }
            return pathToGoal;
        }

        private Direction GetOposedMove(Direction moveMade) {
            switch(moveMade) {
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                case Direction.Front:
                    return Direction.Back;
                case Direction.Back:
                    return Direction.Front;
            }
            return Direction.Start;
        }
    }
}