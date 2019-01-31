using UnityEngine;

public class LightController : MonoBehaviour {

    private new Light light;
    private bool lightConfigured = false;
    private bool audioPlayed = false;
    private AudioSource audioSource;
    private float targetLightIntensity = 0.05f;
    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        light = GetComponent<Light>();
        light.intensity = 1.75f;
    }

    // Update is called once per frame
    void Update() {
        Utils.CheckIfGameRunningAndCallUpdate(() => {
            if(lightConfigured) {
                return;
            }

            if(light.intensity >= targetLightIntensity) {
                light.intensity -= Time.deltaTime * 7.5f;
                if(!audioPlayed) {
                    audioPlayed = true;
                    audioSource.Play();
                }
            } else {
                light.intensity = targetLightIntensity;
                lightConfigured = true;
               
            }
        });
    }
}
