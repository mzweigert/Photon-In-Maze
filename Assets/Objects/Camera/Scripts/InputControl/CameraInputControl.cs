using System;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Provider;
using UnityEngine;

namespace PhotonInMaze.GameCamera {
    internal class CameraInputControl {

        private const float moveSpeed = 20f;

        private Camera camera;
        private CameraViewChanger cameraViewChanger;
        private CameraConfiguration configuration;
        private IMazeConfiguration mazeConfiguration;

        public CameraInputControl(Camera camera, CameraViewChanger cameraViewChanger) {
            this.camera = camera;
            this.cameraViewChanger = cameraViewChanger;
            this.configuration = camera.GetComponent<CameraConfiguration>();
            this.mazeConfiguration = MazeObjectsProvider.Instance.GetMazeConfiguration();
        }

        private PinchPoint pinchPoint;
        private Gestures gestures;

        public void Check() {
            if(CanResetValues()) {
                pinchPoint.Reset();
            }
#if UNITY_ANDROID
            else if(Input.touchCount == 2) {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                gestures = new Gestures(touchZero, touchOne);
                foreach(Gesture gesture in gestures) {
                    switch(gesture.Type) {
                        case GestureType.Pinch:
                            OnPinchEvent(touchZero, touchOne, gesture);
                            break;
                        case GestureType.Swipe:
                            Vector2 targetMovePos = new Vector2(gesture.Delta.y, -gesture.Delta.x);
                            OnMoveEvent(targetMovePos);
                            break;
                    }
                }

            }
#elif UNITY_EDITOR
            else if(Input.GetMouseButton(0)) {
                if(Input.mouseScrollDelta.y != 0) {
                    OnScrollEvent(Input.mousePosition, -Input.mouseScrollDelta.y * moveSpeed);
                } else if(Input.GetAxis("Mouse X") != 0 || (Input.GetAxis("Mouse Y") != 0)) {
                    Vector2 targetMovePos = new Vector2(Input.GetAxis("Mouse Y") * moveSpeed, -Input.GetAxis("Mouse X") * moveSpeed);
                    OnMoveEvent(targetMovePos);
                }
            }
#endif

        }

        internal void Reset() {
            pinchPoint.Reset();
            gestures.Reset();
        }

        private void OnScrollEvent(Vector2 mousePos, float delta) {
            if(!pinchPoint.Initialized || camera.transform.position.y >= configuration.minCameraYPosition + 2.5f) {
                pinchPoint = new PinchPoint(Input.mousePosition, camera);
            }
            ZoomCamera(pinchPoint, delta);
        }

        private bool CanResetValues() {
            bool pinchInitialized = pinchPoint.Initialized;
#if UNITY_ANDROID
            return Input.touchCount < 2 && pinchInitialized;
#elif UNITY_EDITOR
            return !Input.GetMouseButton(0) && pinchInitialized;
#endif
        }

        private void OnMoveEvent(Vector2 targetMovePos) {
            if(configuration.type != GameCameraType.Moved) {
                configuration.type = GameCameraType.Moved;
            }
            Vector3 camPos = camera.transform.position;
            Vector3 newCamPos =
                new Vector3(targetMovePos.x * (mazeConfiguration.Columns / 20f), 0f, targetMovePos.y * (mazeConfiguration.Rows / 20f));
            camPos += (newCamPos / 10f) * (camPos.y / 10f) * configuration.SwipeIntensive;

            float cellSideLength = mazeConfiguration.CellSideLength;
            if(camPos.x < 0) {
                camPos.x = 0;
            } else if(camPos.x > (mazeConfiguration.Columns * cellSideLength) - cellSideLength) {
                camPos.x = mazeConfiguration.Columns * cellSideLength - cellSideLength;
            }
            if(camPos.z < 0) {
                camPos.z = 0;
            } else if(camPos.z > (mazeConfiguration.Rows * cellSideLength) - cellSideLength) {
                camPos.z = mazeConfiguration.Rows * cellSideLength - cellSideLength;
            }
            camera.transform.position = camPos;
        }

        void OnPinchEvent(Touch touchZero, Touch touchOne, Gesture gesture) {
            if(!pinchPoint.Initialized || camera.transform.position.y >= configuration.minCameraYPosition + 2.5f) {
                pinchPoint = new PinchPoint(touchZero, touchOne, camera);
            }

            if(IsTouchHasAnyPhase(touchZero, TouchPhase.Moved) && IsTouchHasAnyPhase(touchOne, TouchPhase.Moved)) {
                ZoomCamera(pinchPoint, gesture.Magnitude);
            }
        }

        private void ZoomCamera(PinchPoint pinchPoint, float magnitude) {
            Vector3 screenPoint = camera.WorldToScreenPoint(pinchPoint.Value);
            Ray ray = camera.ScreenPointToRay(screenPoint);
            if(Physics.Raycast(ray, out RaycastHit hit) && 
                (hit.transform.gameObject.name == mazeConfiguration.Name )) {
                cameraViewChanger.ChangeOnPinch(magnitude * configuration.PinchIntensive, () => pinchPoint.Value);
            }
        }

        private bool IsTouchHasAnyPhase(Touch touch, params TouchPhase[] phases) {
            for(int i = 0; i < phases.Length; i++) {
                if(touch.phase == phases[i]) {
                    return true;
                }
            }
            return false;
        }
    }

}