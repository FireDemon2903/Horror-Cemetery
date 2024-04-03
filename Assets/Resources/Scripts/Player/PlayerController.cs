// Ignore Spelling: DMG

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using Outline = cakeslice.Outline;

[RequireComponent(typeof(PlayerInput))]                                 // Player input
[RequireComponent (typeof(CapsuleCollider), typeof(Rigidbody))]         // Collision (and more)
[RequireComponent(typeof(AudioSource))]                                 // Sound requirements

public class PlayerController : MonoBehaviour, IAlive, IDamage
{
    private static PlayerController _instance;
    public static PlayerController Instance
    {
        get
        {
            if (_instance.IsUnityNull()) Debug.LogError("Player was null");
            return _instance;
        }
    }

    #region--------------- Player Attributes ---------------
    readonly float GrabDist = 10;                                                      // Grab/Interact/Attack distance

    // --------------- Player Movement ---------------
    float BasePlayerSpeed = 15f;                                                       // Base player speed
    //float jumpForce = 100;                                                           // The force with which the player jumps
    public float RotationSens = 50f;                                                   // Mouse sensitivity

    // Priorities crouching speed. If player is crouched, then speed will remain halved,
    // even thought they are technically running in the eyes of the code.
    float SpeedMultiplyer => IsCrouched ? .5f : IsRunning ? 1.5f : 1f;        // Player speed multiplier. Dependant on state
    float Speed => BasePlayerSpeed * SpeedMultiplyer;                          // Total player speed after state checks

    Vector2 newRotation;                                                    // Rotation input
    Vector2 movement;                                                       // Movement input

    Vector3 MovementDirection => new(transform.forward.x * movement.y + transform.right.x * movement.x, 0, transform.forward.z * movement.y + transform.right.z * movement.x);
    Vector3 NewVelocity => MovementDirection.normalized * Speed;

    // --------------- Player Alive ---------------
    //float DMGMult = 1.0f;
    //public float DMG  => 10 * DMGMult;
    //public float Health = 10;
    
    // From IAlive. Cannot be delegates, so no easy multipliers -_-
    public float Health { get; set; } = 10f;
    public float DMG { get; set; } = 10f;

    [Range(0, 1)] public int DMGMode = 0;             // 0: CQC, 1: Gun

    // --------------- Player States ---------------
    bool IsRunning = false;
    bool IsCrouched = false;

#nullable enable
    public GameObject? LastObjectInSight;
#nullable disable
    LayerMask interactiblesLayer;

    // --------------- Components on this object ---------------
    Rigidbody mRigidbody;
    CapsuleCollider mCollider;

    // Audio
    AudioSource[] mAudioSources;                                                    // 0: reading, 1: sound, 2: radio & BGM
    AudioMixer mAudioMixer;                                                         // Move to GM later

    // Light
    Light mLight;
    int CurrLight => lightTypes.IndexOf(mLight.type);
    readonly List<LightType> lightTypes = new() { LightType.Spot, LightType.Point };

    // --------------- Collectibles ---------------
    // Move to dedicated crafting script later:
    public List<GameManager.Parts> OwnedParts = new();

    bool canCraftGun => GameManager.CanCraftItem(OwnedParts, GameManager.Parts.GunBarrel, GameManager.Parts.GunHandle, GameManager.Parts.GunCyllinder);
    bool canCraftBullet => GameManager.CanCraftItem(OwnedParts, GameManager.Parts.Casing, GameManager.Parts.Gunpowder);
    #endregion--------------- Player Attributes ---------------

    #region --------------- Methods ---------------
    #region --------------- Builtin ---------------
    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        else { _instance = this; }


        mRigidbody = GetComponent<Rigidbody>();
        mCollider = GetComponent<CapsuleCollider>();

        mAudioSources = GetComponents<AudioSource>();

        // Assign interactables layer
        interactiblesLayer = LayerMask.GetMask("Interactable");

        // Move to GM later
        mAudioMixer = Resources.Load<AudioMixer>("Audio/PlayerAudioMixer");

        // Assign audio players to mixer-groups
        mAudioSources[0].outputAudioMixerGroup = mAudioMixer.FindMatchingGroups("Readable")[0];
        mAudioSources[1].outputAudioMixerGroup = mAudioMixer.FindMatchingGroups("SFX")[0];
        mAudioSources[2].outputAudioMixerGroup = mAudioMixer.FindMatchingGroups("Music")[0];

        // Assign light
        mLight = GetComponentInChildren<Light>();
        mLight.enabled = false;
    }

    private void FixedUpdate()
    {
        // fucks with colliders
        //mRigidbody.MovePosition(newPosition);

        // TODO add floor/step sounds
        mRigidbody.AddForce(NewVelocity + Physics.gravity, ForceMode.VelocityChange);
    }

    private void Update()
    {
        // If the player looks at anything in the 'Intractable' layer within grab-distance
        if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, GrabDist, interactiblesLayer)
            && !hitInfo.collider.gameObject.isStatic)
        {
            // Check if the object is different from the last one in sight
            if (hitInfo.collider.gameObject != LastObjectInSight)
            {
                // Revert changes to the previous object in sight
                if (LastObjectInSight != null)
                {
                    Destroy(LastObjectInSight.GetComponent<Outline>());
                }
                hitInfo.collider.gameObject.AddComponent<Outline>().color = 2;

                // Update the last object in sight
                LastObjectInSight = hitInfo.collider.gameObject;
            }
        }
        else if (LastObjectInSight != null)
        {
            // Revert changes to the last object in sight when it leaves sight
            Destroy(LastObjectInSight.GetComponent<Outline>());
            LastObjectInSight = null;
        }
        // Draw in editor for debugging
        Debug.DrawRay(transform.position, transform.forward * GrabDist, Color.green);
        Debug.DrawRay(transform.position, mRigidbody.velocity, Color.red);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision != null)
    //    {
    //        if (collision.collider.bounds.Intersects())
    //    }
    //}


    #endregion --------------- Builtin ---------------

    void PlayClip(AudioClip clip)
    {
        if (clip == null) return;

        mAudioSources[0].clip = clip;
        mAudioSources[0].Play();
    }

    /// <summary>
    /// Toggles the light type (options in ´lightTypes´ list)
    /// </summary>
    public void ToggleLightType() { mLight.type = lightTypes[CurrLight + 1 < lightTypes.Count ? CurrLight + 1 : 0]; }

    private void DoRangedDMG()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, Mathf.Infinity, interactiblesLayer)
            && !hitInfo.collider.gameObject.isStatic)
        {
            // make smoke at impact point
            //hitInfo.point
            hitInfo.collider.gameObject.TryDealDamage(source: this);
        }
    }

    public void TakeDMG(IDamage DMGSource)
    {
        if (DMGSource.IsUnityNull()) return;

        if (Health - DMGSource.DMG <= 0)
        {
            Destroy(gameObject);
        }
        Health -= DMGSource.DMG;
    }

    public void DealDMG(IAlive? target)
    {
        // If the player has a gun, do ranged damage
        if (DMGMode == 1) { DoRangedDMG(); return; }

        // Else if the last/current object in sight is not null, then tell the other object to kill itself
        // Try to deal damage to the object. this is a try, because it is not known whether the object can take damage
        LastObjectInSight.TryDealDamage(source: this);
    }


    // TODO: Jump(?), crouch renderer/model
    #region --------------- Inputs ---------------
    /// <summary>
    /// Buttons are WASD.
    /// </summary>
    /// <param name="value">Vector2</param>
    void OnMove(InputValue value) { movement = value.Get<Vector2>(); }

    // TODO: Stop changing the players hit-box when moving the FOV
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
        transform.eulerAngles = newRotation;
    }
    
    /// <summary>
    /// Button: LMB
    /// </summary>
    /// <param name="value">Left mouse button. Interaction: press</param>
    void OnFire(InputValue value)
    {
        DealDMG(null);
    }
    
    /// <summary>
    /// Button: E
    /// </summary>
    /// <param name="value">Button</param>
    void OnInteract()
    {
        // Try to get the interactor, and call interact
        if (LastObjectInSight.TryGetComponent(out Interactor interactor)) interactor.Interact(gameObject);
    }
    
    /// <summary>
    /// Button: V
    /// </summary>
    /// <param name="value"></param>
    void OnStopSound(InputValue value) { mAudioSources[0].Stop(); }
    
    /// <summary>
    /// Button: Left shift. Press And Release
    /// </summary>
    /// <param name="value">Button (check-init)</param>
    void OnRun(InputValue value) { IsRunning = !IsRunning; }
    
    /// <summary>
    /// Button: Left Control. Press And Release
    /// </summary>
    /// <param name="value">Button (check-init)</param>
    void OnCrouch(InputValue value)
    {
        // Change state
        IsCrouched = !IsCrouched;

        // Crouch
        if (IsCrouched)
        {
            // Change collider
            mCollider.direction = 2;

            // Play crouch animation

            // Move camera 5 units down
            Camera.main.transform.localPosition += Vector3.down * 1;
        }
        // Get up
        else
        {
            // Change collider
            mCollider.direction = 1;
            
            // Play get up animation

            // Move camera 5 units down
            Camera.main.transform.localPosition += Vector3.up * 1;
        }
    }
    
    // Primarily used for testing, but not required
    /// <summary>
    /// Button: Spacebar
    /// </summary>
    /// <param name="value">Button (check-init)</param>
    void OnJump(InputValue value) { /*IsJumping = !IsJumping;*/ print($"Bullet: {canCraftBullet}, Gun: {canCraftGun}"); }
    
    /// <summary>
    /// Button: F
    /// Toggles light-type
    /// </summary>
    /// <param name="value">Button</param>
    void OnLightToggle(InputValue value) { mLight.enabled = !mLight.enabled; }
    #endregion --------------- Inputs ---------------
    #endregion --------------- Methods ---------------

}