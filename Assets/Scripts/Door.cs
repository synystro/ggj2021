using UnityEngine;

public class Door : Interactable
{
    float openSpeed = 2f; // velocidade pra abrir
    float targetAngle; // angulo (de fechado ou aberto)
    float initialAngle; // angulo inicial da porta

    [SerializeField] Item key;
    [SerializeField] bool opensIn = true; // se abre pra dentro    
    bool isOpen; // se esta aberta
    [SerializeField] bool isLocked; // se esta fechada
    bool isMoving; // se esta movendo (pra fechar ou abrir)

    public bool IsClosed { get { return !isOpen; } }
    public bool IsLocked { get { return isLocked; } }

    void Start() {
        initialAngle = transform.localEulerAngles.y;
    }

    public override void Interact() {
        base.Interact();
        if(!isLocked)
            OpenClose();
        else if(Inventory.items.Contains(key)) {
            isLocked = false;
            OpenClose();
            print("door unlocked with " + key.name);
        }
        else
            print("this door is locked");
    }
    void OpenClose() {
        if (!isOpen) {
            if (opensIn)
                targetAngle = initialAngle -90f;
            else
                targetAngle = initialAngle +90f;
            isOpen = true;
        }
        else {
            targetAngle = initialAngle;
            isOpen = false;
        }
        isMoving = true;
    }
    void Update()
    {
        if(isMoving) {
            Quaternion smoothRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, targetAngle, 0), openSpeed * Time.deltaTime);
            transform.localRotation = smoothRotation;
            if(transform.localEulerAngles.y == targetAngle) {
                isMoving = false;
            }
        }
    }
}
