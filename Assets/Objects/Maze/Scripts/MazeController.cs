using PhotonInMaze.Common.Flow;
using PhotonInMaze.Game.GameCamera;
using PhotonInMaze.Game.Manager;
using UnityEngine;

//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>

namespace PhotonInMaze.Game.Maze {
    public enum MazeGenerationAlgorithm {
        PureRecursive,
        RandomTree,
        Division
    }

    public partial class MazeController : FlowUpdateBehaviour {

        [SerializeField]
        private MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
        [SerializeField]
        private bool FullRandom = false;
        [SerializeField]
        private int RandomSeed = 12345;

        [Range(5, 50)]
        [SerializeField]
        private int _rows = 5;
        public int Rows { get { return _rows; } }

        [Range(5, 50)]
        [SerializeField]
        private int _columns = 5;
        public int Columns { get { return _columns; } }

        private byte stage = 0;
        public float LenghtOfCellSide { get; } = 4f;
        public float ScaleOfCellSide { get { return LenghtOfCellSide / 4f; } }

        private GameObject wallPrototype, floorPrototype;
        private BasicMazeGenerator mazeGenerator = null;

        public override void OnStart() {
            if(!FullRandom) {
                Random.InitState(RandomSeed);
            }
            wallPrototype = MazeObjectsManager.Instance.GetWall();
            Vector3 wallScale = wallPrototype.transform.localScale;
            wallScale.x = ScaleOfCellSide;
            wallPrototype.transform.localScale = wallScale;

            floorPrototype = MazeObjectsManager.Instance.GetFloor();
            Vector3 floorScale = floorPrototype.transform.localScale;
            floorScale.x = LenghtOfCellSide;
            floorScale.z = LenghtOfCellSide;
            floorPrototype.transform.localScale = floorScale;

            finder = new MazeCellFinder(_rows, _columns);
            mazeGenerator = InitGenerator(_rows, _columns);
        }

        private BasicMazeGenerator InitGenerator(int rows, int columns) {
            switch(Algorithm) {
                case MazeGenerationAlgorithm.RandomTree:
                    return new RandomTreeMazeGenerator(_rows, _columns, LenghtOfCellSide);
                case MazeGenerationAlgorithm.Division:
                    return new DivisionMazeGenerator(_rows, _columns, LenghtOfCellSide);
                case MazeGenerationAlgorithm.PureRecursive:
                default:
                    return new RecursiveMazeGenerator(_rows, _columns, LenghtOfCellSide);
            }
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                .When(State.GenerateMaze)
                .Then(() => {
                    mazeGenerator.GenerateMaze();
                    PathsToGoal = FindPathToGoal();
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
                ObjectsManager.Instance.ReinitializeArrowHintsCount();
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