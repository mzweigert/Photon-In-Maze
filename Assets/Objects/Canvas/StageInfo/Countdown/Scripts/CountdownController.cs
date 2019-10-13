using PhotonInMaze.Common.Flow;
using PhotonInMaze.Provider;
using UnityEngine;
using UnityEngine.UI;


namespace PhotonInMaze.CanvasGame.StageInfo {

    internal class CountdownController : FlowFixedUpdateBehaviour {

        private float elapsed;
        private int toStart;
        private bool audioPlayed, sizeInfoShown;
        private AudioSource audioSource;
        private Text text;
        private Animator animator;

        [SerializeField]
        private SizeInfoController sizeInfoController;

        public override void OnInit() {
            toStart = MazeObjectsProvider.Instance.GetMazeConfiguration().SecondsToStart;
            elapsed = 0.5f;
            audioPlayed = sizeInfoShown = false;
            text = gameObject.GetComponent<Text>();
            text.text = string.Empty;
            gameObject.SetActive(true);
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
               .When(State.CountingDown)
               .Then(CountDownTimeToStart)
               .Build();
        }

        private void CountDownTimeToStart() {
            elapsed += Time.fixedDeltaTime;

            if(elapsed < 1f) {
                return;
            }
            SetSizeInfo();
            PlayAudio();

            elapsed = 0f;
            if(toStart > 0) {
                text.text = toStart.ToString();
            } else if(toStart < 0) {
                GameFlowManager.Instance.Flow.NextState();
                gameObject.SetActive(false);
                sizeInfoController.gameObject.SetActive(false);
            } else {
                sizeInfoController.Hide();
                text.text = "Go";
            }
            animator.SetTrigger("Fade");

            --toStart;

        }

        private void PlayAudio() {
            if(!audioPlayed) {
                audioPlayed = true;
                audioSource.Play();
            }
        }

        private void SetSizeInfo() {
            if(!sizeInfoShown) {
                sizeInfoShown = true;
                sizeInfoController.Show();
            }
        }

        public override int GetInitOrder() {
            return (int)InitOrder.Default;
        }
    }
}