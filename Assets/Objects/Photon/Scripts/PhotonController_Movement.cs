using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Game.Maze;

namespace PhotonInMaze.Game.Photon {
    public partial class PhotonController : FlowObserveableBehviour<PhotonState> {

        private Queue<TargetMazeCell> movementsToMake = new Queue<TargetMazeCell>();

        public LinkedListNode<MazeCell> LastNodeCellFromPathToGoal { get; private set; }

        private TargetMazeCell currentTargetMazeCell;
        public MazeCell CurrentMazeCell { get { return currentTargetMazeCell.value; } }

        private MazeCell lastSaved;
        private readonly float minDistanceToNextMove = 0.1f;
        private readonly float teleportingSpeed = 0.5f;

        private void ChangePositionInfoInPathToGoal(TargetMazeCell targetCell) {
            if(LastNodeCellFromPathToGoal == null) {
                photonState.IsInPathToGoal = false;
            } else if(targetCell.IsGoal) {
                print("Congratulations! You are finished maze!");
            } else if(LastNodeCellFromPathToGoal.Next != null && targetCell.value.Equals(LastNodeCellFromPathToGoal.Next.Value)) {
                LastNodeCellFromPathToGoal = LastNodeCellFromPathToGoal.Next;
                photonState.IndexOfLastCellInPathToGoal++;
            } else if(LastNodeCellFromPathToGoal.Previous != null && targetCell.value.Equals(LastNodeCellFromPathToGoal.Previous.Value)) {
                LastNodeCellFromPathToGoal = LastNodeCellFromPathToGoal.Previous;
                photonState.IndexOfLastCellInPathToGoal--;
            } else if(!targetCell.Equals(LastNodeCellFromPathToGoal.Value) && targetCell.movementEvent == MovementEvent.Teleport) {
                LinkedListNode<MazeCell> nearPathToGoal = mazeController.FindCellFromPathToGoalNear(targetCell.value);
                if(nearPathToGoal != null) {
                    LastNodeCellFromPathToGoal = nearPathToGoal;
                    int index = mazeController.PathsToGoal.IndexOf(nearPathToGoal.Value);
                    photonState.IndexOfLastCellInPathToGoal = index;

                }
            } else if(!targetCell.Equals(LastNodeCellFromPathToGoal.Value) && photonState.IsInPathToGoal) {
                photonState.IsInPathToGoal = false;
            } else if(!photonState.IsInPathToGoal) {
                photonState.IsInPathToGoal = true;
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
                    }
                    photonState.RealPosition = transform.position;
                    break;
                case MovementEvent.Teleport:
                    List<ParticleSystem> particles = GetComponentsInChildren<ParticleSystem>().ToList();
                    particles.ForEach(particle => particle.gameObject.SetActive(false));
                    photonState.RealPosition = transform.position = targetPosition;
                    photonState.IsAcutallyMoving = false;
                    particles.ForEach(particle => particle.gameObject.SetActive(true));
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

            switch(movementDirection) {
                case TouchMovement.Left:
                    if(!lastSaved.Walls.Contains(Direction.Back)) {
                        PushToQueueMoves(lastSaved.Row - 1, lastSaved.Column, MovementEvent.Move);
                    }
                    break;
                case TouchMovement.Right:
                    if(!lastSaved.Walls.Contains(Direction.Front) && !lastSaved.IsGoal) {
                        PushToQueueMoves(lastSaved.Row + 1, lastSaved.Column, MovementEvent.Move);
                    }
                    break;
                case TouchMovement.Up:
                    if(!lastSaved.Walls.Contains(Direction.Left)) {
                        PushToQueueMoves(lastSaved.Row, lastSaved.Column - 1, MovementEvent.Move);
                    }
                    break;
                case TouchMovement.Down:
                    if(!lastSaved.Walls.Contains(Direction.Right)) {
                        PushToQueueMoves(lastSaved.Row, lastSaved.Column + 1, MovementEvent.Move);
                    }
                    break;
            }
        }

        private void PushToQueueMoves(int row, int column, MovementEvent movementEvent) {
            mazeController
                .FindMazeCell(row, column)
                .IfPresent(newCell => {
                    bool cantPushNewMove = mazeController.IsWhiteHolePosition(newCell) && movementEvent == MovementEvent.Move;
                    if(cantPushNewMove) {
                        return;
                    }
                    lastSaved = newCell;
                    Action actionBeforeMove = MakeActionOnNewMove(newCell, movementEvent);
                    TargetMazeCell target = new TargetMazeCell(newCell, movementEvent, actionBeforeMove);
                    movementsToMake.Enqueue(target);
                });
        }

        private Action MakeActionOnNewMove(MazeCell newCell, MovementEvent movementEvent) {
            bool isBlackHole = mazeController.IsBlackHolePosition(newCell) && movementEvent == MovementEvent.Move;
            if(isBlackHole) {
                return () => {
                    GetComponent<Animator>().SetTrigger("InsideWormhole");
                    PhotonSpeed *= teleportingSpeed;
                };
            };
            bool isOutsideWhiteHole = mazeController.IsWhiteHolePosition(newCell) &&
                 movementEvent == MovementEvent.Teleport;
            if(isOutsideWhiteHole) {
                return () => GetComponent<Animator>().SetTrigger("OutsideWormhole");
            }

            return null;
        }
    }
}