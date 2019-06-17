using System.Collections.Generic;
using UnityEngine;
using System;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Model;
using PhotonInMaze.Common;

namespace PhotonInMaze.Photon {
    internal partial class PhotonController : FlowObserveableBehviour<IPhotonState>, IPhotonController {

        private Queue<TargetMazeCell> movementsToMake = new Queue<TargetMazeCell>();

        private TargetMazeCell currentTargetMazeCell;
        public IMazeCell CurrentMazeCell { get { return currentTargetMazeCell.value; } }

        private IMazeCell lastSaved;
        private readonly float minDistanceToNextMove = 0.1f;
        private readonly float teleportingSpeed = 0.5f;

        private void ChangePositionInfoInPathToGoal(TargetMazeCell targetCell) {
            LinkedListNode<IMazeCell> first = pathToGoalManager.GetFirstFromPath();
            if(first.Next != null && targetCell.value.Equals(first.Next.Value)) {

                pathToGoalManager.RemoveFirst();
                if(targetCell.value.IsProperPathToGoal) {
                    photonState.IndexOfLastCellInPathToGoal++;
                }
                return;
            } 
            
            if(targetCell.movementEvent == MovementEvent.Move) {
                pathToGoalManager.AddFirst(targetCell.value);
                if(targetCell.value.IsProperPathToGoal) {
                    photonState.IndexOfLastCellInPathToGoal--;
                }
            } else if(targetCell.movementEvent == MovementEvent.Teleport) {

                IMazeCell firstFromProperPathToGoal = pathToGoalManager.FindPathToGoalFrom(targetCell.value);
                if(firstFromProperPathToGoal != null) {
                    int index = pathToGoalManager.IndexInPath(firstFromProperPathToGoal);
                    photonState.IndexOfLastCellInPathToGoal = index;
                }

            }
        }

        private void TryMakeMove() {
            if(movementsToMake.Count > 0 && !photonState.IsAcutallyMoving) {
                currentTargetMazeCell = movementsToMake.Dequeue();
                currentTargetMazeCell.TryInvokeAction();
                photonState.IsAcutallyMoving = true;
                ChangePositionInfoInPathToGoal(currentTargetMazeCell);
            } else if(photonState.IsAcutallyMoving) {
                MakeMove();
            }
        }

        private void MakeMove() {
            Vector3 targetPosition = new Vector3(CurrentMazeCell.X, transform.position.y, CurrentMazeCell.Y);

            switch(currentTargetMazeCell.movementEvent) {
                case MovementEvent.Move:
                    transform.position = Vector3.Lerp(transform.position, targetPosition, PhotonSpeed);
                    if(Vector3.Distance(transform.position, targetPosition) <= minDistanceToNextMove) {
                        transform.position = targetPosition;
                        photonState.IsAcutallyMoving = false;
                        if(currentTargetMazeCell.IsGoal) {
                            GameFlowManager.Instance.Flow.NextState();
                        }
                    }
                    photonState.RealPosition = transform.position;
                    break;
                case MovementEvent.Teleport:
                    photonState.RealPosition = transform.position = targetPosition;
                    photonState.IsAcutallyMoving = false;
                    break;
                case MovementEvent.ExitFromWormhole:
                    PhotonSpeed *= (1 / teleportingSpeed);
                    photonState.IsAcutallyMoving = false;
                    break;
            }

            NotifyObservers();
        }

        private void SaveMove(TouchMovement movementDirection) {
            if(mazeController == null) {
                return;
            }

            if(movementDirection == TouchMovement.Left && !lastSaved.Walls.Contains(Direction.Back)) {

                PushToQueueMoves(lastSaved.Row - 1, lastSaved.Column, MovementEvent.Move);

            } else if(movementDirection == TouchMovement.Right && !lastSaved.Walls.Contains(Direction.Front) && !lastSaved.IsGoal) {

                PushToQueueMoves(lastSaved.Row + 1, lastSaved.Column, MovementEvent.Move);

            } else if(movementDirection == TouchMovement.Up && !lastSaved.Walls.Contains(Direction.Left)) {

                PushToQueueMoves(lastSaved.Row, lastSaved.Column - 1, MovementEvent.Move);

            } else if(movementDirection == TouchMovement.Down && !lastSaved.Walls.Contains(Direction.Right)) {

                PushToQueueMoves(lastSaved.Row, lastSaved.Column + 1, MovementEvent.Move);

            }
        }

        private void PushToQueueMoves(int row, int column, MovementEvent movementEvent) {
            IMazeCell newCell = mazeCellManager.GetMazeCell(row, column);

            bool cantPushNewMove = mazeController.IsWhiteHolePosition(newCell) && movementEvent == MovementEvent.Move;
            if(cantPushNewMove) {
                return;
            }
            lastSaved = newCell;
            Action actionBeforeMove = MakeActionOnNewMove(newCell, movementEvent);
            TargetMazeCell target = new TargetMazeCell(newCell, movementEvent, actionBeforeMove);
            movementsToMake.Enqueue(target);
        }

        private Action MakeActionOnNewMove(IMazeCell newCell, MovementEvent movementEvent) {
            bool isBlackHole = mazeController.IsBlackHolePosition(newCell) && movementEvent == MovementEvent.Move;
            if(isBlackHole) {
                return () => {
                    animator.SetTrigger("TurnOffLight");
                    animator.SetTrigger("Hide");
                    PhotonSpeed *= teleportingSpeed;
                };
            };
            bool isOutsideWhiteHole = mazeController.IsWhiteHolePosition(newCell) &&
                 movementEvent == MovementEvent.Teleport;
            if(isOutsideWhiteHole) {
                return () => {
                    animator.SetTrigger("TurnOnLight");
                    animator.SetTrigger("Show");
                };
            }

            return null;
        }

        public Vector3 GetInitialPosition() {
            return initialPosition;
        }

        public IMazeCell GetCurrentMazeCellPosition() {
            return CurrentMazeCell;
        }

    }
}