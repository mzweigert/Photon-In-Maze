﻿using PhotonInMaze.Common;
using PhotonInMaze.Common.Model;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace PhotonInMaze.Maze {

    //<summary>
    //Class for representing concrete maze cell.
    //</summary>
    internal class MazeCell : IMazeCell {

        public HashSet<Direction> Walls { get; } = new HashSet<Direction>();
        public bool IsProperPathToGoal { get; internal set; } = false;
        public bool IsGoal { get; internal set; } = false;
        public bool IsTrap { get; internal set; } = false;

        public int Column { get; }
        public int Row { get; }
        public float X { get; }
        public float Y { get; }

        public MazeCell(int row, int column, float cellLengthSide) {
            this.Row = row;
            this.Column = column;
            this.X = column * cellLengthSide;
            this.Y = row * cellLengthSide;
        }

        public override bool Equals(object obj) {
            var cell = obj as MazeCell;
            return cell != null &&
                   Column == cell.Column &&
                   Row == cell.Row;
        }

        public override int GetHashCode() {
            var hashCode = 656739706;
            hashCode = hashCode * -1521134295 + Column.GetHashCode();
            hashCode = hashCode * -1521134295 + Row.GetHashCode();
            return hashCode;
        }

        public Direction GetDirectionTo(IMazeCell next) {
            if(next == null) {
                return Direction.Start;
            } else if(Row < next.Row) {
                return Direction.Right;
            } else if(Row > next.Row) {
                return Direction.Left;
            } else if(Column < next.Column) {
                return Direction.Back;
            } else if(Column > next.Column) {
                return Direction.Front;
            }
            return Direction.Start;
        }

        public Vector2 ToVector2() {
            return new Vector2(X, Y);
        }

        public HashSet<Direction> GetPossibleMovesDirection() {
            HashSet<Direction> availableMoves = new HashSet<Direction>();
            Array allDirections = System.Enum.GetValues(typeof(Direction));
            foreach(Direction direction in allDirections) {
                if(!Walls.Contains(direction) && direction != Direction.Start) {
                    availableMoves.Add(direction);
                }
            }
            return availableMoves;
        }

        public HashSet<Vector2Int> GetPossibleMovesCoords() {
            HashSet<Vector2Int> availableMoves = new HashSet<Vector2Int>();
            Array allDirections = System.Enum.GetValues(typeof(Direction));
            foreach(Direction direction in allDirections) {
                if(Walls.Contains(direction) || direction == Direction.Start) {
                    continue;
                }
                Vector2Int move = MapDirectionToCoords(direction);
                availableMoves.Add(move);
            }
            return availableMoves;
        }

        private Vector2Int MapDirectionToCoords(Direction direction) {
            switch(direction) {
                case Direction.Back:
                    return new Vector2Int(Row - 1, Column);
                case Direction.Left:
                    return new Vector2Int(Row, Column - 1);
                case Direction.Right:
                    return new Vector2Int(Row, Column + 1);
                case Direction.Front:
                    return new Vector2Int(Row + 1, Column);
            }
            return new Vector2Int(Row, Column);
        }

        public bool IsStartCell() {
            return Column == 0 && Row == 0;
        }

        public override string ToString() {
            return string.Format("[MazeCell {0} {1}]", Row, Column);
        }

        public string ToStringAsName() {
            return string.Format("MazeCell_{0}_{1}", Row, Column);
        }

    }
}