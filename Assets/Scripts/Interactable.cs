using UnityEngine;

public class Interactable : MonoBehaviour {

    [SerializeField] Transform interactionInfo;

    public virtual void Interact() {
        print("interacting with " + this.gameObject.name);
    }
    public virtual void DisplayInfo() {
        if(interactionInfo == null)
            return;
        interactionInfo.gameObject.SetActive(true);        
    }
    public virtual void HideInfo() {
        if(interactionInfo == null)
            return;
        interactionInfo.gameObject.SetActive(false);        
    }
    public virtual void TextLookAtPlayer(Vector3 position) {
        if(interactionInfo == null)
            return;
        Vector3 lookPos = new Vector3(position.x, this.transform.position.y, position.z);
        interactionInfo.LookAt(lookPos);
    }
}
