using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Common.Model;
using PhotonInMaze.Provider;
using UnityEngine;

namespace PhotonInMaze.GameCamera {
    public partial class CameraController : FlowObserverBehaviour<IPhotonController, IPhotonState>, ICameraController {

        private enum CameraType {
            Area,
            AbovePhoton,
            BetweenPhotonAndArrow
        }

        private CameraEventManager cameraEventManager = new CameraEventManager();
        private IMazeController mazeScript;
        private new Camera camera;
        private bool followThePhoton;
        private CameraType type;
        private Animator animator;

        [SerializeField]
        [Range(0.1f, 2f)]
        private float cameraSpeed = 0.15f;

        private Vector3 initialCamPosition, currentPhotonPosition, previousPhotonPosition;

        private float distanceBetweenBaseCamPosAndPhoton, currentDistanceBetweenCamAndPhoton,
                      initialOrtographicSize,
                      offsetCam = 3.5f,
                      deltaMagnitudeMultiplier = 0.01f;

        public override void OnNext(IPhotonState state) {
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

        public override void OnInit() {
            camera = GetComponent<Camera>();
            mazeScript = MazeObjectsProvider.Instance.GetMazeController();
            float x = 0f, z = 0f, ratio = (float)Screen.width / Screen.height;
            x = (mazeScript.Columns * mazeScript.ScaleOfCellSide * 2f) - (mazeScript.LenghtOfCellSide / 2);
            z = (mazeScript.Rows * mazeScript.ScaleOfCellSide * 2f) - (mazeScript.LenghtOfCellSide / 2);
            camera.transform.position = initialCamPosition = new Vector3(x, 50, z);

            float sizeForLongerColumnsLength = mazeScript.Columns * (mazeScript.LenghtOfCellSide / 2);
            float sizeForLongerRowsLength = mazeScript.Rows * (mazeScript.LenghtOfCellSide / 2);
            initialOrtographicSize = sizeForLongerColumnsLength * ratio > sizeForLongerRowsLength ?
                                     sizeForLongerColumnsLength : sizeForLongerRowsLength;
            camera.orthographicSize = initialOrtographicSize += offsetCam;
            camera.fieldOfView = CalculateFOV(initialOrtographicSize, initialCamPosition.y);
            camera.orthographic = true;
            type = CameraType.Area;
            currentPhotonPosition = previousPhotonPosition = ObjectsProvider.Instance.GetPhotonController().GetInitialPosition();
            followThePhoton = false;
            animator = transform.GetComponent<Animator>();
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                .WhenIsAny()
                .ThenDo(WaitForCameraEvent)
                .Build();
        }

        private void WaitForCameraEvent() {
            if(cameraEventManager.CanRunCurrent()) {
                cameraEventManager.TryRunCurrent();
            } else if(cameraEventManager.CanLoadNextEvent()) {
                cameraEventManager.TryLoadNext();
            } else if(Input.touchCount == 2 || Input.mouseScrollDelta.y != 0 &&
                      !CanvasObjectsProvider.Instance.GetArrowButtonController().IsArrowPresent()) {

#if UNITY_EDITOR
                float deltaMagnitudeDiff = -Input.mouseScrollDelta.y * 20;
#elif UNITY_ANDROID
            float deltaMagnitudeDiff = CalculatePinchTouch(); 
#endif
                ChangeCameraView(deltaMagnitudeDiff);
            }
        }

        public override int GetInitOrder() {
            return InitOrder.Camera;
        }

    }
}