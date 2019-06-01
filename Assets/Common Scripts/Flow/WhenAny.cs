using System;

namespace PhotonInMaze.Common.Flow {
    internal class WhenAny : IThenAny, IInvoke, IBuild {

        private Action action;

        public IInvoke Build() {
            return this;
        }

        public void Invoke(State currentState = State.Start) {
            action?.Invoke();
        }

        public IBuild ThenDo(Action action) {
            this.action = action;
            return this;
        }
    }

    public interface IWhenAny {
        IThenAny WhenIsAny();
    }

    public interface IThenAny {
        IBuild ThenDo(Action action);
    }

    public interface IBuild {
        IInvoke Build();
    }
}