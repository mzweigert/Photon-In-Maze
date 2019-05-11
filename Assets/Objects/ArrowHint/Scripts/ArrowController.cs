using System;
using System.Collections.Generic;
using UnityEngine;

public enum ArrowState {
    Init,
    Checking,
    Moving,
    Rotating,
    Ending
}

public class ArrowController : MonoObserveable<ArrowState> {

    private LinkedListNode<MazeCell> currentCell;
    private Direction lastMove = Direction.Start, nextMove;
    private ArrowState currentState = ArrowState.Init;
    private int sizeOfPath;

    private PhotonController photonScript;
    private MazeController mazeScript;

    void Awake() {
        Optional<PhotonController> optionalPhotonScript = ObjectsManager.Instance.GetPhotonScript();
        Optional<MazeController> optionalMazeScript = ObjectsManager.Instance.GetMazeScript();
        if(optionalPhotonScript.HasNotValue || optionalMazeScript.HasNotValue) {
            return;
        }
        mazeScript = optionalMazeScript.Get();
        photonScript = optionalPhotonScript.Get();

        currentCell = photonScript.LastNodeCellFromPathToGoal;

        if(currentCell != null) {
            sizeOfPath = (int)Math.Ceiling(currentCell.List.Count * 0.15f);
            NotifyCameraAboutResize(sizeOfPath);
            InitArrowPosition();
            currentState = ArrowState.Moving;
        } else {
            Debug.LogError("Cannot init start cell for arrow!");
        }
    }

    private void NotifyCameraAboutResize(int sizeOfPath) {
        
        LinkedListNode<MazeCell> iterateCell = currentCell.Next;
        MazeCell photonPos = photonScript.CurrentMazeCell;
        float offset = mazeScript.LenghtOfCellSide / 2;
        Vector2 leftUpBound = new Vector2(iterateCell.Value.X - offset, iterateCell.Value.Y - offset);
        Vector2 rightDownBound = new Vector2(iterateCell.Value.X + offset, iterateCell.Value.Y + offset);
        Frame frame = new Frame(leftUpBound, rightDownBound);
        while(iterateCell != null && sizeOfPath > 0) {
            frame.TryResizeX(iterateCell.Value.X, offset);
            frame.TryResizeY(iterateCell.Value.Y, offset);
            sizeOfPath--;
            iterateCell = iterateCell.Next;
        }
        
        frame.TryResizeX(photonPos.X, offset);
        frame.TryResizeY(photonPos.Y, offset);

        ObjectsManager.Instance
            .GetCameraScript()
            .IfPresent(cameraScript => cameraScript.ResizeCameraTo(frame));

    }
    
    // Update is called once per frame
    void Update() {
        if(currentCell == null) {
            Destroy(gameObject, 1.5f);
        } else if(sizeOfPath == 0) {
            Destroy(gameObject);
        } else if(currentState == ArrowState.Checking) {

            nextMove = currentCell.Value.GetDirectionTo(currentCell.Next?.Value);
            SetStateAndNotifyObservers(lastMove != nextMove ? ArrowState.Rotating : ArrowState.Moving);
            currentCell = currentCell.Next;

        } else if(currentState == ArrowState.Moving) {

            Vector3 targetPosition = new Vector3(currentCell.Value.X, transform.position.y, currentCell.Value.Y);
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.45f);
            if(Vector3.Distance(transform.position, targetPosition) <= 0.1f) {
                transform.position = targetPosition;
                SetStateAndNotifyObservers(ArrowState.Checking);
                sizeOfPath--;
            }

        } else if(currentState == ArrowState.Rotating) {
            Quaternion toRotation = GetRotationByMove(nextMove);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 0.35f);
            bool rotationDone = Math.Abs(Math.Abs(transform.rotation.y) - Math.Abs(toRotation.y)) <= 0.1f;
            if(rotationDone) {
                transform.rotation = toRotation;
                lastMove = nextMove;
                SetStateAndNotifyObservers(ArrowState.Moving);
            }
        }
    }

    private void OnDestroy() {
        SetStateAndNotifyObservers(ArrowState.Ending);
    }

    private void SetStateAndNotifyObservers(ArrowState newState) {
        currentState = newState;
        NotifyObservers();
    }

    private void InitArrowPosition() {
        currentCell = currentCell.Next;
        transform.position = new Vector3(currentCell.Value.X, 1f, currentCell.Value.Y);
        MazeCell next = currentCell.Next?.Value;
        if(next == null) {
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
        nextMove = currentCell.Value.GetDirectionTo(next);
        lastMove = nextMove;
        transform.rotation = GetRotationByMove(lastMove);
    }

    private Quaternion GetRotationByMove(Direction move) {
        switch(move) {
            case Direction.Right:
                return Quaternion.Euler(0f, 270f, 0f);
            case Direction.Left:
                return Quaternion.Euler(0f, 90f, 0f);
            case Direction.Back:
                return Quaternion.Euler(0f, 0f, 0f);
            case Direction.Front:
                return Quaternion.Euler(0, 180f, 0f);
        }
        return Quaternion.Euler(0f, -90f, 0f);
    }

    protected override ArrowState GetData() {
        return currentState;
    }
}

