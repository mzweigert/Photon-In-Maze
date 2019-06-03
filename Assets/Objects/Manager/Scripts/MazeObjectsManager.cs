using PhotonInMaze.Common;
using PhotonInMaze.Game.Maze;
using UnityEngine;

namespace PhotonInMaze.Game.Manager {
    public class MazeObjectsManager : SceneSingleton<MazeObjectsManager> {


        [SerializeField]
        private GameObject maze = null;

        [SerializeField]
        private GameObject floor = null;

        [SerializeField]
        private GameObject wall = null;


        [SerializeField]
        private GameObject blackHole = null;

        [SerializeField]
        private GameObject whiteHole = null;

        public GameObject GetMaze() {
            LogIfObjectIsNull(maze, "Maze");
            return maze;
        }

        public MazeController GetMazeScript() {
            var script = maze.GetComponent<MazeController>();
            LogIfObjectIsNull(script, "MazeController");
            return script;
        }

        public GameObject GetFloor() {
            LogIfObjectIsNull(floor, "Floor");
            return floor;
        }

        public GameObject GetWall() {
            LogIfObjectIsNull(wall, "Wall");
            return wall;
        }

        public GameObject GetWhiteHole() {
            LogIfObjectIsNull(whiteHole, "WhiteHole");
            return whiteHole;
        }

        public GameObject GetBlackHole() {
            LogIfObjectIsNull(blackHole, "BlackHole");
            return blackHole;
        }


        private void LogIfObjectIsNull(Object o, string name) {
            if(o == null) {
                Debug.LogError(name + " is null!");
            }
        }

    }
}