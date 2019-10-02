using System;

namespace PhotonInMaze.GameCamera {
    internal class CurrentEventNotPresentException : Exception {

        public CurrentEventNotPresentException() : base("Current event is not present") {

        }
    }
}