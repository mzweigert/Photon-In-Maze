using System;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Game.Manager;
using PhotonInMaze.Game.Maze;
using UnityEngine;

namespace PhotonInMaze.Game.Photon {
    public partial class PhotonController : FlowObserveableBehviour<PhotonState> {

        private MazeController mazeController;
        private PhotonState photonState;

        private Animator animator;
        public Vector3 InitialPosition { get; } = new Vector3(0, 2, 0);

        [Range(0.1f, 2f)]
        public float PhotonSpeed;

        protected override PhotonState GetData() {
            return photonState;
        }

        public override void OnStart() {
            mazeController = MazeObjectsManager.Instance.GetMazeScript();
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
            LastNodeCellFromPathToGoal = mazeController.PathsToGoal.First;
            currentTargetMazeCell = new TargetMazeCell(LastNodeCellFromPathToGoal.Value, MovementEvent.Idle);
            lastSaved = currentTargetMazeCell.value;
            transform.position = InitialPosition;
            photonState = new PhotonState(InitialPosition);
        }

        private void WaitForMove() {
            if(ObjectsManager.Instance.IsArrowPresent()) {
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