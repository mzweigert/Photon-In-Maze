
using System;
using UnityEngine;

public class CameraController : MonoBehaviour, IObserver<PhotonState> {

    private enum CameraType {
        Area,
        AbovePhoton
    }
    private new Camera camera;
    private Vector3 initialCamPosition, targetCamPosition;
    private Vector3 currentPhotonPosition, previousPhotonPosition;
    private float distanceBetweenCamAndPhoton;
    private bool actuallyMoving;
    private CameraType currentType = CameraType.Area;
    private IDisposable unsubscriber;
    private float ortographicSize;

    // Start is called before the first frame update
    void Start() {
        camera = GetComponent<Camera>();
        ObjectsManager.Instance.GetMazeScript().IfPresent(mazeScript => {
            float x = 0f, y = 0f, z = 0f;
            x = (mazeScript.Columns * 2f) - (mazeScript.LenghtSide / 2);
            z = (mazeScript.Rows * 2f) - (mazeScript.LenghtSide / 2);
            transform.position = new Vector3(x, transform.position.y, z);
            initialCamPosition = camera.transform.position;
            GameObject maze = ObjectsManager.Instance.GetMaze();

            float sizeForLongerColumnsLength = mazeScript.Columns * 2.05f;
            float sizeForLongerRowsLength = mazeScript.Rows * 1.25f;
            ortographicSize = (sizeForLongerColumnsLength > sizeForLongerRowsLength ? sizeForLongerColumnsLength : sizeForLongerRowsLength);
            camera.orthographicSize = ortographicSize;

            x = transform.position.y;
            Vector2 source = new Vector2(x, 0);
            float yForLongerColumnsLength = mazeScript.Columns * 2.25f;
            float yForLongerRowsLength = mazeScript.Rows * 1.315f;
            y = (yForLongerColumnsLength > yForLongerRowsLength ? yForLongerColumnsLength : yForLongerRowsLength);
            Vector2 target = new Vector2(x, y);
            float fov = Vector2.Angle(source, target) * 2;
            camera.fieldOfView = fov;

        });
        ObjectsManager.Instance.GetPhotonScript().IfPresent((script) => {
            unsubscriber = script.Subscribe(this);
        });
    }

    void Update() {

        if(Input.touchCount == 2) {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            bool distanceChanged = false;
            if(deltaMagnitudeDiff < 0) {
                Vector3 abovePhoton = new Vector3(currentPhotonPosition.x, currentPhotonPosition.y + 10f, currentPhotonPosition.z);
                camera.transform.position = Vector3.Lerp(camera.transform.position, abovePhoton, deltaMagnitudeDiff * -0.01f);
                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, abovePhoton.y, deltaMagnitudeDiff * -0.01f);

                float currentDistanceBetweenCamAndPhoton = Vector3.Distance(camera.transform.position, currentPhotonPosition);

                distanceChanged = true;

            } else if(deltaMagnitudeDiff > 0) {
                camera.transform.position = Vector3.Lerp(camera.transform.position, initialCamPosition, deltaMagnitudeDiff * 0.01f);
                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, ortographicSize, deltaMagnitudeDiff * 0.01f);

                distanceChanged = true;
            }
            if(distanceChanged) {
                distanceBetweenCamAndPhoton = Vector3.Distance(initialCamPosition, currentPhotonPosition);
                float currentDistanceBetweenCamAndPhoton = Vector3.Distance(camera.transform.position, currentPhotonPosition);
                if(distanceBetweenCamAndPhoton * 0.85f > currentDistanceBetweenCamAndPhoton) {
                    currentType = CameraType.AbovePhoton;
                }  else { 
                    currentType = CameraType.Area;
                }
                
            }
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
        if(currentType == CameraType.AbovePhoton) {
            actuallyMoving = true;
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