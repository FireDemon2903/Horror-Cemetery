using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
//[RequireComponent (typeof(CharacterController))]
public class PlayerControler : MonoBehaviour
{
    // --------------- Player Movement ---------------
    public float BasePlayerSpeed = 10f;                                         // Base player speed
    //float jumpForce = 100;                                                    // Ther force with which the player jumps
    public float mRotationSens = 15f;                                                  // Mouse sensetivity
    readonly float GrabDist = 10;

    // Prioritises crouching speed. If player is crouched, then speed will remain halfed,
    // even thought they are technically running in the eyes of the code.
    float SpeedMultiplyer => IsCrouched ? .5f : IsRunning ? 2f : 1f;        // Player speed multiplyer. Dependant on state
    float Speed => BasePlayerSpeed * SpeedMultiplyer;                       // Total player speed after state checks

    Vector2 newRotation;                                                    // Rotation input
    Vector2 movement;                                                       // Movement input

    // TODO: stop floating and clipping
    Vector3 newPosition => transform.position + (Speed * Time.deltaTime * (transform.forward * movement.y + transform.right * movement.x));

    // used to change color of object when looking at it, and reverting said change
    // for example line 90 - 109:
    // Link:C:\Users\jona015v\wkspaces\Horror Cemetery\Assets\Samples\Input System\1.7.0\In-Game Hints\InGameHintsExample.cs#90
    MaterialPropertyBlock materialPropertyBlock;

    // --------------- Player States ---------------
    bool IsRunning = false;
    bool IsCrouched = false;
    //bool IsJumping = false;

    GameObject LastObjectInSight = null;
    //GameObject ObjectInSight = null;

    // --------------- Components on this object ---------------
    PlayerInput mPlayerInput;
    AudioSource mAudioSource;
    Rigidbody mRigidbody;
    //CharacterController mCharacterController;

    private void Start()
    {
        mPlayerInput = GetComponent<PlayerInput>();
        mAudioSource = GetComponent<AudioSource>();
        mRigidbody = GetComponent<Rigidbody>();
        //mCharacterController = GetComponent<CharacterController>();

        materialPropertyBlock = new();
        //materialPropertyBlock.SetColor("_Color", Color.black);
    }

    private void FixedUpdate()
    {
        mRigidbody.MovePosition(newPosition);
    }

    private void Update()
    {
        // If the player looks at anything in the 'Interactable' layer within grabdistance
        if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, GrabDist)
            && !hitInfo.collider.gameObject.isStatic)
        {
            print(hitInfo.collider.gameObject.name);

            // Check if the object is different from the last one in sight
            if (hitInfo.collider.gameObject != LastObjectInSight)
            {
                // Revert changes to the last object in sight
                if (LastObjectInSight != null)
                {
                    LastObjectInSight.GetComponent<MeshRenderer>().SetPropertyBlock(null);
                }

                // Store the original material properties of the current object
                hitInfo.collider.gameObject.GetComponent<MeshRenderer>().GetPropertyBlock(materialPropertyBlock);

                // Change the color of the current object
                Color newColor = Color.yellow; // Change this to your desired color
                materialPropertyBlock.SetColor("_Color", newColor);
                hitInfo.collider.gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);

                // Update the last object in sight
                LastObjectInSight = hitInfo.collider.gameObject;
            }
        }
        else if (LastObjectInSight != null)
        {
            // Revert changes to the last object in sight when it leaves sight
            LastObjectInSight.GetComponent<MeshRenderer>().SetPropertyBlock(null);
            LastObjectInSight = null;
        }

        //Debug.DrawLine(transform.position, transform.forward * GrabDist, Color.green);
        Debug.DrawRay(transform.position, transform.forward * GrabDist, Color.green);
    }

    // TODO: Jump, fire, interact, crouch hitbox
    #region Inputs
    /// <summary>
    /// Buttons are WASD.
    /// </summary>
    /// <param name="value">Vector2</param>
    void OnMove(InputValue value) { movement = value.Get<Vector2>(); }

    /// <summary>
    /// Delta mouse
    /// </summary>
    /// <param name="value">Vector2</param>
    void OnLook(InputValue value)
    {
        // Get delta
        Vector2 rotate = value.Get<Vector2>();

        // Stop if too small
        if (rotate.sqrMagnitude < 0.01) return;

        // Scale
        float rotateSpeed = mRotationSens * Time.deltaTime;

        // Set y
        newRotation.y += rotate.x * rotateSpeed;

        // Set x and stop from looking between legs
        newRotation.x = Mathf.Clamp(newRotation.x - rotate.y * rotateSpeed, -89, 89);

        // Set new rotation
        transform.localEulerAngles = newRotation;
    }

    /// <summary>
    /// Button: LMB
    /// </summary>
    /// <param name="value">Left mouse button. Interaction: press</param>
    void OnFire(InputValue value) { }

    /// <summary>
    /// Button: E
    /// </summary>
    /// <param name="value">Button</param>
    void OnInteract(InputValue value) { }

    /// <summary>
    /// Button: Left shift. Press And Release
    /// </summary>
    /// <param name="value">Button (checkinit)</param>
    void OnRun(InputValue value) { IsRunning = !IsRunning; }

    /// <summary>
    /// Button: Left Control. Press And Release
    /// </summary>
    /// <param name="value">Button (checkinit)</param>
    void OnCrouch(InputValue value) { IsCrouched = !IsCrouched; }

    /// <summary>
    /// Button: Spacebar
    /// </summary>
    /// <param name="value">Button (checkinit)</param>
    //void OnJump(InputValue value) { IsJumping = !IsJumping; }
    #endregion Inputs

}