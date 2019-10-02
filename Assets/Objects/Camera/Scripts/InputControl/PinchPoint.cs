using UnityEngine;

namespace PhotonInMaze.GameCamera {
    internal struct PinchPoint {

        internal Vector3 Value { private set; get; }
        internal bool Initialized { private set; get; }

        internal PinchPoint(Touch touchZero, Touch touchOne, Camera camera) {
            float x = Mathf.Abs(touchZero.position.x - touchOne.position.x) / 2;
            x = (touchZero.position.x > touchOne.position.x ? touchOne.position.x : touchZero.position.x) + x;

            float y = Mathf.Abs(touchZero.position.y - touchOne.position.y) / 2;
            y = (touchZero.position.y > touchOne.position.y ? touchOne.position.y : touchZero.position.y) + y;
            Vector3 point = camera.ScreenToWorldPoint(new Vector3(x, y, camera.transform.position.y));
            Value = new Vector3(point.x, 2f, point.z);
            Initialized = true;
        }

        public PinchPoint(Vector2 mousePosition, Camera camera) : this() {
            Vector3 point = camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, camera.transform.position.y));
            Value = new Vector3(point.x, 2f, point.z);
            Initialized = true;
        }

        internal void Reset() {
            Value = Vector3.zero;
            Initialized = false;
        }
    }
}