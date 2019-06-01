using PhotonInMaze.Common.Flow;
using PhotonInMaze.Game.Manager;
using PhotonInMaze.Game.Maze;
using UnityEngine;

namespace PhotonInMaze.Game.Photon {
    public partial class PhotonController : FlowObserveableBehviour<PhotonState> {

        private MazeController mazeController;

        private PhotonState photonState;

        private Light photonLight;

        [Range(0.1f, 2f)]
        public float PhotonSpeed;

        protected override PhotonState GetData() {
            return photonState;
        }

        protected override IInvoke Init() {
            mazeController = ObjectsManager.Instance.GetMazeScript();
            LastNodeCellFromPathToGoal = mazeController.PathsToGoal.First;
            currentTargetMazeCell = new TargetMazeCell(LastNodeCellFromPathToGoal.Value, MovementEvent.Idle);
            lastSaved = currentTargetMazeCell.value;
            photonState = new PhotonState(transform.position);

            photonLight = GetComponentInChildren<Light>();
            photonLight.intensity = 0f;

            return GameFlowManager.Instance.Flow
                .When(State.TurnOnPhotonLight)
                .Then(() => {
                    photonLight.intensity = 7.5f;
                    GameFlowManager.Instance.Flow.NextState();
                })
                .OrElseWhen(State.GameRunning)
                .Then(WaitForMove)
                .Build();
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
    }
}