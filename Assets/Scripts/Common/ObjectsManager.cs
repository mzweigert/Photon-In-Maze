using UnityEngine;

public class ObjectsManager : MonoSingleton<ObjectsManager> {

    private class ArrowObserver : System.IObserver<ArrowState> {

        public bool ArrowIsPresent { get; private set; } = false;

        internal ArrowObserver() { }

        public void Subscribe(GameObject newArrow) {
            if(newArrow == null) {
                Debug.LogError("Given newArrow is null!");
                return;
            }
            ArrowController controller = newArrow.GetComponent<ArrowController>();
            if(controller == null) {
                Debug.LogError("Given newArrow doesn't has ArrowController!");
                return;
            }
            controller.Subscribe(this);
            ArrowIsPresent = true;
        }

        public void OnNext(ArrowState state) {
            if(ArrowState.Ending == state) {
                ArrowIsPresent = false;
            }
        }

        public void OnCompleted() {
            throw new System.NotImplementedException();
        }

        public void OnError(System.Exception error) {
            Debug.LogError(error.Message);
        }
    }

    private ObjectsManager() {
        arrowObserver = new ArrowObserver();
    }

    public byte ArrowHintsCount { get; private set; } = 3;
    private ArrowObserver arrowObserver;

    [SerializeField]
    private Camera areaCamera;

    [SerializeField]
    private Light directionalLight;

    [SerializeField]
    private GameObject maze;

    [SerializeField]
    private GameObject photon;

    [SerializeField]
    private GameObject arrow;

    [SerializeField]
    private Canvas canvas;

    public Camera GetAreaCamera() {
        LogIfObjectIsNull(areaCamera, "AreaCamera");
        return areaCamera;
    }

    public Optional<CameraController> GetCameraScript() {
        var script = areaCamera.GetComponent<CameraController>();
        LogIfObjectIsNull(script, "CameraController");
        return Optional<CameraController>.OfNullable(script);

    }
    public Light GetDirectionalLight() {
        LogIfObjectIsNull(directionalLight, "DirectionalLight");
        return directionalLight;
    }

    public Optional<LightController> GetDirectionalLightScript() {
        var script = directionalLight.GetComponent<LightController>();
        LogIfObjectIsNull(script, "DirectionalLightController");
        return Optional<LightController>.OfNullable(script);
    }

    public GameObject GetMaze() {
        LogIfObjectIsNull(maze, "Maze");
        return maze;
    }

    public Optional<MazeController> GetMazeScript() {
        var script = maze.GetComponent<MazeController>();
        LogIfObjectIsNull(script, "MazeController");
        return Optional<MazeController>.OfNullable(script);
    }

    public GameObject GetPhoton() {
        LogIfObjectIsNull(photon, "Photon");
        return photon;
    }

    public Optional<PhotonController> GetPhotonScript() {
        var script = photon.GetComponent<PhotonController>();
        LogIfObjectIsNull(script, "PhotonConroller");
        return Optional<PhotonController>.OfNullable(script);
    }

    public GameObject GetArrow() {
        LogIfObjectIsNull(arrow, "Arrow");
        return arrow;
    }

    public Optional<ArrowController> GetArrowScript() {
        var script = arrow.GetComponent<ArrowController>();
        LogIfObjectIsNull(script, "ArrowController");
        return Optional<ArrowController>.OfNullable(script);
    }

    public Optional<Canvas> GetCanvas() {
        LogIfObjectIsNull(canvas, "Canvas");
        return Optional<Canvas>.OfNullable(canvas);
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
