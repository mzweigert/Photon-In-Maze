using System;
using System.Collections.Generic;
using UnityEngine;

//<summary>
//Basic class for maze generation logic
//</summary>
public abstract class BasicMazeGenerator {

    public int RowCount { get; }
    public int ColumnCount { get; }
    protected MazeCellFinder finder;

    private MazeCell[,] mMaze;
    private int row;
    private int column;

    public BasicMazeGenerator(int rows, int columns, float cellLengthSide) {
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
                MazeCell cell = new MazeCell(row, column, cellLengthSide);
                mMaze[row, column] = cell;
            }
		}
        finder = new MazeCellFinder(RowCount, ColumnCount);


    }

    protected BasicMazeGenerator(int row, int column) {
        this.row = row;
        this.column = column;
    }

    public MazeCell GetMazeCell(int row, int column) {
        if(row >= 0 && column >= 0 && row < RowCount && column < ColumnCount) {
            return mMaze[row, column];
        } else {
            Debug.Log(row + " " + column);
            throw new System.ArgumentOutOfRangeException();
        }
    }

    protected void SetMazeCell(int row, int column, MazeCell cell) {
        if(row >= 0 && column >= 0 && row < RowCount && column < ColumnCount) {
            mMaze[row, column] = cell;
        } else {
            throw new System.ArgumentOutOfRangeException();
        }
    }

    public LinkedList<MazeCell> GenerateMazeAndFindPathToGoal() {
        GenerateMaze();
        return FindPathToGoal();
    }

    protected abstract void GenerateMaze();

    protected LinkedList<MazeCell> FindPathToGoal() {
        MazeCell exitCell = GetMazeCell(RowCount - 1, ColumnCount - 1);
        exitCell.IsGoal = true;
        exitCell.WallFront = false;
        LinkedList<MazeCell> path = FindPathToGoal(exitCell, Direction.Start);
        return path;
    }

    private LinkedList<MazeCell> FindPathToGoal(MazeCell current, Direction moveMade) {
        LinkedList<MazeCell> pathToGoal = new LinkedList<MazeCell>();
        List<Direction> movesAvailable = current.GetPossibleMoveDirection();
        if(moveMade != Direction.Start) {
            movesAvailable.Remove(GetOposedMove(moveMade));
        } else if(current.IsGoal) {
            movesAvailable.Remove(Direction.Front);
        }
        HashSet<MazeCell> visitedCells = new HashSet<MazeCell>();
        while(movesAvailable.Count > 0) {
            finder.FindNextToVisit(movesAvailable, current.Row, current.Column).IfPresent((ctv) => {
                MazeCell next = GetMazeCell(ctv.Row, ctv.Column);
                next.IsPathToGoal = true;
                visitedCells.Add(next);
                movesAvailable.Remove(ctv.MoveMade);
                LinkedList<MazeCell> pathInNext = FindPathToGoal(next, ctv.MoveMade);
                if(pathInNext.Count > 0) {
                    pathToGoal = pathInNext;
                }
            });
        }

        if(current.IsGoal || current.IsStartCell() || finder.IsPathToGoalVisited(visitedCells)) {
            pathToGoal.AddLast(current);
        } else if(!current.IsGoal && finder.IsATrap(visitedCells, current)) {
            current.IsTrap = true;
            current.IsPathToGoal = false;
        }
        return pathToGoal;
    }

    private Direction GetOposedMove(Direction moveMade) {
        switch(moveMade) {
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            case Direction.Front:
                return Direction.Back;
            case Direction.Back:
                return Direction.Front;
        }
        return Direction.Start;
    }
}
