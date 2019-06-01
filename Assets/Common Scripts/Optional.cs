using System;

namespace PhotonInMaze.Common {
    public class Optional<T> {
        private static readonly Optional<T> EMPTY = new Optional<T>();
        private T value;

        private Optional() { }
        private Optional(T arg) {
            if(arg == null) {
                throw new ArgumentNullException();
            }
            value = arg;
        }

        public static Optional<T> Empty() { return EMPTY; }
        public static Optional<T> Of(T arg) { return new Optional<T>(arg); }
        public static Optional<T> OfNullable(T arg) { return arg != null ? Of(arg) : Empty(); }


        public static Optional<T> OfNullable(Func<T> outputArg) { return outputArg != null ? Of(outputArg()) : Empty(); }

        public bool HasValue { get { return value != null; } }
        public bool HasNotValue { get { return value == null; } }

        public void IfPresent(Action<T> action) {
            if(action != null && value != null) {
                action(value);
            }
        }

        public void IfAbsent(Action action) {
            if(action != null && value == null) {
                action.Invoke();
            }
        }

        public void IfPresentOrElse(Action<T> actionIfpresent, Action actionElse) {
            if(actionIfpresent == null || actionElse == null) {
                throw new NullReferenceException();
            } else if(value != null) {
                actionIfpresent(value);
            } else {
                actionElse.Invoke();
            }
        }

        public T Get() { return value; }
        public T OrElse(T other) { return HasValue ? value : other; }
        public T OrElseGet(Func<T> getOther) { return HasValue ? value : getOther(); }
        public T InitIfAbsentAndGet(Func<T> getOther) {
            if(!HasValue) {
                value = getOther();
            }
            return value;
        }

        public T OrElseThrow<E>(Func<E> exceptionSupplier) where E : Exception { 
            if(HasValue){
                return value;
            } else {
                E exception = exceptionSupplier.Invoke();
                throw exception;
            }
        }

        public static explicit operator T(Optional<T> optional) { return OfNullable((T)optional).Get(); }
        public static implicit operator Optional<T>(T optional) { return OfNullable(optional); }

        public override bool Equals(object obj) {
            if(obj is Optional<T>) return true;
            if(!(obj is Optional<T>)) return false;
            return Equals(value, (obj as Optional<T>).value);
        }

        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return HasValue ? $"Optional has <{value}>" : $"Optional has no any value: <{value}>"; }

    }
}