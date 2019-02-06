using UnityEngine;

public class ObjectsManager : SceneSingleton<ObjectsManager> {

    private ObjectsManager() { }

    [SerializeField]
    private new Camera areaCamera;

    [SerializeField]
    private Light directionalLight;

    [SerializeField]
    private GameObject maze;

    [SerializeField]
    private GameObject photon;

    [SerializeField]
    private Canvas canvas;

    public Camera GetAreaCamera() {
        LogIfObjectIsNull(areaCamera, "AreaCamera");
        return areaCamera;
    }

    public Optional<CameraController> GetCameraScriptt() {
        var script = FindObjectOfType<CameraController>();
        LogIfObjectIsNull(script, "CameraController");
        return Optional<CameraController>.OfNullable(script);

    }
    public Light GetDirectionalLight() {
        LogIfObjectIsNull(directionalLight, "DirectionalLight");
        return directionalLight;
    }

    public Optional<LightController> GetDirectionalLightScript() {
        var script = FindObjectOfType<LightController>();
        LogIfObjectIsNull(script, "DirectionalLightController");
        return Optional<LightController>.OfNullable(script);
    }

    public GameObject GetMaze() {
        LogIfObjectIsNull(maze, "Maze");
        return maze;
    }

    public Optional<MazeController> GetMazeScript() {
        var script = FindObjectOfType<MazeController>();
        LogIfObjectIsNull(script, "MazeController");
        return Optional<MazeController>.OfNullable(script);
    }

    public GameObject GetPhoton() {
        LogIfObjectIsNull(photon, "Photon");
        return photon;
    }

    public Optional<PhotonConroller> GetPhotonScript() {
        var script = FindObjectOfType<PhotonConroller>();
        LogIfObjectIsNull(script, "PhotonConroller");
        return Optional<PhotonConroller>.OfNullable(script);
    }

    public Optional<Canvas> GetCanvas() {
        LogIfObjectIsNull(canvas, "Canvas");
        return Optional<Canvas>.OfNullable(canvas);
    }

    private void LogIfObjectIsNull(Object o, string name) {
        if(o == null) {
            Debug.Log(name + " is null!");
        }
    }
}
