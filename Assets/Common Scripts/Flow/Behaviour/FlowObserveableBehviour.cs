using PhotonInMaze.Common.Observer;
using System;
using System.Collections.Generic;

namespace PhotonInMaze.Common.Flow {
    public abstract class FlowObserveableBehviour<DataType> : FlowUpdateBehaviour, IObservable<DataType> {

        private HashSet<IObserver<DataType>> observers = new HashSet<IObserver<DataType>>();

        public IDisposable Subscribe(IObserver<DataType> observer) {
            if(!observers.Contains(observer)) {
                observers.Add(observer);
                // Provide observer with existing data.
                observer.OnNext(GetData());
            }
            return new Unsubscriber<DataType>(observers, observer);
        }

        protected void NotifyObservers() {
            foreach(IObserver<DataType> observer in observers) {
                observer.OnNext(GetData());
            }
        }

        protected abstract DataType GetData();

    }
}