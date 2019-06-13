using PhotonInMaze.Common.Model;

namespace PhotonInMaze.Common.Controller {
    public interface IMazeController {
        int Columns { get; }
        int Rows { get; }
        float ScaleOfCellSide { get; }
        float LenghtOfCellSide { get; }

        bool IsWhiteHolePosition(IMazeCell newCell);
        bool IsBlackHolePosition(IMazeCell newCell);

        IMazeCell GetWormholeExit(int blackHoleId);
        void Recreate(int rows, int columns, MazeGenerationAlgorithm algorithm);
    }
}
