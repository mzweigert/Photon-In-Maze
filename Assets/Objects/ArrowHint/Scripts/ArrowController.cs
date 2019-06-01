using PhotonInMaze.Common.Flow;
using PhotonInMaze.Game.GameCamera;
using PhotonInMaze.Game.Manager;
using PhotonInMaze.Game.Maze;
using PhotonInMaze.Game.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace PhotonInMaze.Game.Arrow {
    public class ArrowController : FlowFixedObserveableBehviour<ArrowState> {

        private LinkedListNode<MazeCell> currentCell;
        private Direction lastMove = Direction.Start, nextMove;
        private ArrowState currentState = ArrowState.Creating;
        private int sizeOfPath;
        private Animator animator;

        private PhotonController photonScript;
        private MazeController mazeScript;

        protected override IInvoke Init() {
            InitOnAwake();
            return GameFlowManager.Instance.Flow
               .When(State.GameRunning)
               .Then(Move)
               .Build();
        }

        private void InitOnAwake() {
            mazeScript = ObjectsManager.Instance.GetMazeScript();
            photonScript = ObjectsManager.Instance.GetPhotonScript();

            currentCell = photonScript.LastNodeCellFromPathToGoal;

            if(currentCell != null) {
                sizeOfPath = (int)Math.Ceiling(currentCell.List.Count * 0.15f);
                sizeOfPath = sizeOfPath < 5 ? 5 : sizeOfPath;
                NotifyCameraAboutResize(sizeOfPath);
                InitArrowPosition();
                animator = GetComponent<Animator>();
            } else {
                Debug.LogError("Cannot init start cell for arrow!");
            }
        }

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
                    transform.position = Vector3.Lerp(transform.position, targetPosition, 0.15f);
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

        private void NotifyCameraAboutResize(int sizeOfPath) {

            LinkedListNode<MazeCell> iterateCell = currentCell.Next;
            MazeCell photonPos = photonScript.CurrentMazeCell;
            float offset = mazeScript.LenghtOfCellSide / 2;
            Vector2 leftUpBound = new Vector2(iterateCell.Value.X - offset, iterateCell.Value.Y - offset);
            Vector2 rightDownBound = new Vector2(iterateCell.Value.X + offset, iterateCell.Value.Y + offset);
            Frame frame = new Frame(leftUpBound, rightDownBound);
            while(iterateCell != null && sizeOfPath > 0) {
                frame.TryResizeX(iterateCell.Value.X, offset);
                frame.TryResizeY(iterateCell.Value.Y, offset);
                sizeOfPath--;
                iterateCell = iterateCell.Next;
            }

            frame.TryResizeX(photonPos.X, offset);
            frame.TryResizeY(photonPos.Y, offset);

            ObjectsManager.Instance
                .GetCameraScript()
                .ResizeCameraTo(frame);
        }

        private void PrepareToDestroy() {
            if(currentCell == null || sizeOfPath == 0) {
                currentState = ArrowState.Destroying;
            } else if(currentCell.Next == null || sizeOfPath == 1) {
                animator.SetTrigger("Destroy");
            }
        }

        bool IsStateInAnimatorHasEnded(string stateName) {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }

        private void OnDestroy() {
            SetStateAndNotifyObservers(ArrowState.Ending);
        }

        private void SetStateAndNotifyObservers(ArrowState newState) {
            currentState = newState;
            NotifyObservers();
        }

        private void InitArrowPosition() {
            currentCell = currentCell.Next;
            transform.position = new Vector3(currentCell.Value.X, 1f, currentCell.Value.Y);
            MazeCell next = currentCell.Next?.Value;
            if(next == null) {
                transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            }
            nextMove = currentCell.Value.GetDirectionTo(next);
            lastMove = nextMove;
            transform.rotation = GetRotationByMove(lastMove);
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

        protected override ArrowState GetData() {
            return currentState;
        }
        
    }

}