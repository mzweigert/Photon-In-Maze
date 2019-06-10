
namespace PhotonInMaze.Common.Flow {
    public enum State {
        Start,
        GenerateMaze,
        GeneratePathToGoal,
        CreateMaze,
        MazeCreated,
        TurnOnLight,
        ShowPhoton,
        CountingDown,
        DestroyPathToGoal,
        DimAreaLight,
        TurnOnPhotonLight,
        GameRunning,
        EndGame,
        HidePhoton,
        TurnOffLight,
        Pause
    }
}