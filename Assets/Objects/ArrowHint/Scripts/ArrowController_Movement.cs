using PhotonInMaze.Common;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using System;
using UnityEngine;

namespace PhotonInMaze.Arrow {
    public partial class ArrowController : FlowFixedObserveableBehviour<ArrowState>, IArrowController {

        private void Move() {

            switch(currentState) {
                case ArrowState.Creating:
                    if(IsStateInAnimatorHasEnded("OnCreate")) {
                        currentState = ArrowState.Moving;
                    }
                    break;
                case ArrowState.Checking:
                    nextMove = currentCell.Value.GetDirectionTo(currentCell.Next?.Value);
                    SetStateAndNotifyObservers(lastMove != nextMove ? ArrowState.Rotating : ArrowState.Moving);
                    currentCell = currentCell.Next;
                    PrepareToDestroy();
                    break;
                case ArrowState.Moving:
                    Vector3 targetPosition = new Vector3(currentCell.Value.X, transform.position.y, currentCell.Value.Y);
                    transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);
                    if(Vector3.Distance(transform.position, targetPosition) > 0.1f) {
                        return;
                    }
                    transform.position = targetPosition;
                    SetStateAndNotifyObservers(ArrowState.Checking);
                    sizeOfPath--;
                    PrepareToDestroy();

                    break;
                case ArrowState.Rotating:
                    Quaternion toRotation = GetRotationByMove(nextMove);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 0.35f);
                    bool rotationDone = Math.Abs(Math.Abs(transform.rotation.y) - Math.Abs(toRotation.y)) <= 0.1f;
                    if(rotationDone) {
                        transform.rotation = toRotation;
                        lastMove = nextMove;
                        SetStateAndNotifyObservers(ArrowState.Moving);
                    }
                    break;
                case ArrowState.Destroying:
                    if(IsStateInAnimatorHasEnded("OnDestroy")) {
                        currentState = ArrowState.Ending;
                        Destroy(gameObject);
                    }
                    break;
            }
        }

        private Quaternion GetRotationByMove(Direction move) {
            switch(move) {
                case Direction.Right:
                    return Quaternion.Euler(0f, 270f, 0f);
                case Direction.Left:
                    return Quaternion.Euler(0f, 90f, 0f);
                case Direction.Back:
                    return Quaternion.Euler(0f, 0f, 0f);
                case Direction.Front:
                    return Quaternion.Euler(0, 180f, 0f);
            }
            return Quaternion.Euler(0f, -90f, 0f);
        }
        
    }

}