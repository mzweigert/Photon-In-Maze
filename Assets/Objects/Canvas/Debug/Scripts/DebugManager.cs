using PhotonInMaze.Common;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Provider;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonInMaze.CanvasGame.DebugGame {
    public class DebugManager : MonoBehaviour {

        private float deltaTime = 0.0f;

        [SerializeField]
        private Button generateBtn;

        [SerializeField]
        private InputField length;

        private MazeGenerationAlgorithm algorithm = MazeGenerationAlgorithm.PureRecursive;

        void Start() {
            generateBtn.interactable = false;
            if(!Debug.isDebugBuild) {
                gameObject.SetActive(false);
            }
        }

        void Update() {
            if(!Debug.isDebugBuild) {
                return;
            } else if(!string.IsNullOrEmpty(length.text) && 
                      !generateBtn.IsInteractable() && 
                      GameFlowManager.Instance.Flow.Is(State.TurnOnLight)) {

                generateBtn.interactable = true;
            }
            
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI() {
            if(!Debug.isDebugBuild) {
                return;
            }

            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 50);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 50;
            style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }

        public void OnAlgorithmChange(int value) {
            switch(value) {
                case 0:
                    algorithm = MazeGenerationAlgorithm.PureRecursive;
                    break;
                case 1:
                    algorithm = MazeGenerationAlgorithm.RandomTree;
                    break;
                case 2:
                    algorithm = MazeGenerationAlgorithm.Division;
                    break;
            }
        }

        public void OnLengthEditChange(string value) {
            if(value.Equals(string.Empty)) {
                generateBtn.interactable = false;
                return;
            }

            int length = int.Parse(value);
            if(length > 50) {
                this.length.text = 50.ToString();
            }
            if(!string.IsNullOrEmpty(this.length.text)) {
                generateBtn.interactable = true;
            }
        }

        public void Generate() {
            generateBtn.interactable = false;
            int length = int.Parse(this.length.text);
            length = length < 5 ? 5 : length;
            this.length.text = length.ToString();
            MazeObjectsProvider.Instance.GetMazeController().Recreate(length, length, algorithm);
        }
    }
}