using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objeto : MonoBehaviour
{
    public Animator animLuz;
    public Color cor;
    public bool estadoAtual = false;



    void Awake()
    {
        animLuz = GetComponentInChildren<Animator>();
    }

    public void Proximidade(bool estado)
    {
        animLuz.SetBool("Ligada", estado);
    }
}