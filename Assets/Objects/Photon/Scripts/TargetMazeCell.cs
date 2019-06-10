using PhotonInMaze.Common.Model;
using System;

namespace PhotonInMaze.Photon {
    internal struct TargetMazeCell {

        internal IMazeCell value;
        internal MovementEvent movementEvent;
        private Action additionalAction;

        public TargetMazeCell(IMazeCell value, MovementEvent movementEvent) {
            this.value = value;
            this.movementEvent = movementEvent;
            this.additionalAction = null;
        }

        public TargetMazeCell(IMazeCell value, MovementEvent movementEvent, Action action) : this(value, movementEvent) {
            this.additionalAction = action;
        }

        public bool IsGoal { get { return value.IsGoal; } }

        internal void TryInvokeAction() {
            additionalAction?.Invoke();
        }
    }
}