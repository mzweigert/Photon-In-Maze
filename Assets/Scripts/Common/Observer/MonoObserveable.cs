﻿using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoObserveable<DataType> : MonoBehaviour, IObservable<DataType> {

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