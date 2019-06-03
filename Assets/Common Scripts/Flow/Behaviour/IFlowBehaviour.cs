
namespace PhotonInMaze.Common.Flow {
    public interface IFlowBehaviour {

        void Prepare();

        int GetInitOrder();

        void OnStart();

        IInvoke OnLoop();

    }
}