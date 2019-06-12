
namespace PhotonInMaze.Common.Flow {
    public abstract class FlowFixedUpdateBehaviour : FlowBehaviour {

        private IInvoke action;

        private void FixedUpdate() {
            if(flowManager != null) {
                action?.Invoke(flowManager.GetCurrentState());
            }
        }

        public abstract IInvoke OnLoop();

        public override void Prepare() {
            base.Prepare();
            this.action = OnLoop();
        }

    }

}
