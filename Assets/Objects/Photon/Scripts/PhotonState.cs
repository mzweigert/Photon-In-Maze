using UnityEngine;

namespace PhotonInMaze.Game.Photon {
    public struct PhotonState {
        public bool IsInPathToGoal { get; internal set; }
        public bool IsAcutallyMoving { get; internal set; }
        public int IndexOfLastCellInPathToGoal { get; internal set; }
        public Vector3 RealPosition { get; internal set; }

        public PhotonState(Vector3 realPosition) {
            IsInPathToGoal = true;
            IndexOfLastCellInPathToGoal = 0;
            RealPosition = realPosition;
            IsAcutallyMoving = false;
        }
    }
}