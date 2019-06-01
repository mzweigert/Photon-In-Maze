using PhotonInMaze.Common.Flow;
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

    public partial class MazeController : FlowBehaviour {

        [SerializeField]
        private MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
        [SerializeField]
        private bool FullRandom = false;
        [SerializeField]
        private int RandomSeed = 12345;

        [Range(5, 50)]
        public int Rows = 5;
        [Range(5, 50)]
        public int Columns = 5;

        public float LenghtOfCellSide { get; } = 4f;
        public float ScaleOfCellSide { get { return LenghtOfCellSide / 4f; } }

       
        private GameObject wallPrototype, floorPrototype;
        private BasicMazeGenerator mazeGenerator = null;

        void Awake() {
            if(!FullRandom) {
                Random.InitState(RandomSeed);
            }
            wallPrototype = ObjectsManager.Instance.GetWall();
            Vector3 wallScale = wallPrototype.transform.localScale;
            wallScale.x = ScaleOfCellSide;
            wallPrototype.transform.localScale = wallScale;

            floorPrototype = ObjectsManager.Instance.GetFloor();
            Vector3 floorScale = floorPrototype.transform.localScale;
            floorScale.x = LenghtOfCellSide;
            floorScale.z = LenghtOfCellSide;
            floorPrototype.transform.localScale = floorScale;

            finder = new MazeCellFinder(Rows, Columns);
            mazeGenerator = InitGenerator(Rows, Columns);
            mazeGenerator.GenerateMaze();
            PathsToGoal = FindPathToGoal();
            CreateMazeWithItems();
        }

        private BasicMazeGenerator InitGenerator(int rows, int columns) {
            switch(Algorithm) {
                case MazeGenerationAlgorithm.RandomTree:
                    return new RandomTreeMazeGenerator(Rows, Columns, LenghtOfCellSide);
                case MazeGenerationAlgorithm.Division:
                    return new DivisionMazeGenerator(Rows, Columns, LenghtOfCellSide);
                case MazeGenerationAlgorithm.PureRecursive:
                default:
                    return new RecursiveMazeGenerator(Rows, Columns, LenghtOfCellSide);
            }
        }

        protected override IInvoke Init() {
            return GameFlowManager.Instance.Flow
                .When(State.DestroyPathToGoal)
                .Then(() => {
                    foreach(GameObject cell in pathToGoalsGameObjects) {
                        Destroy(cell);
                    }
                    GameFlowManager.Instance.Flow.NextState();
                })
                .Build();
        }
    }
}