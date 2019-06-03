namespace PhotonInMaze.Common.Flow {

    public interface IFlowManager {

        State GetCurrentState();

        void RegisterFlowBehaviour(IFlowBehaviour flowBehaviour);

        void ReinitializeFlowBehaviours();

    }
}