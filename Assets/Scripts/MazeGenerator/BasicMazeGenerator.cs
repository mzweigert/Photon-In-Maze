using System;
using System.Collections.Generic;
using UnityEngine;

//<summary>
//Basic class for maze generation logic
//</summary>
public abstract class BasicMazeGenerator {
    public int RowCount { get; }
    public int ColumnCount { get; }

    private MazeCell[,] mMaze;

	public BasicMazeGenerator(int rows, int columns, Func<MazeCell, GameObject> createRealObjFunction) {
		RowCount = Mathf.Abs(rows);
		ColumnCount = Mathf.Abs(columns);
		if (RowCount == 0) {
			RowCount = 1;
		}
		if (ColumnCount == 0) {
			ColumnCount = 1;
		}
		mMaze = new MazeCell[rows,columns];
		for (int row = 0; row < rows; row++) {
			for(int column = 0; column < columns; column++){
                MazeCell cell = new MazeCell(row, column, createRealObjFunction);
                mMaze[row, column] = cell;
            }
		}
	}

	public abstract LinkedList<MazeCell> GenerateMaze();

	public MazeCell GetMazeCell(int row, int column){
		if (row >= 0 && column >= 0 && row < RowCount && column < ColumnCount) {
			return mMaze[row,column];
		}else{
			Debug.Log(row+" "+column);
			throw new System.ArgumentOutOfRangeException();
		}
	}

	protected void SetMazeCell(int row, int column, MazeCell cell){
		if (row >= 0 && column >= 0 && row < RowCount && column < ColumnCount) {
			mMaze[row,column] = cell;
		}else{
			throw new System.ArgumentOutOfRangeException();
		}
	}


    protected bool IsATrap(HashSet<MazeCell> visitedCells, MazeCell currentCell) {
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

    protected bool IsPathToGoalVisited(HashSet<MazeCell> visitedCells) {
        HashSet<MazeCell>.Enumerator enumerator = visitedCells.GetEnumerator();
        while(enumerator.MoveNext()) {
            if(enumerator.Current.IsPathToGoal || enumerator.Current.IsGoal) {
                return true;
            }
        }
        return false;
    }

    protected Optional<CellToVisit> FindNextToVisit(List<Direction> movesAvailable, int row, int column) {
        int randomCell = UnityEngine.Random.Range(0, movesAvailable.Count);
        bool isEndCell = row + 1 == RowCount && column + 1 == ColumnCount;
        switch(movesAvailable[randomCell]) {
            case Direction.Start:
                return Optional<CellToVisit>.Empty();
            case Direction.Right:
                return new CellToVisit(row, column + 1, Direction.Right);
            case Direction.Front:
                return isEndCell? 
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
