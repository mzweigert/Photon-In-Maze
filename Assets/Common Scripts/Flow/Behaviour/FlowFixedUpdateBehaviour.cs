
namespace PhotonInMaze.Common.Flow {
    public abstract class FlowFixedUpdateBehaviour : FlowBehaviour {

        private void FixedUpdate() {
            TryInvokeLoop();
        }
    }

}
