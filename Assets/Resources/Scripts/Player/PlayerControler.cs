using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using Outline = cakeslice.Outline;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody))]              // Player Reqs
[RequireComponent(typeof(AudioMixer), typeof(AudioSource))]             // Sound Reqs
public class PlayerControler : MonoBehaviour
{
    // Game Manager
    public GameObject gameManagerObj { get; private set; }
    MixLevels mixLevel;

    // --------------- Player Movement ---------------
    public float BasePlayerSpeed = 10f;                                         // Base player speed
    //float jumpForce = 100;                                                    // Ther force with which the player jumps
    public float RotationSens = 1f;                                                  // Mouse sensetivity
    readonly float GrabDist = 10;                                               // Grab/Interact/Attack distance

    // Prioritises crouching speed. If player is crouched, then speed will remain halfed,
    // even thought they are technically running in the eyes of the code.
    float SpeedMultiplyer => IsCrouched ? .5f : IsRunning ? 2f : 1f;        // Player speed multiplyer. Dependant on state
    float Speed => BasePlayerSpeed * SpeedMultiplyer;                       // Total player speed after state checks

    Vector2 newRotation;                                                    // Rotation input
    Vector2 movement;                                                       // Movement input
    // TODO: stop floating and clipping

    Vector3 movementDirection => new(transform.forward.x * movement.y + transform.right.x * movement.x, 0, transform.forward.z * movement.y + transform.right.z * movement.x);
    //                              (transform.forward * movement.y + transform.right * movement.x)
    Vector3 newPosition => transform.position + (Speed * Time.deltaTime * movementDirection);

    // --------------- Player States ---------------
    bool IsRunning = false;
    bool IsCrouched = false;
    //bool IsJumping = false;

    //MaterialPropertyBlock materialPropertyBlock;              // No longer in use
    //Color HighlightColour = Color.grey;
    GameObject LastObjectInSight = null;
    LayerMask interactiblesLayer = 8;

    // --------------- Components on this object ---------------
    // RB
    Rigidbody mRigidbody;
    //PlayerInput mPlayerInput;
    
    // Audio
    AudioSource[] mAudioSources;                                                    // 0: reading, 1: sound, 2: radio & bgm
    AudioMixer mAudioMixer;         // Move to GM later

    // Light
    Light mLight;
    int CurrLight => lightTypes.IndexOf(mLight.type);
    readonly List<UnityEngine.LightType> lightTypes = new() { UnityEngine.LightType.Spot, UnityEngine.LightType.Point };

    // --------------- Collectibles ---------------

    // TODO: Crafting and pickup
    public enum Gunparts { GunBarrel, GunHandle, GunCyllinder }
    public enum Bulletparts { Gunpowder, Casing }

    //bool HasGun = false;
    //int Bullets = 0;                            // Full bullets

    // Move to dedicated crafting script later:
    public List<Enum> OwnedParts = new();

    // This does not work...
    static bool CanCraftItem<Enum>(List<Enum> ownedParts, params Enum[] requiredParts) { return requiredParts.All(part => ownedParts.Contains(part)); }
    bool canCraftGun => CanCraftItem(OwnedParts, Gunparts.GunBarrel, Gunparts.GunHandle, Gunparts.GunCyllinder);
    bool canCraftBullet => CanCraftItem(OwnedParts, Bulletparts.Casing, Bulletparts.Casing);

    // --------------- Methods ---------------
    private void Awake()
    {
        //mPlayerInput = GetComponent<PlayerInput>();
        mRigidbody = GetComponent<Rigidbody>();

        mAudioSources = GetComponents<AudioSource>();

        gameManagerObj = GameObject.Find("GM");

        // Move to GM later
        mAudioMixer = Resources.Load<AudioMixer>("Audio/PlayerAudioMixer");

        mAudioSources[0].outputAudioMixerGroup = mAudioMixer.FindMatchingGroups("Readable")[0];
        mAudioSources[1].outputAudioMixerGroup = mAudioMixer.FindMatchingGroups("SFX")[0];

        // Assign lights
        mLight = GetComponentInChildren<Light>();

        //materialPropertyBlock = new();
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
            // ... And the layer is correct (doesn't work if searching on layer 8 for some reason...)
            if (hitInfo.collider.gameObject.layer == interactiblesLayer)
            {
                // Check if the object is different from the last one in sight
                if (hitInfo.collider.gameObject != LastObjectInSight)
                {
                    // Revert changes to the last object in sight
                    if (LastObjectInSight != null)
                    {
                        Destroy(LastObjectInSight.GetComponent<Outline>());
                        //LastObjectInSight.GetComponent<MeshRenderer>().SetPropertyBlock(null);
                    }

                    // Store the original material properties of the current object
                    //hitInfo.collider.gameObject.GetComponent<MeshRenderer>().GetPropertyBlock(materialPropertyBlock);

                    // Change the color of the current object
                    //materialPropertyBlock.SetColor("_Color", HighlightColour);
                    //hitInfo.collider.gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);
                    var a = hitInfo.collider.gameObject.AddComponent<Outline>();
                    a.color = 1;        // Green

                    // Update the last object in sight
                    LastObjectInSight = hitInfo.collider.gameObject;
                }
            }
        }
        else if (LastObjectInSight != null)
        {
            // Revert changes to the last object in sight when it leaves sight
            //LastObjectInSight.GetComponent<MeshRenderer>().SetPropertyBlock(null);
            Destroy(LastObjectInSight.GetComponent<Outline>());
            LastObjectInSight = null;
        }

        Debug.DrawRay(transform.position, transform.forward * GrabDist, Color.green);
    }

    void PlayClip(AudioClip clip)
    {
        if (clip == null) return;

        mAudioSources[0].clip = clip;
        mAudioSources[0].Play();

        // TODO: Display to player that they can stop audio by pressing 'V'
    }

    public void ToggleLightType()
    {
        mLight.type = lightTypes[CurrLight == 0 ? 1 : 0];
    }

    // TODO: Jump(?), fire, crouch hitbox
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
        float rotateSpeed = RotationSens * Time.deltaTime;

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
    void OnInteract() { if (LastObjectInSight) LastObjectInSight.SendMessage("Interact", gameObject, options: SendMessageOptions.RequireReceiver); }
    
    /// <summary>
    /// Button: V
    /// </summary>
    /// <param name="value"></param>
    void OnStopSound(InputValue value) { mAudioSources[0].Stop(); }

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
    void OnJump(InputValue value) { /*IsJumping = !IsJumping;*/print(OwnedParts.Count); print($"Bullet: {canCraftBullet}, Gun: {canCraftGun}"); }
    /// <summary>
    /// Button: F
    /// Toggles lighttype
    /// </summary>
    /// <param name="value">Button</param>
    void OnLightToggle(InputValue value) { mLight.enabled = !mLight.enabled; }



    #endregion Inputs
}