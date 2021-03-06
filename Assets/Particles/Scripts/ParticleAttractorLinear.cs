using UnityEngine;

namespace PhotonInMaze.Particles {
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleAttractorLinear : Target {
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
            if(TargetVal == null) {
                Destroy(gameObject);
                return;
            }
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
            numParticlesAlive = ps.GetParticles(particles);
            float step = speed * Time.deltaTime * 0.5f;
            for(int i = 0; i < numParticlesAlive; i++) {
                particles[i].position = Vector3.Lerp(particles[i].position, TargetVal.position, step);
            }
            ps.SetParticles(particles, numParticlesAlive);
        }
    }
}
