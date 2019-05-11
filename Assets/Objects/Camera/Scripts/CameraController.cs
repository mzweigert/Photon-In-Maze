using System;
using UnityEngine;

public class CameraController : MonoObserver<PhotonController, PhotonState> {

    private enum CameraType {
        Area,
        AbovePhoton,
        BetweenPhotonAndArrow
    }

    private CameraEventManager cameraEventManager = new CameraEventManager();
    private MazeController mazeScript;
    private new Camera camera;
    private bool followThePhoton = false;
    private CameraType type;
    [SerializeField]
    [Range(0.1f, 2f)]
    private float cameraSpeed = 0.15f;

    private Vector3 initialCamPosition, targetCamPosition,
                    currentPhotonPosition, previousPhotonPosition;

    private float distanceBetweenBaseCamPosAndPhoton, currentDistanceBetweenCamAndPhoton,
                  initialOrtographicSize,
                  offsetCam = 3.5f,
                  deltaMagnitudeMultiplier = 0.01f;

    // Start is called before the first frame update
    void Start() {
        camera = GetComponent<Camera>();
        Optional<MazeController> optionalMazeScript = ObjectsManager.Instance.GetMazeScript();
        if(optionalMazeScript.HasNotValue) {
            return;
        }
        mazeScript = optionalMazeScript.Get();

     
        float x = 0f, z = 0f, ratio = (float)Screen.width / Screen.height;
        x = (mazeScript.Columns * mazeScript.ScaleOfCellSide * 2f) - (mazeScript.LenghtOfCellSide / 2) ;
        z = (mazeScript.Rows * mazeScript.ScaleOfCellSide * 2f) - (mazeScript.LenghtOfCellSide / 2);
        transform.position = new Vector3(x, transform.position.y, z);
        initialCamPosition = camera.transform.position;
        GameObject maze = ObjectsManager.Instance.GetMaze();

        float sizeForLongerColumnsLength = mazeScript.Columns * (mazeScript.LenghtOfCellSide / 2);
        float sizeForLongerRowsLength = mazeScript.Rows * (mazeScript.LenghtOfCellSide / 2);
        initialOrtographicSize = sizeForLongerColumnsLength * ratio > sizeForLongerRowsLength ?
                                 sizeForLongerColumnsLength : sizeForLongerRowsLength;
        camera.orthographicSize = initialOrtographicSize += offsetCam;

        camera.fieldOfView = CalculateFOV(initialOrtographicSize, initialCamPosition.y);
    }

    void Update() {

        if(cameraEventManager.CanRunCurrent()) {
            cameraEventManager.TryRunCurrent();
        } else if(cameraEventManager.CanLoadNextEvent()) {
            cameraEventManager.TryLoadNext();
        } else if(Input.touchCount == 2) {

            float deltaMagnitudeDiff = CalculatePinchTouch();
            float speed;
            if(deltaMagnitudeDiff < 0) {
                cameraEventManager.Add(OneShotEvent.Of(() => {
                    Vector3 abovePhoton = new Vector3(currentPhotonPosition.x, currentPhotonPosition.y + 8f, currentPhotonPosition.z);
                    speed = deltaMagnitudeDiff * -deltaMagnitudeMultiplier;
                    LerpCameraPosition(speed, abovePhoton);
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, abovePhoton.y, speed);
                    type = TryChangeCameraViewType() ? CameraType.AbovePhoton : CameraType.Area;
                }));

            } else if(deltaMagnitudeDiff > 0) {
                cameraEventManager.Add(OneShotEvent.Of(() => {
                    speed = deltaMagnitudeDiff * deltaMagnitudeMultiplier;
                    LerpCameraPosition(speed, initialCamPosition);
                    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, initialOrtographicSize, speed);
                    type = TryChangeCameraViewType() ? CameraType.AbovePhoton : CameraType.Area;
                }));

            }
        }

    }

    private float CalculatePinchTouch() {
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // Find the position in the previous frame of each touch.
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // Find the difference in the distances between each frame.
        return prevTouchDeltaMag - touchDeltaMag;
    }

    private bool LerpCameraPosition(float speed, Vector3 targetPos) {
        camera.transform.position = Vector3.Lerp(camera.transform.position, targetPos, speed);
        return Vector3.Distance(camera.transform.position, targetPos) <= 0.1f;
    }

    private bool TryChangeCameraViewType(float fov = 0, float ortographicSize = 0) {
        distanceBetweenBaseCamPosAndPhoton = Mathf.Abs(initialCamPosition.y - currentPhotonPosition.y);
        currentDistanceBetweenCamAndPhoton = Mathf.Abs(camera.transform.position.y - currentPhotonPosition.y);

        bool isNearPhoton = distanceBetweenBaseCamPosAndPhoton * 0.35f > currentDistanceBetweenCamAndPhoton;
        followThePhoton = distanceBetweenBaseCamPosAndPhoton * 0.8f > currentDistanceBetweenCamAndPhoton;
             

        if(isNearPhoton && camera.orthographic) {
            camera.orthographic = false;
            camera.fieldOfView = fov > 0? fov : CalculateFOV(camera.orthographicSize, camera.transform.position.y);
        } else if(!isNearPhoton && !camera.orthographic) {
            camera.orthographic = true;
            camera.orthographicSize = ortographicSize > 0? ortographicSize : CalculateOrtographicSize();
        }
        return isNearPhoton;
    }

    private float CalculateOrtographicSize() {
        float tg = Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2);
        return tg * camera.transform.position.y; ;
    }

    private float CalculateFOV(float x, float y) {
        Vector2 source, target;
        source = new Vector2(0, y);
        target = new Vector2(x, y);
        return Vector2.Angle(source, target) * 2;
    }


    public void ResizeCameraTo(Frame frame) {
     
        if(mazeScript == null) {
            Debug.LogError("MazeScript not found!");
            return;
        } else if(frame.IsFrameBoundsVisibleOnCamera(camera)) {
            return;
        }
        float halfXDistance = frame.GetXDistance() / 2,
              halfYDistance = frame.GetYDistance() / 2;

        float tg, y, angle, ortSize;
        if(frame.GetXDistance() * camera.aspect > frame.GetYDistance()) {
            angle = camera.fieldOfView / 2f;
            tg = Mathf.Tan(angle * (Mathf.PI / 180));
            y = halfXDistance / tg + (halfYDistance * 0.2f);
            ortSize = halfXDistance;
        } else {
            angle = Mathf.Rad2Deg * Mathf.Atan(Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2) * camera.aspect);
            tg = Mathf.Tan(angle * (Mathf.PI / 180));
            y = halfYDistance / tg + (halfXDistance * 0.2f);
            ortSize = halfYDistance / camera.aspect;
        }
        targetCamPosition = new Vector3(frame.GetCenterOfX(), y, frame.GetCenterOfY());

        cameraEventManager.Add(RepeatedlyTriggeredEvent.Of(() => {
            bool done = LerpCameraPosition(cameraSpeed, targetCamPosition);
            if(camera.orthographic) {
                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, ortSize, cameraSpeed);
            } else {
                TryChangeCameraViewType(ortographicSize: ortSize);
            }
            type = CameraType.BetweenPhotonAndArrow;
            return done;
        }));
    }

    public override void OnNext(PhotonState state) {
        if(state.RealPosition.Equals(currentPhotonPosition)) {
            return;
        }

        previousPhotonPosition = currentPhotonPosition;
        currentPhotonPosition = state.RealPosition;

        if(!followThePhoton && IsPhotonVisibleOnCamera(currentPhotonPosition)) {
            return;
        }

        if(type == CameraType.BetweenPhotonAndArrow) {
            if(state.IsAcutallyMoving) {
                type = CameraType.AbovePhoton;
            }
            targetCamPosition = new Vector3(currentPhotonPosition.x, camera.transform.position.y, currentPhotonPosition.z);
            cameraEventManager.Add(RepeatedlyTriggeredEvent.Of(() => LerpCameraPosition(cameraSpeed, targetCamPosition)));
        } else {
            targetCamPosition = camera.transform.position;
            float deltaX = Math.Abs(currentPhotonPosition.x - previousPhotonPosition.x);
            float deltaZ = Math.Abs(currentPhotonPosition.z - previousPhotonPosition.z);
            if(deltaX > 0) {
                targetCamPosition.x += (currentPhotonPosition.x > previousPhotonPosition.x) ? deltaX : -deltaX;
            } else if(deltaZ > 0) {
                targetCamPosition.z += (currentPhotonPosition.z > previousPhotonPosition.z) ? deltaZ : -deltaZ;
            }
            camera.transform.position = targetCamPosition;
        }
    }

    private bool IsPhotonVisibleOnCamera(Vector3 currentPhotonPosition) {
        Frame frame = new Frame(currentPhotonPosition, mazeScript.LenghtOfCellSide / 2);
        return frame.IsFrameBoundsVisibleOnCamera(camera);
    }
}