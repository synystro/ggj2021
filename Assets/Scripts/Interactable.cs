using UnityEngine;

public class Interactable : MonoBehaviour {

    [SerializeField] Transform interactionInfo;

    public virtual void Interact() {
        print("interacting with " + this.gameObject.name);
    }
    public virtual void DisplayInfo() {
        interactionInfo.gameObject.SetActive(true);        
    }
    public virtual void HideInfo() {
        interactionInfo.gameObject.SetActive(false);        
    }
    public virtual void TextLookAtPlayer(Vector3 position) {
        Vector3 lookPos = new Vector3(position.x, this.transform.position.y, position.z);
        interactionInfo.LookAt(lookPos);
    }
}
