using PhotonInMaze.Common.Model;

namespace PhotonInMaze.Maze {

    //<summary>
    //Class for representing concrete maze cell.
    //</summary>
    internal struct NextCell {

        internal IMazeCell value;
        internal Direction nextMove;

        public NextCell(IMazeCell value, Direction nextMove) {
            this.value = value;
            this.nextMove = nextMove;
        }
    }
}