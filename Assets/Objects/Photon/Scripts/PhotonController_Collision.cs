using System.Collections.Generic;
using UnityEngine;

public partial class PhotonController : MonoObserveable<PhotonState> {

    [SerializeField]
    private Transform leakedLightContainer;

    [SerializeField]
    private GameObject particleLightTemplate;

    private Dictionary<int, ParticleLight> blackHolesToLeakedLights = new Dictionary<int, ParticleLight>();

    void OnTriggerEnter(Collider other) {
        if(other.name.StartsWith("_LightAbsorbArea")) {
            GameObject particleLightInstance = Instantiate(particleLightTemplate, leakedLightContainer);
            particleLightInstance.name = "ParticleLight";
            ParticleSystem particleSystem = particleLightInstance.GetComponent<ParticleSystem>();
            ParticleLight light = new ParticleLight(particleLightInstance, this, other.transform.parent.Find("_LightDestroyArea"));
            blackHolesToLeakedLights.Add(GetKey(other), light);
        } else if(other.name.Equals("_DestroyArea")) {
            HandeOnTriggerExitBlackHole(GetKey(other));
            photonLight.intensity *= 0.75f;
        }
    }

    void OnTriggerStay(Collider other) {
        if(other.name.StartsWith("_LightAbsorbArea")) {
            HandleOnTriggerStayWithBlackHole(other);
        }
    }

    private void HandleOnTriggerStayWithBlackHole(Collider other) {
        int key = GetKey(other);
        bool notExists = !blackHolesToLeakedLights.TryGetValue(key, out ParticleLight particleLight);
        if(notExists) {
            return;
        }

        if(particleLight.CanChange) {
            float x = other.transform.position.x - particleLight.Object.transform.position.x;
            float y = other.transform.position.z - particleLight.Object.transform.position.z;
            float newAngle = Mathf.Atan2(x, y) * Mathf.Rad2Deg;
            particleLight.SetRotation(newAngle);
            Transform lightDestroyAreaTransform = other.transform.parent.Find("_LightDestroyArea").transform;
            lightDestroyAreaTransform.localEulerAngles = new Vector3(0f, 0f, newAngle);
            blackHolesToLeakedLights[key] = particleLight;
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.name.StartsWith("_LightAbsorbArea")) {
            HandeOnTriggerExitBlackHole(GetKey(other));
        }
    }

    private void HandeOnTriggerExitBlackHole(int key) {
        bool notExists = !blackHolesToLeakedLights.TryGetValue(key, out ParticleLight particleLight);
        if(notExists) {
            return;
        }
        blackHolesToLeakedLights[key].Unsubscribe();
        blackHolesToLeakedLights.Remove(key);
        Destroy(particleLight.Object);
    }

    private int GetKey(Collider collider) => collider.transform.parent.Find("_LightAbsorbArea").GetInstanceID();
}