using System;
using PhotonInMaze.Common.Flow;

namespace PhotonInMaze.GameCamera {
    internal class CameraEventController : FlowUpdateBehaviour {

        private CameraEventManager cameraEventManager;

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                .WhenIsAny()
                .ThenDo(WaitForCameraEvent)
                .Build();
        }

        public override int GetInitOrder() {
            return (int)InitOrder.CameraController;
        }

        public override void OnInit() {
            cameraEventManager = new CameraEventManager();
        }

        private void WaitForCameraEvent() {
            if(cameraEventManager.CanRunCurrent()) {
                cameraEventManager.TryRunCurrent();
            } else if(cameraEventManager.CanLoadNextEvent()) {
                cameraEventManager.TryLoadNext();
            }
        }

        public void AddEventToQueue(ICameraEvent cameraEvent) {
            cameraEventManager.Add(cameraEvent);
        }

        public bool IsQueueEmpty() {
            return cameraEventManager.IsEmpty();
        }
    }
}