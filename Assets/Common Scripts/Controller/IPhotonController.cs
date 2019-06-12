using PhotonInMaze.Common.Model;
using System;
using UnityEngine;

namespace PhotonInMaze.Common.Controller {
    public interface IPhotonController : IObservable<IPhotonState> {
        Vector3 GetInitialPosition();
        IMazeCell GetCurrentMazeCellPosition();
    }
}
