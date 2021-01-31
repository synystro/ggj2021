using UnityEngine;

public class Animator : MonoBehaviour {
    UnityEngine.Animator animator;
    public bool isWalking;
    void Start() {
        animator = this.GetComponentInChildren<UnityEngine.Animator>();        
    }
    void Update() {
        if(isWalking)
            animator.SetBool("isWalking", true);
        else
            animator.SetBool("isWalking", false);        
    }
}
