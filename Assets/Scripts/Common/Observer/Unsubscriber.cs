using System;
using System.Collections.Generic;

internal class Unsubscriber<DataType> : IDisposable {

    private HashSet<IObserver<DataType>> _observers;
    private IObserver<DataType> _observer;

    internal Unsubscriber(HashSet<IObserver<DataType>> observers, IObserver<DataType> observer) {
        this._observers = observers;
        this._observer = observer;
    }

    public void Dispose() {
        if(_observers.Contains(_observer)) {
            _observers.Remove(_observer);
        }
    }
}