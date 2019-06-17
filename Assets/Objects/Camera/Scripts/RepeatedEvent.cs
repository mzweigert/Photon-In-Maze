using System;

namespace PhotonInMaze.GameCamera {
    internal class RepeatedEvent : ICameraEvent {

        private bool isDone = false;
        private readonly Func<bool> camEvent;

        private RepeatedEvent(Func<bool> camEvent) {
            this.camEvent = camEvent;
        }

        public static ICameraEvent Of(Func<bool> camEvent) {
            return new RepeatedEvent(camEvent);
        }

        public bool IsDone() {
            return isDone;
        }

        public void Run() {
            if(!isDone) {
                isDone = camEvent.Invoke();
            }
        }

    }
}