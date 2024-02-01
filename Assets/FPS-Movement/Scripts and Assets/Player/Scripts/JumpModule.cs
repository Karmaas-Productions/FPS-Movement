using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

[CustomEditor(typeof(JumpModule))]
public class RequiredScriptsJump : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        JumpModule script = (JumpModule)target;

        // Display standard script fields
        DrawDefaultInspector();

        // Display information about required scripts
        EditorGUILayout.LabelField("Required Modules:", "Movement Manager, Movement Module");
    }
}

public class JumpModule : MonoBehaviour
{
    
#region Variables
    
    [Header("Customize")]
    [Tooltip("The force applied to the player when they jump. The recommended value is 5.")]
    [SerializeField] private float jumpForce;
    
    [Tooltip("The maximum number of jumps that the player can perform. The recommended value is 2.")]
    [SerializeField] private int maxJumps;
    
    [Tooltip("The amount of time that the player has to wait before they can jump again. The recommended value is 0.125.")]
    [SerializeField] private float jumpCooldown;
    
    [Tooltip("The layer used to tell the script what the ground is. Set this to the layer that your ground is on.")]
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Private Variables")]
    [SerializeField] private int jumpsRemaining;
    [SerializeField] private float lastJumpTime;
    
    [Header("References")]
    [SerializeField] private MovementManager movementManager;
    
#endregion

#region Jump

    private void Start()
    {
        ResetJumps();
    }

    private void ResetJumps()
    {
        jumpsRemaining = maxJumps;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if ((groundLayer.value & 1 << collision.gameObject.layer) != 0)
        {
            ResetJumps();
        }
    }

    public void StartJump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0 && Time.time - lastJumpTime >= jumpCooldown && movementManager.canJump)
        {
            Jump();
        }
    }

    public void Jump()
    {
        if (jumpsRemaining > 0 && movementManager.canJump)
        {
            movementManager.rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpsRemaining--;
        }

        lastJumpTime = Time.time;
    }
    
#endregion
}
