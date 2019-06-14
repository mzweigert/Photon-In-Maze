using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common;
using UnityEngine;
using System;

namespace PhotonInMaze.CanvasGame.Arrow {
    internal class ArrowObserver : System.IObserver<ArrowState> {

        public bool ArrowIsPresent { get; private set; } = false;
        private GameObject arrow;
        private IDisposable unsubscriber;

        public ArrowObserver() { }

        public void Subscribe(GameObject newArrow) {
            if(newArrow == null) {
                Debug.LogError("Given newArrow is null!");
                return;
            }
            if(arrow != null) {
                Debug.LogWarning("Arrow in scene exist!");
                return;
            }
            IArrowController controller = newArrow.GetComponent<IArrowController>();
            if(controller == null) {
                Debug.LogError("Given newArrow doesn't has ArrowController!");
                return;
            }
            unsubscriber = controller.Subscribe(this);
            ArrowIsPresent = true;
            this.arrow = newArrow;
        }

        public void RemoveArrow() {
            if(arrow != null) {
                unsubscriber.Dispose();
                UnityEngine.Object.Destroy(arrow);
            }
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