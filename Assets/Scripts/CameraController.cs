using System;
using UnityEngine;



public class CameraController : MonoBehaviour, IObserver<PhotonState> {

    private enum CameraType {
        Area,
        AbovePhoton
    }

    private Vector2 fingerStart, fingerEnd;
    private RaycastHit hit;
    private Vector3 photonPosition;
    private Camera areaCamera;
    private Camera photonCamera;
    private Rect photonCameraCords;
    private bool initCameraPosition;
    private CameraType currentType = CameraType.Area;
    private IDisposable unsubscriber;

    // Start is called before the first frame update
    void Start() {
        areaCamera = ObjectsManager.Instance.GetAreaCamera();
        photonCamera = ObjectsManager.Instance.GetPhotonCamera();
        photonCamera.enabled = false;
        photonCameraCords = new Rect(
            Screen.width * photonCamera.rect.x,
            Screen.height * photonCamera.rect.y,
            Screen.width * photonCamera.rect.width,
            Screen.height * photonCamera.rect.height);
        ObjectsManager.Instance.GetMazeScript().IfPresent(mazeScript => {
            float x = 0f, y = 0f, z = 0f;
            x = (mazeScript.Columns * 2f) - (mazeScript.LenghtSide / 2);
            z = (mazeScript.Rows * 2f) - (mazeScript.LenghtSide / 2);
            transform.position = new Vector3(x, transform.position.y, z);

            GameObject maze = ObjectsManager.Instance.GetMaze();

            if(areaCamera.orthographic) {
                float sizeForLongerColumnsLength = mazeScript.Columns * 2.05f;
                float sizeForLongerRowsLength = mazeScript.Rows * 1.25f;
                float size = (sizeForLongerColumnsLength > sizeForLongerRowsLength ? sizeForLongerColumnsLength : sizeForLongerRowsLength);
                areaCamera.orthographicSize = size;
                photonCamera.orthographicSize = size;
            } else {
                x = transform.position.y;
                Vector2 source = new Vector2(x, 0);
                float yForLongerColumnsLength = mazeScript.Columns * 2.25f;
                float yForLongerRowsLength = mazeScript.Rows * 1.315f;
                y = (yForLongerColumnsLength > yForLongerRowsLength ? yForLongerColumnsLength : yForLongerRowsLength);
                Vector2 target = new Vector2(x, y);
                float fov = Vector2.Angle(source, target) * 2;
                areaCamera.fieldOfView = fov;
                photonCamera.fieldOfView = fov;
            }
        });
    }

    void Update() {

        if(Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began) {
                fingerStart = touch.position;
                fingerEnd = touch.position;
            } else if(touch.phase == TouchPhase.Ended) {
                fingerEnd = touch.position;
                if(Vector2.Distance(fingerStart, fingerEnd) <= 1) {
                    TryChangeCamera(touch.position);
                }
                fingerStart = touch.position;
            }
        }
        #if UNITY_EDITOR
                else if(Input.GetMouseButtonDown(0)) {
                    Vector3 mousePos = Input.mousePosition;
                    TryChangeCamera(mousePos);
                }
        #endif


        if(!initCameraPosition) {
            return;
        } else if(currentType == CameraType.AbovePhoton) {
            Vector3 targetPosition = photonPosition;
            targetPosition.y += 10f;
            bool done = ChangeCameraPosition(targetPosition);
            if(done) {
                photonCamera.enabled = true;
            }
        } else if(currentType == CameraType.Area) {
            ChangeCameraPosition(photonCamera.transform.position);
        }

    }

    private void TryChangeCamera(Vector3 pos) {
        switch(currentType) {
            case CameraType.Area:
                ObjectsManager.Instance.GetPhotonScript().IfPresent((script) => {
                    Ray ray = areaCamera.ScreenPointToRay(pos);
                    bool clicked = Physics.Raycast(ray, out hit);
                    bool? isPhoton = hit.transform?.name.Equals(script.gameObject.name);
                    if(clicked && hit.collider && isPhoton.Value) {
                        currentType = CameraType.AbovePhoton;
                        initCameraPosition = true;
                        unsubscriber = script.Subscribe(this);
                    }
                });
                break;
            case CameraType.AbovePhoton:
                if(photonCameraCords.Contains(pos)) {
                    currentType = CameraType.Area;
                    initCameraPosition = true;
                    photonCamera.enabled = false;
                    unsubscriber?.Dispose();
                }
                break;
        }
    }

    private bool ChangeCameraPosition(Vector3 targetPosition) {
        areaCamera.transform.position = Vector3.Lerp(areaCamera.transform.position, targetPosition, 0.5f);
        if(Vector3.Distance(areaCamera.transform.position, targetPosition) <= 0.1f) {
            areaCamera.transform.position = targetPosition;
            initCameraPosition = false;
            return true;
        }
        return false;
    }

    public void OnCompleted() {
        throw new NotImplementedException();
    }

    public void OnError(Exception error) {
        Debug.LogError(error.Message);
    }

    public void OnNext(PhotonState value) {
        if(value.RealPosition.Equals(photonPosition)) {
            return;
        }
        photonPosition = value.RealPosition;
        if(!initCameraPosition && currentType == CameraType.AbovePhoton) {
            Vector3 targetPosition = photonPosition;
            targetPosition.y += 10f;
            areaCamera.transform.position = targetPosition;
        }
    }
}
