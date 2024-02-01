using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

[CustomEditor(typeof(WallRunModule))]
public class RequiredScriptsWallRun : Editor
{
    public override void OnInspectorGUI()
    {
        WallRunModule script = (WallRunModule)target;

        // Display standard script fields
        DrawDefaultInspector();

        // Display information about required scripts
        EditorGUILayout.LabelField("Required Modules:", "Movement Manager, Movement Module, Camera Module");
    }
}

public class WallRunModule : MonoBehaviour
{
    
#region Variables
    
    [Header("Customize")]
    [Tooltip("The movement speed that the player will reach when wall running. The recommended value is 175.")]
    [SerializeField] private float wallRunSpeed;

    [Tooltip("The max speed that the player will reach when wall running. The recommended value is 175.")]
    [SerializeField] private float wallRunMaxSpeed;
    
    [Tooltip("The layer used to tell the script what a wall is. Set this to the layer that your walls are on.")]
    [SerializeField] private LayerMask wallLayer;
    
    [Tooltip("The distance that the player can be from a wall to start wall running. The recommended value is 1.")]
    [SerializeField] private float wallDistance;
    
    [Tooltip("The minimum height that the player can be from the ground to start wall running. The recommended value is 1.5.")]
    [SerializeField] private float minimumHeight;
    
    [Tooltip("The force applied to the player when they jump off of a wall. The recommended value is 5.")]
    [SerializeField] private float wallRunJumpForce;
    
    [Tooltip("The amount of tilt applied to the camera when wall running. The recommended value is 20.")]
    [SerializeField] private float wallRunCameraTilt;
    
    [Tooltip("The amount of FOV applied to the camera when wall running. The recommended value is 60.")]
    [SerializeField] private float wallRunFov;

    [Header("References")]
    [SerializeField] private MovementManager movementManager;
    [SerializeField] private MovementModule movementModule;
    [SerializeField] private CameraModule cameraModule;

    [Header("Private Variables")]
    private bool wallLeft;
    private bool wallRight;
    private bool jumped;
    private bool shoulduseEndWallRun = true;
    private bool useTilt = true;
    
#endregion

#region WallRun

    public void StartWallJump(InputAction.CallbackContext context)
    {
        if (wallRight)
        {
            Vector3 wallRunDirection = wallLeft ? -Vector3.right : Vector3.right;
            
            Vector3 jumpDirection = wallLeft ? Vector3.right : -Vector3.right;
            
            movementManager.rb.velocity = new Vector3(movementManager.rb.velocity.x, 0, movementManager.rb.velocity.z);
            
            movementManager.rb.AddForce(jumpDirection * wallRunJumpForce, ForceMode.Impulse);
            
            ResetJump();
        }

        if (wallLeft)
        {
            Vector3 wallRunDirection = wallLeft ? -Vector3.right : Vector3.right;
            
            Vector3 jumpDirection = wallRunDirection;
            
            movementManager.rb.velocity = new Vector3(movementManager.rb.velocity.x, 0, movementManager.rb.velocity.z);
            
            movementManager.rb.AddForce(jumpDirection * wallRunJumpForce, ForceMode.Impulse);

            ResetJump();
        }
    }

    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumHeight);
    }

    void CheckWall()
    {
        wallLeft = Physics.Raycast(transform.position, -transform.right, wallDistance, wallLayer);
        wallRight = Physics.Raycast(transform.position, transform.right, wallDistance, wallLayer);
    }
    
    private void Update()
    {
        CheckWall();

        if (CanWallRun() && movementManager.canWallRun)
        {
            if (wallLeft || wallRight)
            {
                if (!movementManager.isWallRunning)
                {
                    StartWallRun();
                    movementManager.canClimb = false;
                }
            }
            else
            {
                if (movementManager.isWallRunning)
                {
                    StopWallRun();
                    movementManager.canClimb = true;
                }
            }
        }
        else
        {
            if (movementManager.isWallRunning)
            {
                StopWallRun();
                movementManager.canClimb = true;
            }
        }
        
        if (movementManager.isWallRunning)
        {
            float tiltDirection = wallLeft ? -wallRunCameraTilt : wallRunCameraTilt;

            if (useTilt)
            {
                cameraModule.DoTilt(tiltDirection);
            }
        }

        if (jumped && movementManager.isGrounded)
        {
            movementManager.canWalk = true;
            jumped = false;
            shoulduseEndWallRun = true;

            movementManager.isWallRunning = false;

            useTilt = true;

            movementManager.rb.useGravity = true;
        }
    }

    void StartWallRun()
    {
        movementManager.isWallRunning = true;

        movementManager.rb.useGravity = false;

        cameraModule.DoFov(wallRunFov);
        
        Vector3 wallRunDirection = wallLeft ? -Vector3.right : Vector3.right;
        float tiltDirection = wallLeft ? -wallRunCameraTilt : wallRunCameraTilt;

        if (useTilt)
        {
            cameraModule.DoTilt(tiltDirection);
        }
        
        movementManager.canSprint = false;
        
        movementModule.moveSpeed = wallRunSpeed;
        movementModule.moveSpeed = wallRunMaxSpeed;
    }

    void StopWallRun()
    {
        if (shoulduseEndWallRun)
        {
            movementManager.isWallRunning = false;

            movementManager.rb.useGravity = true;

            if (useTilt == true)
            {
                cameraModule.DoTilt(0f);
            }

            cameraModule.DoFov(cameraModule.defaultFov);
            
            movementManager.canWalk = true;
            
            movementManager.canSprint = true;

            movementModule.moveSpeed = movementModule.normalMoveSpeed;
            movementModule.maxSpeed = movementModule.normalMaxSpeed;
        }
    }

    void ResetJump()
    {
        movementManager.canWalk = false;

        useTilt = false;

        shoulduseEndWallRun = false;

        jumped = true;

        cameraModule.DoTilt(0f);

        cameraModule.DoFov(cameraModule.defaultFov);

        movementManager.rb.useGravity = true;
        
        movementManager.canSprint = true;

        movementModule.moveSpeed = movementModule.normalMoveSpeed;
        movementModule.maxSpeed = movementModule.normalMaxSpeed;
    }
    
#endregion

}
