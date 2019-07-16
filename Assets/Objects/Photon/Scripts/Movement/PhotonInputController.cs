using PhotonInMaze.Provider;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PhotonInMaze.Photon {
    internal class PhotonInputControl {

        private Vector2 fingerStart, fingerEnd;
        private bool canSwipe = true;
        private EventSystem es;
        private float aspectRatio;

        private PhotonMovementQueue movementQueue;

        internal PhotonInputControl(PhotonMovementQueue movementQueue) {
            aspectRatio = ObjectsProvider.Instance.GetAreaCamera().aspect;
            this.movementQueue = movementQueue;
        }

        internal void CheckButtonPress() {
            if(CheckIfAnyPressed(KeyCode.W, KeyCode.UpArrow)) {
                movementQueue.SaveMove(TouchMovement.Up);
            } else if(CheckIfAnyPressed(KeyCode.A, KeyCode.LeftArrow)) {
                movementQueue.SaveMove(TouchMovement.Left);
            } else if(CheckIfAnyPressed(KeyCode.S, KeyCode.DownArrow)) {
                movementQueue.SaveMove(TouchMovement.Down);
            } else if(CheckIfAnyPressed(KeyCode.D, KeyCode.RightArrow)) {
                movementQueue.SaveMove(TouchMovement.Right);
            }
        }

        private bool CheckIfAnyPressed(params KeyCode[] codes) {
            foreach(KeyCode code in codes) {
                if(Input.GetKeyDown(code)) {
                    return true;
                }
            }
            return false;
        }

        internal void CheckTouch() {
            if(Input.touchCount == 1) {
                Touch touch = Input.GetTouch(0);
                es = EventSystem.current;
                if(es.IsPointerOverGameObject(touch.fingerId) && es.currentSelectedGameObject != null) {
                    canSwipe = false;
                    return;
                }

                if(touch.phase == TouchPhase.Began) {
                    fingerStart = touch.position;
                    fingerEnd = touch.position;
                }
                if(touch.phase == TouchPhase.Moved && canSwipe) {
                    fingerEnd = touch.position;
                    TouchMovementEvent movementEvent = TouchMovementEvent.GetTouchMovementDirection(fingerStart, fingerEnd);
                    bool moveToShort = !((movementEvent.IsHorizontal() && movementEvent.delta > Screen.width / (8 * aspectRatio)) ||
                        (movementEvent.IsVertical() && movementEvent.delta > (Screen.height  / 8)));
                    if(moveToShort) {
                        return;
                    }
                    movementQueue.SaveMove(movementEvent.direction);
                    fingerStart = touch.position;
                    canSwipe = false;
                }
                if(touch.phase == TouchPhase.Ended) {
                    canSwipe = true;
                }
            }
        }

    }
}