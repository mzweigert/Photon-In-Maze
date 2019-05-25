using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public partial class MazeController : MonoBehaviour {

    public Dictionary<int, MazeCell> Wormholes { get; private set; } = new Dictionary<int, MazeCell>();
    public HashSet<MazeCell> BlackHolesPositions { get; private set; } = new HashSet<MazeCell>();
    public HashSet<MazeCell> WhiteHolesPositions { get; private set; } = new HashSet<MazeCell>();

    private enum HoleType {
        Black, White
    }

    private Optional<GameObject> CreateHole(HoleType type, MazeCell cell, Transform cellGameObject, ref byte counter) {
        if(cell.Walls.Count < 3) {
            return Optional<GameObject>.Empty();
        }
        GameObject hole = null;
        counter++;
        if(!cell.IsStartCell() && !cell.IsGoal && counter == 10) {
            if(type == HoleType.Black) {
                GameObject blackHolePrototype = ObjectsManager.Instance.GetBlackHole();
                hole = Instantiate(blackHolePrototype, cellGameObject);
                hole.name = string.Format("BlackHole_{0}_{1}", cell.Row, cell.Column);
            } else if(type == HoleType.White) {
                GameObject whiteHolePrefab = ObjectsManager.Instance.GetWhiteHole();
                hole = Instantiate(whiteHolePrefab, cellGameObject);
                hole.name = string.Format("WhiteHole_{0}_{1}", cell.Row, cell.Column);
            }
            int sortingOrder = System.Math.Abs(hole.transform.GetInstanceID());
            while(sortingOrder > short.MaxValue) {
                sortingOrder -= short.MaxValue;
            }
            counter = 1;
        }

        return Optional<GameObject>.OfNullable(hole);
    }

    private Dictionary<int, MazeCell> CreateWormholes(List<ObjectMazeCell> blackholes, List<ObjectMazeCell> whiteholes) {
        int length;
        length = CalculateSizeOfWormholes(blackholes, whiteholes);

        System.Random random = new System.Random();
        Dictionary<int, MazeCell> wormholes = new Dictionary<int, MazeCell>();
        blackholes = blackholes.GetRange(0, length).OrderBy(x => random.Next()).ToList();
        whiteholes = whiteholes.GetRange(0, length).OrderBy(x => random.Next()).ToList();

        for(int i = 0; i < length; i++) {
            int blackHoleId = blackholes[i].gameObject.transform.GetInstanceID();
            MazeCell whiteHoleCell = whiteholes[i].cell;
            wormholes.Add(blackHoleId, whiteHoleCell);
            WhiteHolesPositions.Add(whiteHoleCell);
            BlackHolesPositions.Add(blackholes[i].cell);
        }

        return wormholes;
    }

    private static int CalculateSizeOfWormholes(List<ObjectMazeCell> blackholes, List<ObjectMazeCell> whiteholes) {
        int length;
        if(whiteholes.Count < blackholes.Count) {
            length = whiteholes.Count;
            for(int i = whiteholes.Count; i < blackholes.Count; i++) Destroy(blackholes[i].gameObject);
        } else if(blackholes.Count > whiteholes.Count) {
            length = blackholes.Count;
            for(int i = blackholes.Count; i < whiteholes.Count; i++) Destroy(whiteholes[i].gameObject);
        } else {
            length = blackholes.Count;
        }

        return length;
    }

    internal bool IsWhiteHolePosition(MazeCell newCell) {
        return WhiteHolesPositions.Contains(newCell);
    }

    internal bool IsBlackHolePosition(MazeCell newCell) {
        return BlackHolesPositions.Contains(newCell);
    }
}