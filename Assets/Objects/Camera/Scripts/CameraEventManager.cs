using PhotonInMaze.Common;
using System.Collections.Generic;


namespace PhotonInMaze.GameCamera {
    public class CameraEventManager {

        private Queue<ICameraEvent> events = new Queue<ICameraEvent>();
        private Optional<ICameraEvent> current = Optional<ICameraEvent>.Empty();

        public void Add(ICameraEvent camEvent) => events.Enqueue(camEvent);

        public bool CanLoadNextEvent() {
            return events.Count > 0 && current.HasNotValue;
        }

        public void TryLoadNext() {
            current = Optional<ICameraEvent>.OfNullable(events.Dequeue());
        }

        public bool CanRunCurrent() {
            return current.HasValue && !current.Get().IsDone();
        }

        public void TryRunCurrent() {
            if(current.HasNotValue) {
                throw new CurrentEventNotPresentException();
            } else {
                ICameraEvent camEvent = current.Get();
                camEvent.Run();
                if(camEvent.IsDone()) {
                    current = Optional<ICameraEvent>.Empty();
                }

            }
        }
    }
}