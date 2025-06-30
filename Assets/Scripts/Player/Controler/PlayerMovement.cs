using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;
    private Vector3 moveDirection;
    private float moveSpeed, horizontalInput, verticalInput;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Gravity")]
    public float normalGravity;
    private float gravityScale;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayer;
    private bool grounded;

    public Transform cameraTrasform, bodyTransform;
    private Rigidbody rb;
    private bool isAlive;

    private void Start()
    {
        moveSpeed = walkSpeed;
        gravityScale = normalGravity;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.linearDamping = 5f;
        rb.useGravity = false;
        isAlive = true;
        PlayerStatus.PlayerDeath += OnDeath;
        PlayerStatus.PlayerRevive += OnRevive;
    }

    private void Update()
    {
        if (isAlive)
        {
            PlayerInput();
            GroundCheck();
            RotatePlayerBody();
            SpeedControl();
        }
    }

    private void FixedUpdate()
    {
        if (isAlive)
        {
            MovePlayer();
            ApplyGravity();
        }
    }

    private void OnDeath()
    {
        isAlive = false;
    }

    private void OnRevive()
    {
        moveSpeed = walkSpeed;
        gravityScale = normalGravity;
        isAlive = true;
    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && grounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (Input.GetKeyUp(jumpKey) && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.x * 0.5f, rb.linearVelocity.z);

        if (Input.GetKeyDown(runKey))
            moveSpeed = runSpeed;

        if (Input.GetKeyUp(runKey))
            moveSpeed = walkSpeed;
    }

    private void MovePlayer()
    {
        moveDirection = bodyTransform.forward * verticalInput + bodyTransform.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
    }

    private void ApplyGravity()
    {
        if (!grounded)
            rb.AddForce(Vector3.down * gravityScale, ForceMode.Force);
    }

    private void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, (playerHeight / 2f) + 0.2f, groundLayer);
    }

    private void RotatePlayerBody()
    {
        bodyTransform.rotation = Quaternion.Euler(0, cameraTrasform.rotation.eulerAngles.y, 0);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
}
