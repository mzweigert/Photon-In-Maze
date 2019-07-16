using PhotonInMaze.Common.Flow;
using UnityEngine;

namespace PhotonInMaze.CanvasGame.Background {
    internal class BackgroundController : FlowFixedUpdateBehaviour {

        private int lightLayer = 0;
        private Animator animator;

        public override void OnInit() {
            animator = GetComponent<Animator>();
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                           .When(State.TurnOnLight)
                           .Then(() => ChangeLight(lightLayer, "TurnOnLight"))
                           .OrElseWhen(State.TurnOffLight)
                           .Then(() => ChangeLight(lightLayer, "TurnOffLight"))
                           .Build();
        }

        public override int GetInitOrder() {
            return (int)InitOrder.Default;
        }

        private void ChangeLight(int layer, string transitionName) {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(layer);
            if(!animator.IsInTransition(layer) && !info.IsName(transitionName)) {
                animator.SetTrigger(transitionName);
            }
            if(info.length < info.normalizedTime && info.IsName(transitionName)) {
                GameFlowManager.Instance.Flow.NextState();
            }
        }
    }
}