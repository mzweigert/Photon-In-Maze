using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Provider;
using UnityEngine;

namespace PhotonInMaze.GameCamera {
    internal class CameraConfiguration : FlowBehaviour {

        [SerializeField]
        [Range(0.5f, 2f)]
        private float cameraSpeed;

        internal float CameraSpeed { get { return cameraSpeed * 0.15f; } }

        [SerializeField]
        [Range(0.5f, 2f)]
        private float swipeIntensive = 1f;

        internal float SwipeIntensive { get { return swipeIntensive * 0.5f; } }

        [SerializeField]
        [Range(0.5f, 2f)]
        private float pinchIntensive = 1f;

        internal float PinchIntensive { get { return pinchIntensive * 0.01f; } }

        internal const float offsetCam = 3.5f;

        internal readonly float minCameraYPosition = 10f;

        internal GameCameraType type;
        internal bool followThePhoton;
        internal float initialOrtographicSize;
        internal Vector3 initialCameraPosition;


        public override void OnInit() {
            type = GameCameraType.Area;
            followThePhoton = false;
            Camera camera = GetComponent<Camera>();
            IMazeConfiguration mazeConfiguration = MazeObjectsProvider.Instance.GetMazeConfiguration(); 
            float x = 0f, z = 0f, ratio = (float)Screen.width / Screen.height;
            x = (mazeConfiguration.Columns * 2f) - (mazeConfiguration.CellSideLength / 2);
            z = (mazeConfiguration.Rows * 2f) - (mazeConfiguration.CellSideLength / 2);
            camera.transform.position = initialCameraPosition = new Vector3(x, 50, z);

            float sizeForLongerColumnsLength = mazeConfiguration.Columns * (mazeConfiguration.CellSideLength / 2);
            float sizeForLongerRowsLength = mazeConfiguration.Rows * (mazeConfiguration.CellSideLength / 2);
            initialOrtographicSize = sizeForLongerColumnsLength * ratio > sizeForLongerRowsLength ?
                                     sizeForLongerColumnsLength : sizeForLongerRowsLength;
            camera.orthographicSize = initialOrtographicSize += offsetCam;
            CameraViewCalculator calculator = new CameraViewCalculator(camera);
            camera.fieldOfView = calculator.CalculateFOV(initialOrtographicSize, camera.transform.position.y);
            camera.orthographic = true;
        }

        public override int GetInitOrder() {
            return (int) InitOrder.CameraConfiguration;
        }
    }
}