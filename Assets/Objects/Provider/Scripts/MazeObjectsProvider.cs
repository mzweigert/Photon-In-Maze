using PhotonInMaze.Common;
using PhotonInMaze.Common.Controller;
using UnityEngine;

namespace PhotonInMaze.Provider {
    public class MazeObjectsProvider : SceneSingleton<MazeObjectsProvider> {


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

        public IMazeController GetMazeController() {
            var script = maze.GetComponent<IMazeController>();
            LogIfObjectIsNull(script, "MazeController");
            return script;
        }

        public IPathToGoalManager GetPathToGoalManager() {
            var script = maze.GetComponent<IPathToGoalManager>();
            LogIfObjectIsNull(script, "PathToGoalManager");
            return script;
        }

        public IMazeCellManager GetMazeCellManager() {
            var script = maze.GetComponent<IMazeCellManager>();
            LogIfObjectIsNull(script, "MazeCellManager");
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


        private void LogIfObjectIsNull(object o, string name) {
            if(o == null) {
                Debug.LogError(name + " is null!");
            }
        }

    }
}