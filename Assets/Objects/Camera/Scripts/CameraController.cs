using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Common.Model;
using PhotonInMaze.Provider;
using UnityEngine;

namespace PhotonInMaze.GameCamera {
    internal partial class CameraController : FlowObserverBehaviour<IPhotonMovementController, IPhotonState>, ICameraController {

        private CameraViewCalculator viewCalculator;
        private CameraViewChanger viewChanger;
        private CameraInputControl inputControl;
        private CameraConfiguration configuration;
        private CameraEventController eventController;

        private new Camera camera;

        private Vector3 currentPhotonPosition, previousPhotonPosition;

        public override void OnNext(IPhotonState state) {
            if(state.RealPosition.Equals(currentPhotonPosition)) {
                return;
            }

            previousPhotonPosition = currentPhotonPosition;
            currentPhotonPosition = state.RealPosition;

            if(configuration.type == GameCameraType.Moved) {
                ICameraEvent cameraEvent;
                if(configuration.followThePhoton) {
                    configuration.type = GameCameraType.AbovePhoton;
                    cameraEvent = viewChanger.BackAbovePosition(() => currentPhotonPosition);
                } else {
                    configuration.type = GameCameraType.Area;
                    cameraEvent = viewChanger.BackToInitialPosition();
                }
                eventController.AddEventToQueue(cameraEvent);
                inputControl.Reset();
            } else if(configuration.type == GameCameraType.BetweenPhotonAndArrow || configuration.type == GameCameraType.Zoomed) {
                configuration.type = GameCameraType.AbovePhoton;
                ICameraEvent cameraEvent = viewChanger.BackAbovePosition(() => currentPhotonPosition);
                eventController.AddEventToQueue(cameraEvent);
            } else if(configuration.followThePhoton || !viewCalculator.IsPhotonVisibleOnCamera(currentPhotonPosition)) {
                Vector3 targetCamPosition = viewCalculator.CalculatePositionBasedOnPhotonPositions(currentPhotonPosition, previousPhotonPosition);
                camera.transform.position = targetCamPosition;
            }
        }


        public override void OnInit() {
            camera = GetComponent<Camera>();

            currentPhotonPosition = previousPhotonPosition =
                ObjectsProvider.Instance.GetPhotonConfiguration().InitialPosition;

            eventController = camera.GetComponent<CameraEventController>();
            configuration = camera.GetComponent<CameraConfiguration>();
            viewCalculator = new CameraViewCalculator(camera);
            viewChanger = new CameraViewChanger(camera, viewCalculator);
            inputControl = new CameraInputControl(camera, viewChanger);
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                .WhenIsAnyAnd(() => eventController.IsQueueEmpty())
                .ThenDo(inputControl.Check)
                .Build();
        }

        public override int GetInitOrder() {
            return (int) InitOrder.CameraController;
        }

        public void ResizeCameraTo(Frame frame) {

            if(frame.IsFrameBoundsVisibleOnCamera(camera)) {
                return;
            }

            Vector3 targetCamPosition = viewCalculator.CalculateResizePosition(frame);
            float ortSize = frame.GetXDistance() * camera.aspect > frame.GetYDistance() ?
                            frame.GetXDistance() / 2 :
                            (frame.GetYDistance() / 2) / camera.aspect;

            ICameraEvent cameraEvent = viewChanger.GetResizeEvent(() => targetCamPosition, () => currentPhotonPosition, ortSize);
            eventController.AddEventToQueue(cameraEvent);
        }
    }
}