using PhotonInMaze.Common.Controller;
using PhotonInMaze.Common.Flow;
using PhotonInMaze.Provider;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonInMaze.CanvasGame.StageInfo {

    public class SizeInfoController : FlowBehaviour {

        private Text text;
        private Color color;
        private Animator animator;
        private string sizeInfoText;

        public override int GetInitOrder() {
            return (int)InitOrder.Default;
        }

        public override void OnInit() {
            IMazeConfiguration mazeConfiguration = MazeObjectsProvider.Instance.GetMazeConfiguration();
            text = gameObject.GetComponent<Text>();
            text.text = string.Empty;
            sizeInfoText = mazeConfiguration.Rows + "x" + mazeConfiguration.Columns;
            gameObject.SetActive(true);
            animator = GetComponent<Animator>();
        }

        public void Hide() {
            animator.enabled = true;
            animator.SetTrigger("Fade");
        }

        public void Show() {
            animator.enabled = false;
            text.color = new Color(0f, 0f, 0f, 0.5f);
            text.text = sizeInfoText;
        }
    }
}