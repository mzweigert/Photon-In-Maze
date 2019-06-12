using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Provider;
using UnityEngine;

//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>

namespace PhotonInMaze.Maze {
    public enum MazeGenerationAlgorithm {
        PureRecursive,
        RandomTree,
        Division
    }

    public partial class MazeController : FlowUpdateBehaviour, IMazeController {

        [Range(5, 50)]
        [SerializeField]
        private int _rows = 5;
        public int Rows { get { return _rows; } }

        [Range(5, 50)]
        [SerializeField]
        private int _columns = 5;
        public int Columns { get { return _columns; } }

        private byte stage = 0;

        private GameObject wallPrototype, floorPrototype;

        public float ScaleOfCellSide { get { return LenghtOfCellSide / 4f; } }
        public float LenghtOfCellSide { get { return 4f; } }
      

        public override void OnInit() {
          
            wallPrototype = MazeObjectsProvider.Instance.GetWall();
            Vector3 wallScale = wallPrototype.transform.localScale;
            wallScale.x = ScaleOfCellSide;
            wallPrototype.transform.localScale = wallScale;

            floorPrototype = MazeObjectsProvider.Instance.GetFloor();
            Vector3 floorScale = floorPrototype.transform.localScale;
            floorScale.x = LenghtOfCellSide;
            floorScale.z = LenghtOfCellSide;
            floorPrototype.transform.localScale = floorScale;
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                .When(State.CreateMaze)
                .Then(() => {
                    CreateMazeWithItems();
                    GameFlowManager.Instance.Flow.NextState();
                })
                .OrElseWhen(State.MazeCreated)
                .Then(() => StartCoroutine(GameFlowManager.Instance.Flow.ChangeStateAfterFrameEnd()))
                .OrElseWhen(State.DestroyPathToGoal)
                .Then(() => {
                    foreach(GameObject cell in pathToGoalsGameObjects) {
                        Destroy(cell);
                    }
                    GameFlowManager.Instance.Flow.NextState();
                })
                .OrElseWhen(State.EndGame)
                .Then(NextStage)
                .Build();
        }

        private void NextStage() {
            if(GameFlowManager.Instance.Flow.Is(State.EndGame)) {
                stage++;
                _rows += stage;
                _columns += stage;
                CanvasObjectsProvider.Instance
                    .GetArrowButtonController()
                    .ReinitializeArrowHintsCount();
                GameFlowManager.Instance.ReinitializeFlowBehaviours();
            } else {
                Debug.LogWarning("Cannot set next stage. Game should has state: " + State.EndGame);
            }
        }

        public override int GetInitOrder() {
            return InitOrder.Maze;
        }
    }
}