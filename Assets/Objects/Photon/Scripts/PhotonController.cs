using UnityEngine;

public partial class PhotonController : MonoObserveable<PhotonState> {

    private MazeController mazeController;

    private PhotonState photonState;

    private Light photonLight;
    private bool photonLightAlreadySet;

    [Range(0.1f, 2f)]
    public float PhotonSpeed;

    // Start is called before the first frame update
    void Start() {
        Optional<MazeController> optionalMazeController = ObjectsManager.Instance.GetMazeScript();
        if(optionalMazeController.HasNotValue) {
            Debug.LogError("MazeController not preset!");
            return;
        }
        mazeController = optionalMazeController.Get();
        LastNodeCellFromPathToGoal = mazeController.PathsToGoal.First;
        currentTargetMazeCell = new TargetMazeCell(LastNodeCellFromPathToGoal.Value, MovementEvent.Idle);
        lastSaved = currentTargetMazeCell.value;
        photonState = new PhotonState(transform.position);

        photonLight = GetComponentInChildren<Light>();
        photonLight.intensity = 0f;
    }

    // Update is called once per frame
    void Update() {
        if(mazeController == null || ObjectsManager.Instance.IsArrowPresent()) {
            return;
        }
      
        if(GameFlow.Instance.Is(GameFlow.State.LightTurnedOff) && photonLight && !photonLightAlreadySet) {
            photonLight.intensity = 7.5f;
            photonLightAlreadySet = true;
            GameFlow.Instance.StartGame();
        }

        GameFlow.Instance.CallUpdateWhenGameIsRunning(() => {

            TryMakeMove();

#if UNITY_EDITOR 
            CheckButtonPress();
#endif
            CheckTouch();


        });
    }

    protected override PhotonState GetData() {
        return photonState;
    }
}