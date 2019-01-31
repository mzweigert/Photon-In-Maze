using UnityEngine;

public class CameraController : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {
        ObjectsManager.Instance.GetMazeScript().ForValuePresented(mazeScript => {
            float x = 0f, y = 0f, z = 0f;
            x = (mazeScript.Columns * 2f) - (mazeScript.LenghtSide / 2);
            z = (mazeScript.Rows * 2f) - (mazeScript.LenghtSide / 2);
            transform.position = new Vector3(x, transform.position.y, z);

            GameObject maze = ObjectsManager.Instance.GetMaze();

            Camera camera = ObjectsManager.Instance.GetCamera();
            if(camera.orthographic) {
                float sizeForLongerColumnsLength = mazeScript.Columns * 2.05f;
                float sizeForLongerRowsLength = mazeScript.Rows * 1.25f;
                float size = (sizeForLongerColumnsLength > sizeForLongerRowsLength ? sizeForLongerColumnsLength : sizeForLongerRowsLength);
                camera.orthographicSize = size;
            } else {
                x = transform.position.y;
                Vector2 source = new Vector2(x, 0);
                float yForLongerColumnsLength = mazeScript.Columns * 2.25f;
                float yForLongerRowsLength = mazeScript.Rows * 1.315f;
                y = (yForLongerColumnsLength > yForLongerRowsLength ? yForLongerColumnsLength : yForLongerRowsLength);
                Vector2 target = new Vector2(x, y);
                camera.fieldOfView = Vector2.Angle(source, target) * 2;
            }
        });
    }
}
