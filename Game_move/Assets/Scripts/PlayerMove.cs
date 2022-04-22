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
        CorrectLader();

    }

    public Vector2 moveVector;
    public float speed = 2f;
    public bool faceRight = true;
    
    void walk()
    {
        moveVector.x = Input.GetAxisRaw("Horizontal");
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
    public int jumpCount = 0;
    public int maxJumpValue = 2;

    void Jump()
    {
        //Прыжок на X
        if (Input.GetKeyDown(KeyCode.X) && (onGround || (++jumpCount < maxJumpValue)) && !jumpLock)
        {
            rd.AddForce(Vector2.up * jumpForce);
        }

        if (onGround) { jumpCount = 0; }
    }

    public bool onGround;
    public Transform GroundCheck;
    public float checkRadius = 0.5f;
    public LayerMask Ground;
    private float GroundCheckRadius;

    void CheckingGround()
    {
        onGround = Physics2D.OverlapCircle(GroundCheck.position, checkRadius, Ground);
        anim.SetBool("onGround", onGround);
    }


    public Transform TopCheck;
    private float TopCheckRadius;
    public LayerMask Roof;
    public Collider2D poseStand;
    public Collider2D poseSquat;
    private bool jumpLock = false;

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
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(bottom_Ladder.position, check_RADIUS);
    }

    public Transform CHECK_Ladder;
    public bool checkedLadder;
    public LayerMask LadderMask;
    public Transform bottom_Ladder;
    public bool bottomCheckedLadder;

    void CheckingLadder()
    {
        checkedLadder = Physics2D.OverlapPoint(CHECK_Ladder.position, LadderMask);
        bottomCheckedLadder = Physics2D.OverlapPoint(bottom_Ladder.position, LadderMask);
    }

    public float ladderSpeed = 1.5f;
    void LaddersMechanics()
    {
        if (onLadder) 
        { 
            rd.bodyType = RigidbodyType2D.Kinematic;
            rd.velocity = new Vector2(rd.velocity.x, moveVector.y * ladderSpeed);
        }
        else { rd.bodyType = RigidbodyType2D.Dynamic; }
    }

    void LadderUpDown()
    {
        moveVector.y = Input.GetAxisRaw("Vertical");
        anim.SetFloat("moveY", moveVector.y);
    }

    float vertInput;
    public bool onLadder;
    void LADDERS()
    {
        vertInput = Input.GetAxisRaw("Vertical");
        if (checkedLadder || bottomCheckedLadder)
        {
            if (!checkedLadder && bottomCheckedLadder) // Персонаж сверху
            {
                if (vertInput > 0) { onLadder = false; }
                else if (vertInput < 0) { onLadder = true; }
            }
            else if (checkedLadder && bottomCheckedLadder) //Персонаж на лестнице
            {
                if (vertInput > 0) { onLadder = true; }
                else if (vertInput < 0) { onLadder = true; }
            }
            else if (checkedLadder && !bottomCheckedLadder) //Персонаж снизу
            {
                if (vertInput > 0) { onLadder = true; }
                else if (vertInput < 0) { onLadder = false; }
            }
        }
        else { onLadder = false; }
        LaddersMechanics();
        anim.SetBool("onLadder", checkedLadder);
        
    }
    bool corrected = true;
    void CorrectLader()
    {
        if (onLadder && corrected) { corrected = !corrected; Debug.Log("Корректировка"); }
        else if (!onLadder && !corrected) { corrected = true; }
    }
}
