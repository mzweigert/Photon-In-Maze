using System.Collections.Generic;
using UnityEngine;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Model;
using PhotonInMaze.Provider;

namespace PhotonInMaze.Photon {
    internal class PhotonMovementController : FlowObserveableBehviour<IPhotonState>, IPhotonMovementController {

        private readonly float minDistanceToNextMove = 0.1f;

        private TargetMazeCell currentTargetMazeCell;
        public IMazeCell CurrentMazeCell { get { return currentTargetMazeCell.value; } }

        private IPathToGoalManager pathToGoalManager;
        private PhotonState photonState;

        private PhotonConfiguration configuration;
        private PhotonInputControl inputControl;

        internal PhotonMovementQueue Queue { get; private set; }

        public override void OnInit() {
            configuration = GetComponent<PhotonConfiguration>();
            Queue = new PhotonMovementQueue();
            inputControl = new PhotonInputControl(Queue);
            pathToGoalManager = MazeObjectsProvider.Instance.GetPathToGoalManager();
            IMazeCell startCell = MazeObjectsProvider.Instance.GetMazeCellManager().GetStartCell();
            currentTargetMazeCell = new TargetMazeCell(startCell, MovementEvent.Idle);
            transform.position = configuration.InitialPosition;
            photonState = new PhotonState(transform.position);
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                .When(State.GameRunning)
                .Then(WaitForMove)
                .Build();
        }

        private void WaitForMove() {
            if(CanvasObjectsProvider.Instance.GetArrowButtonController().IsArrowPresent()) {
                return;
            }
            TryMakeMove();

#if UNITY_EDITOR
            inputControl.CheckButtonPress();
#endif
            inputControl.CheckTouch();
        }

        public override int GetInitOrder() {
            return (int)InitOrder.PhotonMovement;
        }

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
            if(Queue.IsNotEmpty() && !photonState.IsAcutallyMoving) {
                currentTargetMazeCell = Queue.Dequeue();
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
                    transform.position = Vector3.LerpUnclamped(transform.position, targetPosition, configuration.Speed);
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
                    configuration.IncraseSpeed() ;
                    photonState.IsAcutallyMoving = false;
                    break;
            }

            NotifyObservers();
        }

        protected override IPhotonState GetData() {
            return photonState;
        }
    }
}