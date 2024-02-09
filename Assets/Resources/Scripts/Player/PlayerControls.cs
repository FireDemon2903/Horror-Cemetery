using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(AudioSource))]
public class PlayerControls : MonoBehaviour
{
    // --------------- Player Movespeed ---------------
    public float BasePlayerSpeed = 10;
    // Prioritises crouching speed. If player is crouched, then speed will remain halfed,
    // even thought they are technically running in the eyes of the code.
    float SpeedMultiplyer => IsCrouched ? .5f : IsRunning ? 2f : 1f;
    float Speed => BasePlayerSpeed * SpeedMultiplyer;

    // --------------- Player States ---------------
    bool IsRunning = false;
    bool IsCrouched = false;
    bool IsJumping = false;

    // --------------- Components on this object ---------------
    PlayerInput mPlayerInput;
    AudioSource mAudioSource;

    private void Start()
    {
        mPlayerInput = GetComponent<PlayerInput>();
        mAudioSource = GetComponent<AudioSource>();
    }

    #region Inputs
    /// <summary>
    /// Buttons are WASD.
    /// </summary>
    /// <param name="value">Vector2</param>
    void OnMove(InputValue value)
    {

    }
    /// <summary>
    /// Delta mouse
    /// </summary>
    /// <param name="value">Vector2</param>
    void OnLook(InputValue value)
    {

    }
    /// <summary>
    /// Button: LMB
    /// </summary>
    /// <param name="value">Left mouse button. Interaction: press</param>
    void OnFire(InputValue value)
    {

    }
    /// <summary>
    /// Button: E
    /// </summary>
    /// <param name="value">Button</param>
    void OnInteract(InputValue value)
    {

    }
    /// <summary>
    /// Button: Left shift. Press And Release
    /// </summary>
    /// <param name="value">Button (checkinit)</param>
    void OnRun(InputValue value)
    {
        IsRunning = !IsRunning;
    }
    /// <summary>
    /// Button: Spacebar
    /// </summary>
    /// <param name="value">Button (checkinit)</param>
    void OnJump(InputValue value)
    {
        IsJumping = !IsJumping;

        // TODO: Jump logic
    }
    /// <summary>
    /// Button: Left Control. Press And Release
    /// </summary>
    /// <param name="value">Button (checkinit)</param>
    void OnCrouch(InputValue value)
    {
        IsCrouched = !IsCrouched;
    }
    #endregion Inputs

}
