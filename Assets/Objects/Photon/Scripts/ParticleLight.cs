using System;
using UnityEngine;

internal class ParticleLight : IObserver<PhotonState> {

    public GameObject Object { private set; get; }
    public bool CanChange { private set; get; }
    private IDisposable unsubscriber;

    public ParticleLight(GameObject particleLightObject, MonoObserveable<PhotonState> observeable, Transform destroyArea) {
        this.Object = particleLightObject;
        this.CanChange = true;
        this.unsubscriber = observeable?.Subscribe(this);
        Collider collider = destroyArea.GetComponent<Collider>();
        if(collider) {
            this.SetDestroyTrigger(collider);
        }
    }

    private void SetDestroyTrigger(Collider destroyCollider) {
        ParticleSystem[] chunks = Object.GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem chunk in chunks) {
            chunk.trigger.SetCollider(0, destroyCollider);
        }
    }

    internal void SetRotation(float yRotation) {
        float previousYRotation = (float)Math.Round(Object.transform.eulerAngles.y, 2);
        Object.transform.eulerAngles = new Vector3(45, (float)Math.Round(yRotation, 2), 0);
        SetParticleSystemEjectionForce(yRotation);
        if(Mathf.Approximately(previousYRotation, Object.transform.eulerAngles.y)) { CanChange = false; }
    }

    private void SetParticleSystemEjectionForce(float yRotation) {
        float gravity, lifetime, orbitalZ;
        int mod90 = Math.Abs((int)yRotation % 90);
        bool isNear90 = (mod90 <= 95 && mod90 >= 85) || (mod90 >= 0 && mod90 <= 5);
        if(isNear90) {
            gravity = 0.3f; lifetime = 1f; orbitalZ = 1.65f;
        } else {
            gravity = 0.25f; lifetime = 1.75f; orbitalZ = 1.25f;
        }

        ParticleSystem[] chunks = Object.GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem chunk in chunks) {
            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = chunk.velocityOverLifetime;
            ParticleSystem.MainModule mainModule = chunk.main;
            mainModule.gravityModifier = gravity;
            mainModule.startLifetime = lifetime;
            if(velocityOverLifetime.orbitalZMultiplier != 0) {
                velocityOverLifetime.orbitalZMultiplier = Mathf.Sign(velocityOverLifetime.orbitalZMultiplier) * orbitalZ;
            }
            Debug.Log(chunk.velocityOverLifetime.orbitalZ.constant);
            Debug.Log(chunk.velocityOverLifetime.orbitalZMultiplier);
        }
        
    }

    public void OnNext(PhotonState state) {
        if(state.IsAcutallyMoving && !CanChange) { CanChange = true; }
    }

    public void OnCompleted() {
        throw new NotImplementedException();
    }

    public void OnError(Exception error) {
        Debug.LogError(error);
    }

    public void Unsubscribe() {
        if(unsubscriber != null) {
            unsubscriber.Dispose();
        } else {
            Debug.LogError("Cannot unsubscribe, object is null!");
        }
    }
}