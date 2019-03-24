using System.Linq;
using UnityEngine;

public class BlackHoleController : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if(other.name.Equals("Photon")) {
            Animator animator = GetComponentInParent<Animator>();
            animator.SetTrigger("Destroy");
            float animationLength = animator.runtimeAnimatorController
                .animationClips
                .First(clip => clip.name.Equals("OnDestroy"))
                .length;

            Destroy(transform.parent.gameObject, animationLength);
        }
    }

}
