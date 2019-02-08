using System;
using System.Collections.Generic;
using UnityEngine;

public struct PhotonState {
    public bool IsInPathToGoal { get; internal set; }
    public int PositionInPathToGoal { get; internal set; }
    public Vector3 RealPosition { get; internal set; }

    public PhotonState(bool isInPathToGoal, int positionInPathToGoal, Vector3 realPosition) : this() {
        IsInPathToGoal = isInPathToGoal;
        PositionInPathToGoal = positionInPathToGoal;
        RealPosition = realPosition;
    }
}

public enum Movement {
    Left,
    Right,
    Up,
    Down
};

public class PhotonConroller : MonoBehaviour, IObservable<PhotonState> {

    private Optional<MazeController> mazeController = Optional<MazeController>.Empty();

    private Vector2 fingerStart, fingerEnd;
    private bool canSwipe = true;
    private bool actuallyMoving = false;

    private float minDistanceToNextMove = 0.1f;
    private MazeCell currentCell, lastSaved;
    private LinkedListNode<MazeCell> currentFromPathToGoal;
    private Queue<MazeCell> movementsToMake = new Queue<MazeCell>();

    private List<IObserver<PhotonState>> observers = new List<IObserver<PhotonState>>();
    private PhotonState photonState;

    private Light photonLight;
    private bool photonLightAlreadySet;

    [Range(0.1f, 2f)]
    public float PhotonSpeed;

    // Start is called before the first frame update
    void Start() {
        mazeController = ObjectsManager.Instance.GetMazeScript();
        if(!mazeController.HasValue) {
            Debug.LogError("MazeController not preset!");
            return;
        }
        photonState = new PhotonState(true, 0, transform.position);
        currentFromPathToGoal = mazeController.Get().PathsToGoal.First;
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
        if(GameEvent.Instance.IsLightTurnedOff && photonLight && !photonLightAlreadySet) {
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
                Vector3 targetPosition = new Vector3(currentCell.X, transform.position.y, currentCell.Y);
                transform.position = Vector3.Lerp(transform.position, targetPosition, PhotonSpeed);
                if(Vector3.Distance(transform.position, targetPosition) <= minDistanceToNextMove) {
                    transform.position = targetPosition;
                    actuallyMoving = false;
                }
                photonState.RealPosition = transform.position;
                observers.ForEach((observer) => observer.OnNext(photonState));
            }

#if UNITY_EDITOR
            CheckButtonPress();
#endif
            CheckTouch();

        });
    }

    private void CheckButtonPress() {
        if(CheckIfAnyPressed(KeyCode.W, KeyCode.UpArrow)) {
            NextMove(Movement.Up);
        } else if(CheckIfAnyPressed(KeyCode.A, KeyCode.LeftArrow)) {
            NextMove(Movement.Left);
        } else if(CheckIfAnyPressed(KeyCode.S, KeyCode.DownArrow)) {
            NextMove(Movement.Down);
        } else if(CheckIfAnyPressed(KeyCode.D, KeyCode.RightArrow)) {
            NextMove(Movement.Right);
        }
    }

    private bool CheckIfAnyPressed(params KeyCode[] codes) {
        foreach(KeyCode code in codes) {
            if(Input.GetKeyDown(code)) {
                return true;
            }
        }
        return false;
    }

    private void CheckTouch() {
        if(Input.touchCount == 1) {
            Touch touch = Input.GetTouch(0);
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
    }

    private void ChangePositionInfoInPathToGoal(MazeCell currentCell) {
        if(currentFromPathToGoal == null) {
            photonState.IsInPathToGoal = false;
        } else if(currentCell.IsGoal) {
            print("Congratulations! You are finished maze!");
        } else if(currentFromPathToGoal.Next != null && currentCell.Equals(currentFromPathToGoal.Next.Value)) {
            currentFromPathToGoal = currentFromPathToGoal.Next;
            photonState.PositionInPathToGoal++;
        } else if(currentFromPathToGoal.Previous != null && currentCell.Equals(currentFromPathToGoal.Previous.Value)) {
            currentFromPathToGoal = currentFromPathToGoal.Previous;
            photonState.PositionInPathToGoal--;
        } else if(!currentCell.Equals(currentFromPathToGoal.Value)) {
            if(photonState.IsInPathToGoal) {
                photonState.IsInPathToGoal = false;
            }
            return;
        }
        if(!photonState.IsInPathToGoal) {
            photonState.IsInPathToGoal = true;
        }
    }

    private void NextMove(Movement movementDirection) {
        if(!mazeController.HasValue) {
            return;
        }

        switch(movementDirection) {
            case Movement.Left:
                if(!lastSaved.WallBack) {
                    UpdateCellPosition(lastSaved.Row - 1, lastSaved.Column);
                }
                break;
            case Movement.Right:
                if(!lastSaved.WallFront && !lastSaved.IsGoal) {
                    UpdateCellPosition(lastSaved.Row + 1, lastSaved.Column);
                }
                break;
            case Movement.Up:
                if(!lastSaved.WallLeft) {
                    UpdateCellPosition(lastSaved.Row, lastSaved.Column - 1);
                }
                break;
            case Movement.Down:
                if(!lastSaved.WallRight) {
                    UpdateCellPosition(lastSaved.Row, lastSaved.Column + 1);
                }
                break;
        }
    }

    private void UpdateCellPosition(int row, int column) {
        MazeController mazeScript = mazeController.Get();
        mazeScript.GetMazeCell(row, column).IfPresent(newCell => {
            lastSaved = newCell;
            movementsToMake.Enqueue(newCell);
        });
    }

    private Movement GetTouchMovementDirection() {
        float xMove = Mathf.Abs(fingerStart.x - fingerEnd.x);
        float yMove = Mathf.Abs(fingerStart.y - fingerEnd.y);
        if(xMove > yMove) {
            return (fingerEnd.x - fingerStart.x) > 0.65f ? Movement.Right : Movement.Left;
        } else {
            return (fingerEnd.y - fingerStart.y) > 0.65 ? Movement.Up : Movement.Down;
        }
    }

    public IDisposable Subscribe(IObserver<PhotonState> observer) {
        if(!observers.Contains(observer)) {
            observers.Add(observer);
            // Provide observer with existing data.
            observer.OnNext(photonState);
        }
        return new Unsubscriber<PhotonState>(observers, observer);
    }

}
