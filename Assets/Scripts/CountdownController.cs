using UnityEngine;
using UnityEngine.UI;

public class CountdownController : MonoBehaviour {

    private float elapsed = 0.5f;
    private int toStart = 3;
    private bool audioPlayed = false;
    private AudioSource audioSource;
    // Use this for initialization
    void Start() {
        audioSource = GetComponent<AudioSource>();
        gameObject.GetComponent<Text>().fontSize = Mathf.CeilToInt(Screen.width * 0.4875f);
    }

    private void Update() {
        elapsed += Time.deltaTime;

        if(elapsed >= 1f) {
            if(!audioPlayed) {
                audioPlayed = true;
                audioSource.Play();
            } else if(toStart < 0) {
                GameEvent.Instance.TryTurnOffLight();
                Destroy(gameObject);
            }
            elapsed = 0f;
            gameObject.GetComponent<Text>().text = toStart > 0 ? (toStart).ToString() : "Go";
            --toStart;
        }
    }

}
