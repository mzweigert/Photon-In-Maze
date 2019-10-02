using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Model;
using PhotonInMaze.Provider;
using System;
using UnityEngine;

namespace PhotonInMaze.GameCamera {
    internal class CameraViewCalculator {

        private Camera camera;

        public CameraViewCalculator(Camera camera) {
            this.camera = camera;
        }

        internal float CalculateOrtographicSize() {
            float tg = Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2);
            return tg * camera.transform.position.y; ;
        }

        internal float CalculateFOV(float x, float y) {
            Vector2 source, target;
            source = new Vector2(0, y);
            target = new Vector2(x, y);
            return Vector2.Angle(source, target) * 2;
        }

        internal Vector3 CalculatePositionBasedOnPhotonPositions(Vector3 currentPhotonPosition, Vector3 previousPhotonPosition) {
            Vector3 targetCamPosition = camera.transform.position;
            float deltaX = Math.Abs(currentPhotonPosition.x - previousPhotonPosition.x);
            if(deltaX > 0) {
                targetCamPosition.x += (currentPhotonPosition.x > previousPhotonPosition.x) ? deltaX : -deltaX;
            }
            float deltaZ = Math.Abs(currentPhotonPosition.z - previousPhotonPosition.z);
            if(deltaZ > 0) {
                targetCamPosition.z += (currentPhotonPosition.z > previousPhotonPosition.z) ? deltaZ : -deltaZ;
            }
            return targetCamPosition;
        }

        internal bool IsAllMazeVisibleOnCamera() {
            IMazeConfiguration mazeConfiguration = MazeObjectsProvider.Instance.GetMazeConfiguration();
            float columnsSize = mazeConfiguration.Columns * mazeConfiguration.CellSideLength;
            float rowsSize = mazeConfiguration.Rows * mazeConfiguration.CellSideLength;
            Frame mazeFrame = new Frame(Vector2.zero, new Vector2(columnsSize, rowsSize));
            return mazeFrame.IsFrameBoundsVisibleOnCamera(camera);
        }

        internal Vector3 CalculateResizePosition(Frame frame) {
            float halfXDistance = frame.GetXDistance() / 2, halfYDistance = frame.GetYDistance() / 2;

            float tg, y, angle;
            if(frame.GetXDistance() * camera.aspect > frame.GetYDistance()) {
                angle = camera.fieldOfView / 2f;
                tg = Mathf.Tan(angle * (Mathf.PI / 180));
                y = halfXDistance / tg + (halfYDistance * 0.2f);
            } else {
                angle = Mathf.Rad2Deg * Mathf.Atan(Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2) * camera.aspect);
                tg = Mathf.Tan(angle * (Mathf.PI / 180));
                y = halfYDistance / tg + (halfXDistance * 0.2f);
            }
            return new Vector3(frame.GetCenterOfX(), y, frame.GetCenterOfY());
        }

        internal bool IsPhotonVisibleOnCamera(Vector3 currentPhotonPosition) {
            float lengthOfCellSide = MazeObjectsProvider.Instance.GetMazeConfiguration().CellSideLength;
            Frame frame = new Frame(currentPhotonPosition, lengthOfCellSide / 2);
            return frame.IsFrameBoundsVisibleOnCamera(camera);
        }
    }
}