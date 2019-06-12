using PhotonInMaze.Common.Model;
using UnityEngine;

namespace PhotonInMaze.Photon {
    internal struct PhotonState : IPhotonState {
        public bool IsAcutallyMoving { get; internal set; }
        public int IndexOfLastCellInPathToGoal { get; internal set; }
        public Vector3 RealPosition { get; internal set; }

        public PhotonState(Vector3 realPosition) {
            IndexOfLastCellInPathToGoal = 0;
            RealPosition = realPosition;
            IsAcutallyMoving = false;
        }
    }

}