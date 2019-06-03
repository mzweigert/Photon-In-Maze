using System;
using UnityEngine;

namespace PhotonInMaze.Common.Flow {
    public abstract class FlowObserverBehaviour<Controller, DataType> : FlowUpdateBehaviour, IObserver<DataType>
    where Controller : FlowObserveableBehviour<DataType> {

        private IDisposable unsubscriber;

        private new void Awake() {
            base.Awake();
            Controller controller = FindObjectOfType<Controller>();
            if(controller != null) {
                unsubscriber = controller.Subscribe(this);
            } else {
                Debug.LogError(string.Format("Cannot find {0} script!", controller));
            }
        }

        public void OnCompleted() {
            throw new NotImplementedException();
        }

        public void OnError(Exception error) {
            Debug.LogError(error.Message);
        }

        public abstract void OnNext(DataType state);

        public void Unsubscribe() {
            if(unsubscriber != null) {
                unsubscriber.Dispose();
            } else {
                Debug.LogError("Cannot unsubscribe, object is null!");
            }
        }

    }
}