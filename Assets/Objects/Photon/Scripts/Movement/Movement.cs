
using UnityEngine;

namespace PhotonInMaze.Photon {
    internal struct TouchMovementEvent {
        internal TouchMovement direction;
        internal float delta;

        public TouchMovementEvent(TouchMovement direction, float delta) {
            this.direction = direction;
            this.delta = delta;
        }

        internal static TouchMovementEvent GetTouchMovementDirection(Vector2 fingerStart, Vector2 fingerEnd) {
            float xMove = Mathf.Abs(fingerStart.x - fingerEnd.x);
            float yMove = Mathf.Abs(fingerStart.y - fingerEnd.y);
            float delta;
            TouchMovement move;
            if(xMove > yMove) {
                delta = fingerEnd.x - fingerStart.x;
                move = delta > 0 ? TouchMovement.Right : TouchMovement.Left;
            } else {
                delta = fingerEnd.y - fingerStart.y;
                move = delta > 0 ? TouchMovement.Up : TouchMovement.Down;
            }
            return new TouchMovementEvent(move, Mathf.Abs(delta));
        }

        internal bool IsHorizontal() {
            return direction == TouchMovement.Left || direction == TouchMovement.Right;
        }

        internal bool IsVertical() {
            return direction == TouchMovement.Down || direction == TouchMovement.Up;
        }
    }

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