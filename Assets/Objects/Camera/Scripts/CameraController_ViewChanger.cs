using PhotonInMaze.Common.Flow;
using PhotonInMaze.Game.Photon;
using UnityEngine;

namespace PhotonInMaze.Game.GameCamera {
    public partial class CameraController : FlowObserverBehaviour<PhotonController, PhotonState> {

        private void ChangeCameraView(float delta) {
            if(delta == 0f) {
                return;
            }
            ICameraEvent cameraEvent = null;
            if(delta < 0) {
                cameraEvent = OneShotEvent.Of(() => {
                    Vector3 abovePhoton = new Vector3(currentPhotonPosition.x, currentPhotonPosition.y + 8f, currentPhotonPosition.z);
                    float speed = delta * -deltaMagnitudeMultiplier;
                    LerpCameraPosition(speed, abovePhoton);
                    LerpCameraOrtographicSize(speed, abovePhoton.y);
                    type = TryChangeCameraViewType() ? CameraType.AbovePhoton : CameraType.Area;
                });
            } else {
                cameraEvent = OneShotEvent.Of(() => {
                    float speed = delta * deltaMagnitudeMultiplier;
                    LerpCameraPosition(speed, initialCamPosition);
                    LerpCameraOrtographicSize(speed, initialOrtographicSize);
                    type = TryChangeCameraViewType() ? CameraType.AbovePhoton : CameraType.Area;
                });
            }
            cameraEventManager.Add(cameraEvent);
        }

        private bool BackAbovePhoton() {
            Vector3 targetCamPosition = new Vector3(currentPhotonPosition.x, camera.transform.position.y, currentPhotonPosition.z);
            return LerpCameraPosition(cameraSpeed * 2, targetCamPosition);
        }

        private bool LerpCameraPosition(float speed, Vector3 targetPos) {
            camera.transform.position = Vector3.Lerp(camera.transform.position, targetPos, speed);
            if(Vector3.Distance(camera.transform.position, targetPos) <= 0.1f) {
                camera.transform.position = targetPos;
                return true;
            }
            return false;
        }

        private bool LerpCameraOrtographicSize(float speed, float targetOrtographicSize) {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetOrtographicSize, speed);
            if(Mathf.Abs(camera.orthographicSize - targetOrtographicSize) <= 0.1f) {
                camera.orthographicSize = targetOrtographicSize;
                return true;
            }
            return false;
        }

        public void ResizeCameraTo(Frame frame) {

            if(mazeScript == null) {
                Debug.LogError("MazeScript not found!");
                return;
            } else if(frame.IsFrameBoundsVisibleOnCamera(camera)) {
                return;
            }
            Vector3 targetCamPosition = CalculateResizePosition(frame);
            float ortSize = frame.GetXDistance() * camera.aspect > frame.GetYDistance() ?
                            frame.GetXDistance() / 2 :
                            (frame.GetYDistance() / 2) / camera.aspect;

            ICameraEvent cameraEvent = RepeatedEvent.Of(() => {
                bool changeCameraPositionDone = LerpCameraPosition(cameraSpeed, targetCamPosition);
                bool lerpCameraOrtographicSizeDone = false;
                if(camera.orthographic) {
                    lerpCameraOrtographicSizeDone = LerpCameraOrtographicSize(cameraSpeed, ortSize);
                } else {
                    TryChangeCameraViewType(ortographicSize: ortSize);
                    lerpCameraOrtographicSizeDone = true;
                }
                type = CameraType.BetweenPhotonAndArrow;
                return changeCameraPositionDone && lerpCameraOrtographicSizeDone;
            });
            cameraEventManager.Add(cameraEvent);
        }

        private bool TryChangeCameraViewType(float fov = 0, float ortographicSize = 0) {
            distanceBetweenBaseCamPosAndPhoton = Mathf.Abs(initialCamPosition.y - currentPhotonPosition.y);
            currentDistanceBetweenCamAndPhoton = Mathf.Abs(camera.transform.position.y - currentPhotonPosition.y);

            bool isNearPhoton = distanceBetweenBaseCamPosAndPhoton * 0.35f > currentDistanceBetweenCamAndPhoton;
            followThePhoton = distanceBetweenBaseCamPosAndPhoton * 0.8f > currentDistanceBetweenCamAndPhoton;


            if(isNearPhoton && camera.orthographic) {
                camera.orthographic = false;
                camera.fieldOfView = fov > 0 ? fov : CalculateFOV(camera.orthographicSize, camera.transform.position.y);
            } else if(!isNearPhoton && !camera.orthographic) {
                camera.orthographic = true;
                camera.orthographicSize = ortographicSize > 0 ? ortographicSize : CalculateOrtographicSize();
            }
            return isNearPhoton;
        }
    }
}