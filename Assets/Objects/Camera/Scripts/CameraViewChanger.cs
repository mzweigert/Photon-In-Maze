using System;
using UnityEngine;

namespace PhotonInMaze.GameCamera {
    internal class CameraViewChanger {

        private Camera camera;

        private CameraConfiguration configuration;
        private CameraViewCalculator viewCalculator;

        internal CameraViewChanger(Camera camera, CameraViewCalculator viewCalculator) {
            this.camera = camera;
            this.configuration = camera.GetComponent<CameraConfiguration>();
            this.viewCalculator = viewCalculator;
        }

        internal void ChangeOnPinch(float speed, Func<Vector3> currentTargetPosition) {
            if(speed == 0) {
                return;
            }

            Vector3 aboveTarget = currentTargetPosition.Invoke();
            aboveTarget.y = configuration.minCameraYPosition;

            if(speed < 0) {
                if(camera.orthographic) {
                    CameraLerper.LerpCameraPosition(camera, -speed, aboveTarget);
                } else {
                    CameraLerper.MoveTovardsPosition(camera, -speed * 10f, aboveTarget);
                }
                CameraLerper.LerpCameraOrtographicSize(camera, -speed, aboveTarget.y);
                configuration.type = TryChangeCameraViewType(aboveTarget) ? GameCameraType.Zoomed : GameCameraType.Moved;
            } else {
                if(camera.orthographic) {
                    CameraLerper.LerpCameraPosition(camera, speed, configuration.initialCameraPosition);
                } else {
                    CameraLerper.MoveTovardsPosition(camera, speed * 10f, configuration.initialCameraPosition);
                }
                CameraLerper.LerpCameraOrtographicSize(camera, speed, configuration.initialOrtographicSize);
                configuration.type = TryChangeCameraViewType(aboveTarget) ? GameCameraType.Zoomed : GameCameraType.Moved;
            }
        }

        internal ICameraEvent BackToInitialPosition() {
            bool cameraPositionReached, cameraOrtSizeReached;
            return RepeatedEvent.Of(() => {
                cameraPositionReached = CameraLerper.LerpCameraPosition(camera, configuration.CameraSpeed * 2, configuration.initialCameraPosition);
                cameraOrtSizeReached = CameraLerper.LerpCameraOrtographicSize(camera, configuration.CameraSpeed * 2, configuration.initialOrtographicSize);
                return cameraPositionReached && cameraOrtSizeReached;
            });
        }

        internal ICameraEvent BackAbovePosition(Func<Vector3> currentTargetPosition) {
            return RepeatedEvent.Of(() => {
                Vector3 targetCamPosition = currentTargetPosition.Invoke();
                targetCamPosition.y = camera.transform.position.y;
                return CameraLerper.LerpCameraPosition(camera, configuration.CameraSpeed * 2, targetCamPosition);
            });
        }

        internal ICameraEvent GetResizeEvent(Func<Vector3> currentTargetPosition, Func<Vector3> currentPhotonPosition, float ortSize) {
            return RepeatedEvent.Of(() => {
                bool changeCameraPositionDone = CameraLerper.LerpCameraPosition(camera, configuration.CameraSpeed, currentTargetPosition.Invoke());
                bool lerpCameraOrtographicSizeDone = false;
                if(camera.orthographic) {
                    lerpCameraOrtographicSizeDone = CameraLerper.LerpCameraOrtographicSize(camera, configuration.CameraSpeed, ortSize);
                } else {
                    TryChangeCameraViewType(currentPhotonPosition.Invoke());
                    lerpCameraOrtographicSizeDone = true;
                }
                configuration.type = GameCameraType.BetweenPhotonAndArrow;
                return changeCameraPositionDone && lerpCameraOrtographicSizeDone;
            });
        }

        private bool TryChangeCameraViewType(Vector3 targetPosition) {
            float distanceBetweenBaseCamPosAndTarget = Mathf.Abs(configuration.initialCameraPosition.y - targetPosition.y);
            float currentDistanceBetweenCamAndTarget = Mathf.Abs(camera.transform.position.y - targetPosition.y);

            bool isNearTarget = distanceBetweenBaseCamPosAndTarget * 0.35f > currentDistanceBetweenCamAndTarget;

            if(isNearTarget && camera.orthographic) {
                camera.orthographic = false;
                camera.fieldOfView = viewCalculator.CalculateFOV(camera.orthographicSize, camera.transform.position.y);
                configuration.followThePhoton = true;
            } else if(!isNearTarget && !camera.orthographic) {
                camera.orthographic = true;
                float ortSize = viewCalculator.CalculateOrtographicSize();
                if(ortSize > configuration.initialOrtographicSize) {
                    ortSize = configuration.initialOrtographicSize;
                }
                camera.orthographicSize = ortSize;
                configuration.followThePhoton = false;
            }
            return isNearTarget;
        }
    }
}