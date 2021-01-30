using UnityEngine;

public class Door : Interactable
{
    float openSpeed = 2f; // velocidade pra abrir
    float targetAngle; // angulo (de fechado ou aberto)

    [SerializeField] bool opensIn = true; // se abre pra dentro    
    bool isOpen; // se esta aberta
    bool isLocked; // se esta fechada
    bool isMoving; // se esta movendo (pra fechar ou abrir)

    public bool IsLocked { get { return isLocked; } }

    public override void Interact() {
        base.Interact();
        if(!isLocked)
            OpenClose();
        else
            print("this door is locked");
    }
    void OpenClose() {
        if (!isOpen) {
            if (opensIn)
                targetAngle = -90f;
            else
                targetAngle = 90f;
            isOpen = true;
        }
        else {
            targetAngle = 0f;
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
