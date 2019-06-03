using PhotonInMaze.Common.Flow;
using PhotonInMaze.Game.GameCamera;
using PhotonInMaze.Game.Manager;
using PhotonInMaze.Game.Maze;
using PhotonInMaze.Game.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace PhotonInMaze.Game.Arrow {
    public partial class ArrowController : FlowFixedObserveableBehviour<ArrowState> {

        private LinkedListNode<MazeCell> currentCell;
        private Direction lastMove = Direction.Start, nextMove;
        private ArrowState currentState = ArrowState.Creating;
        private int sizeOfPath;
        private Animator animator;

        private PhotonController photonScript;
        private MazeController mazeScript;

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
               .When(State.GameRunning)
               .Then(Move)
               .Build();
        }

        public override void OnStart() {
            mazeScript = MazeObjectsManager.Instance.GetMazeScript();
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

        public override int GetInitOrder() {
            return InitOrder.Arrow;
        }
    }

}