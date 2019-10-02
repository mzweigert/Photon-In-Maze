using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using UnityEngine;

namespace PhotonInMaze.Photon {

    public class PhotonConfiguration : FlowBehaviour, IPhotonConfiguration {

        public Vector3 InitialPosition { get; } = new Vector3(0, 2, 0);

        private readonly float teleportingSpeed = 0.5f;

        [Range(0.1f, 2f)]
        [SerializeField]
        private float speed;

        public float Speed { get { return speed; } }

        internal void DecraseSpeed() {
            speed *= teleportingSpeed;
        }

        internal void IncraseSpeed() {
            speed *= (1 / teleportingSpeed);
        }

        public override int GetInitOrder() {
            return (int)InitOrder.PhotonConfiguration;
        }

        public override void OnInit() {
            transform.position = InitialPosition;
        }
    }
}