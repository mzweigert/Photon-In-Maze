using PhotonInMaze.Common;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Game.Arrow;
using PhotonInMaze.Game.GameCamera;
using PhotonInMaze.Game.MazeLight;
using PhotonInMaze.Game.Photon;
using UnityEngine;

namespace PhotonInMaze.Game.Manager {
    public class ObjectsManager : SceneSingleton<ObjectsManager> {

        [Range(1, 255)]
        [SerializeField]
        private byte _initialArrowHintsCount;

        public byte ArrowHintsCount { get; private set; }

        private ArrowObserver arrowObserver;

        private ObjectsManager() { }

        private void Start() {
            arrowObserver = new ArrowObserver();
            ArrowHintsCount = _initialArrowHintsCount;
        }

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

        public CameraController GetCameraScript() {
            var script = areaCamera.GetComponent<CameraController>();
            LogIfObjectIsNull(script, "CameraController");
            return script;

        }
        public Light GetAreaLight() {
            LogIfObjectIsNull(areaLight, "AreaLight");
            return areaLight;
        }

        public AreaLightController GetAreaLightScript() {
            var script = areaLight.GetComponent<AreaLightController>();
            LogIfObjectIsNull(script, "AreaLightController");
            return script;
        }

        public GameObject GetPhoton() {
            LogIfObjectIsNull(photon, "Photon");
            return photon;
        }

        public PhotonController GetPhotonScript() {
            var script = photon.GetComponent<PhotonController>();
            LogIfObjectIsNull(script, "PhotonConroller");
            return script;
        }

        public GameObject GetArrow() {
            LogIfObjectIsNull(arrow, "Arrow");
            return arrow;
        }

        public ArrowController GetArrowScript() {
            var script = arrow.GetComponent<ArrowController>();
            LogIfObjectIsNull(script, "ArrowController");
            return script;
        }

        public bool IsArrowPresent() {
            return arrowObserver.ArrowIsPresent;
        }

        public byte SpawnArrow() {
            if(ArrowHintsCount > 0) {
                var newArrow = Instantiate(arrow, MazeObjectsManager.Instance.GetMaze().transform);
                newArrow.name = "Arrow";
                arrowObserver.Subscribe(newArrow);
                ArrowHintsCount--;
            }
            return ArrowHintsCount;
        }

        private void LogIfObjectIsNull(Object o, string name) {
            if(o == null) {
                Debug.LogError(name + " is null!");
            }
        }

        public void ReinitializeArrowHintsCount() {
            if(GameFlowManager.Instance.Flow.Is(State.EndGame)) {
                ArrowHintsCount = _initialArrowHintsCount;
            }
        }
    }
}