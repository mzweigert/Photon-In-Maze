using PhotonInMaze.Common;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Common.Model;
using PhotonInMaze.Maze.Generator;
using PhotonInMaze.Provider;
using System.Collections.Generic;

namespace PhotonInMaze.Maze {
    public class PathToGoalManager : FlowUpdateBehaviour, IPathToGoalManager {

        public OrderedSet<IMazeCell> PathToGoal { get; private set; }

        private NextCellToVisitFinder helper;
        private IMazeCellManager mazeCellManager;

        public override void OnInit() {
            IMazeController mazeController = MazeObjectsProvider.Instance.GetMazeController();
            helper = new NextCellToVisitFinder(mazeController.Rows, mazeController.Columns);
            mazeCellManager = MazeObjectsProvider.Instance.GetMazeCellManager();
            PathToGoal = new OrderedSet<IMazeCell>();
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                .When(State.GeneratePathToGoal)
                .Then(() => {
                    PathToGoal = FindPathToGoal();
                    GameFlowManager.Instance.Flow.NextState();
                })
                .Build();
        }

        public override int GetInitOrder() {
            return InitOrder.PathToGoalManager;
        }

        private OrderedSet<IMazeCell> FindPathToGoal() {
            MazeCell exitCell = mazeCellManager.GetExitCell() as MazeCell;
            exitCell.IsGoal = true;
            exitCell.Walls.Remove(Direction.Front);
            return FindInitialPathToGoal(exitCell, Direction.Start);
        }

        private OrderedSet<IMazeCell> FindInitialPathToGoal(IMazeCell current, Direction moveMade) {
            OrderedSet<IMazeCell> pathToGoal = new OrderedSet<IMazeCell>();
            HashSet<Direction> movesAvailable = current.GetPossibleMovesDirection();
            if(moveMade != Direction.Start) {
                movesAvailable.Remove(GetOposedMove(moveMade));
            } else if(current.IsGoal) {
                movesAvailable.Remove(Direction.Front);
            }
            HashSet<IMazeCell> visitedCells = new HashSet<IMazeCell>();
            while(movesAvailable.Count > 0) {
                helper.FindNextToVisit(movesAvailable, current.Row, current.Column).IfPresent((ctv) => {
                    MazeCell next = (MazeCell)mazeCellManager.GetMazeCell(ctv.Row, ctv.Column);
                    next.IsProperPathToGoal = true;
                    visitedCells.Add(next);
                    movesAvailable.Remove(ctv.MoveMade);
                    OrderedSet<IMazeCell> nextInPath = FindInitialPathToGoal(next, ctv.MoveMade);
                    if(nextInPath.Count > 0) {
                        pathToGoal = nextInPath;
                    }
                });
            }

            if(current.IsGoal || current.IsStartCell() || mazeCellManager.IsPathToGoalVisited(visitedCells)) {
                pathToGoal.AddLast(current);
            } else if(!current.IsGoal && mazeCellManager.IsATrap(visitedCells, current)) {
                (current as MazeCell).IsTrap = true;
                (current as MazeCell).IsProperPathToGoal = false;
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

        public IMazeCell FindPathToGoalFrom(IMazeCell cell) {
            return FindPathToGoalFrom(cell, Direction.Start);
        }

        private IMazeCell FindPathToGoalFrom(IMazeCell cell, Direction moveMade) {
            HashSet<Direction> availableMoves = cell.GetPossibleMovesDirection();

            if(PathToGoal.Contains(cell)) {
                PathToGoal.RemoveAllBackwards(cell);
                return cell;

            } else if(moveMade != Direction.Start) {
                availableMoves.Remove(GetOposedMove(moveMade));
            } else if(cell.IsGoal) {
                availableMoves.Remove(Direction.Front);
            }

            while(availableMoves.Count > 0) {
                NextCell nextCell = FindNextCellToVisit(availableMoves, cell);
                availableMoves.Remove(nextCell.nextMove);

                if(nextCell.value == null) {
                    break;
                }

                IMazeCell found = FindPathToGoalFrom(nextCell.value, nextCell.nextMove);
                if(found != null) {
                    PathToGoal.AddFirst(cell);
                    return cell.IsProperPathToGoal ? cell : found;
                }
            }
            return null;
        }

        private NextCell FindNextCellToVisit(HashSet<Direction> availableMoves, IMazeCell cell) {
            IMazeCell possibleNext = null;
            Direction nextMove = Direction.Start;
            foreach(Direction availableMove in availableMoves) {
                switch(availableMove) {
                    case Direction.Back:
                        possibleNext = mazeCellManager.GetMazeCell(cell.Row - 1, cell.Column);
                        nextMove = Direction.Back;
                        break;
                    case Direction.Left:
                        possibleNext = mazeCellManager.GetMazeCell(cell.Row, cell.Column - 1);
                        nextMove = Direction.Left;
                        break;
                    case Direction.Right:
                        possibleNext = mazeCellManager.GetMazeCell(cell.Row, cell.Column + 1);
                        nextMove = Direction.Right;
                        break;
                    case Direction.Front:
                        possibleNext = mazeCellManager.GetMazeCell(cell.Row + 1, cell.Column);
                        nextMove = Direction.Front;
                        break;
                }
                if(possibleNext != null && possibleNext.IsProperPathToGoal) {
                    break;
                }
            }
            return new NextCell(possibleNext, nextMove);
        }


        public LinkedListNode<IMazeCell> GetFirstFromPath() {
            return PathToGoal.First;
        }

        public int GetPathToGoalSize() {
            return PathToGoal.Count;
        }

        public void RemoveFirst() {
            PathToGoal.RemoveFirst();
        }

        public void AddFirst(IMazeCell value) {
            PathToGoal.AddFirst(value);
        }


        public int IndexInPath(IMazeCell value) {
            return PathToGoal.IndexOf(value);
        }

    }
}