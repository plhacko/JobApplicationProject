using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController CharacterController;

    [SerializeField] float Speed = 12f;
    [SerializeField] float Gravity = -9.81f;

    [SerializeField] Transform GroundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    [SerializeField] float JumpHeight = 3f;

    Vector3 Velocity;
    bool IsOnGround;

    void Start()
    {
        CharacterController = GetComponent<CharacterController>();                
    }

    void Update()
    {
        // ground check
        IsOnGround = Physics.CheckSphere(GroundCheck.position, groundDistance, groundMask);
        if (IsOnGround && Velocity.y < 0)
            Velocity.y = -2f;


        // xz  movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        // apply movement
        Vector3 move = transform.right * x + transform.forward * z; ;
        float speedBoost = Input.GetKey(KeyCode.LeftShift) ? 4f : 1f; // TODO: rm : mostly for debuging
        CharacterController.Move(move * Speed * Time.deltaTime * speedBoost);


        // jump
        if(Input.GetButtonDown("Jump") && IsOnGround)
            Velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        // gravity
        Velocity.y += Gravity * Time.deltaTime;
        // apply velocity
        CharacterController.Move(Velocity * Time.deltaTime);
    }
}
