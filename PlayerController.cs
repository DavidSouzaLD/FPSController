using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityScale = 1f;

    [Header("Collider")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float groundRayCheckDistance;

    [Header("Reference")]
    [SerializeField] private CharacterController m_CharacterController;

    private bool requestJump;
    private float verticalVelocity;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();

        InputSystem.actions.FindAction("Jump").started += OnJump;
    }

    private void Update()
    {
        Vector2 moveInput = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().normalized;
        Vector3 forward = moveInput.x * transform.right + moveInput.y * transform.forward;

        if (m_CharacterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f;
        }

        // Jump
        if (requestJump && m_CharacterController.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2.0f * Physics.gravity.y * gravityScale);
            requestJump = false;
        }

        verticalVelocity += Time.deltaTime * Physics.gravity.y * gravityScale;
        Vector3 finalMove = (Time.deltaTime * verticalVelocity * Vector3.up) + (moveSpeed * Time.deltaTime * forward);

        m_CharacterController.Move(finalMove);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (m_CharacterController.isGrounded)
        {
            requestJump = true;
        }
    }
}
