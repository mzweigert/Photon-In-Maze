using System;
using UnityEngine;

public struct Frame {

    private Vector2 leftUpBound;
    private Vector2 rightDownBound;

    public Frame(Vector2 leftUpBound, Vector2 rightDownBound) {
        this.leftUpBound = leftUpBound;
        this.rightDownBound = rightDownBound;
    }

    public Frame(Vector3 initialPoint, float offset) : this(initialPoint, initialPoint) {
        leftUpBound.x -= offset;
        leftUpBound.y -= offset;
        rightDownBound.x += offset;
        rightDownBound.x += offset;
    }

    public void TryResizeX(float newX, float offset = 0) {
        if(newX < leftUpBound.x) {
            leftUpBound.x = newX - offset;
        } else if(newX + offset > rightDownBound.x) {
            rightDownBound.x = newX + offset;
        }
    }

    public void TryResizeY(float newY, float offset = 0) {
        if(newY - offset < leftUpBound.y) {
            leftUpBound.y = newY - offset;
        } else if(newY + offset > rightDownBound.y) {
            rightDownBound.y = newY + offset;
        }
    }
    
    public Vector2 GetLeftUpBound() {
        return leftUpBound;
    }

    public Vector2 GetRightDownBound() {
        return rightDownBound;
    }

    public bool IsFrameBoundsVisibleOnCamera(Camera camera) {
        return IsPointVisibleOnScreen(camera, leftUpBound) && IsPointVisibleOnScreen(camera, rightDownBound);
    }

    private bool IsPointVisibleOnScreen(Camera camera, Vector2 point) {
        Vector3 castToVector3 = new Vector3(point.x, 1f, point.y);
        Vector3 viewportPoint = camera.WorldToViewportPoint(castToVector3);
        return (viewportPoint.z > 0 && (new Rect(0, 0, 1, 1)).Contains(viewportPoint));
    }

    public float GetCenterOfX() {
        return leftUpBound.x + GetXDistance() / 2;
    }

    public float GetCenterOfY() {
        return leftUpBound.y + GetYDistance() / 2;
    }

    public float GetXDistance() {
        return rightDownBound.x - leftUpBound.x;
    }

    public float GetYDistance() {
        return rightDownBound.y - leftUpBound.y;
    }


}