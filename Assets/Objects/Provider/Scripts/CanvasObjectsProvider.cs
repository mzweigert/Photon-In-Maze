using System;
using PhotonInMaze.Common;
using PhotonInMaze.Common.Controller;
using UnityEngine;

namespace PhotonInMaze.Provider {
    public class CanvasObjectsProvider : SceneSingleton<CanvasObjectsProvider> {

        [SerializeField]
        private Canvas canvas = null;

        [SerializeField]
        private GameObject backgroundPanel;

        [SerializeField]
        private GameObject arrowButton;

        public Canvas GetCanvas() {
            LogIfObjectIsNull(canvas, "Canvas");
            return canvas;
        }

        public IArrowButtonController GetArrowButtonController() {
            var arrowButton = GetArrowButton();
            IArrowButtonController controller = arrowButton.GetComponent<IArrowButtonController>();
            LogIfObjectIsNull(controller, "ArrowButtonController");
            return controller;
        }

        public GameObject GetArrowButton() {
            LogIfObjectIsNull(arrowButton, "ArrowButtonController");
            return arrowButton;
        }

        public GameObject GetBackgroundPanel() {
            LogIfObjectIsNull(backgroundPanel, "BackgroundPanel");
            return backgroundPanel;
        }

        private void LogIfObjectIsNull(object o, string name) {
            if(o == null) {
                Debug.LogError(name + " is null!");
            }
        }

    }
}