using System;
using System.Collections;
using UnityEngine;

namespace PhotonInMaze.Common.Flow {

    public class GameFlow : IWhen, IWhenAny {

        public State CurrentState { get; private set; } = State.GenerateMaze;
        public State savedState = State.GenerateMaze;

        public void NextState() {
            if(Is(State.EndGame)) {
                CurrentState = 0;
            } else {
                CurrentState = CurrentState + 1;
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
