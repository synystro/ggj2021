using UnityEngine;

public class MouseLook : MonoBehaviour {

    [SerializeField] Transform playerBody;
    [SerializeField] LayerMask interactableMask;
    float mouseSensitivity = 300f;
    float xRotation = 0f;

    Interactable interactableObj;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update() {
        HandleLook();
        Aim();
    }
    void HandleLook() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
    void Aim() {
        float rayDistance = 1.5f;
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position, this.gameObject.transform.forward, out hit, rayDistance, interactableMask)) {
            interactableObj = hit.collider.GetComponent<Interactable>();
            interactableObj.DisplayInfo();
            interactableObj.TextLookAtPlayer(this.transform.position);
            if(Input.GetButtonDown("Interact"))
                interactableObj.Interact();
        } else if(interactableObj != null) {
            interactableObj.HideInfo();
            interactableObj = null;
        }

        Debug.DrawRay(this.transform.position, this.gameObject.transform.forward * rayDistance, Color.green);
    }
}
