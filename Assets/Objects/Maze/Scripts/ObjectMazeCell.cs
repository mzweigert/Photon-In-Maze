using PhotonInMaze.Common.Model;
using System.Collections.Generic;
using UnityEngine;


namespace PhotonInMaze.Maze {
    internal struct ObjectMazeCell {

        internal IMazeCell cell;
        internal GameObject gameObject;

        public ObjectMazeCell(IMazeCell cell, GameObject gameObject) {
            this.cell = cell;
            this.gameObject = gameObject;
        }

        public override bool Equals(object obj) {
            if(!(obj is ObjectMazeCell)) {
                return false;
            }

            var cell = (ObjectMazeCell)obj;
            return EqualityComparer<IMazeCell>.Default.Equals(this.cell, cell.cell) &&
                   EqualityComparer<int>.Default.Equals(gameObject.GetInstanceID(), cell.gameObject.GetInstanceID());
        }

        public override int GetHashCode() {
            var hashCode = 574081665;
            hashCode = hashCode * -1521134295 + EqualityComparer<IMazeCell>.Default.GetHashCode(cell);
            hashCode = hashCode * -1521134295 + EqualityComparer<int>.Default.GetHashCode(gameObject.GetInstanceID());
            return hashCode;
        }
    }
}