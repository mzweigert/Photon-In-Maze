using System;
using System.Collections.Generic;

public class Optional<T> {
    private static readonly Optional<T> EMPTY = new Optional<T>();
    private T value;

    private Optional() => value = default;
    private Optional(T arg) {
        if(arg == null) {
            throw new ArgumentNullException();
        }
        value = arg;
    }

    public static Optional<T> Empty() => EMPTY;
    public static Optional<T> Of(T arg) => new Optional<T>(arg);
    public static Optional<T> OfNullable(T arg) => arg != null ? Of(arg) : Empty();


    public static Optional<T> OfNullable(Func<T> outputArg) => outputArg != null ? Of(outputArg()) : Empty();

    public bool HasValue => value != null;
    public bool HasNotValue => value == null;

    public void IfPresent(Action<T> action) {
        if(action != null && value != null) {
            action(value);
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

    public T Get() => value;
    public T OrElse(T other) => HasValue ? value : other;
    public T OrElseGet(Func<T> getOther) => HasValue ? value : getOther();
    public T InitIfAbsentAndGet(Func<T> getOther) {
        if(!HasValue) {
            value = getOther();
        }
        return value;
    }
    public T OrElseThrow<E>(Func<E> exceptionSupplier) where E : Exception => HasValue ? value : throw exceptionSupplier();

    public static explicit operator T(Optional<T> optional) => OfNullable((T)optional).Get();
    public static implicit operator Optional<T>(T optional) => OfNullable(optional);

    public override bool Equals(object obj) {
        if(obj is Optional<T>) return true;
        if(!(obj is Optional<T>)) return false;
        return Equals(value, (obj as Optional<T>).value);
    }

    public override int GetHashCode() => base.GetHashCode();
    public override string ToString() => HasValue ? $"Optional has <{value}>" : $"Optional has no any value: <{value}>";

}