
namespace PhotonInMaze.Common.Flow {
    public abstract class FlowUpdateBehaviour : FlowBehaviour {

        private IInvoke action;

        private void Update() {
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
