using System;

public class GameFlow {

    public enum State {
        Start,
        CanTurnOffLight,
        LightTurnedOff,
        GameRunning
    }

    public State CurrentState { get; private set; } = State.Start;

    protected GameFlow() { }
    private static Lazy<GameFlow> instance = new Lazy<GameFlow>(() => new GameFlow());
    public static GameFlow Instance { get { return instance.Value; } }

    public void Reinitialize() {
        instance.Value.CurrentState = State.Start;
    }

    public void PrepareToTurnOffLight() {
        TryChangeCurrentState(State.Start, State.CanTurnOffLight);
    }

    public void TurnOffLight() {
        TryChangeCurrentState(State.CanTurnOffLight, State.LightTurnedOff);
    }

    public void StartGame() {
        TryChangeCurrentState(State.LightTurnedOff, State.GameRunning);
    }

    private void TryChangeCurrentState(State expected, State next) {
        if(Is(expected)) {
            CurrentState = next;
        } else {
            throw new InvalidOperationException(string.Format("Unsupported flow from {0} to {1}", CurrentState, next));
        }
    }

    public bool Is(State state) {
        return CurrentState == state;
    }

    public void CallUpdateWhenGameIsRunning(Action action) {
        if(CurrentState == State.GameRunning) {
            action.Invoke();
        }
    }

}
