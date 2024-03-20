using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using Outline = cakeslice.Outline;

[RequireComponent(typeof(PlayerInput))]                                 // Player input
[RequireComponent (typeof(CapsuleCollider), typeof(Rigidbody))]         // Collision (and more)
[RequireComponent(typeof(AudioSource))]                                 // Sound Reqs

public class PlayerControler : MonoBehaviour
{
    private static PlayerControler _instance;
    public static PlayerControler Instance
    {
        get
        {
            if (_instance.IsUnityNull()) Debug.LogError("Player was null");
            return _instance;
        }
    }

    #region--------------- Player Attributes ---------------
    readonly float GrabDist = 10;                                               // Grab/Interact/Attack distance

    // --------------- Player Movement ---------------
    public float BasePlayerSpeed = 10f;                                         // Base player speed
    //float jumpForce = 100;                                                    // Ther force with which the player jumps
    public float RotationSens = 50f;                                                   // Mouse sensetivity

    // Prioritises crouching speed. If player is crouched, then speed will remain halfed,
    // even thought they are technically running in the eyes of the code.
    float SpeedMultiplyer => IsCrouched ? .25f : IsRunning ? 3f : 1f;        // Player speed multiplyer. Dependant on state
    float Speed => BasePlayerSpeed * SpeedMultiplyer;                       // Total player speed after state checks

    Vector2 newRotation;                                                    // Rotation input
    Vector2 movement;                                                       // Movement input
    // TODO: stop clipping

    //Vector3 movementDirection => new(transform.forward.x * movement.y + transform.right.x * movement.x, 0, transform.forward.z * movement.y + transform.right.z * movement.x);
    //Vector3 newPosition => transform.position + (Speed * Time.deltaTime * movementDirection);

    Vector3 movementDirection => new(transform.forward.x * movement.y + transform.right.x * movement.x, Physics.gravity.y, transform.forward.z * movement.y + transform.right.z * movement.x);
    Vector3 newVelocity => movementDirection * Speed;    

    // --------------- Player Alive ---------------
    float DMGMult = 1.0f;
    public float DMG  => 10 * DMGMult;
    public float Health = 10;
    [Range(0, 1)] public int DMGMode = 0;             // 0: CQC, 1: Gun

    // --------------- Player States ---------------
    bool IsRunning = false;
    bool IsCrouched = false;

    public GameObject LastObjectInSight;
    LayerMask interactiblesLayer;

    // --------------- Components on this object ---------------
    Rigidbody mRigidbody;
    CapsuleCollider mCollider;

    // Audio
    AudioSource[] mAudioSources;                                                    // 0: reading, 1: sound, 2: radio & bgm
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

        // Assign interactibles layer
        interactiblesLayer = LayerMask.GetMask("Interactable");

        // Move to GM later
        mAudioMixer = Resources.Load<AudioMixer>("Audio/PlayerAudioMixer");

        // Assign audio players to mixergroups
        mAudioSources[0].outputAudioMixerGroup = mAudioMixer.FindMatchingGroups("Readable")[0];
        mAudioSources[1].outputAudioMixerGroup = mAudioMixer.FindMatchingGroups("SFX")[0];
        mAudioSources[2].outputAudioMixerGroup = mAudioMixer.FindMatchingGroups("Music")[0];

        // Assign light
        mLight = GetComponentInChildren<Light>();
        mLight.enabled = false;
    }

    private void FixedUpdate()
    {
        //mRigidbody.MovePosition(newPosition);

        mRigidbody.velocity = newVelocity;
    }

    private void Update()
    {
        // If the player looks at anything in the 'Interactable' layer within grabdistance
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
            hitInfo.collider.gameObject.SendMessage("TakeDMG", this, options: SendMessageOptions.DontRequireReceiver);
        }
    }


    // TODO: Jump(?), crouch renderer/model
    #region --------------- Inputs ---------------
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
        transform.eulerAngles = newRotation;
    }
    
    /// <summary>
    /// Button: LMB
    /// </summary>
    /// <param name="value">Left mouse button. Interaction: press</param>
    void OnFire(InputValue value)
    {
        // If the player has a gun, do ranged damage
        if (DMGMode == 1) { DoRangedDMG(); return; }
        // Else if the last/current object in sight is not null, then tell the other object to kill itself
        if (!LastObjectInSight.IsUnityNull()) LastObjectInSight.SendMessage("TakeDMG", this, options: SendMessageOptions.DontRequireReceiver); }
    
    /// <summary>
    /// Button: E
    /// </summary>
    /// <param name="value">Button</param>
    void OnInteract()
    {
        if (!LastObjectInSight.IsUnityNull())
        {
            switch (LastObjectInSight.tag)
            {
                // If interacting with enemy, maybe do something
                case "Enemy":
                    // TODO: Interacting with enemy
                    break;

                // Basic interactable
                default:
                    LastObjectInSight.SendMessage("Interact", gameObject, options: SendMessageOptions.RequireReceiver);
                    break;
            }
        }
    }
    
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
    void OnCrouch(InputValue value)
    {
        // Change state
        IsCrouched = !IsCrouched;

        // Chouch
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
    /// <param name="value">Button (checkinit)</param>
    void OnJump(InputValue value) { /*IsJumping = !IsJumping;*/ print($"Bullet: {canCraftBullet}, Gun: {canCraftGun}"); }
    
    /// <summary>
    /// Button: F
    /// Toggles lighttype
    /// </summary>
    /// <param name="value">Button</param>
    void OnLightToggle(InputValue value) { mLight.enabled = !mLight.enabled; }
    #endregion --------------- Inputs ---------------
    #endregion --------------- Methods ---------------

}