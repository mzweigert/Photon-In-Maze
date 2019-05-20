using UnityEngine;

internal class ArrowObserver : System.IObserver<ArrowState> {

    public bool ArrowIsPresent { get; private set; } = false;

    internal ArrowObserver() { }

    public void Subscribe(GameObject newArrow) {
        if(newArrow == null) {
            Debug.LogError("Given newArrow is null!");
            return;
        }
        ArrowController controller = newArrow.GetComponent<ArrowController>();
        if(controller == null) {
            Debug.LogError("Given newArrow doesn't has ArrowController!");
            return;
        }
        controller.Subscribe(this);
        ArrowIsPresent = true;
    }

    public void OnNext(ArrowState state) {
        if(ArrowState.Ending == state) {
            ArrowIsPresent = false;
        }
    }

    public void OnCompleted() {
        throw new System.NotImplementedException();
    }

    public void OnError(System.Exception error) {
        Debug.LogError(error.Message);
    }
}