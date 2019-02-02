using System.Collections.Generic;
using UnityEngine;

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

    public Optional<CellToVisit> FindNextToVisit(List<Direction> movesAvailable, int row, int column) {
        int randomCell = UnityEngine.Random.Range(0, movesAvailable.Count);
        bool isEndCell = row + 1 == RowCount && column + 1 == ColumnCount;
        switch(movesAvailable[randomCell]) {
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
}