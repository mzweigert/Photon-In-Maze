using System.Collections.Generic;
using UnityEngine;


public class PhotonConroller : MonoBehaviour {

    private Vector2 fingerStart;
    private Vector2 fingerEnd;
    private Vector3 currentPos;
    private Vector3 nextPos;
    private MazeController mazeController;
    private MazeCell currentCell;
    private bool canSwipe;
    private bool actuallyMoving;
    private float minDistanceToNextMove = 0.001f;
    private Queue<Vector3> movements;

    public enum Movement {
        Left,
        Right,
        Up,
        Down
    };

    // Start is called before the first frame update
    void Start() {
        movements = new Queue<Vector3>();
        currentPos = transform.position;
        nextPos = currentPos;
        canSwipe = true;
        actuallyMoving = false;
        mazeController = FindObjectOfType<MazeController>();
        currentCell = mazeController.GetMazeCell(0, 0).Get();
    }

    // Update is called once per frame
    void Update() {
        Utils.CheckIfGameRunningAndCallUpdate(() => {
            if(movements.Count > 0 && !actuallyMoving) {
                currentPos = movements.Dequeue();
                actuallyMoving = true;
            } else if(actuallyMoving) {
                transform.position = Vector3.Lerp(transform.position, currentPos, 0.5f);
                if(Vector3.Distance(transform.position, currentPos) <= minDistanceToNextMove) {
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
        Vector3 newPosition = Vector3.zero;

        switch(movementDirection) {
            case Movement.Left:
                if(!currentCell.WallBack) {
                    newPosition = new Vector3(nextPos.x, nextPos.y, nextPos.z - mazeController.LenghtSide);
                    mazeController.GetMazeCell(currentCell.Row - 1, currentCell.Column)
                        .ForValuePresented(cell => UpdateCellPosition(cell, newPosition));
                }
                break;
            case Movement.Right:
                if(!currentCell.WallFront) {
                    newPosition = new Vector3(nextPos.x, nextPos.y, nextPos.z + mazeController.LenghtSide);
                    mazeController.GetMazeCell(currentCell.Row + 1, currentCell.Column)
                        .ForValuePresented(cell => UpdateCellPosition(cell, newPosition));
                }
                break;
            case Movement.Up:
                if(!currentCell.WallLeft) {
                    newPosition = new Vector3(nextPos.x - mazeController.LenghtSide, nextPos.y, nextPos.z);
                    mazeController.GetMazeCell(currentCell.Row, currentCell.Column - 1)
                        .ForValuePresented(cell => UpdateCellPosition(cell, newPosition));
                }
                break;
            case Movement.Down:
                if(!currentCell.WallRight) {
                    newPosition = new Vector3(nextPos.x + mazeController.LenghtSide, nextPos.y, nextPos.z);
                    mazeController.GetMazeCell(currentCell.Row, currentCell.Column +1)
                        .ForValuePresented(cell => UpdateCellPosition(cell, newPosition));
                }
                break;
        }
    }

    private void UpdateCellPosition(MazeCell cell, Vector3 newPosition) {
        currentCell = cell;
        movements.Enqueue(newPosition);
        nextPos = newPosition;
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
