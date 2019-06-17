
using PhotonInMaze.Common;

namespace PhotonInMaze.Maze.Generator {
    //<summary>
    //Class representation of target cell
    //</summary>
    internal struct CellToVisit {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public Direction MoveMade { get; private set; }

        public CellToVisit(int row, int column, Direction move) {
            Row = row;
            Column = column;
            MoveMade = move;
        }

        public override bool Equals(object obj) {
            if(!(obj is CellToVisit)) {
                return false;
            }

            var visit = (CellToVisit)obj;
            return Row == visit.Row &&
                   Column == visit.Column &&
                   MoveMade == visit.MoveMade;
        }

        public override int GetHashCode() {
            var hashCode = 739816171;
            hashCode = hashCode * -1521134295 + Row.GetHashCode();
            hashCode = hashCode * -1521134295 + Column.GetHashCode();
            hashCode = hashCode * -1521134295 + MoveMade.GetHashCode();
            return hashCode;
        }

        public override string ToString() {
            return string.Format("[MazeCell {0} {1}]", Row, Column);
        }

    }
}