using PhotonInMaze.Common;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Model;
using PhotonInMaze.Provider;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PhotonInMaze.Photon {
    internal class PhotonMovementQueue {

        private IMazeCell lastSaved;
        private Queue<TargetMazeCell> movementsToMake = new Queue<TargetMazeCell>();

        private IMazeCellManager mazeCellManager;
        private IMazeController mazeController;
        private PhotonAnimationManager animationManager;

        public PhotonMovementQueue() {
            this.animationManager = new PhotonAnimationManager();
            this.lastSaved = MazeObjectsProvider.Instance.GetMazeCellManager().GetStartCell();
            this.mazeCellManager = MazeObjectsProvider.Instance.GetMazeCellManager();
            this.mazeController = MazeObjectsProvider.Instance.GetMazeController();
        }

        internal void SaveMove(TouchMovement movementDirection) {

            if(movementDirection == TouchMovement.Left && !lastSaved.Walls.Contains(Direction.Back)) {

                PushMove(lastSaved.Row - 1, lastSaved.Column, MovementEvent.Move);

            } else if(movementDirection == TouchMovement.Right && !lastSaved.Walls.Contains(Direction.Front) && !lastSaved.IsGoal) {

                PushMove(lastSaved.Row + 1, lastSaved.Column, MovementEvent.Move);

            } else if(movementDirection == TouchMovement.Up && !lastSaved.Walls.Contains(Direction.Left)) {

                PushMove(lastSaved.Row, lastSaved.Column - 1, MovementEvent.Move);

            } else if(movementDirection == TouchMovement.Down && !lastSaved.Walls.Contains(Direction.Right)) {

                PushMove(lastSaved.Row, lastSaved.Column + 1, MovementEvent.Move);

            }
        }

        internal void PushMove(int row, int column, MovementEvent movementEvent) {
            IMazeCell newCell = mazeCellManager.GetMazeCell(row, column);

            bool cantPushNewMove = mazeController.IsWhiteHolePosition(newCell) && movementEvent == MovementEvent.Move;
            if(cantPushNewMove) {
                return;
            }
            lastSaved = newCell;
            Action actionBeforeMove = animationManager.MakeActionOnNewMove(newCell, movementEvent);
            TargetMazeCell target = new TargetMazeCell(newCell, movementEvent, actionBeforeMove);
            movementsToMake.Enqueue(target);
        }

        internal TargetMazeCell Dequeue() {
            return movementsToMake.Dequeue();
        }

        internal bool IsNotEmpty() {
            return movementsToMake.Count > 0;
        }
    }
}