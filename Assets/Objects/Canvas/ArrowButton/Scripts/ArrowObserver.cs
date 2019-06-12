using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common;
using UnityEngine;

namespace PhotonInMaze.CanvasGame.Arrow {
    internal class ArrowObserver : System.IObserver<ArrowState> {

        public bool ArrowIsPresent { get; private set; } = false;

        public ArrowObserver() { }

        public void Subscribe(GameObject newArrow) {
            if(newArrow == null) {
                Debug.LogError("Given newArrow is null!");
                return;
            }
            IArrowController controller = newArrow.GetComponent<IArrowController>();
            if(controller == null) {
                Debug.LogError("Given newArrow doesn't has ArrowController!");
                return;
            }
            controller.Subscribe(this);
            ArrowIsPresent = true;
        }

        public void OnNext(ArrowState state) {
            if(ArrowState.Ending == state) {
                ArrowIsPresent = false;
            }
        }

        public void OnCompleted() {
            throw new System.NotImplementedException();
        }

        public void OnError(System.Exception error) {
            Debug.LogError(error.Message);
        }
    }
}