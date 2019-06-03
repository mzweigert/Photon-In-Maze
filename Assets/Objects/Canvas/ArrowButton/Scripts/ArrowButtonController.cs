using PhotonInMaze.Common.Flow;
using PhotonInMaze.Game.Manager;
using UnityEngine;
using UnityEngine.UI;


namespace PhotonInMaze.Game.CanvasGame.Arrow {
    public class ArrowButtonController : FlowUpdateBehaviour {

        private enum AnimationState {
            Idle,
            Show,
            WaitToBack,
            Hide,
        }

        private Button button;
        private Text text;
        private Vector2 showPosition, hidePosition;
        private AnimationState currentTextState = AnimationState.Show;
        private float timer = 0f;
        private Color textColor;

        public void SpawnArrow() {
            bool isNotGameRunningOrArrowAlreadyPresent = 
                !GameFlowManager.Instance.Flow.Is(State.GameRunning) ||
                ObjectsManager.Instance.IsArrowPresent();
            if(isNotGameRunningOrArrowAlreadyPresent) {
                return;
            }
            byte arrowsLeft = ObjectsManager.Instance.SpawnArrow();
            if(arrowsLeft == 0) {
                button.interactable = false;
            }
            text.text = arrowsLeft + "x";
            currentTextState = AnimationState.Show;
        }

        public override void OnStart() {
            text = GetComponentInChildren<Text>();
            button = GetComponentInChildren<Button>();
            text.rectTransform.SetAsFirstSibling();
            text.text = ObjectsManager.Instance.ArrowHintsCount + "x";
            hidePosition = text.rectTransform.anchoredPosition = new Vector3(-40, 0, 0);

            showPosition = new Vector3(-125f, text.rectTransform.anchoredPosition.y);
            textColor = text.color;
        }

        public override IInvoke OnLoop() {
            return GameFlowManager.Instance.Flow
                .WhenIsAny()
                .ThenDo(WaitForAnimating)
                .Build();
        }

        private void WaitForAnimating() {
            if(currentTextState == AnimationState.Show) {
                text.rectTransform.anchoredPosition = Vector3.Lerp(text.rectTransform.anchoredPosition, showPosition, 0.1f);
                if(Vector3.Distance(text.rectTransform.anchoredPosition, showPosition) <= 0.01f) {
                    text.rectTransform.anchoredPosition = showPosition;
                    currentTextState = AnimationState.WaitToBack;
                }
            } else if(currentTextState == AnimationState.WaitToBack) {
                timer += Time.deltaTime;
                if(timer > 1.5f) {
                    currentTextState = AnimationState.Hide;
                    timer = 0f;
                }
            } else if(currentTextState == AnimationState.Hide) {
                text.color = Color.Lerp(text.color, Color.clear, 0.1f);
                if(text.color.a - Color.clear.a <= 0.01f) {
                    text.rectTransform.anchoredPosition = hidePosition;
                    currentTextState = AnimationState.Idle;
                    if(ObjectsManager.Instance.ArrowHintsCount > 0) {
                        text.color = textColor;
                    }
                }
            }
        }

        public override int GetInitOrder() {
            return InitOrder.Default;
        }
    }
}