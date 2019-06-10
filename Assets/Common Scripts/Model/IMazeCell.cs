
using System.Collections.Generic;
using UnityEngine;

namespace PhotonInMaze.Common.Model {
    public interface IMazeCell {
        HashSet<Direction> Walls { get; }

        float X { get; }
        float Y { get; }

        int Row { get; }
        int Column { get; }

        bool IsGoal { get; }
        bool IsProperPathToGoal { get; }
        bool IsTrap { get; }

        Direction GetDirectionTo(IMazeCell next);
        HashSet<Direction> GetPossibleMovesDirection();
        HashSet<Vector2Int> GetPossibleMovesCoords();

        bool IsStartCell();
        string ToStringAsName();
    }
}
