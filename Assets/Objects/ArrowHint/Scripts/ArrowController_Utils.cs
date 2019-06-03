using PhotonInMaze.Common.Flow;
using PhotonInMaze.Game.GameCamera;
using PhotonInMaze.Game.Manager;
using PhotonInMaze.Game.Maze;
using PhotonInMaze.Game.Photon;
using System;

namespace PhotonInMaze.Game.Arrow {
    public partial class ArrowController : FlowFixedObserveableBehviour<ArrowState> {

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

        protected override ArrowState GetData() {
            return currentState;
        }

    }

}