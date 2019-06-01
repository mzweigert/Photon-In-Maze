using UnityEngine;

namespace PhotonInMaze.Common {
    public abstract class SceneSingleton<T> : MonoBehaviour where T : Object {
        private static T _instance;

        public static T Instance {
            get {
                if(_instance == null) {
                    _instance = FindObjectOfType<T>();
                }
                return _instance;
            }
        }
    }
}
