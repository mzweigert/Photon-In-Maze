using System;
using System.Collections.Generic;
using UnityEngine;

public struct PhotonInPathToGoalInfo {
    public bool IsInPathToGoal { get; set; }
    public int PositionInPathToGoal { get; set; }
}

public enum Movement {
    Left,
    Right,
    Up,
    Down
};

public class PhotonConroller : MonoBehaviour, IObservable<PhotonInPathToGoalInfo> {

    private Optional<MazeController> mazeController;

    private Vector2 fingerStart;
    private Vector2 fingerEnd;
    private bool canSwipe;
    private bool actuallyMoving;

    private float minDistanceToNextMove = 0.1f;
    private MazeCell currentCell, lastSaved;
    private LinkedListNode<MazeCell> currentFromPathToGoal;
    private Queue<MazeCell> movementsToMake;

    private List<IObserver<PhotonInPathToGoalInfo>> observers;
    private PhotonInPathToGoalInfo photonInPathToGoalInfo;

    private Light photonLight;
    private bool photonLightAlreadySet;
   

    // Start is called before the first frame update
    void Start() {
        mazeController = ObjectsManager.Instance.GetMazeScript();
        if(!mazeController.HasValue) {
            Debug.LogError("MazeController not preset!");
            return;
        }
        observers = new List<IObserver<PhotonInPathToGoalInfo>>();
        movementsToMake = new Queue<MazeCell>();
        canSwipe = true;
        actuallyMoving = false;
        photonInPathToGoalInfo = new PhotonInPathToGoalInfo();
        photonInPathToGoalInfo.IsInPathToGoal = true;
        currentFromPathToGoal = mazeController.Get()
            .PathsToGoal
            .First;
        currentCell = currentFromPathToGoal.Value;
        lastSaved = currentCell;
        photonLight = GetComponentInChildren<Light>();
        photonLight.intensity = 0f;
    }

    // Update is called once per frame
    void Update() {
        mazeController = ObjectsManager.Instance.GetMazeScript();
        if(!mazeController.HasValue) {
            return;
        }
        if(GameEvent.Instance.IsLightTurnedOff && !photonLightAlreadySet) {
            photonLight.intensity = 7.5f;
            photonLightAlreadySet = true;
            GameEvent.Instance.StartGame();
        }

        GameEvent.Instance.CallUpdateWhenGameIsRunning(() => {
            
            if(movementsToMake.Count > 0 && !actuallyMoving) {
                currentCell = movementsToMake.Dequeue();
                actuallyMoving = true;
                ChangePositionInfoInPathToGoal(currentCell);
            } else if(actuallyMoving) {
                Vector3 targetPosition 
                        = new Vector3(currentCell.RealObjectPosition.x, transform.position.y, currentCell.RealObjectPosition.z);
                transform.position = Vector3.Lerp(transform.position, targetPosition, 0.5f);
                if(Vector3.Distance(transform.position, targetPosition) <= minDistanceToNextMove) {
                    transform.position = targetPosition;
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

    private void ChangePositionInfoInPathToGoal(MazeCell currentCell) {
        if(currentCell.IsGoal) {
            print("Congratulations! You are finished maze!");
        } else if(currentFromPathToGoal.Next != null && currentCell.Equals(currentFromPathToGoal.Next.Value)) {
            currentFromPathToGoal = currentFromPathToGoal.Next;
            photonInPathToGoalInfo.PositionInPathToGoal++;
        } else if(currentFromPathToGoal.Previous != null && currentCell.Equals(currentFromPathToGoal.Previous.Value)) {
            currentFromPathToGoal = currentFromPathToGoal.Previous;
            photonInPathToGoalInfo.PositionInPathToGoal--;
        } else if(!currentCell.Equals(currentFromPathToGoal.Value)) {
            if(photonInPathToGoalInfo.IsInPathToGoal) {
                photonInPathToGoalInfo.IsInPathToGoal = false;
            }
            return;
        }
        if(!photonInPathToGoalInfo.IsInPathToGoal) {
            photonInPathToGoalInfo.IsInPathToGoal = true;
        }

        observers.ForEach((observer) => observer.OnNext(photonInPathToGoalInfo));
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
                          .IfPresent(newCell => UpdateCellPosition(newCell));
                }
                break;
            case Movement.Right:
                if(!lastSaved.WallFront && !lastSaved.IsGoal) {
                    mazeScript.GetMazeCell(lastSaved.Row + 1, lastSaved.Column)
                         .IfPresent(newCell => UpdateCellPosition(newCell));
                }
                break;
            case Movement.Up:
                if(!lastSaved.WallLeft) {
                    mazeScript.GetMazeCell(lastSaved.Row, lastSaved.Column - 1)
                         .IfPresent(newCell => UpdateCellPosition(newCell));
                }
                break;
            case Movement.Down:
                if(!lastSaved.WallRight) {
                    mazeScript.GetMazeCell(lastSaved.Row, lastSaved.Column + 1)
                        .IfPresent(newCell => UpdateCellPosition(newCell));
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

    public IDisposable Subscribe(IObserver<PhotonInPathToGoalInfo> observer) {
        if(!observers.Contains(observer)) {
            observers.Add(observer);
            // Provide observer with existing data.
            observer.OnNext(photonInPathToGoalInfo);
        }
        return new Unsubscriber<PhotonInPathToGoalInfo>(observers, observer);
    }
}
