using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private SpriteRenderer Sprites;
    private EdgeCollider2D EdgeCollider;
    private Rigidbody2D RigidBody;
    private Animator CharAnimator;

    [SerializeField]
    private LayerMask TileLayerMask;
    [SerializeField]
    private LayerMask RopeLayerMask;
    [SerializeField]
    private string VideoPlayerTag = "VideoPlayer";
    [SerializeField]
    private CameraController cameraController;
    [SerializeField]
    private float MoveSpeed = 3f;
    [SerializeField]
    private float JumpFactor = 7f;
    [SerializeField]
    private float ClimbSpeed = 2f;
    [SerializeField]
    private float RayDistanceGround = 0.016f;
    [SerializeField]
    private float RayDistanceWall = 0.01f;
    [SerializeField]
    private float XOffset = 0.1f;
    [SerializeField]
    private float YOffset = 0.1f;

    private float HVelocity;

    private bool CanJump = false;
    private bool CanClimb = false;

    private bool lastDirection = false;

    private Collider2D currentCollision;
    private Collider2D previousCollision;

    void Start()
    {
        Sprites = GetComponent<SpriteRenderer>();
        EdgeCollider = GetComponent<EdgeCollider2D>();
        RigidBody = GetComponent<Rigidbody2D>();
        CharAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool CheckGrounded = IsGrounded();
        bool CheckOnWall = IsOnWall();
        bool CheckOnRope = IsOnRope();

        // Animations
        Sprites.flipX = CanClimb ? true : lastDirection;
        CharAnimator.SetFloat("XVelocity", Mathf.Abs(RigidBody.velocity.x));
        CharAnimator.SetFloat("YVelocity", RigidBody.velocity.y);
        CharAnimator.SetBool("Grounded", CheckGrounded);
        CharAnimator.SetBool("OnWall", CheckOnWall || CanClimb);

        HVelocity = Input.GetAxisRaw("Horizontal");

        if (HVelocity != 0f)
        {
            lastDirection = HVelocity < 0f ? true : false;
        }

        // Disallows jumping in air
        if (!CheckGrounded && !CheckOnWall)
        {
            CanJump = false;
        }

        if (!CheckOnRope)
        {
            CanClimb = false;
        }

        if (Input.GetKeyDown(KeyCode.W) && CheckOnRope)
        {
            CanClimb = true;
        }
        else if (Input.GetKeyUp(KeyCode.W) && CheckOnRope)
        {
            CanClimb = false;
        }

        // Jumps if allowed
        if (Input.GetButtonDown("Jump") && (CheckGrounded || (CheckOnWall && currentCollision == null)))
        {
            CanJump = true;
        }
    }

    private void FixedUpdate()
    {
        // Executing movement in physics update
        if (CanJump)
        {
            Jump();
        }

        if (CanClimb)
        {
            Climb();
        }

        MovePlayer();
    }

    private void MovePlayer()
    {
        RigidBody.velocity = new Vector2(HVelocity * MoveSpeed, RigidBody.velocity.y);
    }

    private void Jump()
    {
        RigidBody.velocity = new Vector2(RigidBody.velocity.x, JumpFactor);
        CanJump = false;
    }

    private void Climb()
    {
        RigidBody.velocity = new Vector2(RigidBody.velocity.x, ClimbSpeed);
    }

    private bool IsGrounded()
    {
        Vector3 RayOffset = new Vector3(XOffset, 0, 0);
        Debug.DrawRay(EdgeCollider.bounds.center + RayOffset, Vector2.down * (EdgeCollider.bounds.extents.y + RayDistanceGround));
        Debug.DrawRay(EdgeCollider.bounds.center - RayOffset, Vector2.down * (EdgeCollider.bounds.extents.y + RayDistanceGround));
        RaycastHit2D RightHit = Physics2D.Raycast(EdgeCollider.bounds.center + RayOffset, Vector2.down, EdgeCollider.bounds.extents.y + RayDistanceGround, TileLayerMask);
        RaycastHit2D LeftHit = Physics2D.Raycast(EdgeCollider.bounds.center - RayOffset, Vector2.down, EdgeCollider.bounds.extents.y + RayDistanceGround, TileLayerMask);

        if (RightHit.collider != null || LeftHit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsOnWall()
    {
        Vector3 RayOffset = new Vector3(0, YOffset, 0);
        Debug.DrawRay(EdgeCollider.bounds.center + RayOffset, Vector2.left * (EdgeCollider.bounds.extents.y + RayDistanceWall));
        Debug.DrawRay(EdgeCollider.bounds.center - RayOffset, Vector2.left * (EdgeCollider.bounds.extents.y + RayDistanceWall));
        RaycastHit2D LeftHit1 = Physics2D.Raycast(EdgeCollider.bounds.center + RayOffset, Vector2.left, EdgeCollider.bounds.extents.y + RayDistanceWall, TileLayerMask);
        RaycastHit2D LeftHit2 = Physics2D.Raycast(EdgeCollider.bounds.center - RayOffset, Vector2.left, EdgeCollider.bounds.extents.y + RayDistanceWall, TileLayerMask);
        RaycastHit2D LeftHit3 = Physics2D.Raycast(EdgeCollider.bounds.center, Vector2.left, EdgeCollider.bounds.extents.y + RayDistanceWall, TileLayerMask);

        Debug.DrawRay(EdgeCollider.bounds.center + RayOffset, Vector2.right * (EdgeCollider.bounds.extents.y + RayDistanceWall));
        Debug.DrawRay(EdgeCollider.bounds.center - RayOffset, Vector2.right * (EdgeCollider.bounds.extents.y + RayDistanceWall));
        RaycastHit2D RightHit1 = Physics2D.Raycast(EdgeCollider.bounds.center + RayOffset, Vector2.right, EdgeCollider.bounds.extents.y + RayDistanceWall, TileLayerMask);
        RaycastHit2D RightHit2 = Physics2D.Raycast(EdgeCollider.bounds.center - RayOffset, Vector2.right, EdgeCollider.bounds.extents.y + RayDistanceWall, TileLayerMask);
        RaycastHit2D RightHit3 = Physics2D.Raycast(EdgeCollider.bounds.center, Vector2.right, EdgeCollider.bounds.extents.y + RayDistanceWall, TileLayerMask);

        if (LeftHit1.collider != null || LeftHit2.collider != null || LeftHit3.collider != null || RightHit1.collider != null || RightHit2.collider != null || RightHit3.collider != null)
        {
            if (CanJump)
            {
                if (LeftHit1.collider != null)
                {
                    currentCollision = LeftHit1.collider;
                }
                else if (LeftHit2.collider != null)
                {
                    currentCollision = LeftHit2.collider;
                }
                else if (LeftHit3.collider != null)
                {
                    currentCollision = LeftHit3.collider;
                }
                else if (RightHit1.collider != null)
                {
                    currentCollision = RightHit1.collider;
                }
                else if (RightHit2.collider != null)
                {
                    currentCollision = RightHit2.collider;
                }
                else if (RightHit3.collider != null)
                {
                    currentCollision = RightHit3.collider;
                }
            }
            return true;
        }
        else
        {
            currentCollision = null;
            return false;
        }
    }

    private bool IsOnRope()
    {
        Vector3 RayOffset = new Vector3(0, YOffset, 0);
        RaycastHit2D LeftHit1 = Physics2D.Raycast(EdgeCollider.bounds.center + RayOffset, Vector2.left, EdgeCollider.bounds.extents.y + RayDistanceWall, RopeLayerMask);
        RaycastHit2D LeftHit2 = Physics2D.Raycast(EdgeCollider.bounds.center - RayOffset, Vector2.left, EdgeCollider.bounds.extents.y + RayDistanceWall, RopeLayerMask);
        RaycastHit2D LeftHit3 = Physics2D.Raycast(EdgeCollider.bounds.center, Vector2.left, EdgeCollider.bounds.extents.y + RayDistanceWall, RopeLayerMask);

        RaycastHit2D RightHit1 = Physics2D.Raycast(EdgeCollider.bounds.center + RayOffset, Vector2.right, EdgeCollider.bounds.extents.y + RayDistanceWall, RopeLayerMask);
        RaycastHit2D RightHit2 = Physics2D.Raycast(EdgeCollider.bounds.center - RayOffset, Vector2.right, EdgeCollider.bounds.extents.y + RayDistanceWall, RopeLayerMask);
        RaycastHit2D RightHit3 = Physics2D.Raycast(EdgeCollider.bounds.center, Vector2.right, EdgeCollider.bounds.extents.y + RayDistanceWall, RopeLayerMask);

        if (LeftHit1.collider != null || LeftHit2.collider != null || LeftHit3.collider != null || RightHit1.collider != null || RightHit2.collider != null || RightHit3.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == VideoPlayerTag)
        {
            cameraController.SetCameraState(1);
            cameraController.SetVideoPlayerLocation(collision.transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == VideoPlayerTag)
        {
            cameraController.SetCameraState(0);
        }
    }
}
