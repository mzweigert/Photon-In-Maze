using UnityEngine;

public class GameSingleton : MonoBehaviour {

    private static bool created = false;
    void Awake() {
        if(!created) {
            // this is the first instance - make it persist
            DontDestroyOnLoad(gameObject);
            created = true;
        } else {
            Destroy(gameObject);
        }
    }
}
