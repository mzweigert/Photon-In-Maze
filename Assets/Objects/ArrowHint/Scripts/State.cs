using UnityEngine;
using UnityEditor;


namespace PhotonInMaze.Game.Arrow {
    public enum ArrowState {
        Creating,
        Checking,
        Moving,
        Rotating,
        Destroying,
        Ending
    }
}