using PhotonInMaze.Common.Flow;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonInMaze.Game.CanvasGame.TimeToEnd {
    public class TimeToEndController : FlowBehaviourSingleton<TimeToEndController> {

        public float TimeToEnd { get; private set; } = 120f;
        string minutes, seconds;

        protected override IInvoke Init() {
            return GameFlowManager.Instance.Flow
                .When(State.GameRunning)
                .Then(CountDownTimeToEnd)
                .Build();
        }

        private void CountDownTimeToEnd() {
            TimeToEnd -= Time.deltaTime;

            if(TimeToEnd <= 0f) {
                GameFlowManager.Instance.Flow.NextState();
            } else {

                seconds = (TimeToEnd % 60).ToString("00");
                minutes = Mathf.Floor(TimeToEnd / 60).ToString("00");
                if(seconds.Equals("60")) {
                    seconds = "00"; minutes = (int.Parse(minutes) + 1).ToString("00");
                }
                gameObject.GetComponent<Text>().text = string.Format("Time to end: {0} : {1}", minutes, seconds);
            }
        }
    }
}