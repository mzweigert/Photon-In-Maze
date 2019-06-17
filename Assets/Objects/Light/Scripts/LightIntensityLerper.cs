using System;
using UnityEngine;

namespace PhotonInMaze.MazeLight {
    internal class LightIntensityLerper {

        private const float speed = 7.5f;

        private Light light;
        private float targetLightIntensity;
        private Action onReachTargetIntensity;
        private bool lerpDone;

        public LightIntensityLerper(Light light, float targetLightIntensity, Action onReachTargetIntensity = null) {
            this.light = light;
            this.targetLightIntensity = targetLightIntensity;
            this.onReachTargetIntensity = onReachTargetIntensity;
        }

        public void Invoke() {
            light.intensity = Mathf.Lerp(light.intensity, targetLightIntensity, Time.deltaTime * speed);
            if(Mathf.Abs(light.intensity - targetLightIntensity) <= 0.1f && !lerpDone) {
                light.intensity = targetLightIntensity;
                onReachTargetIntensity?.Invoke();
                lerpDone = true;
            }
        }

        public bool IsDone() {
            return light.intensity.Equals(targetLightIntensity);
        }
    }
}