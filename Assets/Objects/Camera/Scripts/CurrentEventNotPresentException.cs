using System;

namespace PhotonInMaze.Game.GameCamera {
    public class CurrentEventNotPresentException : Exception {

        public CurrentEventNotPresentException() : base("Current event is not present") {

        }
    }
}