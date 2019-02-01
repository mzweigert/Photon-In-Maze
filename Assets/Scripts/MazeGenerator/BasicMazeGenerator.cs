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
}
