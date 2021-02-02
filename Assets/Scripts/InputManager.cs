using UnityEngine;

public class InputManager : MonoBehaviour {
    [SerializeField] Transform smartphone;
    [SerializeField] Transform smartphoneScreenLight;
    [SerializeField] Transform smartphoneTorch;

    float smartphoneInitialAngle;
    float targetAngle = 180f;
    float flipSpeed = 4f;

    bool isFlipping;
    bool isFlipped;

    void Start() {
        Cursor.visible = false;
        // initialize inventory
        Inventory.Init();
        smartphoneInitialAngle = smartphone.localEulerAngles.y;
        isFlipped = true;        
    }
    void Update() {
        if(Input.GetButtonDown("TorchLight"))
            ToggleTorch();
        if(isFlipping) {
            Quaternion smoothRotation = Quaternion.Lerp(smartphone.localRotation, Quaternion.Euler(smartphone.localEulerAngles.x, targetAngle, smartphone.localEulerAngles.z), flipSpeed * Time.deltaTime);
            smartphone.localRotation = smoothRotation;
            if(smartphone.localEulerAngles.y == targetAngle)
                isFlipping = false;
        }
    }
    void ToggleTorch() {
        smartphoneScreenLight.gameObject.SetActive(!smartphoneScreenLight.gameObject.activeSelf);
        smartphoneTorch.gameObject.SetActive(!smartphoneTorch.gameObject.activeSelf);

        HandleSmartphoneFlip();
        
    }
    void HandleSmartphoneFlip() {
        if(isFlipped) {
            targetAngle = -180f;
            isFlipped = false;
        }
        else {
            targetAngle = smartphoneInitialAngle;
            isFlipped = true;
        }
        isFlipping = true;
    }
}
