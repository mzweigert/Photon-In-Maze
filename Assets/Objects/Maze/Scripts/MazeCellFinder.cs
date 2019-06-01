using PhotonInMaze.Common;
using System.Collections.Generic;


namespace PhotonInMaze.Game.Maze {
    public class MazeCellFinder {

        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }

        public MazeCellFinder(int rowCount, int columnCount) {
            RowCount = rowCount;
            ColumnCount = columnCount;
        }

        public bool IsATrap(HashSet<MazeCell> visitedCells, MazeCell currentCell) {
            if(currentCell.Row == 0 && currentCell.Column == 0) {
                return false;
            }
            if(visitedCells.Count == 0) {
                return true;
            }
            bool allVisitedIsTrap = true;
            HashSet<MazeCell>.Enumerator enumerator = visitedCells.GetEnumerator();
            while(enumerator.MoveNext()) {
                allVisitedIsTrap = allVisitedIsTrap && enumerator.Current.IsTrap;
            }
            return allVisitedIsTrap;
        }

        public bool IsPathToGoalVisited(HashSet<MazeCell> visitedCells) {
            HashSet<MazeCell>.Enumerator enumerator = visitedCells.GetEnumerator();
            while(enumerator.MoveNext()) {
                if(enumerator.Current.IsPathToGoal || enumerator.Current.IsGoal) {
                    return true;
                }
            }
            return false;
        }

        public Optional<CellToVisit> FindNextToVisit(HashSet<Direction> availableMoves, int row, int column) {

            bool isEndCell = row + 1 == RowCount && column + 1 == ColumnCount;
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