using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PhotonInMaze.Common.Flow {
    public abstract class FlowBehaviour : MonoBehaviour, IFlowBehaviour {

        private IFlowManager flowManager;
        private IInvoke action;

        protected void Awake() {
            IEnumerable<IFlowManager> flowManager = FindObjectsOfType<MonoBehaviour>().OfType<IFlowManager>();
            if(flowManager.Count() > 1) {
                Debug.LogWarning("Scene has many mono behaviours objects which implements IFlowManager." +
                    " Only one will be taken into account.");

            } else if(flowManager.Count() <= 0) {
                Debug.LogError("No mono behaviours objects which implements IFlowManager in scene!");
                throw new MissingMemberException();
            }
            this.flowManager = flowManager.First();
            this.flowManager.RegisterFlowBehaviour(this);
        }

        internal void TryInvokeLoop() {
            if(flowManager != null) {
                action?.Invoke(flowManager.GetCurrentState());
            }
        }

        public abstract void OnStart();
        public abstract IInvoke OnLoop();
        public abstract int GetInitOrder();

        public void Prepare() {
            OnStart();
            this.action = OnLoop();
        }
    }

}
