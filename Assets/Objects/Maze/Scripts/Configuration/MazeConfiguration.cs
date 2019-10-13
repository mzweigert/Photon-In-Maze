using PhotonInMaze.Common;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Maze.Generator;
using UnityEngine;

namespace PhotonInMaze.Maze {
    internal class MazeConfiguration : MonoBehaviour, IMazeConfiguration {

        [Range(1, 10)]
        [SerializeField]
        private int _secondsToStart = 3;
        public int SecondsToStart { get { return _secondsToStart; } }

        [Range(5, 50)]
        [SerializeField]
        private int _rows = 5;
        public int Rows { get { return _rows; } }

        [Range(5, 50)]
        [SerializeField]
        private int _columns = 5;
        public int Columns { get { return _columns; } }

        [SerializeField]
        private MazeGenerationAlgorithm _algorithm = MazeGenerationAlgorithm.PureRecursive;
        public MazeGenerationAlgorithm Algorithm { get { return _algorithm; } }

        public float CellSideLength { get { return 4f; } }

        public string Name { get { return gameObject.name; } }

        private byte stage = 0;

        public IMazeGenerator GetGenerator() {
            switch(Algorithm) {
                case MazeGenerationAlgorithm.RandomTree:
                    return new RandomTreeMazeGenerator(Rows, Columns, CellSideLength);
                case MazeGenerationAlgorithm.Division:
                    return new DivisionMazeGenerator(Rows, Columns, CellSideLength);
                case MazeGenerationAlgorithm.PureRecursive:
                default:
                    return new RecursiveMazeGenerator(Rows, Columns, CellSideLength);
            }
        }

        internal void IncraseValues() {
            stage++;
            _rows += 1;
            _columns += 1;
        }

        internal void SetValues(int rows, int columns, MazeGenerationAlgorithm algorithm, byte stage) {
            this._rows = rows;
            this._columns = columns;
            this._algorithm = algorithm;
            this.stage = stage;
        }
    }
}