using UnityEngine;
using System.Collections.Generic;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Common;
using PhotonInMaze.Provider;
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Model;

namespace PhotonInMaze.Maze {
    internal partial class MazeController : FlowUpdateBehaviour, IMazeController {

        private void CreateMazeWithItems() {
            GameObject area = CreateArea();
            List<ObjectMazeCell> blackholes = new List<ObjectMazeCell>();
            List<ObjectMazeCell> whiteholes = new List<ObjectMazeCell>();
            HoleType holeType = HoleType.Black;
            GameObject cells = CreateCellsRoot();
            byte i = 1;
            for(int row = 0; row < configuration.Rows; row++) {
                for(int column = 0; column < configuration.Columns; column++) {
                    IMazeCell cell = MazeObjectsProvider.Instance.GetMazeCellManager().GetMazeCell(row, column);
                    GameObject cellGameObject = new GameObject() { name = cell.ToStringAsName() };
                    if(cell.IsProperPathToGoal) {
                        GameObject floor = CreateFloor(cellGameObject);
                        pathToGoalManager.pathToGoalFloors.Add(cell, floor);
                    }
                    cellGameObject.transform.parent = cells.transform;
                    cellGameObject.transform.position = new Vector3(cell.X, area.transform.position.y, cell.Y);
                    CreateWalls(cell, cellGameObject);
                    CreateHole(holeType, cell, cellGameObject.transform, ref i).IfPresent(hole => {
                        ObjectMazeCell objectMazeCell = new ObjectMazeCell(cell, hole);
                        if(holeType == HoleType.Black) {
                            blackholes.Add(objectMazeCell);
                            holeType = HoleType.White;
                        } else {
                            whiteholes.Add(objectMazeCell);
                            holeType = HoleType.Black;
                        }
                    });
                }
            }
            Wormholes = CreateWormholes(blackholes, whiteholes);
        }

        private GameObject CreateFloor(GameObject cellGameObject) {
            Vector3 pos = cellGameObject.transform.position;
            pos.y = cellGameObject.transform.position.y + 0.25f;
            GameObject cellInPath = Instantiate(floorPrototype, pos, Quaternion.identity, cellGameObject.transform);
            cellInPath.name = "PathToGoalFloor";
            return cellInPath;
        }

        private GameObject CreateCellsRoot() {
            Transform cellsRootTransform = transform.Find("Cells");
            if(cellsRootTransform != null) {
                Destroy(cellsRootTransform.gameObject);
            } 

            GameObject cells = new GameObject() { name = "Cells" };
            cells.transform.parent = transform;
            return cells;
        }

        private GameObject CreateArea() {
            Transform areaTransform = transform.Find("Area");
            if(areaTransform != null) {
                Destroy(areaTransform.gameObject);
            }
            GameObject area = new GameObject { name = "Area" };
            area.transform.parent = transform;
            float rowBound = (configuration.Rows * configuration.LenghtOfCellSide / 2) - configuration.LenghtOfCellSide / 2;
            float columnBound = (configuration.Columns * configuration.LenghtOfCellSide / 2) - configuration.LenghtOfCellSide / 2;
            GameObject floor = Instantiate(floorPrototype, new Vector3(columnBound, 0, rowBound), 
                Quaternion.Euler(0, 0, 0), area.transform);
            floor.transform.localScale = new Vector3(configuration.Columns * configuration.LenghtOfCellSide, 0.25f,
                configuration.Rows * configuration.LenghtOfCellSide);
            floor.name = "Floor";
            Vector3 wallPos; Quaternion wallQuat; float xLength, offset;
            for(int i = 0; i < 4; i++) {
                bool column = i % 2 == 0;

                if(column) {
                    offset = i > 1 ? configuration.LenghtOfCellSide : -configuration.LenghtOfCellSide;
                    wallPos = new Vector3(i * columnBound + offset / 2, area.transform.position.y, rowBound);
                    wallQuat = Quaternion.Euler(0, (i - 1) * 90, 0);
                    xLength = configuration.Rows;

                } else {
                    offset = i > 2 ? configuration.LenghtOfCellSide : -configuration.LenghtOfCellSide;
                    wallPos = new Vector3(columnBound, area.transform.position.y, (i - 1) * rowBound + offset / 2);
                    wallQuat = Quaternion.Euler(0, (i + 1) * 90, 0);
                    xLength = configuration.Columns;
                }

                GameObject wall = Instantiate(wallPrototype, wallPos, wallQuat);
                wall.name = "Wall";
                wall.transform.localScale = new Vector3(xLength, wall.transform.localScale.y, wall.transform.localScale.z);
                wall.transform.parent = area.transform;
            }

            return area;
        }

        private void CreateWalls(IMazeCell cell, GameObject cellGameObject) {
            GameObject wall;
            if(cell.Walls.Contains(Direction.Right) && cell.Column + 1 < configuration.Columns) {
                wall = Instantiate(wallPrototype, 
                    new Vector3(cell.X + configuration.LenghtOfCellSide / 2, 0, cell.Y) + wallPrototype.transform.position,
                    Quaternion.Euler(0, 90, 0)) as GameObject;// right
                wall.name = "WallRight";
                wall.transform.parent = cellGameObject.transform;
            }
            if(cell.Walls.Contains(Direction.Front) && cell.Row + 1 < configuration.Rows) {
                wall = Instantiate(wallPrototype, 
                    new Vector3(cell.X, 0, cell.Y + configuration.LenghtOfCellSide / 2) + wallPrototype.transform.position,
                    Quaternion.Euler(0, 0, 0)) as GameObject;// front
                wall.name = "WallFront";
                wall.transform.parent = cellGameObject.transform;
            }
            if(cell.Walls.Contains(Direction.Left) && cell.Column > 0) {
                wall = Instantiate(wallPrototype, 
                    new Vector3(cell.X - configuration.LenghtOfCellSide / 2, 0, cell.Y) + wallPrototype.transform.position, 
                    Quaternion.Euler(0, 270, 0)) as GameObject;// left
                wall.name = "WallLeft";
                wall.transform.parent = cellGameObject.transform;
            }
            if(cell.Walls.Contains(Direction.Back) && cell.Row > 0) {
                wall = Instantiate(wallPrototype, 
                    new Vector3(cell.X, 0, cell.Y - configuration.LenghtOfCellSide / 2) + wallPrototype.transform.position, 
                    Quaternion.Euler(0, 180, 0)) as GameObject;// back
                wall.name = "WallBack";
                wall.transform.parent = cellGameObject.transform;
            }

        }

    }
}