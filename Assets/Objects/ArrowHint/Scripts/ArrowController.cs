using PhotonInMaze.Common;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Common.Model;
using PhotonInMaze.Provider;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace PhotonInMaze.Arrow {
    internal partial class ArrowController : FlowFixedObserveableBehviour<ArrowState>, IArrowController {

        private LinkedListNode<IMazeCell> currentCell;
        private Direction lastMove = Direction.Start, nextMove;
        private ArrowState currentState = ArrowState.Creating;
        private int sizeOfPath;
        private Animator animator;

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
               .When(State.GameRunning)
               .Then(Move)
               .Build();
        }

        public override void OnInit() {
      
            currentCell = MazeObjectsProvider.Instance.GetPathToGoalManager().GetFirstFromPath();

            if(currentCell != null) {
                sizeOfPath = (int)Math.Ceiling(currentCell.List.Count * 0.20f);
                sizeOfPath = sizeOfPath < 5 ? 5 : sizeOfPath;
                NotifyCameraAboutResize(sizeOfPath);
                InitArrowPosition();
                animator = GetComponent<Animator>();
            } else {
                Debug.LogError("Cannot init start cell for arrow!");
            }
        }

        private void InitArrowPosition() {
            currentCell = currentCell.Next;
            transform.position = new Vector3(currentCell.Value.X, 1f, currentCell.Value.Y);
            IMazeCell next = currentCell.Next?.Value;
            if(next == null) {
                transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            }
            nextMove = currentCell.Value.GetDirectionTo(next);
            lastMove = nextMove;
            transform.rotation = GetRotationByMove(lastMove);
        }

        private void NotifyCameraAboutResize(int sizeOfPath) {

            LinkedListNode<IMazeCell> iterateCell = currentCell.Next;
            IMazeCell photonPos = ObjectsProvider.Instance.GetPhotonController().GetCurrentMazeCellPosition();
            float offset = MazeObjectsProvider.Instance.GetMazeConfiguration().LenghtOfCellSide / 2;
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

            ObjectsProvider.Instance
                .GetCameraController()
                .ResizeCameraTo(frame);
        }

        public override int GetInitOrder() {
            return InitOrder.Arrow;
        }

    }

}