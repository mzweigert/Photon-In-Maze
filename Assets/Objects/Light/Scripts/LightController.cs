using UnityEngine;

public class LightController : MonoObserver<PhotonController, PhotonState> {

    private MazeController mazeController;
    private new Light light;
    private bool lightConfigured = false;
    private bool audioPlayed = false;
    private AudioSource audioSource;

    private float maxLightIntensity = 1.75f;
    private float minLightIntensity = 0.075f;
    private float onePercentLightInensity;

    private int pathToGoalCount;
    private int lastPathToGoalIndex;

    // Start is called before the first frame update
    void Start() {
        Optional<MazeController> optionalMazeController = ObjectsManager.Instance.GetMazeScript();
        if(optionalMazeController.HasNotValue) {
            Debug.LogError("MazeController not preset!");
            return;
        }
        mazeController = optionalMazeController.Get();
        audioSource = GetComponent<AudioSource>();
        light = GetComponent<Light>();
        onePercentLightInensity = (maxLightIntensity - minLightIntensity) / 100;
        light.intensity = maxLightIntensity;
        pathToGoalCount = mazeController.PathsToGoal.Count;
    }

    // Update is called once per frame
    void Update() {

        if(!GameFlow.Instance.Is(GameFlow.State.CanTurnOffLight)) {
            return;
        }

        if(!lightConfigured) {
            InitLight();
        }
    }

    private void InitLight() {
        if(light.intensity >= minLightIntensity) {
            light.intensity -= Time.deltaTime * 7.5f;
            if(!audioPlayed) {
                audioPlayed = true;
                audioSource.Play();
            }
        } else {
            light.intensity = minLightIntensity;
            lightConfigured = true;
            GameFlow.Instance.TurnOffLight();
        }
    }

    public override void OnNext(PhotonState state) {
        if(state.IndexOfLastCellInPathToGoal != lastPathToGoalIndex) {
            lastPathToGoalIndex = state.IndexOfLastCellInPathToGoal;
            float delta = onePercentLightInensity * (((float)lastPathToGoalIndex / pathToGoalCount) * 100f);
            light.intensity = minLightIntensity + (delta * 0.1f);
        }
    }
}
