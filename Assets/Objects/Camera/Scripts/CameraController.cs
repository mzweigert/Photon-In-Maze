using PhotonInMaze.Common.Flow;
using PhotonInMaze.Game.Manager;
using PhotonInMaze.Game.Maze;
using PhotonInMaze.Game.Photon;
using UnityEngine;


namespace PhotonInMaze.Game.GameCamera {
    public partial class CameraController : FlowObserverBehaviour<PhotonController, PhotonState> {

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

        private Vector3 initialCamPosition, currentPhotonPosition, previousPhotonPosition;

        private float distanceBetweenBaseCamPosAndPhoton, currentDistanceBetweenCamAndPhoton,
                      initialOrtographicSize,
                      offsetCam = 3.5f,
                      deltaMagnitudeMultiplier = 0.01f;

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
                type = CameraType.AbovePhoton;
                cameraEventManager.Add(RepeatedEvent.Of(BackAbovePhoton));
            } else {
                Vector3 targetCamPosition = CalculatePositionBasedOnPhotonPositions(currentPhotonPosition, previousPhotonPosition);
                camera.transform.position = targetCamPosition;
            }
        }

        protected override IInvoke Init() {
            InitOnStart();
            return GameFlowManager.Instance.Flow
                .WhenIsAny()
                .ThenDo(WaitForCameraEvent)
                .Build();
        }

        private void InitOnStart() {
            camera = GetComponent<Camera>();
            mazeScript = ObjectsManager.Instance.GetMazeScript();

            float x = 0f, z = 0f, ratio = (float)Screen.width / Screen.height;
            x = (mazeScript.Columns * mazeScript.ScaleOfCellSide * 2f) - (mazeScript.LenghtOfCellSide / 2);
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

        private void WaitForCameraEvent() {
            if(cameraEventManager.CanRunCurrent()) {
                cameraEventManager.TryRunCurrent();
            } else if(cameraEventManager.CanLoadNextEvent()) {
                cameraEventManager.TryLoadNext();
            } else if(Input.touchCount == 2 || Input.mouseScrollDelta.y != 0 &&
                      !ObjectsManager.Instance.IsArrowPresent()) {

#if UNITY_EDITOR
                float deltaMagnitudeDiff = -Input.mouseScrollDelta.y * 20;
#elif UNITY_ANDROID
            float deltaMagnitudeDiff = CalculatePinchTouch(); 
#endif
                ChangeCameraView(deltaMagnitudeDiff);
            }

        }

    }
}