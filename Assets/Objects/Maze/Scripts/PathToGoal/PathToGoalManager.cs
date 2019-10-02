using PhotonInMaze.Common;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Common.Model;
using PhotonInMaze.Maze.Generator;
using PhotonInMaze.Provider;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PhotonInMaze.Maze {
    internal partial class PathToGoalManager : FlowUpdateBehaviour, IPathToGoalManager {

        public OrderedSet<IMazeCell> PathToGoal { get; private set; }

        public override void OnInit() {
            IMazeConfiguration configuration = MazeObjectsProvider.Instance.GetMazeConfiguration();
            helper = new NextCellToVisitFinder(configuration.Rows, configuration.Columns);
            mazeCellManager = MazeObjectsProvider.Instance.GetMazeCellManager();
            PathToGoal = new OrderedSet<IMazeCell>();
            pathToGoalFloors = new Dictionary<IMazeCell, GameObject>();
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                .When(State.GeneratePathToGoal)
                .Then(() => {
                    PathToGoal = FindPathToGoal();
                    GameFlowManager.Instance.Flow.NextState();
                })
                .OrElseWhen(State.PaintPathToGoal)
                .Then(PaintPathToGoal)
                .OrElseWhen(State.HidePathToGoal)
                .Then((Action)(() => {
                    foreach(GameObject cell in pathToGoalFloors.Values) {
                        Destroy(cell);
                    }
                    GameFlowManager.Instance.Flow.NextState();
                }))
                .Build();
        }

 
        public override int GetInitOrder() {
            return (int)InitOrder.PathToGoalManager;
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