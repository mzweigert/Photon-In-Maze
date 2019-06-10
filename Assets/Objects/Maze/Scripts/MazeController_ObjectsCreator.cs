using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PhotonInMaze.Common;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Provider;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Model;

namespace PhotonInMaze.Maze {
    public partial class MazeController : FlowUpdateBehaviour, IMazeController {

        public Dictionary<int, IMazeCell> Wormholes { get; private set; } = new Dictionary<int, IMazeCell>();
        public HashSet<IMazeCell> BlackHolesPositions { get; private set; }
        public HashSet<IMazeCell> WhiteHolesPositions { get; private set; }

        private enum HoleType {
            Black, White
        }

        private Optional<GameObject> CreateHole(HoleType type, IMazeCell cell, Transform cellGameObject, ref byte counter) {
            if(cell.Walls.Count < 3) {
                return Optional<GameObject>.Empty();
            }
            GameObject hole = null;
            counter++;
            if(!cell.IsStartCell() && !cell.IsGoal && counter == 10) {
                if(type == HoleType.Black) {
                    GameObject blackHolePrototype = MazeObjectsProvider.Instance.GetBlackHole();
                    hole = Instantiate(blackHolePrototype, cellGameObject);
                    hole.name = string.Format("BlackHole_{0}_{1}", cell.Row, cell.Column);
                } else if(type == HoleType.White) {
                    GameObject whiteHolePrefab = MazeObjectsProvider.Instance.GetWhiteHole();
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

        private Dictionary<int, IMazeCell> CreateWormholes(List<ObjectMazeCell> blackholes, List<ObjectMazeCell> whiteholes) {
            int length;
            length = CalculateSizeOfWormholes(blackholes, whiteholes);

            System.Random random = new System.Random();
            Dictionary<int, IMazeCell> wormholes = new Dictionary<int, IMazeCell>();
            blackholes = blackholes.GetRange(0, length).OrderBy(x => random.Next()).ToList();
            whiteholes = whiteholes.GetRange(0, length).OrderBy(x => random.Next()).ToList();
            BlackHolesPositions = new HashSet<IMazeCell>();
            WhiteHolesPositions = new HashSet<IMazeCell>();
            for(int i = 0; i < length; i++) {
                int blackHoleId = blackholes[i].gameObject.transform.GetInstanceID();
                IMazeCell whiteHoleCell = whiteholes[i].cell;
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

        public bool IsWhiteHolePosition(IMazeCell newCell) {
            return WhiteHolesPositions.Contains(newCell);
        }

        public bool IsBlackHolePosition(IMazeCell newCell) {
            return BlackHolesPositions.Contains(newCell);
        }

        public IMazeCell GetWormholeExit(int blackHoleId) {
            return Wormholes[blackHoleId];
        }

    }
}