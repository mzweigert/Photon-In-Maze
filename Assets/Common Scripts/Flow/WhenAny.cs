using System;

namespace PhotonInMaze.Common.Flow {
    internal class WhenAny : IThenAny, IInvoke, IBuild {

        private Action action;
        private Func<bool> predicate;

        public WhenAny() {
            this.predicate = () => true;
        }
        public WhenAny(Func<bool> predicate) {
            this.predicate = predicate;
        }

        public IInvoke Build() {
            return this;
        }

        public void Invoke(State currentState = State.GenerateMaze) {
            if(predicate.Invoke()) {
                action?.Invoke();
            }
        }

        public IBuild ThenDo(Action action) {
            this.action = action;
            return this;
        }
    }

    public interface IWhenAny {
        IThenAny WhenIsAny();

        IThenAny WhenIsAnyAnd(Func<bool> predicate);
    }

    public interface IThenAny {
        IBuild ThenDo(Action action);
    }

    public interface IBuild {
        IInvoke Build();
    }
}