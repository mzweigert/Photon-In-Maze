﻿using System.Collections.Generic;
using UnityEngine;

namespace PhotonInMaze.Game.Maze {
    //<summary>
    //Maze generation by dividing area in two, adding spaces in walls and repeating recursively.
    //Non-recursive realisation of algorithm.
    //</summary>
    public class DivisionMazeGenerator : BasicMazeGenerator {

        public DivisionMazeGenerator(int row, int column, float cellLengthSide) : base(row, column, cellLengthSide) {

        }

        //<summary>
        //Class representing maze area to be divided
        //</summary>
        private struct IntRect {
            public int left;
            public int right;
            public int top;
            public int bottom;

            public override string ToString() {
                return string.Format("[IntRect {0}-{1} {2}-{3}]", left, right, bottom, top);
            }
        }

        private Queue<IntRect> rectsToDivide = new Queue<IntRect>();

        public override void GenerateMaze() {
            for(int row = 0; row < RowCount; row++) {
                GetMazeCell(row, 0).Walls.Add(Direction.Left);
                GetMazeCell(row, ColumnCount - 1).Walls.Add(Direction.Right);
            }
            for(int column = 0; column < ColumnCount; column++) {
                GetMazeCell(0, column).Walls.Add(Direction.Back);
                GetMazeCell(RowCount - 1, column).Walls.Add(Direction.Front);
            }

            rectsToDivide.Enqueue(new IntRect() { left = 0, right = ColumnCount, bottom = 0, top = RowCount });
            while(rectsToDivide.Count > 0) {
                IntRect currentRect = rectsToDivide.Dequeue();
                int width = currentRect.right - currentRect.left;
                int height = currentRect.top - currentRect.bottom;
                if(width > 1 && height > 1) {
                    if(width > height) {
                        divideVertical(currentRect);
                    } else if(height > width) {
                        divideHorizontal(currentRect);
                    } else if(height == width) {
                        if(Random.Range(0, 100) > 42) {
                            divideVertical(currentRect);
                        } else {
                            divideHorizontal(currentRect);
                        }
                    }
                } else if(width > 1 && height <= 1) {
                    divideVertical(currentRect);
                } else if(width <= 1 && height > 1) {
                    divideHorizontal(currentRect);
                }
            }
        }

        //<summary>
        //Divides selected area vertically
        //</summary>
        private void divideVertical(IntRect rect) {
            int divCol = Random.Range(rect.left, rect.right - 1);
            for(int row = rect.bottom; row < rect.top; row++) {
                GetMazeCell(row, divCol).Walls.Add(Direction.Right);
                GetMazeCell(row, divCol + 1).Walls.Add(Direction.Left);
            }
            int space = Random.Range(rect.bottom, rect.top);
            GetMazeCell(space, divCol).Walls.Remove(Direction.Right);
            if(divCol + 1 < rect.right) {
                GetMazeCell(space, divCol + 1).Walls.Remove(Direction.Left);
            }
            rectsToDivide.Enqueue(new IntRect() { left = rect.left, right = divCol + 1, bottom = rect.bottom, top = rect.top });
            rectsToDivide.Enqueue(new IntRect() { left = divCol + 1, right = rect.right, bottom = rect.bottom, top = rect.top });
        }

        //<summary>
        //Divides selected area horiszontally
        //</summary>
        private void divideHorizontal(IntRect rect) {
            int divRow = Random.Range(rect.bottom, rect.top - 1);
            for(int col = rect.left; col < rect.right; col++) {
                GetMazeCell(divRow, col).Walls.Add(Direction.Front);
                GetMazeCell(divRow + 1, col).Walls.Add(Direction.Back);
            }
            int space = Random.Range(rect.left, rect.right);
            GetMazeCell(divRow, space).Walls.Remove(Direction.Front);
            if(divRow + 1 < rect.top) {
                GetMazeCell(divRow + 1, space).Walls.Remove(Direction.Back);
            }
            rectsToDivide.Enqueue(new IntRect() { left = rect.left, right = rect.right, bottom = rect.bottom, top = divRow + 1 });
            rectsToDivide.Enqueue(new IntRect() { left = rect.left, right = rect.right, bottom = divRow + 1, top = rect.top });
        }
    }
}