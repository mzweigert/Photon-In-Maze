using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Common.Model;
using System.Collections.Generic;
using UnityEngine;

namespace PhotonInMaze.Maze {
    internal partial class PathToGoalManager : FlowUpdateBehaviour, IPathToGoalManager {

        [SerializeField]
        private Material pathLine, pathCurve, pathEnd;

        internal Dictionary<IMazeCell, GameObject> pathToGoalFloors { get; private set; }

        private struct FloorState {
            public float yRotation;
            public Material material;

            public FloorState(float yRotation, Material material) {
                this.yRotation = yRotation;
                this.material = material;
            }
        }

        private void PaintPathToGoal() {
            LinkedListNode<IMazeCell> current = PathToGoal.First;

            while(current != null) {
                GameObject currentFloor = pathToGoalFloors[current.Value];
                Renderer renderer = currentFloor.GetComponent<Renderer>();
                LinkedListNode<IMazeCell> next = current.Next, previous = current.Previous;
                FloorState floorState = new FloorState();
                if(previous == null) {
                    floorState = InitEnd(current.Value, next.Value);
                } else if(next == null) {
                    floorState = InitEnd(current.Value, previous.Value);
                } else {
                    floorState = InitFloor(current, next, previous);
                }
                currentFloor.transform.rotation = Quaternion.Euler(0f, floorState.yRotation, 0f);
                renderer.material = floorState.material;
                current = next;
            }
            GameFlowManager.Instance.Flow.NextState();
        }

        private FloorState InitEnd(IMazeCell current, IMazeCell related) {
            if(related.Row < current.Row) {
                return new FloorState(90f, pathEnd);
            } else if(related.Row > current.Row) {
                return new FloorState(270f, pathEnd);
            } else if(related.Column < current.Column) {
                return new FloorState(180f, pathEnd);
            } else {
                return new FloorState(0f, pathEnd);
            }
        }

        private FloorState InitFloor(LinkedListNode<IMazeCell> current, LinkedListNode<IMazeCell> next, LinkedListNode<IMazeCell> previous) {
            FloorState floorState = new FloorState();
            if(current.Value.Row < next.Value.Row) {
                if(previous.Value.Row < current.Value.Row) {
                    floorState = new FloorState(90f, pathLine);
                } else {
                    floorState = new FloorState(previous.Value.Column < current.Value.Column ? 180f : -90f, pathCurve);
                }
            } else if(current.Value.Row > next.Value.Row) {
                if(previous.Value.Row > current.Value.Row) {
                    floorState = new FloorState(90f, pathLine);
                } else {
                    floorState = new FloorState(previous.Value.Column < current.Value.Column ? 90f : 0f, pathCurve);
                }
            } else if(current.Value.Column < next.Value.Column) {
                if(previous.Value.Column < current.Value.Column) {
                    floorState = new FloorState(0f, pathLine);
                } else {
                    floorState = new FloorState(previous.Value.Row < current.Value.Row ? 0 : -90f, pathCurve);
                }
            } else if(current.Value.Column > next.Value.Column) {
                if(previous.Value.Column > current.Value.Column) {
                    floorState = new FloorState(0f, pathLine);
                } else {
                    floorState = new FloorState(previous.Value.Row < current.Value.Row ? 90f : 180f, pathCurve);
                }
            }

            return floorState;
        }
    }
}