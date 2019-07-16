using System;
using UnityEngine;


namespace PhotonInMaze.GameCamera {
    internal class CameraLerper {

        static internal bool LerpCameraPosition(Camera camera, float delta, Vector3 targetPos) {
            camera.transform.position = Vector3.Lerp(camera.transform.position, targetPos, delta);
            return CheckPositionRange(camera, targetPos);
        }


        static internal bool LerpCameraOrtographicSize(Camera camera, float delta, float targetOrtographicSize) {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetOrtographicSize, delta);
            if(Mathf.Abs(camera.orthographicSize - targetOrtographicSize) <= 0.1f) {
                camera.orthographicSize = targetOrtographicSize;
                return true;
            }
            return false;
        }

        static internal bool MoveTovardsPosition(Camera camera, float delta, Vector3 targetPos) {
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, targetPos, delta);
            return CheckPositionRange(camera, targetPos);
        }

        private static bool CheckPositionRange(Camera camera, Vector3 targetPos) {
            if(Vector3.Distance(camera.transform.position, targetPos) <= 0.1f) {
                camera.transform.position = targetPos;
                return true;
            }
            return false;
        }
    }
}