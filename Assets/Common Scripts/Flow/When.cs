using System;
using System.Collections.Generic;

namespace PhotonInMaze.Common.Flow {
    internal class When : IThen, IOrElse, IInvoke {

        private Dictionary<State, Action> stateByAction = new Dictionary<State, Action>();
        private Action orElseAction;
        private State possibleState;

        public When(State possibleState) {
            this.possibleState = possibleState;
        }

        public void Invoke(State currentState = State.GenerateMaze) {
            if(stateByAction.ContainsKey(currentState)) {
                stateByAction[currentState].Invoke();
            } else if(orElseAction != null) {
                orElseAction.Invoke();
            }
        }

        public IInvoke OrElse(Action action) {
            orElseAction = action;
            return this;
        }

        public IThen OrElseWhen(State possibleState) {
            this.possibleState = possibleState;
            return this;
        }

        public IOrElse Then(Action action) {
            stateByAction.Add(possibleState, action);
            return this;
        }

        public IInvoke Build() {
            return this;
        }

        public IInvoke ThenDo(Action action) {
            orElseAction = action;
            return this;
        }

    }

    public interface IWhen {
        IThen When(State state);
    }

    public interface IThen {
        IOrElse Then(Action action);
    }

    public interface IInvoke {
        void Invoke(State currentState = State.GenerateMaze);
    }

    public interface IOrElse {

        IInvoke OrElse(Action action);

        IThen OrElseWhen(State possibleState);

        IInvoke Build();
    }

}