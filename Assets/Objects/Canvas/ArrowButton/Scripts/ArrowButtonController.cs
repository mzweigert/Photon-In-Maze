using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Provider;
using UnityEngine;
using UnityEngine.UI;


namespace PhotonInMaze.CanvasGame.Arrow {
    internal class ArrowButtonController : FlowUpdateBehaviour, IArrowButtonController {

        [Range(1, 255)]
        [SerializeField]
        private byte _initialArrowHintsCount;

        public byte ArrowHintsCount { get; private set; }
        private const byte debugArrowHintsCount = 99;

        private enum AnimationState {
            Idle,
            Show,
            WaitToHide,
            Hide,
        }

        private ArrowObserver arrowObserver;

        private Button button;
        private Text text;
        private Vector2 showPosition, hidePosition;
        private AnimationState currentTextState;
        private float timer = 0f;
        private Color textColor = new Color(0.025f, 1f, 0.025f);

        public void ReinitializeArrowHintsCount() {
            if(GameFlowManager.Instance.Flow.Is(State.EndGame)) {
                ArrowHintsCount = Debug.isDebugBuild ? debugArrowHintsCount : _initialArrowHintsCount;
                arrowObserver.RemoveArrow();
            }
        }

        public void SpawnArrow() {

            bool isNotGameRunningOrArrowAlreadyPresent = 
                !GameFlowManager.Instance.Flow.Is(State.GameRunning) ||
                arrowObserver.ArrowIsPresent;
            if(isNotGameRunningOrArrowAlreadyPresent) {
                return;
            }

            if(ArrowHintsCount > 0) {
                GameObject arrowPrefab = ObjectsProvider.Instance.GetArrow();
                var newArrow = Instantiate(arrowPrefab, MazeObjectsProvider.Instance.GetMaze().transform);
                newArrow.name = "Arrow";
                arrowObserver.Subscribe(newArrow);
                ArrowHintsCount--;
               
            }

            if(ArrowHintsCount == 0) {
                button.interactable = false;
            }
            text.text = ArrowHintsCount + "x";
            currentTextState = AnimationState.Show;
        }

        public override void OnInit() {
            arrowObserver = new ArrowObserver();
            ArrowHintsCount = Debug.isDebugBuild ? debugArrowHintsCount : _initialArrowHintsCount;
            text = GetComponentInChildren<Text>();
            button = GetComponentInChildren<Button>();
            text.rectTransform.SetAsFirstSibling();
            text.text = ArrowHintsCount + "x";
            hidePosition = text.rectTransform.anchoredPosition = new Vector3(-40, 0, 0);

            showPosition = new Vector3(-150f, text.rectTransform.anchoredPosition.y);
            currentTextState = AnimationState.Show;
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
                text.color = Color.Lerp(text.color, textColor, 0.25f);
                if(Vector3.Distance(text.rectTransform.anchoredPosition, showPosition) <= 0.01f && text.color.a >= 0.9f) {
                    text.rectTransform.anchoredPosition = showPosition;
                    currentTextState = AnimationState.WaitToHide;
                    text.color = textColor;
                }
            } else if(currentTextState == AnimationState.WaitToHide) {
                timer += Time.deltaTime;
                if(timer > 1.5f) {
                    currentTextState = AnimationState.Hide;
                    timer = 0f;
                }
            } else if(currentTextState == AnimationState.Hide) {
                text.color = Color.Lerp(text.color, Color.clear, 0.25f);
                if(text.color.a - Color.clear.a <= 0.01f) {
                    text.rectTransform.anchoredPosition = hidePosition;
                    currentTextState = AnimationState.Idle;
                    text.color = Color.clear;
                }
            }
        }

        public override int GetInitOrder() {
            return (int) InitOrder.Default;
        }

        public bool IsArrowPresent() {
            return arrowObserver.ArrowIsPresent;
        }
    }
}