using UnityEngine;

public class Interactable : MonoBehaviour {

    public virtual void Interact() {
        print("interacting with " + this.gameObject.name);
    }
    public virtual void DisplayInfo() {
        
    }
}
