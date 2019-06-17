using PhotonInMaze.Common;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Provider;
using UnityEngine;

//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>

namespace PhotonInMaze.Maze {

    internal partial class MazeController : FlowUpdateBehaviour, IMazeController {

        private GameObject wallPrototype, floorPrototype;

        private IMazeGenerator generator = null;

        private IMazeConfiguration configuration;
        private PathToGoalManager pathToGoalManager;

        public override void OnInit() {
            configuration = MazeObjectsProvider.Instance.GetMazeConfiguration();
            pathToGoalManager = MazeObjectsProvider.Instance.GetPathToGoalManager() as PathToGoalManager;
            wallPrototype = MazeObjectsProvider.Instance.GetWall();
            Vector3 wallScale = wallPrototype.transform.localScale;
            wallScale.x = 1f;
            wallPrototype.transform.localScale = wallScale;

            floorPrototype = MazeObjectsProvider.Instance.GetFloor();
            Vector3 floorScale = floorPrototype.transform.localScale;
            floorScale.x = floorScale.z = configuration.LenghtOfCellSide;
            floorPrototype.transform.localScale = floorScale;
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                .When(State.CreateMaze)
                .Then(() => {
                    CreateMazeWithItems();
                    GameFlowManager.Instance.Flow.NextState();
                })
                .OrElseWhen(State.GenerateMaze)
                .Then(() => {
                    generator = configuration.GetGenerator();
                    generator.GenerateMaze();
                    GameFlowManager.Instance.Flow.NextState();
                })
                .OrElseWhen(State.MazeCreated)
                .Then(() => StartCoroutine(GameFlowManager.Instance.Flow.ChangeStateAfterFrameEnd()))
                .OrElseWhen(State.EndGame)
                .Then(NextStage)
                .Build();
        }

        public override int GetInitOrder() {
            return InitOrder.Maze;
        }

        private void NextStage() {
            if(GameFlowManager.Instance.Flow.Is(State.EndGame)) {
                (configuration as MazeConfiguration).IncraseValues();
                CanvasObjectsProvider.Instance
                    .GetArrowButtonController()
                    .ReinitializeArrowHintsCount();
                GameFlowManager.Instance.ReinitializeFlowBehaviours();
            } else {
                Debug.LogWarning("Cannot set next stage. Game should has state: " + State.EndGame);
            }
        }

        public void Recreate(int rows, int columns, MazeGenerationAlgorithm algorithm) {
            (configuration as MazeConfiguration).SetValues(rows - 1, columns - 1, algorithm, 0);
            while(!GameFlowManager.Instance.Flow.Is(State.HidePhoton)) {
                GameFlowManager.Instance.Flow.NextState();
            }
        }
    }
}