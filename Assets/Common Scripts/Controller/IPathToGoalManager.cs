using PhotonInMaze.Common.Model;
using System.Collections.Generic;

namespace PhotonInMaze.Common.Controller {
    public interface IPathToGoalManager {
        LinkedListNode<IMazeCell> GetFirstFromPath();
        int GetPathToGoalSize();
        void RemoveFirst();
        void AddFirst(IMazeCell value);
        int IndexInPath(IMazeCell value);
        IMazeCell FindPathToGoalFrom(IMazeCell value);
    }
}
