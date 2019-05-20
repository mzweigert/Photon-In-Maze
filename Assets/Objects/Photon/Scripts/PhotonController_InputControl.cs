using UnityEngine;
using UnityEngine.EventSystems;

public partial class PhotonController : MonoObserveable<PhotonState> {

    private Vector2 fingerStart, fingerEnd;
    private bool canSwipe = true;

    private void CheckButtonPress() {
        if(CheckIfAnyPressed(KeyCode.W, KeyCode.UpArrow)) {
            SaveMove(TouchMovement.Up);
        } else if(CheckIfAnyPressed(KeyCode.A, KeyCode.LeftArrow)) {
            SaveMove(TouchMovement.Left);
        } else if(CheckIfAnyPressed(KeyCode.S, KeyCode.DownArrow)) {
            SaveMove(TouchMovement.Down);
        } else if(CheckIfAnyPressed(KeyCode.D, KeyCode.RightArrow)) {
            SaveMove(TouchMovement.Right);
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

    private void CheckTouch() {
        if(Input.touchCount == 1) {
            Touch touch = Input.GetTouch(0);
            if(EventSystem.current.IsPointerOverGameObject(touch.fingerId)) {
                canSwipe = false;
                return;
            } 

            if(touch.phase == TouchPhase.Began) {
                fingerStart = touch.position;
                fingerEnd = touch.position;
            }
            if(touch.phase == TouchPhase.Moved && canSwipe) {
                fingerEnd = touch.position;
                TouchMovement movementDirection = GetTouchMovementDirection();
                SaveMove(movementDirection);
                fingerStart = touch.position;
                canSwipe = false;
            }
            if(touch.phase == TouchPhase.Ended) {
                canSwipe = true;
            }
        }
    }

    private TouchMovement GetTouchMovementDirection() {
        float xMove = Mathf.Abs(fingerStart.x - fingerEnd.x);
        float yMove = Mathf.Abs(fingerStart.y - fingerEnd.y);
        if(xMove > yMove) {
            return (fingerEnd.x - fingerStart.x) > 0.65f ? TouchMovement.Right : TouchMovement.Left;
        } else {
            return (fingerEnd.y - fingerStart.y) > 0.65 ? TouchMovement.Up : TouchMovement.Down;
        }
    }

}
