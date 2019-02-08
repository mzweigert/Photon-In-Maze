
using System;
using UnityEngine;

internal class CameraProjectionValues {
    internal readonly float fieldOfViewOnChange;
    internal readonly float ortographicSizeOnChange;

    public CameraProjectionValues(float fov, float ortographicSize) {
        this.fieldOfViewOnChange = fov;
        this.ortographicSizeOnChange = ortographicSize;
    }

}

public class CameraController : MonoBehaviour, IObserver<PhotonState> {

    private enum CameraType {
        Area,
        AbovePhoton
    }


    private Optional<CameraProjectionValues> cameraProjectionValues = Optional<CameraProjectionValues>.Empty();
    private Optional<MazeController> mazeScript = Optional<MazeController>.Empty();

    private new Camera camera;
    private CameraType currentType = CameraType.Area;
    private bool followThePhoton = false;

    private Vector3 initialCamPosition, targetCamPosition,
                    currentPhotonPosition, previousPhotonPosition;

    private Vector2 source, target;

    private float distanceBetweenBaseCamPosAndPhoton, currentDistanceBetweenCamAndPhoton,
                  initialOrtographicSize,
                  offsetCam = 3.5f,
                  deltaMagnitudeMultiplier = 0.01f;

    // Start is called before the first frame update
    void Start() {
        camera = GetComponent<Camera>();
        mazeScript = ObjectsManager.Instance.GetMazeScript();
        mazeScript.IfPresent(script => {
            float x = 0f, z = 0f, ratio = (float)Screen.width / Screen.height;
            x = (script.Columns * 2f) - (script.LenghtSide / 2);
            z = (script.Rows * 2f) - (script.LenghtSide / 2);
            transform.position = new Vector3(x, transform.position.y, z);
            initialCamPosition = camera.transform.position;
            GameObject maze = ObjectsManager.Instance.GetMaze();

            float sizeForLongerColumnsLength = script.Columns * (script.LenghtSide / 2);
            float sizeForLongerRowsLength = script.Rows * (script.LenghtSide / 2);
            initialOrtographicSize = sizeForLongerColumnsLength * ratio > sizeForLongerRowsLength ?
                                     sizeForLongerColumnsLength : sizeForLongerRowsLength;
            camera.orthographicSize = initialOrtographicSize += offsetCam;

            source = new Vector2(0, transform.position.y);
            float xForLongerColumnsLength = script.Columns * (script.LenghtSide / 2) + script.LenghtSide;
            float xForLongerRowsLength = script.Rows * (script.LenghtSide / 2) + script.LenghtSide;
            x = (xForLongerColumnsLength * ratio > xForLongerRowsLength ? xForLongerColumnsLength : xForLongerRowsLength);
            target = new Vector2(x + offsetCam, transform.position.y);
            float fov = Vector2.Angle(source, target) * 2;
            camera.fieldOfView = fov;
        });

        ObjectsManager.Instance.GetPhotonScript().IfPresent((script) => script.Subscribe(this));
    }

    void Update() {

        if(Input.touchCount == 2) {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            if(deltaMagnitudeDiff < 0) {
                Vector3 abovePhoton = new Vector3(currentPhotonPosition.x, currentPhotonPosition.y + 10f, currentPhotonPosition.z);
                camera.transform.position = 
                    Vector3.Lerp(camera.transform.position, abovePhoton, deltaMagnitudeDiff * -deltaMagnitudeMultiplier);
                camera.orthographicSize = 
                    Mathf.Lerp(camera.orthographicSize, abovePhoton.y, deltaMagnitudeDiff * -deltaMagnitudeMultiplier);
                ChangeProjectionIfCamNearPhoton();

            } else if(deltaMagnitudeDiff > 0) {
                camera.transform.position =
                    Vector3.Lerp(camera.transform.position, initialCamPosition, deltaMagnitudeDiff * deltaMagnitudeMultiplier);
                camera.orthographicSize = 
                    Mathf.Lerp(camera.orthographicSize, initialOrtographicSize, deltaMagnitudeDiff * deltaMagnitudeMultiplier);
                ChangeProjectionIfCamNearPhoton();
            }

        }

    }

    private void ChangeProjectionIfCamNearPhoton() {
        distanceBetweenBaseCamPosAndPhoton = Vector3.Distance(initialCamPosition, currentPhotonPosition);
        currentDistanceBetweenCamAndPhoton = Vector3.Distance(camera.transform.position, currentPhotonPosition);

        followThePhoton = distanceBetweenBaseCamPosAndPhoton * 0.8f > currentDistanceBetweenCamAndPhoton;
        bool isCameraNearPhoton = distanceBetweenBaseCamPosAndPhoton * 0.35f > currentDistanceBetweenCamAndPhoton;

        if(currentType == CameraType.Area && isCameraNearPhoton) {

            currentType = CameraType.AbovePhoton;

            float fov = cameraProjectionValues
                .InitIfAbsentAndGet(() => {
                    source = new Vector2(0, transform.position.y);
                    target = new Vector2(camera.orthographicSize, transform.position.y);
                    return new CameraProjectionValues(Vector2.Angle(source, target) * 2, camera.orthographicSize);
                })
                .fieldOfViewOnChange;

            camera.fieldOfView = fov;
            camera.orthographic = false;

        } else if(currentType == CameraType.AbovePhoton && !isCameraNearPhoton) {
            cameraProjectionValues.IfPresent(values => camera.orthographicSize = values.ortographicSizeOnChange);
            currentType = CameraType.Area;
            camera.orthographic = true;
        }
    }

    public void OnCompleted() {
        throw new NotImplementedException();
    }

    public void OnError(Exception error) {
        Debug.LogError(error.Message);
    }

    public void OnNext(PhotonState value) {
        if(value.RealPosition.Equals(currentPhotonPosition)) {
            return;
        }

        previousPhotonPosition = currentPhotonPosition;
        currentPhotonPosition = value.RealPosition;

        if(followThePhoton) {
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
}