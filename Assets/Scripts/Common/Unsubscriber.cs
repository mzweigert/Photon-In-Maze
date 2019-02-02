using System;
using System.Collections.Generic;

internal class Unsubscriber<PhotonInPathToGoalInfo> : IDisposable {
    private List<IObserver<PhotonInPathToGoalInfo>> _observers;
    private IObserver<PhotonInPathToGoalInfo> _observer;

    internal Unsubscriber(List<IObserver<PhotonInPathToGoalInfo>> observers, IObserver<PhotonInPathToGoalInfo> observer) {
        this._observers = observers;
        this._observer = observer;
    }

    public void Dispose() {
        if(_observers.Contains(_observer)) {
            _observers.Remove(_observer);
        }
    }
}