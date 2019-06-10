using PhotonInMaze.Common;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;

namespace PhotonInMaze.Arrow {
    public partial class ArrowController : FlowFixedObserveableBehviour<ArrowState>, IArrowController {

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