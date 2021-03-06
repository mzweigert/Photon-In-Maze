﻿
namespace PhotonInMaze.Common.Flow {
    public abstract class FlowFixedBehaviourSingleton<T> : FlowFixedUpdateBehaviour where T : UnityEngine.Object {
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
