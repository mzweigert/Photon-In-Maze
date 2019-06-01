using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PhotonInMaze.Common.Flow {
    public abstract class FlowBehaviour : MonoBehaviour {
        protected IInvoke action;
        private IGetCurrentState currentState;
        void Start() {
            action = Init();
            IEnumerable<IGetCurrentState> getter = FindObjectsOfType<MonoBehaviour>().OfType<IGetCurrentState>();
            if(getter.Count() > 1) {
                Debug.LogWarning("Scene has many mono behaviours objects which implements IGetCurrentState." +
                    " Only one will be taken into account.");

            } else if(getter.Count() <= 0) {
                Debug.LogError("No mono behaviours objects which implements IGetCurrentState in scene!");
                throw new MissingMemberException();
            }
            currentState = getter.First();
        }

        protected abstract IInvoke Init();

        void Update() {
            if(action != null && currentState != null) {
                action.Invoke(currentState.GetCurrentState());
            }
        }
    }

}
