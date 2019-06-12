using UnityEngine;

namespace PhotonInMaze.Common.Model {
    public interface IPhotonState {
        bool IsAcutallyMoving { get; }
        int IndexOfLastCellInPathToGoal { get; }
        Vector3 RealPosition { get; }
    }
}