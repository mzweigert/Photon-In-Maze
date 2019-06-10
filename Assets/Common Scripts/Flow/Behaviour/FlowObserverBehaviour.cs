using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PhotonInMaze.Common.Flow {
    public abstract class FlowObserverBehaviour<Controller, DataType> : FlowUpdateBehaviour, IObserver<DataType>
    where Controller : IObservable<DataType> {

        private List<IDisposable> unsubscribers = new List<IDisposable>();

        private new void Awake() {
            base.Awake();
            IEnumerable<Controller> controllers = FindObjectsOfType<FlowObserveableBehviour<DataType>>().OfType<Controller>();
            if(controllers != null) {
                foreach(Controller controller in controllers) {
                    IDisposable unsubscriber = controller.Subscribe(this);
                    unsubscribers.Add(unsubscriber);
                }
            } else {
                Debug.LogError(string.Format("Cannot find {0} script!", controllers));
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
            if(unsubscribers != null) {
                unsubscribers.ForEach(unsubscriber => unsubscriber.Dispose());
            } else {
                Debug.LogError("Cannot unsubscribe, object is null!");
            }
        }

    }
}