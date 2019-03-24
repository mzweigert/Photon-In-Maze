using System;

public class OneShotEvent : ICameraEvent {

    private bool isDone = false;
    private readonly Action camEvent;

    private OneShotEvent(Action camEvent) {
        this.camEvent = camEvent;
    }

    public static ICameraEvent Of(Action camEvent) {
        return new OneShotEvent(camEvent);
    }

    public bool IsDone() {
        return isDone;
    }

    public void Run() {
        if(!isDone) {
            camEvent.Invoke();
            isDone = true;
        }
    }

}