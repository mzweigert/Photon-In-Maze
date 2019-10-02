using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Common.Model;
using PhotonInMaze.Provider;
using UnityEngine;


namespace PhotonInMaze.MazeLight {

    internal class AreaLightController : FlowObserverBehaviour<IPhotonMovementController, IPhotonState>, IAreaLightController {

        private AudioSource audioSource;

        private new Light light;
        private LightIntensityLerper dimLight, turnOnLight;
        [SerializeField]
        [Range(0.075f, 1.75f)]
        private float maxLightIntensity = 1.75f, minLightIntensity = 0.075f;

        private float onePercentLightInensity;

        private int pathToGoalCount = 1;
        private int lastPathToGoalIndex;
       

        public override void OnInit() {
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
                .Then(() => pathToGoalCount = MazeObjectsProvider.Instance.GetPathToGoalManager().GetPathToGoalSize())
                .Build();
        }


        public override void OnNext(IPhotonState state) {
            if(state.IndexOfLastCellInPathToGoal != lastPathToGoalIndex) {
                lastPathToGoalIndex = state.IndexOfLastCellInPathToGoal;
                float delta = onePercentLightInensity * (((float)lastPathToGoalIndex / pathToGoalCount) * 100f);
                light.intensity = minLightIntensity + (delta * 0.1f);
            }
        }

        public override int GetInitOrder() {
            return (int)InitOrder.DirectionalLight;
        }

    }
}