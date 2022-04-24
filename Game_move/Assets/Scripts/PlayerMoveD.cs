using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveD : MonoBehaviour
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
        //CorrectLadder();
        //LadderCenter();
    }

    public Vector2 moveVector;
    public float speed = 2f;
    public bool faceRight = true;
    
    void walk()
    {
        moveVector.x = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("moveX", Mathf.Abs(moveVector.x));
        if (!onLadder) { rb.velocity = new Vector2(moveVector.x * speed, rb.velocity.y); }
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
        if (onGround && !onLadder)
        {
            //Прыжок на X
            if (Input.GetKeyDown(KeyCode.X) && (onGround || (++jumpCount < maxJumpValue)) && !jumpLock)
            {
                rb.AddForce(Vector2.up * jumpForce);
            }

            if (onGround) { jumpCount = 0; }
        }
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
        if (rb.velocity.y != 0 && bottomCheckedLadder) { anim.SetBool("onGround", false); }
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


    float vertInput;
    public bool onLadder;
    void LADDERS()
    {
        vertInput = Input.GetAxisRaw("Vertical");
        if (checkedLadder || bottomCheckedLadder)
        {
            if (!checkedLadder && bottomCheckedLadder) // Персонаж сверху
            {
                if (vertInput > 0) { onLadder = false; anim.Play("idle"); }
                else if (vertInput < 0) { onLadder = true; }
            }
            else if (checkedLadder && bottomCheckedLadder) // Персонаж на лестнице
            {
                if (vertInput > 0) { onLadder = true; }
                else if (vertInput < 0) { onLadder = true; }
                else if (Input.GetKeyDown(KeyCode.X) && rb.velocity.y == 0) { onLadder = false; }
            }
            else if (checkedLadder && !bottomCheckedLadder) // Персонаж снизу
            {
                if (vertInput > 0) { onLadder = true; }
                else if (vertInput < 0) { onLadder = false; }
            }
        }
        else { onLadder = false; } // Персонаж вне лестницы
        LaddersMechanics();
        anim.SetBool("onLadder", onLadder);
    }


    //bool corrected = true;
    //void CorrectLadder()
    //{
    //    if (onLadder && corrected) { corrected = !corrected; LadderCenter(); }
    //    else if (!onLadder && !corrected) { corrected = true; }
    //}

    //float ladderCenter;
    //void LadderCenter()
    //{
    //    ladderCenter = Physics2D.OverlapPoint(CHECK_Ladder.position, LadderMask).gameObject.transform.position.x; Debug.Log(ladderCenter);
    //    ladderCenter = Physics2D.OverlapPoint(CHECK_Ladder.position, LadderMask).GetComponent< TilemapCollider2D> ().bounds.center.x; Debug.Log(ladderCenter);
    //}



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "LadderStairs")
        {
            Physics2D.IgnoreCollision(collision.GetComponent<EdgeCollider2D>(), GetComponent<CapsuleCollider2D>(), true);
            Physics2D.IgnoreCollision(collision.GetComponent<EdgeCollider2D>(), GetComponent<BoxCollider2D>(), true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "LadderStairs")
        {
            Physics2D.IgnoreCollision(collision.GetComponent<EdgeCollider2D>(), GetComponent<CapsuleCollider2D>(), false);
            Physics2D.IgnoreCollision(collision.GetComponent<EdgeCollider2D>(), GetComponent<BoxCollider2D>(), false);
        }
    }
}
