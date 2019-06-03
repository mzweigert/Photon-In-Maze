using PhotonInMaze.Common;
using PhotonInMaze.Game.Maze;
using UnityEngine;

namespace PhotonInMaze.Game.Manager {
    public class CanvasObjectsManager : SceneSingleton<CanvasObjectsManager> {

        [SerializeField]
        private Canvas canvas = null;

        [SerializeField]
        private GameObject backgroundPanel;

        public Canvas GetCanvas() {
            LogIfObjectIsNull(canvas, "Canvas");
            return canvas;
        }

        public GameObject GetBackgroundPanel() {
            LogIfObjectIsNull(backgroundPanel, "BackgroundPanel");
            return backgroundPanel;
        }

        private void LogIfObjectIsNull(Object o, string name) {
            if(o == null) {
                Debug.LogError(name + " is null!");
            }
        }

    }
}