using PhotonInMaze.Common.Flow;
using UnityEngine;
using UnityEngine.UI;


namespace PhotonInMaze.Game.CanvasGame.CountDown {
    public class CountdownController : FlowBehaviour {

        private float elapsed = 0.5f;
        private int toStart = 3;
        private bool audioPlayed = false;
        private AudioSource audioSource;

        protected override IInvoke Init() {
            audioSource = GetComponent<AudioSource>();
            gameObject.GetComponent<Text>().fontSize = Mathf.CeilToInt(Screen.width * 0.4875f);
            GameFlowManager.Instance.Flow.NextState();
            return GameFlowManager.Instance.Flow
               .When(State.CountingDown)
               .Then(CountDownTimeToStart)
               .Build();

        }

        private void CountDownTimeToStart() {
            elapsed += Time.deltaTime;

            if(elapsed >= 1f) {
                if(!audioPlayed) {
                    audioPlayed = true;
                    audioSource.Play();
                } else if(toStart < 0) {
                    GameFlowManager.Instance.Flow.NextState();
                    Destroy(gameObject);
                }
                elapsed = 0f;
                gameObject.GetComponent<Text>().text = toStart > 0 ? (toStart).ToString() : "Go";
                --toStart;
            }
        }

    }
}