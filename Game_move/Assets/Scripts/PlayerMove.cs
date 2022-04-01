using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public Rigidbody2D rd;
    public Animator anim;
    public SpriteRenderer sr;


    void Start()
    {
        rd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

   
    void Update()
    {
        walk();
        Reflect();
        Jump();
        CheckingGround();
    }

    public Vector2 moveVector;
    public float speed = 2f;

    public bool faceRight = true;

    void walk()
    {
        moveVector.x = Input.GetAxis("Horizontal");
        anim.SetFloat("moveX", Mathf.Abs(moveVector.x));
        rd.velocity = new Vector2(moveVector.x * speed, rd.velocity.y);
    }

    void Reflect()
    {
        if ((moveVector.x > 0 && !faceRight) || (moveVector.x < 0 && faceRight))
        {
            transform.localScale *= new Vector2(-1, 1);
            faceRight = !faceRight;
        }
    }

    public float jumpForce = 7f;

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            rd.AddForce(Vector2.up * jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && onGround)
        {
            rd.AddForce(Vector2.up * jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.W) && onGround)
        {
            rd.AddForce(Vector2.up * jumpForce);
        }
    }

    public bool onGround;
    public Transform GroundCheck;
    public float checkRadius = 0.5f;
    public LayerMask Ground;

    void CheckingGround()
    {
        onGround = Physics2D.OverlapCircle(GroundCheck.position, checkRadius, Ground);
        anim.SetBool("onGround", onGround);
    }
}
