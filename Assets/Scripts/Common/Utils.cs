using System;

public class Utils {
   
    private Utils() { }

    public static void CheckIfGameRunningAndCallUpdate(Action update) {
        if(!Values.Instance.IsGameRunning) {
            return;
        }

        update.Invoke();
    }
}