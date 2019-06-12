using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Common.Model;
using PhotonInMaze.Provider;
using UnityEngine;

namespace PhotonInMaze.Photon {
    public partial class PhotonController : FlowObserveableBehviour<IPhotonState>, IPhotonController {

        private IMazeController mazeController;
        private IPathToGoalManager pathToGoalManager;
        private IMazeCellManager mazeCellManager;

        private PhotonState photonState;

        private Animator animator;
        private Vector3 initialPosition { get; } = new Vector3(0, 2, 0);

        [Range(0.1f, 2f)]
        public float PhotonSpeed;

        protected override IPhotonState GetData() {
            return photonState;
        }

        public override void OnInit() {
            mazeController = MazeObjectsProvider.Instance.GetMazeController();
            pathToGoalManager = MazeObjectsProvider.Instance.GetPathToGoalManager();
            mazeCellManager = MazeObjectsProvider.Instance.GetMazeCellManager();
            animator = GetComponent<Animator>();
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                .When(State.ShowPhoton)
                .Then(() => {
                    AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(1);
                    if(info.IsName("Hide")) {
                        animator.SetTrigger("Show");
                    }
                    GameFlowManager.Instance.Flow.NextState();
                })
                .OrElseWhen(State.TurnOnPhotonLight)
                .Then(() => {
                    animator.SetTrigger("TurnOnLight");
                    GameFlowManager.Instance.Flow.NextState();
                })
                .OrElseWhen(State.HidePhoton)
                .Then(() => {
                    bool isHiding = IsAnmationPlaying(1, "Hide");
                    bool isTurningLightOff = IsAnmationPlaying(2, "TurnOffLight");
                    if(!isHiding && !isTurningLightOff) {
                        GameFlowManager.Instance.Flow.NextState();
                    }
                })
                .OrElseWhen(State.MazeCreated)
                .Then(InitPhotonPosition)
                .OrElseWhen(State.GameRunning)
                .Then(WaitForMove)
                .Build();
        }

        private bool IsAnmationPlaying(int layer, string transitionName) {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(layer);
            if(!animator.IsInTransition(layer) && !info.IsName(transitionName)) {
                animator.SetTrigger(transitionName);
                return true;
            } else if(animator.IsInTransition(layer)) {
                return true;
            }
            return info.length > info.normalizedTime && info.IsName(transitionName);
        }

        private void InitPhotonPosition() {
            currentTargetMazeCell = new TargetMazeCell(mazeCellManager.GetStartCell(), MovementEvent.Idle);
            lastSaved = currentTargetMazeCell.value;
            transform.position = initialPosition;
            photonState = new PhotonState(initialPosition);
        }

        private void WaitForMove() {
            if(CanvasObjectsProvider.Instance.GetArrowButtonController().IsArrowPresent()) {
                return;
            }
            TryMakeMove();

#if UNITY_EDITOR
            CheckButtonPress();
#endif
            CheckTouch();
        }

        public override int GetInitOrder() {
            return InitOrder.Photon;
        }

    }
}