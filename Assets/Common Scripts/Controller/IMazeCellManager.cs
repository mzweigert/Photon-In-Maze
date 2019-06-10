

using PhotonInMaze.Common.Model;
using System.Collections.Generic;

namespace PhotonInMaze.Common.Controller {
    public interface IMazeCellManager {
        IMazeCell GetMazeCell(int row, int column);
        bool IsPathToGoalVisited(HashSet<IMazeCell> visitedCells);
        bool IsATrap(HashSet<IMazeCell> visitedCells, IMazeCell current);
        IMazeCell GetExitCell();
        IMazeCell GetStartCell();
    }
}
