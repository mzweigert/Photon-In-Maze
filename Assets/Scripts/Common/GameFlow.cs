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
        if(Is(State.Start)) {
            CurrentState = State.CanTurnOffLight;
        } else {
            throw new InvalidOperationException(string.Format("Unsupported flow fro {0} to {1}", CurrentState, State.CanTurnOffLight));
        }
    }

    public void TurnOffLight() {
        if(Is(State.CanTurnOffLight)) {
            CurrentState = State.LightTurnedOff;
        } else {
            throw new InvalidOperationException(string.Format("Unsupported flow fro {0} to {1}", CurrentState, State.LightTurnedOff));
        }
    }

    public void StartGame() {
        if(Is(State.LightTurnedOff)) {
            CurrentState = State.GameRunning;
        } else {
            throw new InvalidOperationException(string.Format("Unsupported flow fro {0} to {1}", CurrentState, State.GameRunning));
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
