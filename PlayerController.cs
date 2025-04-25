using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityScale;

    [Header("Collider")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float groundRayCheckDistance;

    [Header("Reference")]
    [SerializeField] private CharacterController m_CharacterController;

    private bool requestJump;
    private bool isGrounded;
    private bool isRunning;
    private float verticalVelocity;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        InputSystem.actions.FindAction("Jump").started += OnJump;
    }

    private void Update()
    {
        isGrounded = GroundHittedObj().transform != null;
        isRunning = InputSystem.actions.FindAction("Run").ReadValue<float>() != 0;

        Vector2 moveInput = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().normalized;
        Vector3 forward = moveInput.x * transform.right + moveInput.y * transform.forward;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f;
        }

        // Jump
        if (requestJump && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2.0f * Physics.gravity.y * gravityScale);
            requestJump = false;
        }

        verticalVelocity += Time.deltaTime * Physics.gravity.y * gravityScale;
        Vector3 finalMove = (Time.deltaTime * verticalVelocity * Vector3.up) + ((isRunning ? runSpeed : moveSpeed) * Time.deltaTime * forward);

        m_CharacterController.Move(finalMove);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            requestJump = true;
        }
    }

    private RaycastHit GroundHittedObj()
    {
        Vector3 initPoint = transform.position + m_CharacterController.center + (-transform.up * ((m_CharacterController.height / 2f) - 0.01f));
        Vector3 endPoint = -transform.up * groundRayCheckDistance;
        Debug.DrawRay(initPoint, endPoint, Color.magenta);
        return Physics.Raycast(initPoint, endPoint, out RaycastHit hit, groundRayCheckDistance, groundLayers) ? hit : new();
    }
}
