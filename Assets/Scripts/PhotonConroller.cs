using System.Collections.Generic;
using UnityEngine;


public class PhotonConroller : MonoBehaviour {

    private Vector2 fingerStart;
    private Vector2 fingerEnd;
    private Optional<MazeController> mazeController;
    private bool canSwipe;
    private bool actuallyMoving;
    private float minDistanceToNextMove = 0.001f;
    private MazeCell currentCell, lastSaved;
    private Queue<MazeCell> movementsToMake;

    public bool IsInPathToGoal { get; private set; }

    public enum Movement {
        Left,
        Right,
        Up,
        Down
    };

    // Start is called before the first frame update
    void Start() {
        mazeController = ObjectsManager.Instance.GetMazeScript();
        if(!mazeController.HasValue) {
            Debug.LogError("MazeController not preset!");
            return;
        }
        movementsToMake = new Queue<MazeCell>();
        canSwipe = true;
        actuallyMoving = false;
        currentCell = mazeController.Get().GetMazeCell(0, 0).Get();
        lastSaved = currentCell;
    }

    // Update is called once per frame
    void Update() {
        mazeController = ObjectsManager.Instance.GetMazeScript();
        if(!mazeController.HasValue) {
            return;
        }
        Utils.CheckIfGameRunningAndCallUpdate(() => {
            if(movementsToMake.Count > 0 && !actuallyMoving) {
                currentCell = movementsToMake.Dequeue();
                actuallyMoving = true;
                ObjectsManager.Instance
                .GetMazeScript()
                .ForValuePresented((script) => IsInPathToGoal = script.PathsToGoal.Contains(currentCell));
            } else if(actuallyMoving) {
                transform.position = Vector3.Lerp(transform.position, currentCell.RealObjectPosition, 0.5f);
                if(Vector3.Distance(transform.position, currentCell.RealObjectPosition) <= minDistanceToNextMove) {
                    actuallyMoving = false;
                }
            }
           
            foreach(Touch touch in Input.touches) {
                if(touch.phase == TouchPhase.Began) {
                    fingerStart = touch.position;
                    fingerEnd = touch.position;
                }
                if(touch.phase == TouchPhase.Moved && canSwipe) {
                    fingerEnd = touch.position;

                    Movement movementDirection = GetTouchMovementDirection();
                    NextMove(movementDirection);
                    fingerStart = touch.position;
                    canSwipe = false;
                }
                if(touch.phase == TouchPhase.Ended) {
                    canSwipe = true;
                }
            }
        });
    }

    private void NextMove(Movement movementDirection) {
        if(!mazeController.HasValue) {
            return;
        }
        MazeController mazeScript = mazeController.Get();

        switch(movementDirection) {
            case Movement.Left:
                if(!lastSaved.WallBack) {
                    mazeScript.GetMazeCell(lastSaved.Row - 1, lastSaved.Column)
                          .ForValuePresented(newCell => UpdateCellPosition(newCell));
                }
                break;
            case Movement.Right:
                if(!lastSaved.WallFront) {
                    mazeScript.GetMazeCell(lastSaved.Row + 1, lastSaved.Column)
                         .ForValuePresented(newCell => UpdateCellPosition(newCell));
                }
                break;
            case Movement.Up:
                if(!lastSaved.WallLeft) {
                    mazeScript.GetMazeCell(lastSaved.Row, lastSaved.Column - 1)
                         .ForValuePresented(newCell => UpdateCellPosition(newCell));
                }
                break;
            case Movement.Down:
                if(!lastSaved.WallRight) {
                    mazeScript.GetMazeCell(lastSaved.Row, lastSaved.Column + 1)
                        .ForValuePresented(newCell => UpdateCellPosition(newCell));
                }
                break;
        }
    }

    private void UpdateCellPosition(MazeCell newCell) {
        lastSaved = newCell;
        movementsToMake.Enqueue(newCell);
    }

    private Movement GetTouchMovementDirection() {
        float xMove = Mathf.Abs(fingerStart.x - fingerEnd.x);
        float yMove = Mathf.Abs(fingerStart.y - fingerEnd.y);
        if(xMove > yMove) {
            return (fingerEnd.x - fingerStart.x) > 0 ? Movement.Right : Movement.Left;
        } else {
            return (fingerEnd.y - fingerStart.y) > 0 ? Movement.Up : Movement.Down;
        }
    }
}
