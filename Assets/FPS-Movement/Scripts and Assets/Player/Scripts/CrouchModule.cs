using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

[CustomEditor(typeof(CrouchModule))]
public class RequiredScriptsCrouch : Editor
{
    public override void OnInspectorGUI()
    {
        CrouchModule script = (CrouchModule)target;

        // Display standard script fields
        DrawDefaultInspector();

        // Display information about required scripts
        EditorGUILayout.LabelField("Required Modules:", "Movement Manager, Movement Module");
    }
}

public class CrouchModule : MonoBehaviour
{
    
#region Variables
    
    [Header("Customize")]
    [SerializeField] [Tooltip("Speed value for movement speed when crouching. The recommended value is 10.")]
    private float crouchMoveSpeed;
    
    [SerializeField] [Tooltip("The maximum speed that the player can move at when crouching. The recommended value is 10.")]
    private float crouchMaxSpeed;
    
    [SerializeField] [Tooltip("The height of the player when crouching. The recommended value is 0.5.")]
    private float crouchHeight;

    [Header("References")]
    [SerializeField] private MovementManager movementManager;
    [SerializeField] private MovementModule movementModule;
    
#endregion

#region Crouch

    private void FixedUpdate()
    {
        if (!movementManager.isSprinting)
        {
            if (movementManager.isCrouching)
            {
                Crouch();
            }
            else
            {
                StopCrouch();
            }
        }
    }
    
    private void Crouch()
    {
        movementModule.moveSpeed = crouchMoveSpeed;
        movementModule.maxSpeed = crouchMaxSpeed;

        movementManager.orientation.localScale = new Vector3(1, crouchHeight, 1);

        movementManager.rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }
    
    private void StopCrouch()
    {
        movementModule.moveSpeed = movementModule.normalMoveSpeed;
        movementModule.maxSpeed = movementModule.normalMaxSpeed;

        movementManager.orientation.localScale = new Vector3(1, movementManager.playerHeight, 1);
    }
    
    public void StartCrouch(InputAction.CallbackContext context)
    {
        if (movementManager.canCrouch)
        {
            movementManager.isCrouching = true;
        }
    }
    
    public void StopCrouch(InputAction.CallbackContext context)
    {
        movementManager.isCrouching = false;
    }
    
#endregion

}