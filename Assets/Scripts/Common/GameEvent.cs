using System;

public class GameEvent {

    public bool IsGameRunning { get; private set; }
    public bool CanTurnOffLight { get; private set; }
    public bool IsLightTurnedOff { get; private set; }

    private GameEvent() {
        IsGameRunning = false;
        CanTurnOffLight = false;
    }

    private static GameEvent instance;
    public static GameEvent Instance {
        get {
            if(instance == null) {
                instance = new GameEvent();
            }
            return instance;
        }
    }

    

    public void TryTurnOffLight() {
        if(CanTurnOffLight || IsLightTurnedOff) {
            throw new InvalidOperationException("Light already turned off!");
        }
        CanTurnOffLight = true;
    }

    public void LightTurnedOff() {
        if(IsLightTurnedOff) {
            throw new InvalidOperationException("Light already turned off!");
        }
        IsLightTurnedOff = true;
    }

    public void StartGame() {
        if(IsGameRunning) {
            throw new InvalidOperationException("Game already running!");
        }
        IsGameRunning = true;
    }

    public void CheckIfGameRunningAndCallUpdate(Action action) {
        if(IsGameRunning) {
            action.Invoke();
        }
    }
}
