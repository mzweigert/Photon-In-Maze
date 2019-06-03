using PhotonInMaze.Common.Flow;
using PhotonInMaze.Game.Manager;
using PhotonInMaze.Game.Photon;
using UnityEngine;


namespace PhotonInMaze.Game.MazeLight {
    public class AreaLightController : FlowObserverBehaviour<PhotonController, PhotonState> {

        private AudioSource audioSource;

        private new Light light;
        private LightIntensityLerper dimLight, turnOnLight;
        private float maxLightIntensity = 1.75f, minLightIntensity = 0.075f;
        private float onePercentLightInensity;

        private int pathToGoalCount = 1;
        private int lastPathToGoalIndex;
       

        public override void OnStart() {
            audioSource = GetComponent<AudioSource>();
            light = GetComponent<Light>();
            onePercentLightInensity = (maxLightIntensity - minLightIntensity) / 100;
            light.intensity = maxLightIntensity;
            dimLight = new LightIntensityLerper(light, minLightIntensity, () => {
                audioSource.Play();
                GameFlowManager.Instance.Flow.NextState();
            });
            turnOnLight = new LightIntensityLerper(light, maxLightIntensity, GameFlowManager.Instance.Flow.NextState);
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                .When(State.DimAreaLight)
                .Then(dimLight.Invoke)
                .OrElseWhen(State.MazeCreated)
                .Then(() => pathToGoalCount = MazeObjectsManager.Instance.GetMazeScript().PathsToGoal.Count)
                .Build();
        }


        public override void OnNext(PhotonState state) {
            if(state.IndexOfLastCellInPathToGoal != lastPathToGoalIndex) {
                lastPathToGoalIndex = state.IndexOfLastCellInPathToGoal;
                float delta = onePercentLightInensity * (((float)lastPathToGoalIndex / pathToGoalCount) * 100f);
                light.intensity = minLightIntensity + (delta * 0.1f);
            }
        }

        public override int GetInitOrder() {
            return InitOrder.DirectionalLight;
        }

    }
}