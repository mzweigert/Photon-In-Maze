
namespace PhotonInMaze.Common.Flow {

    public class GameFlow : IWhen, IWhenAny {

        public State CurrentState { get; private set; } = State.Start;
        public State savedState = State.Start;

        public State NextState() {
            switch(CurrentState) {
                case State.Start:
                    CurrentState = State.CountingDown;
                    break;
                case State.CountingDown:
                    CurrentState = State.DestroyPathToGoal;
                    break;
                case State.DestroyPathToGoal:
                    CurrentState = State.TurnOffLight;
                    break;
                case State.TurnOffLight:
                    CurrentState = State.TurnOnPhotonLight;
                    break;
                case State.TurnOnPhotonLight:
                    CurrentState = State.GameRunning;
                    break;
                case State.GameRunning:
                    CurrentState = State.EndGame;
                    break;
                case State.EndGame:
                    break;
            }
            return CurrentState;
        }

        public void Pause() {
            if(IsNot(State.Pause)) {
                this.savedState = CurrentState;
                CurrentState = State.Pause;
            }
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
