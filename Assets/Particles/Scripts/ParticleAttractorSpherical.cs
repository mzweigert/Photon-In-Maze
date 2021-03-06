using System.Collections;
using UnityEngine;

namespace PhotonInMaze.Particles {
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleAttractorSpherical : Target {
        ParticleSystem ps;
        ParticleSystem.Particle[] particles;
        public float speed = 5f;
        int numParticlesAlive;
        void Start() {
            ps = GetComponent<ParticleSystem>();
            if(!GetComponent<Transform>()) {
                GetComponent<Transform>();
            }
        }
        void Update() {
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
            numParticlesAlive = ps.GetParticles(particles);
            float step = speed * Time.deltaTime;
            for(int i = 0; i < numParticlesAlive; i++) {
                particles[i].position = Vector3.SlerpUnclamped(particles[i].position, TargetVal.position, step);
            }
            ps.SetParticles(particles, numParticlesAlive);
        }
    }
}