
namespace PhotonInMaze.Photon {
    internal enum TouchMovement {
        Left,
        Right,
        Up,
        Down
    };

    internal enum MovementEvent {
        Idle,
        Move,
        Teleport,
        ExitFromWormhole
    };
}