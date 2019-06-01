

namespace PhotonInMaze.Common.Flow {
    
    public class GameFlowManager : SceneSingleton<GameFlowManager>, IGetCurrentState {

        public GameFlow Flow { get; } = new GameFlow();

        public State GetCurrentState() {
            return Flow.CurrentState;
        }
    }
}