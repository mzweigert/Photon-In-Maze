
namespace PhotonInMaze.Common.Flow {
    public abstract class FlowUpdateBehaviour : FlowBehaviour {

        private void Update() {
            TryInvokeLoop();
        }
    }

}
