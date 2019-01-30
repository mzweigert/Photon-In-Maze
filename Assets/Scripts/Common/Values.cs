
using System;
using UnityEngine;

public class Values {

    public bool IsGameRunning { get; private set; }

    private Values() {
        IsGameRunning = false;
    }

    private static Values instance;
    public static Values Instance {
        get {
            if(instance == null) {
                instance = new Values();
            }
            return instance;
        }
    }

    public void StartGame() {
        if(IsGameRunning) {
            throw new InvalidOperationException("Game already running!");
        }
        IsGameRunning = true;
    }

    public static class COLOR {
        public static Color32 Navy = new Color32(19, 26, 132, 255);
        public static Color32 Ocean = new Color32(1, 96, 100, 255);
        public static Color32 Cerulean = new Color32(4, 146, 194, 255);
    }
}