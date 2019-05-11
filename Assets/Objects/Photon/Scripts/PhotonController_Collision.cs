using System;
using System.Collections.Generic;
using UnityEngine;

public partial class PhotonController : MonoObserveable<PhotonState> {

    [SerializeField]
    private Transform leakedLightContainer;

    [SerializeField]
    private GameObject particleLightTemplate;

    private static readonly string LIGHT_ABSORB_AREA = "_LightAbsorbArea";

    private Dictionary<int, GameObject> blackHolesToLeakedLights = new Dictionary<int, GameObject>();

    void OnTriggerEnter(Collider other) {
        if(other.name.Equals(LIGHT_ABSORB_AREA)) {
            GameObject particleLightInstance = Instantiate(particleLightTemplate, leakedLightContainer);
            particleLightInstance.name = "ParticleLight";
            particleLightInstance.GetComponent<Target>().TargetVal = other.transform.parent.Find("_DarkOrb");
            blackHolesToLeakedLights.Add(GetKey(other), particleLightInstance);
        } else if(other.name.Equals("_DestroyArea")) {
            HandeOnTriggerExitBlackHole(GetKey(other));
            photonLight.intensity *= 0.75f;
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.name.StartsWith(LIGHT_ABSORB_AREA)) {
            HandeOnTriggerExitBlackHole(GetKey(other));
        }
    }

    private void HandeOnTriggerExitBlackHole(int key) {
        bool notExists = !blackHolesToLeakedLights.TryGetValue(key, out GameObject particleLight);
        if(notExists) {
            return;
        }
        blackHolesToLeakedLights.Remove(key);
        Destroy(particleLight);
    }

    private int GetKey(Collider collider) => collider.transform.parent.Find(LIGHT_ABSORB_AREA).GetInstanceID();

}