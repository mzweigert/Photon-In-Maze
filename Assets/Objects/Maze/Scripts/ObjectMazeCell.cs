using System.Collections.Generic;
using UnityEngine;


namespace PhotonInMaze.Game.Maze {
    public struct ObjectMazeCell {

        internal MazeCell cell;
        internal GameObject gameObject;

        public ObjectMazeCell(MazeCell cell, GameObject gameObject) {
            this.cell = cell;
            this.gameObject = gameObject;
        }

        public override bool Equals(object obj) {
            if(!(obj is ObjectMazeCell)) {
                return false;
            }

            var cell = (ObjectMazeCell)obj;
            return EqualityComparer<MazeCell>.Default.Equals(this.cell, cell.cell) &&
                   EqualityComparer<int>.Default.Equals(gameObject.GetInstanceID(), cell.gameObject.GetInstanceID());
        }

        public override int GetHashCode() {
            var hashCode = 574081665;
            hashCode = hashCode * -1521134295 + EqualityComparer<MazeCell>.Default.GetHashCode(cell);
            hashCode = hashCode * -1521134295 + EqualityComparer<int>.Default.GetHashCode(gameObject.GetInstanceID());
            return hashCode;
        }
    }
}