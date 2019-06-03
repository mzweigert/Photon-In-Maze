
using System;
using System.Collections;
using UnityEngine;

namespace PhotonInMaze.Common.Flow {

    public class GameFlow : IWhen, IWhenAny {

        public State CurrentState { get; private set; } = State.GenerateMaze;
        public State savedState = State.Start;

        public void NextState() {
            switch(CurrentState) {
                case State.Start:
                    CurrentState = State.GenerateMaze;
                    break;
                case State.GenerateMaze:
                    CurrentState = State.MazeCreated;
                    break;
                case State.MazeCreated:
                    CurrentState = State.TurnOnLight;
                    break;
                case State.TurnOnLight:
                    CurrentState = State.ShowPhoton;
                    break;
                case State.ShowPhoton:
                    CurrentState = State.CountingDown;
                    break;
                case State.CountingDown:
                    CurrentState = State.DestroyPathToGoal;
                    break;
                case State.DestroyPathToGoal:
                    CurrentState = State.DimAreaLight;
                    break;
                case State.DimAreaLight:
                    CurrentState = State.TurnOnPhotonLight;
                    break;
                case State.TurnOnPhotonLight:
                    CurrentState = State.GameRunning;
                    break;
                case State.GameRunning:
                    CurrentState = State.HidePhoton;
                    break;
                case State.HidePhoton:
                    CurrentState = State.TurnOffLight;
                    break;
                case State.TurnOffLight:
                    CurrentState = State.EndGame;
                    break;
                case State.EndGame:
                    CurrentState = State.GenerateMaze;
                    break;
            }
        }

        public void Pause() {
            if(IsNot(State.Pause)) {
                this.savedState = CurrentState;
                CurrentState = State.Pause;
            }
        }

        public IEnumerator ChangeStateAfterFrameEnd() {
            yield return new WaitForEndOfFrame();
            NextState();
        }

        public void UnPause() {
            if(Is(State.Pause)) {
                CurrentState = this.savedState;
            }
        }

        public bool IsNot(State state) {
            return !Is(state);
        }

        public bool Is(State state) {
            return CurrentState == state;
        }

        public IThen When(State state) {
            return new When(state);
        }

        public IThenAny WhenIsAny() {
            return new WhenAny();
        }
    }

}
