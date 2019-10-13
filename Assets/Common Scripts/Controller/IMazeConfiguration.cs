
namespace PhotonInMaze.Common.Controller {
    public interface IMazeConfiguration {
        int Columns { get; }
        int Rows { get; }
        int SecondsToStart { get; }
        MazeGenerationAlgorithm Algorithm { get; }
        float CellSideLength { get; }
        string Name { get; }

        IMazeGenerator GetGenerator();
    }
}
