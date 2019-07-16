
namespace PhotonInMaze.Common.Controller {
    public interface IMazeConfiguration {
        int Columns { get; }
        int Rows { get; }
        MazeGenerationAlgorithm Algorithm { get; }
        float CellSideLength { get; }
        string Name { get; }

        IMazeGenerator GetGenerator();
    }
}
