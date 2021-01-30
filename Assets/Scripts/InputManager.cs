﻿using UnityEngine;

public class InputManager : MonoBehaviour {
    [SerializeField] Transform smartphoneScreenLight;
    [SerializeField] Transform smartphoneTorch;
    void Update() {
        if(Input.GetButtonDown("TorchLight"))
            ToggleTorch();
    }
    void ToggleTorch() {
        smartphoneScreenLight.gameObject.SetActive(!smartphoneScreenLight.gameObject.activeSelf);
        smartphoneTorch.gameObject.SetActive(!smartphoneTorch.gameObject.activeSelf);            
    }
}
