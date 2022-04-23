using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        GroundCheckRadius = GroundCheck.GetComponent<CircleCollider2D>().radius;

        TopCheckRadius = TopCheck.GetComponent<CircleCollider2D>().radius;
    }

    void Update()
    {
        walk();
        Reflect();
        Jump();
        CheckingGround();
        SquatCheck();

        CheckingLadder();
        LaddersMechanics();
        LadderUpDown();
        LADDERS();
    }

    public Vector2 moveVector;
    public float speed = 2f;
    public bool faceRight = true;
    
    void walk()
    {
        moveVector.x = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("moveX", Mathf.Abs(moveVector.x));
        rb.velocity = new Vector2(moveVector.x * speed, rb.velocity.y);
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
    public int jumpCount = 0;
    public int maxJumpValue = 2;

    void Jump()
    {
        //Прыжок на X
        if (Input.GetKeyDown(KeyCode.X) && (onGround || (++jumpCount < maxJumpValue)) && !jumpLock)
        {
            rb.AddForce(Vector2.up * jumpForce);
        }

        if (onGround) { jumpCount = 0; }
    }

    //Проверка земли
    public bool onGround;
    public Transform GroundCheck;
    public LayerMask Ground;
    public float checkRadius = 0.5f;
    private float GroundCheckRadius;

    void CheckingGround()
    {
        onGround = Physics2D.OverlapCircle(GroundCheck.position, checkRadius, Ground);
        anim.SetBool("onGround", onGround);
    }


    public Transform TopCheck;
    public LayerMask Roof;
    public Collider2D poseStand;
    public Collider2D poseSquat;
    private float TopCheckRadius;
    private bool jumpLock = false;

    //Ползанье
    void SquatCheck()
    {
        if (Input.GetKey(KeyCode.DownArrow) && onGround)
        {
            anim.SetBool("squat", true);
            poseStand.enabled = false;
            poseSquat.enabled = true;
            jumpLock = true;
        }
        else if (!Physics2D.OverlapCircle(TopCheck.position, TopCheckRadius, Roof))
        {
            anim.SetBool("squat", false);
            poseStand.enabled = true;
            poseSquat.enabled = false;
            jumpLock = false;
        }
    }


    public float check_RADIUS = 0.04f;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(CHECK_Ladder.position, check_RADIUS);
    }

    public Transform CHECK_Ladder;
    public bool checkedLadder;
    public LayerMask LadderMask;
    void CheckingLadder()
    {
        checkedLadder = Physics2D.OverlapPoint(CHECK_Ladder.position, LadderMask);
        
    }


    public float ladderSpeed = 1.5f;
    void LaddersMechanics()
    {
        if (onLadder)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = new Vector2(rb.velocity.x, moveVector.y * ladderSpeed);
        }
        else { rb.bodyType = RigidbodyType2D.Dynamic; }
    }


    void LadderUpDown()
    {
        moveVector.y = Input.GetAxisRaw("Vertical");
        anim.SetFloat("moveY", moveVector.y);
    }


    public bool onLadder;
    void LADDERS()
    {
        if (checkedLadder && Input.GetAxisRaw("Vertical") != 0)
        {
            onLadder = true;
        }
        else if (!checkedLadder)
        {
            onLadder = false;
        }

        anim.SetBool("onLadder", checkedLadder);
    }
}
