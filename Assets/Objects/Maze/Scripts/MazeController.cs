using PhotonInMaze.Common;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Maze.Generator;
using PhotonInMaze.Provider;
using UnityEngine;

//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>

namespace PhotonInMaze.Maze {

    public partial class MazeController : FlowUpdateBehaviour, IMazeController {

        [SerializeField]
        internal MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;

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

        private BasicMazeGenerator generator = null;

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
                .OrElseWhen(State.GenerateMaze)
                .Then(() => {
                    generator = InitGenerator(_rows, _columns);
                    generator.GenerateMaze();
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

        public override int GetInitOrder() {
            return InitOrder.Maze;
        }

        private BasicMazeGenerator InitGenerator(int rows, int columns) {
            switch(Algorithm) {
                case MazeGenerationAlgorithm.RandomTree:
                    return new RandomTreeMazeGenerator(rows, columns, LenghtOfCellSide);
                case MazeGenerationAlgorithm.Division:
                    return new DivisionMazeGenerator(rows, columns, LenghtOfCellSide);
                case MazeGenerationAlgorithm.PureRecursive:
                default:
                    return new RecursiveMazeGenerator(rows, columns, LenghtOfCellSide);
            }
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

        public void Recreate(int rows, int columns, MazeGenerationAlgorithm algorithm) {
            stage = 0;
            _rows = rows - 1;
            _columns = columns - 1;
            Algorithm = algorithm;
            while(!GameFlowManager.Instance.Flow.Is(State.HidePhoton)) {
                GameFlowManager.Instance.Flow.NextState();
            }
        }
    }
}