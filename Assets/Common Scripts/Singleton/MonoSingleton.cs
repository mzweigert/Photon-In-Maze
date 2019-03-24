using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour, System.IDisposable where T : Object {
    private static T _instance;

    public static T Instance {
        get {
            if(_instance == null) {
                _instance = FindObjectOfType<T>();
            }
            return _instance;
        }
    }

    public void Dispose() {
        Destroy(_instance);
    }
}