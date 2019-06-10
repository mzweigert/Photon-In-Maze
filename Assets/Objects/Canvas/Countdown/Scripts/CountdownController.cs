using PhotonInMaze.Common.Flow;
using UnityEngine;
using UnityEngine.UI;


namespace PhotonInMaze.CanvasGame.CountDown {
    public class CountdownController : FlowFixedUpdateBehaviour {

        private float elapsed;
        private int toStart;
        private bool audioPlayed;
        private AudioSource audioSource;
        private Text text;

        public override void OnInit() {
            toStart = 3;
            elapsed = 0.5f;
            audioPlayed = false;
            text = gameObject.GetComponent<Text>();
            text.fontSize = Mathf.CeilToInt(Screen.width * 0.4875f);
            text.text = string.Empty;
            gameObject.SetActive(true);
            audioSource = GetComponent<AudioSource>();
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
               .When(State.CountingDown)
               .Then(CountDownTimeToStart)
               .Build();
        }

        private void CountDownTimeToStart() {
            elapsed += Time.fixedDeltaTime;

            if(elapsed >= 1f) {
                if(!audioPlayed) {
                    audioPlayed = true;
                    audioSource.Play();
                } else if(toStart < 0) {
                    GameFlowManager.Instance.Flow.NextState();
                    gameObject.SetActive(false);
                }
                elapsed = 0f;
                text.text = toStart > 0 ? (toStart).ToString() : "Go";
                --toStart;
            }
        }

        public override int GetInitOrder() {
            return InitOrder.Default;
        }
    }
}