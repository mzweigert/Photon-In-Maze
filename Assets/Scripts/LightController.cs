﻿using System;
using UnityEngine;

public class LightController : MonoBehaviour, IObserver<PhotonInPathToGoalInfo> {

    private Optional<MazeController> mazeController;
    private Optional<PhotonConroller> photonController;
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
        mazeController = ObjectsManager.Instance.GetMazeScript();
        photonController = ObjectsManager.Instance.GetPhotonScript();
        if(!mazeController.HasValue || !photonController.HasValue) {
            Debug.LogError("MazeController or PhontonController not preset!");
            return;
        }
        audioSource = GetComponent<AudioSource>();
        light = GetComponent<Light>();
        onePercentLightInensity = (maxLightIntensity - minLightIntensity) / 100;
        light.intensity = maxLightIntensity;
        pathToGoalCount = mazeController.Get().PathsToGoal.Count;
        photonController.Get().Subscribe(this);
    }

    // Update is called once per frame
    void Update() {
        if(!mazeController.HasValue || !photonController.HasValue) {
            Debug.LogError("MazeController or PhontonController not preset!");
            return;
        }

        if(!GameEvent.Instance.CanTurnOffLight) {
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
            GameEvent.Instance.LightTurnedOff();
        }
    }

    public void OnCompleted() {
        throw new NotImplementedException();
    }

    public void OnError(Exception error) {
        Debug.LogError(error.Message);
    }

    public void OnNext(PhotonInPathToGoalInfo value) {
        if(value.PositionInPathToGoal != lastPathToGoalIndex) {
            lastPathToGoalIndex = value.PositionInPathToGoal;
            float delta = onePercentLightInensity * (((float)lastPathToGoalIndex / pathToGoalCount) * 100f);
            light.intensity = minLightIntensity + (delta * 0.05f);
        }
    }
}