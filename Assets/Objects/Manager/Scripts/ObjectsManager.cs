using UnityEngine;

public class ObjectsManager : MonoSingleton<ObjectsManager> {

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

    public GameObject GetBlackHole() {
        LogIfObjectIsNull(arrow, "BlackHole");
        return blackHole;
    }

    public GameObject GetWhiteHole() {
        LogIfObjectIsNull(whiteHole, "WhiteHole");
        return whiteHole;
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
