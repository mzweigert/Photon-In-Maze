using UnityEngine;
using UnityEngine.UI;

public class TimeToEndController : MonoSingleton<TimeToEndController> {

    public float TimeToEnd { get; private set; } = 120f;
    string minutes, seconds;

    // Update is called once per frame
    void Update() {
        GameFlow.Instance.CallUpdateWhenGameIsRunning(() => {
            TimeToEnd -= Time.deltaTime;

            if(TimeToEnd <= 0f) {
                //GameFlow.Instance.EndGame();

            } else {

                seconds = (TimeToEnd % 60).ToString("00");
                minutes = Mathf.Floor(TimeToEnd / 60).ToString("00");
                if(seconds.Equals("60")) {
                    seconds = "00"; minutes = (int.Parse(minutes) + 1).ToString("00");
                }
                gameObject.GetComponent<Text>().text = string.Format("Time to end: {0} : {1}", minutes, seconds);
            }

        });
        
    }
}
