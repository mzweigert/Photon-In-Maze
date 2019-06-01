using PhotonInMaze.Game.Maze;
using System;

namespace PhotonInMaze.Game.Photon {
    public struct TargetMazeCell {

        internal MazeCell value;
        internal MovementEvent movementEvent;
        private Action additionalAction;

        public TargetMazeCell(MazeCell value, MovementEvent movementEvent) {
            this.value = value;
            this.movementEvent = movementEvent;
            this.additionalAction = null;
        }

        public TargetMazeCell(MazeCell value, MovementEvent movementEvent, Action action) : this(value, movementEvent) {
            this.value = value;
            this.movementEvent = movementEvent;
            this.additionalAction = action;
        }

        public bool IsGoal { get { return value.IsGoal; } }

        internal void TryInvokeAction() {
            additionalAction?.Invoke();
        }
    }
}