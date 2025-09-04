using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D rb;
    public float speed = 4.0f;
    [SerializeField] private Animator animator;
    Vector2 movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();

        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;

    }

    // Update is called once per frame
    void Update()
    {
        //move = MoveAction.ReadValue<Vector2>();
        //Debug.Log(move);
        //Vector2 position = (Vector2)transform.position + move * 4.0f * Time.deltaTime;
        //transform.position = position;


    }
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * speed, movement.y * speed);

    }
    public void Move(InputAction.CallbackContext context)
    {
        if (movement.x != 0)
        {
            animator.SetBool("IsRunning", true);

        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
        movement = context.ReadValue<Vector2>();

    }

}
