using System;

public class RepeatedlyTriggeredEvent : ICameraEvent {

    private bool isDone = false;
    private readonly Func<bool> camEvent;

    private RepeatedlyTriggeredEvent(Func<bool> camEvent) {
        this.camEvent = camEvent;
    }

    public static ICameraEvent Of(Func<bool> camEvent) {
        return new RepeatedlyTriggeredEvent(camEvent);
    }

    public bool IsDone() {
        return isDone;
    }

    public void Run() {
        if(!isDone) {
            isDone = camEvent.Invoke();
        }
    }

}