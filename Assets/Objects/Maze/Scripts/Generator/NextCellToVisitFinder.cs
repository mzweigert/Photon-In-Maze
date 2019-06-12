using PhotonInMaze.Common;
using System.Collections.Generic;

namespace PhotonInMaze.Maze.Generator {
    public class NextCellToVisitFinder {

        private int rows, columns;

        public NextCellToVisitFinder(int rows, int columns) {
            this.rows = rows;
            this.columns = columns;
        }

        public Optional<CellToVisit> FindNextToVisit(HashSet<Direction> availableMoves, int row, int column) {

            bool isEndCell = row + 1 == rows && column + 1 == columns;
            switch(GetRandomFromSet(availableMoves)) {
                case Direction.Start:
                    return Optional<CellToVisit>.Empty();
                case Direction.Right:
                    return new CellToVisit(row, column + 1, Direction.Right);
                case Direction.Front:
                    return isEndCell ?
                        Optional<CellToVisit>.Empty() :
                        new CellToVisit(row + 1, column, Direction.Front);
                case Direction.Left:
                    return new CellToVisit(row, column - 1, Direction.Left);
                case Direction.Back:
                    return new CellToVisit(row - 1, column, Direction.Back);
            }
            return Optional<CellToVisit>.Empty();
        }

        private Direction GetRandomFromSet(HashSet<Direction> availableMoves) {
            UnityEngine.Random.InitState(UnityEngine.Random.Range(2, 12347));
            int randomCell = UnityEngine.Random.Range(0, availableMoves.Count);
            int i = 0;
            foreach(Direction availableMove in availableMoves) {
                if(i == randomCell) {
                    return availableMove;
                }
                i++;
            }
            return Direction.Start;
        }

    }
}