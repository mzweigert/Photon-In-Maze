using PhotonInMaze.Common.Flow;
using PhotonInMaze.Game.Manager;
using PhotonInMaze.Game.Maze;
using PhotonInMaze.Game.Photon;
using UnityEngine;


namespace PhotonInMaze.Game.MazeLight {
    public class LightController : FlowObserverBehaviour<PhotonController, PhotonState> {

        private MazeController mazeController;
        private new Light light;
        private bool audioPlayed = false;
        private AudioSource audioSource;

        private float maxLightIntensity = 1.75f;
        private float minLightIntensity = 0.075f;
        private float onePercentLightInensity;

        private int pathToGoalCount;
        private int lastPathToGoalIndex;

        public override void OnNext(PhotonState state) {
            if(state.IndexOfLastCellInPathToGoal != lastPathToGoalIndex) {
                lastPathToGoalIndex = state.IndexOfLastCellInPathToGoal;
                float delta = onePercentLightInensity * (((float)lastPathToGoalIndex / pathToGoalCount) * 100f);
                light.intensity = minLightIntensity + (delta * 0.1f);
            }
        }

        protected override IInvoke Init() {
            mazeController = ObjectsManager.Instance.GetMazeScript();
            audioSource = GetComponent<AudioSource>();
            light = GetComponent<Light>();
            onePercentLightInensity = (maxLightIntensity - minLightIntensity) / 100;
            light.intensity = maxLightIntensity;
            pathToGoalCount = mazeController.PathsToGoal.Count;

            return GameFlowManager.Instance.Flow
                .When(State.TurnOffLight)
                .Then(InitLight)
                .Build();
        }

        private void InitLight() {
            if(light.intensity >= minLightIntensity) {
                light.intensity -= Time.deltaTime * 7.5f;
                if(!audioPlayed) {
                    audioPlayed = true;
                    audioSource.Play();
                }
            } else {
                light.intensity = minLightIntensity;
                GameFlowManager.Instance.Flow.NextState();
            }
        }

    }
}