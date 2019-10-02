using PhotonInMaze.Common.Model;
using System;

namespace PhotonInMaze.Common.Controller {
    public interface IPhotonMovementController : IObservable<IPhotonState> {
        IMazeCell CurrentMazeCell { get; }
    }
}
