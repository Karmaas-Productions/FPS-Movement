using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

[CustomEditor(typeof(SprintModule))]
public class RequiredScriptsSprint : Editor
{
    public override void OnInspectorGUI()
    {
        SprintModule script = (SprintModule)target;

        // Display standard script fields
        DrawDefaultInspector();

        // Display information about required scripts
        EditorGUILayout.LabelField("Required Modules:", "Movement Manager, Movement Module");
    }
}

public class SprintModule : MonoBehaviour
{
    
#region Variables

    [Header("Customize")]
    [Tooltip("The value used to set how fast you move while sprinting. The recommended value is 25")]
    [SerializeField] private float sprintMoveSpeed;
    
    [Tooltip("The value used to set the max speed you can move while sprinting. The recommended value is 25")]
    [SerializeField] private float sprintMaxSpeed;

    [Header("References")]
    [SerializeField] private MovementManager movementManager;
    [SerializeField] private MovementModule movementModule;
    
#endregion

#region Sprint

    private void FixedUpdate()
    {
        if (movementManager.isCrouching == false)
        {
            if (movementManager.isSprinting && movementManager.canSprint)
            {
                Sprint();
            }
            else if (movementManager.isSprinting == false)
            {
                StopSprint();
            }
        }
    }

    public void StartSprint(InputAction.CallbackContext context)
    {
        movementManager.isSprinting = true;
    }
    
    public void StopSprint(InputAction.CallbackContext context)
    {
        movementManager.isSprinting = false;
    }
    
    private void Sprint()
    {
        movementModule.moveSpeed = sprintMoveSpeed;
        movementModule.maxSpeed = sprintMaxSpeed;
    }
    
    private void StopSprint()
    {
        movementModule.moveSpeed = movementModule.normalMoveSpeed;
        movementModule.maxSpeed = movementModule.normalMaxSpeed;
    }
    
#endregion

}