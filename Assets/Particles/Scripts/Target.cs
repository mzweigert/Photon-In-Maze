using UnityEngine;

public class Target : MonoBehaviour {

    private Transform _target;
    public Transform TargetVal {
        get { return _target; }
        set {
            if(value != null) {
                this._target = value;
            } else {
                throw new UnassignedReferenceException();
            }
        }
    }

}
