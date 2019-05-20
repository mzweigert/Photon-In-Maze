using System;
using UnityEngine;

public partial class CameraController : MonoObserver<PhotonController, PhotonState> {

    private float CalculatePinchTouch() {
        if(Input.touchCount < 2) {
            return 0f;
        }
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // Find the position in the previous frame of each touch.
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // Find the difference in the distances between each frame.
        return prevTouchDeltaMag - touchDeltaMag;
    }

    private float CalculateOrtographicSize() {
        float tg = Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2);
        return tg * camera.transform.position.y; ;
    }

    private float CalculateFOV(float x, float y) {
        Vector2 source, target;
        source = new Vector2(0, y);
        target = new Vector2(x, y);
        return Vector2.Angle(source, target) * 2;
    }

    private Vector3 CalculatePositionBasedOnPhotonPositions(Vector3 currentPhotonPosition, Vector3 previousPhotonPosition) {
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


    private Vector3 CalculateResizePosition(Frame frame) {
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

    private bool IsPhotonVisibleOnCamera(Vector3 currentPhotonPosition) {
        Frame frame = new Frame(currentPhotonPosition, mazeScript.LenghtOfCellSide / 2);
        return frame.IsFrameBoundsVisibleOnCamera(camera);
    }
}