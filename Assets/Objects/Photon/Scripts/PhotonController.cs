using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum Movement {
    Left,
    Right,
    Up,
    Down
};

public partial class PhotonController : MonoObserveable<PhotonState> {

    private Optional<MazeController> mazeController = Optional<MazeController>.Empty();

    private Vector2 fingerStart, fingerEnd;
    private bool canSwipe = true;

    public LinkedListNode<MazeCell> LastNodeCellFromPathToGoal { get; private set; }
    public MazeCell CurrentMazeCell { get; private set; }

    private MazeCell lastSaved;
    private Queue<MazeCell> movementsToMake = new Queue<MazeCell>();
    private readonly float minDistanceToNextMove = 0.1f;

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
        LastNodeCellFromPathToGoal = mazeController.Get().PathsToGoal.First;
        CurrentMazeCell = LastNodeCellFromPathToGoal.Value;
        lastSaved = CurrentMazeCell;
        photonState = new PhotonState(transform.position);

        photonLight = GetComponentInChildren<Light>();
        photonLight.intensity = 0f;
    }

    // Update is called once per frame
    void Update() {
        mazeController = ObjectsManager.Instance.GetMazeScript();
        if(!mazeController.HasValue) {
            return;
        }
        if(GameFlow.Instance.Is(GameFlow.State.LightTurnedOff) && photonLight && !photonLightAlreadySet) {
            photonLight.intensity = 7.5f;
            photonLightAlreadySet = true;
            GameFlow.Instance.StartGame();
        }

        GameFlow.Instance.CallUpdateWhenGameIsRunning(() => {

            if(movementsToMake.Count > 0 && !photonState.IsAcutallyMoving) {
                CurrentMazeCell = movementsToMake.Dequeue();
                photonState.IsAcutallyMoving = true;
                ChangePositionInfoInPathToGoal(CurrentMazeCell);
            } else if(photonState.IsAcutallyMoving) {
                Vector3 targetPosition = new Vector3(CurrentMazeCell.X, transform.position.y, CurrentMazeCell.Y);
                transform.position = Vector3.Lerp(transform.position, targetPosition, PhotonSpeed);
                if(Vector3.Distance(transform.position, targetPosition) <= minDistanceToNextMove) {
                    transform.position = targetPosition;
                    photonState.IsAcutallyMoving = false;
                }
                photonState.RealPosition = transform.position;
                NotifyObservers();
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
            if(EventSystem.current.IsPointerOverGameObject(touch.fingerId)) {
                canSwipe = false;
                return;
            } else if(ObjectsManager.Instance.IsArrowPresent()) {
                canSwipe = true;
                return;
            }

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
        if(LastNodeCellFromPathToGoal == null) {
            photonState.IsInPathToGoal = false;
        } else if(currentCell.IsGoal) {
            print("Congratulations! You are finished maze!");
        } else if(LastNodeCellFromPathToGoal.Next != null && currentCell.Equals(LastNodeCellFromPathToGoal.Next.Value)) {
            LastNodeCellFromPathToGoal = LastNodeCellFromPathToGoal.Next;
            photonState.IndexOfLastCellInPathToGoal++;
        } else if(LastNodeCellFromPathToGoal.Previous != null && currentCell.Equals(LastNodeCellFromPathToGoal.Previous.Value)) {
            LastNodeCellFromPathToGoal = LastNodeCellFromPathToGoal.Previous;
            photonState.IndexOfLastCellInPathToGoal--;
        } else if(!currentCell.Equals(LastNodeCellFromPathToGoal.Value)) {
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
                if(!lastSaved.Walls.Contains(Direction.Back)) {
                    UpdateCellPosition(lastSaved.Row - 1, lastSaved.Column);
                }
                break;
            case Movement.Right:
                if(!lastSaved.Walls.Contains(Direction.Front) && !lastSaved.IsGoal) {
                    UpdateCellPosition(lastSaved.Row + 1, lastSaved.Column);
                }
                break;
            case Movement.Up:
                if(!lastSaved.Walls.Contains(Direction.Left)) {
                    UpdateCellPosition(lastSaved.Row, lastSaved.Column - 1);
                }
                break;
            case Movement.Down:
                if(!lastSaved.Walls.Contains(Direction.Right)) {
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

    protected override PhotonState GetData() {
        return photonState;
    }
}
