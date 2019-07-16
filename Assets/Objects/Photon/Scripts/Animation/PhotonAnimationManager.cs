
using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Model;
using PhotonInMaze.Provider;
using System;
using UnityEngine;

namespace PhotonInMaze.Photon {
    public class PhotonAnimationManager {

        private IMazeController mazeController;
        private PhotonConfiguration configuration;
        private Animator animator;


        internal PhotonAnimationManager() {
            this.mazeController = MazeObjectsProvider.Instance.GetMazeController();
            GameObject photon = ObjectsProvider.Instance.GetPhoton();
            this.configuration = photon.GetComponent<PhotonConfiguration>();
            this.animator = photon.GetComponent<Animator>();
        }

        internal Action MakeActionOnNewMove(IMazeCell newCell, MovementEvent movementEvent) {
            bool isBlackHole = mazeController.IsBlackHolePosition(newCell) && movementEvent == MovementEvent.Move;
            if(isBlackHole) {
                return () => {
                    animator.SetTrigger("TurnOffLight");
                    animator.SetTrigger("Hide");
                    configuration.DecraseSpeed();
                };
            };
            bool isOutsideWhiteHole = mazeController.IsWhiteHolePosition(newCell) &&
                 movementEvent == MovementEvent.Teleport;
            if(isOutsideWhiteHole) {
                return () => {
                    animator.SetTrigger("TurnOnLight");
                    animator.SetTrigger("Show");
                };
            }

            return null;
        }

        internal void TurnOnLight() {
            animator.SetTrigger("TurnOnLight");
        }

        internal void ShowPhoton() {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(1);
            if(info.IsName("Hide")) {
                animator.SetTrigger("Show");
            }
        }

        internal bool IsNotHidingAndTurningLightOff() {
            bool isHiding = IsAnmationPlaying(1, "Hide");
            bool isTurningLightOff = IsAnmationPlaying(2, "TurnOffLight");
            return !isHiding && !isTurningLightOff;
        }

        internal bool IsAnmationPlaying(int layer, string transitionName) {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(layer);
            if(!animator.IsInTransition(layer) && !info.IsName(transitionName)) {
                animator.SetTrigger(transitionName);
                return true;
            } else if(animator.IsInTransition(layer)) {
                return true;
            }
            return info.length > info.normalizedTime && info.IsName(transitionName);
        }

    }
}