using PhotonInMaze.Common;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using UnityEngine;

namespace PhotonInMaze.Provider {
    public class ObjectsProvider : SceneSingleton<ObjectsProvider> {


        private ObjectsProvider() { }

        [SerializeField]
        private Camera areaCamera = null;

        [SerializeField]
        private Light areaLight = null;

        [SerializeField]
        private GameObject photon = null;

        [SerializeField]
        private GameObject arrow = null;

        public Camera GetAreaCamera() {
            LogIfObjectIsNull(areaCamera, "AreaCamera");
            return areaCamera;
        }

        public ICameraController GetCameraController() {
            var script = areaCamera.GetComponent<ICameraController>();
            LogIfObjectIsNull(script, "CameraController");
            return script;

        }
        public Light GetAreaLight() {
            LogIfObjectIsNull(areaLight, "AreaLight");
            return areaLight;
        }

        public IAreaLightController GetAreaLightController() {
            var script = areaLight.GetComponent<IAreaLightController>();
            LogIfObjectIsNull(script, "AreaLightController");
            return script;
        }

        public GameObject GetPhoton() {
            LogIfObjectIsNull(photon, "Photon");
            return photon;
        }

        public IPhotonController GetPhotonController() {
            var script = photon.GetComponent<IPhotonController>();
            LogIfObjectIsNull(script, "PhotonConroller");
            return script;
        }

        public GameObject GetArrow() {
            LogIfObjectIsNull(arrow, "Arrow");
            return arrow;
        }

        public IArrowController GetArrowController() {
            var script = arrow.GetComponent<IArrowController>();
            LogIfObjectIsNull(script, "ArrowController");
            return script;
        }


        private void LogIfObjectIsNull(object o, string name) {
            if(o == null) {
                Debug.LogError(name + " is null!");
            }
        }

    }
}