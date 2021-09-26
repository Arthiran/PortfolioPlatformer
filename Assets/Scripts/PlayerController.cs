using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private int CanWallJumpAgain = 0;

    private Vector3 RopeOffset;

    public TextMeshProUGUI textMesh;

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

        HVelocity = !CanClimb ? Input.GetAxisRaw("Horizontal") : 0;

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
        if (Input.GetButtonDown("Jump") && (CheckGrounded || (CheckOnWall && CanWallJumpAgain == 0)))
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
        CanWallJumpAgain = 1;
    }

    private void Climb()
    {
        transform.position = new Vector3(RopeOffset.x+0.275f, transform.position.y, transform.position.z);
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
            return true;
        }
        else
        {
            CanWallJumpAgain = 0;
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
            if (LeftHit1.collider != null)
            {
                RopeOffset = LeftHit1.point;
            }
            else if (LeftHit2.collider != null)
            {
                RopeOffset = LeftHit2.point;
            }
            else if (LeftHit3.collider != null)
            {
                RopeOffset = LeftHit3.point;
            }
            else if (RightHit1.collider != null)
            {
                RopeOffset = RightHit1.point;
            }
            else if (RightHit2.collider != null)
            {
                RopeOffset = RightHit2.point;
            }
            else if (RightHit3.collider != null)
            {
                RopeOffset = RightHit3.point;
            }
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
