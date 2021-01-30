using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorDeObjetos : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
        if (other.CompareTag("Objeto"))
            other.GetComponent<Objeto>().Proximidade(true);
    }

    private void OnTriggerExit(Collider other)
    {
        print(other.name);
        if (other.CompareTag("Objeto"))
            other.GetComponent<Objeto>().Proximidade(false);
    }
}