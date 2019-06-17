using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Model;
using PhotonInMaze.Provider;
using System.Collections.Generic;
using UnityEngine;

namespace PhotonInMaze.Maze.Generator {
    //<summary>
    //Basic class for maze generation logic
    //</summary>
    internal abstract class BasicMazeGenerator : IMazeGenerator {

        public int RowCount { get; }
        public int ColumnCount { get; }

        private int row;
        private int column;

        protected NextCellToVisitFinder finder;
        protected IMazeCellManager manager;
        protected HashSet<IMazeCell> visited;

        public BasicMazeGenerator(int rows, int columns, float cellLengthSide) {
            RowCount = Mathf.Abs(rows);
            ColumnCount = Mathf.Abs(columns);
            if(RowCount == 0) {
                RowCount = 1;
            }
            if(ColumnCount == 0) {
                ColumnCount = 1;
            }
            finder = new NextCellToVisitFinder(rows, columns);
            manager = MazeObjectsProvider.Instance.GetMazeCellManager();
            visited = new HashSet<IMazeCell>();
        }

        protected BasicMazeGenerator(int row, int column) {
            this.row = row;
            this.column = column;
        }

        public abstract void GenerateMaze();

    }
}