using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhotonInMaze.Common.Flow {
    
    public class GameFlowManager : SceneSingleton<GameFlowManager>, IFlowManager {

        private bool isStartInvoked = false;
        public GameFlow Flow { get; } = new GameFlow();

        private SortedList flowBehavioursToInitAfterStart = new SortedList(
            Comparer<int>.Create((first, second) => {
                int result = first.CompareTo(second);
                return result == 0 ? 1 : result;
            })
        );

        public void RegisterFlowBehaviour(IFlowBehaviour flowBehaviour) {
            if(isStartInvoked) {
                flowBehaviour.Prepare();
            } else {
                flowBehavioursToInitAfterStart.Add(flowBehaviour.GetInitOrder(), flowBehaviour);
            }
        }

        public IEnumerator Start() {
            yield return new WaitForEndOfFrame();
            foreach(IFlowBehaviour flowBehaviour in flowBehavioursToInitAfterStart.Values) {
                flowBehaviour.Prepare();
            }
            isStartInvoked = true;
        }

        public State GetCurrentState() {
            return Flow.CurrentState;
        }

        public void ReinitializeFlowBehaviours() {
            Flow.NextState();
            foreach(IFlowBehaviour flowBehaviour in flowBehavioursToInitAfterStart.Values) {
                flowBehaviour.Prepare();
            }
        }
    }
}