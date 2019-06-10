using System;

namespace PhotonInMaze.GameCamera {
    public class CurrentEventNotPresentException : Exception {

        public CurrentEventNotPresentException() : base("Current event is not present") {

        }
    }
}