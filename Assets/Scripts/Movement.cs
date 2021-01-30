using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour {
    
    CharacterController controller;
    float walkSpeed = 2f;
    float runSpeed = 5f;
    float speed = 2f;
    float jumpHeight = 2f;
    float gravity = -9.81f * 2;
    Vector3 velocity;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    float groundDistance = 0.4f;
    bool isGrounded;

    void Awake() {
        controller = this.GetComponent<CharacterController>();    
    }
    void Update() {

        speed = walkSpeed;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if(Input.GetButton("Run"))
            speed = runSpeed;

        HandleMovement();

        if(Input.GetButtonDown("Jump") && isGrounded)
            HandleJump();              
    }
    void HandleMovement() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);     
    }
    void HandleJump() {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}