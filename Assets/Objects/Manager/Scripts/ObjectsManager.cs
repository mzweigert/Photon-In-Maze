using PhotonInMaze.Common;
using PhotonInMaze.Game.Arrow;
using PhotonInMaze.Game.GameCamera;
using PhotonInMaze.Game.Maze;
using PhotonInMaze.Game.MazeLight;
using PhotonInMaze.Game.Photon;
using UnityEngine;

namespace PhotonInMaze.Game.Manager {
    public class ObjectsManager : SceneSingleton<ObjectsManager> {

        private ObjectsManager() {
            arrowObserver = new ArrowObserver();
        }

        [Range(1, 255)]
        [SerializeField]
        private byte _arrowHintsCount;
        public byte ArrowHintsCount {
            get {
                return _arrowHintsCount;
            }
            private set {
                _arrowHintsCount = value;
            }
        }

        private ArrowObserver arrowObserver;

        [SerializeField]
        private Camera areaCamera = null;

        [SerializeField]
        private Light directionalLight = null;

        [SerializeField]
        private GameObject maze = null;

        [SerializeField]
        private GameObject floor = null;

        [SerializeField]
        private GameObject wall = null;

        [SerializeField]
        private GameObject photon = null;

        [SerializeField]
        private GameObject arrow = null;

        [SerializeField]
        private GameObject blackHole = null;

        [SerializeField]
        private GameObject whiteHole = null;

        [SerializeField]
        private Canvas canvas = null;

        public Camera GetAreaCamera() {
            LogIfObjectIsNull(areaCamera, "AreaCamera");
            return areaCamera;
        }

        public CameraController GetCameraScript() {
            var script = areaCamera.GetComponent<CameraController>();
            LogIfObjectIsNull(script, "CameraController");
            return script;

        }
        public Light GetDirectionalLight() {
            LogIfObjectIsNull(directionalLight, "DirectionalLight");
            return directionalLight;
        }

        public LightController GetDirectionalLightScript() {
            var script = directionalLight.GetComponent<LightController>();
            LogIfObjectIsNull(script, "DirectionalLightController");
            return script;
        }

        public GameObject GetMaze() {
            LogIfObjectIsNull(maze, "Maze");
            return maze;
        }

        public MazeController GetMazeScript() {
            var script = maze.GetComponent<MazeController>();
            LogIfObjectIsNull(script, "MazeController");
            return script;
        }

        public GameObject GetFloor() {
            LogIfObjectIsNull(floor, "Floor");
            return floor;
        }

        public GameObject GetWall() {
            LogIfObjectIsNull(wall, "Wall");
            return wall;
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

        public GameObject GetBlackHole() {
            LogIfObjectIsNull(arrow, "BlackHole");
            return blackHole;
        }

        public GameObject GetWhiteHole() {
            LogIfObjectIsNull(whiteHole, "WhiteHole");
            return whiteHole;
        }

        public Canvas GetCanvas() {
            LogIfObjectIsNull(canvas, "Canvas");
            return canvas;
        }

        public bool IsArrowPresent() {
            return arrowObserver.ArrowIsPresent;
        }

        public byte SpawnArrow() {
            if(ArrowHintsCount > 0) {
                var newArrow = Instantiate(arrow, maze.transform);
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

    }
}