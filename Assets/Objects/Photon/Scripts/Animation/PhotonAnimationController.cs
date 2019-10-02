using PhotonInMaze.Common.Flow;
using PhotonInMaze.Provider;
using UnityEngine;

namespace PhotonInMaze.Photon {
    internal partial class PhotonAnimationController : FlowUpdateBehaviour {

        private PhotonAnimationManager animationManager;

        public override void OnInit() {
            Animator animator = GetComponent<Animator>();
            PhotonConfiguration configuration = GetComponent<PhotonConfiguration>();
            animationManager = new PhotonAnimationManager();
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                .When(State.ShowPhoton)
                .Then(() => {
                    animationManager.ShowPhoton();
                    GameFlowManager.Instance.Flow.NextState();
                })
                .OrElseWhen(State.TurnOnPhotonLight)
                .Then(() => {
                    animationManager.TurnOnLight();
                    GameFlowManager.Instance.Flow.NextState();
                })
                .OrElseWhen(State.HidePhoton)
                .Then(() => {
                    if(animationManager.IsNotHidingAndTurningLightOff()) {
                        GameFlowManager.Instance.Flow.NextState();
                    }
                })    
                .Build();
        }


        public override int GetInitOrder() {
            return (int)InitOrder.PhotonAnimation;
        }

    }
}