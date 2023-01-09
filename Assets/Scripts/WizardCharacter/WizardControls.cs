using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class WizardControls : CharacterControls
{

    private Tween walkScaleTween;
    private float originalYScale;
    [Header("Wizard Controls")]
    public Color flameColor;
    float yVelocityThreshold = 0.125f;
    // [SerializeField] Sprite staffFrontSprite;
    // [SerializeField] Sprite staffBackSprite;
    // [SerializeField] Sprite swordFrontSprite;
    // [SerializeField] Sprite swordBackSprite;
    [SerializeField] KeyCode weaponSwitchCode = KeyCode.F;
    [SerializeField] PolygonCollider2D frontBackCollider;
    [SerializeField] PolygonCollider2D leftCollider;
    [SerializeField] PolygonCollider2D rightCollider;
    
    

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        originalYScale = transform.localScale.y;
        FacingForward = true;
        InitializeWeapon();
        // animator.enabled = false;
    }

    protected override void InitializeWeapon()
    {
        base.InitializeWeapon();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateConnectedObjects();
        UpdateCurrentCollider();
        // m_Rigidbody2D.angularVelocity = 0;
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateWalkScaling();
        UpdateFrontBackSprite();
        // UpdatePlayerDirection();
        // UpdateWeaponSwitchAnimation();
    }

    protected override void ControlUpdate()
    {
        base.ControlUpdate();
        // if(Math.Abs(m_Rigidbody2D.velocity.x) < .01f){
        //     Flip(false);S
        // }
    }

    protected override void UpdateInput()
    {
        // if(controls.Gameplay.PrimaryAttack.triggered){
        //     if(CurrentWeaponType == WeaponType.Staff){
        //         staffLightSprite.ShootBeam();
        //     }
        // }
            
        base.UpdateInput();
    }

    protected override void UpdateWalkAnim()
    {
        base.UpdateWalkAnim();
       
    }

    void UpdateCurrentCollider(){
        if(spriteRenderer.sprite == leftSprite && !leftCollider.enabled){
            leftCollider.enabled = true;
            rightCollider.enabled = false;
            frontBackCollider.enabled = false;
            rightCollider.offset = new Vector2(0f, .06f);

            // Vector2 boxColliderOffset = boxCollider.offset;
            // boxColliderOffset.x = -0.03007561f;

            // boxCollider.offset = boxColliderOffset;
            // boxColliderTrigger.offset = boxColliderOffset;
            return;
        } 
        
        if(spriteRenderer.sprite == rightSprite && !rightCollider.enabled){
            leftCollider.enabled = false;
            rightCollider.enabled = true;
            frontBackCollider.enabled = false;
            leftCollider.offset = new Vector2(0f, .06f);

            // Vector2 boxColliderOffset = boxCollider.offset;
            // boxColliderOffset.x = .09f;

            // boxCollider.offset = boxColliderOffset;
            // boxColliderTrigger.offset = boxColliderOffset;
            return;
        }

        if(!frontBackCollider.enabled && (spriteRenderer.sprite == frontSprite || spriteRenderer.sprite == backSprite)){
            leftCollider.enabled = false;
            rightCollider.enabled = false;
            frontBackCollider.enabled = true;

            Vector2 boxColliderOffset = boxCollider.offset;
            if(spriteRenderer.sprite == frontSprite){
                // boxColliderOffset.x = -0.03007561f;

                // boxCollider.offset = boxColliderOffset;
                // boxColliderTrigger.offset = boxColliderOffset;
                frontBackCollider.offset = new Vector2(0f, .06f);
            } else {
                frontBackCollider.offset = new Vector2(-.01f, .06f);
                // boxColliderOffset.x = -.01f;

                // boxCollider.offset = boxColliderOffset;
                // boxColliderTrigger.offset = boxColliderOffset;
            }
        }
    }

    // protected override void UpdatePlayerDirection(){
    //     // if(spriteRenderer == frontSprite){
    //     //     PlayerDirection = PlayerDirection.Down;
    //     // } else if(spriteRenderer == backSprite){
    //     //     PlayerDirection = PlayerDirection.Up;
    //     // } else if(spriteRenderer == rightSprite){
    //     //     PlayerDirection = PlayerDirection.Right;
    //     // } else {
    //     //     PlayerDirection = PlayerDirection.Left;
    //     // }
    // }

    void UpdateConnectedObjects(){
        if(swordSlashing){
            return;
        }
        // bool staffLightSpriteFlip = spriteRenderer.sprite == rightSprite || spriteRenderer.sprite == backSprite;
        bool staffLightSpriteFlip = CharacterDirection == CharacterDirection.Up ||  CharacterDirection == CharacterDirection.Right;
        
        staffLightSprite.Flip(staffLightSpriteFlip);
        staffLightSprite.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 2;

        // staff.Flip(staffLightSpriteFlip);
        // staff.gameObject.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 1;

        // sword.Flip(staffLightSpriteFlip);
        // sword.gameObject.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 1;

        WFL_FlipType flipType = WFL_FlipType.None;
        if(spriteRenderer.sprite ==  rightSprite){
            flipType = WFL_FlipType.Flip;
        } else if(spriteRenderer.sprite == backSprite){
            flipType = WFL_FlipType.UpFlip;
        }
        wizardFloorLight.Flip(flipType);

    }

    void UpdateWalkScaling(){
        if(translatingToPosition){
            return;
        }

        if(Velocity.magnitude > .01f && (moveInput.magnitude > .05f || eventControlsSuspended)){
            if(walkScaleTween == null){
                walkScaleTween = transform.DOScaleY(transform.localScale.y * 1.05f, .275f).SetLoops(-1, LoopType.Yoyo);
            }
        }
        
        if(Velocity.magnitude <= .01f || (moveInput.magnitude < .05f && !eventControlsSuspended)){
            if(walkScaleTween != null){
                walkScaleTween.Pause();
                walkScaleTween = null;
                transform.localScale = new Vector3(transform.localScale.x, originalYScale, transform.localScale.z);
            }
        }
    }

    void UpdateFrontBackSprite(){
        if(interactionPause){
            return;
        }

        if(spriteRenderer.sprite != backSprite){
            FacingForward = true;
            return;
        }
        if(spriteRenderer.sprite == backSprite){
            FacingForward = false;
            return;
        }

        // if(Velocity.y == 0 || animatingWeaponSwitch){
        //     return;
        // }

        // if(Velocity.y > yVelocityThreshold){
        //     if(spriteRenderer.sprite == backSprite){
        //         return;
        //     }

        //     FacingForward = false;
        //     // spriteRenderer.sprite = backSprite;
        // } else if(movementInputDetected_Y){
        //     if(spriteRenderer.sprite == frontSprite){
        //         return;
        //     }

        //     FacingForward = true;
        //     // spriteRenderer.sprite = frontSprite;
        // }
    }

    protected override void UpdateInteractionPause()
    {
        base.UpdateInteractionPause();

        if(justFinishedInteraction && !eventControlsSuspended){
            spriteRenderer.sprite = frontSprite;
            justFinishedInteraction = false;
            
        }
    }

    // protected override void UpdateWeaponSwitch(){
    //     base.UpdateWeaponSwitch();
    // }

    // void UpdateWeaponSwitchAnimation(){
    //     if(!animatingWeaponSwitch || interactionPause){
    //         return;
    //     }
    //     bool staffDisappearing = false, staffAppearing = false, swordDisappearing = false, swordAppearing = false;

    //     // Debug.Log("animator layer count: " + animator.layerCount);
    //     // Debug.Log($"animator normalized time (0): {animator.GetCurrentAnimatorStateInfo(0).normalizedTime}");
    //     // Debug.Log($"animator normalized time (1): {animator.GetCurrentAnimatorStateInfo(1).normalizedTime}");
    //     if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f){
    //         if(animator.GetBool("StaffDisappearing")){
    //             swordAppearing = true;
    //         } else if(animator.GetBool("SwordDisappearing")){
    //             staffAppearing = true;
    //         } else {
    //             if(currentWeaponType == WeaponType.Staff){
    //                 frontSprite = staffFrontSprite;
    //                 backSprite = staffBackSprite;
    //             } else if(currentWeaponType == WeaponType.Sword){
    //                 frontSprite = swordFrontSprite;
    //                 backSprite = swordBackSprite;
    //             }

    //             spriteRenderer.sprite = FacingForward ? frontSprite : backSprite;

    //             animatingWeaponSwitch = false;
    //         }
            
    //         animator.SetBool("StaffDisappearing", staffDisappearing);
    //         animator.SetBool("SwordDisappearing", swordDisappearing);
    //         animator.SetBool("StaffAppearing", staffAppearing);
    //         animator.SetBool("SwordAppearing", swordAppearing);

    //         if(!animatingWeaponSwitch){
    //             animator.enabled = false;
    //         }
    //     }
    // }

    protected override void PauseForInteraction(GameObject interactableObject){
        if(walkScaleTween != null){
            walkScaleTween.Pause();
            walkScaleTween = null;
        }
        
        transform.localScale = originalScale;
        if(currentWeaponType == WeaponType.Staff){
            staffLightSprite.SuspendShooting = true;
        }
        FacingForward = true;

        base.PauseForInteraction(interactableObject);
    }

    protected override void SetWeaponType()
    {
        base.SetWeaponType();
    }

    protected override void EndSwordAnimation()
    {
        base.EndSwordAnimation();

        // spriteRenderer.sprite = prevSprite;
        // UpdateConnectedObjects();
    }

    public override bool InitiateFall(Collider2D other)
    {
        if(base.InitiateFall(other)){
            staffLightSprite.ScaleTweenOrbitingParticles(0f, fallTimeLength);
            return true;
        }

        return false;
    }

    protected override void Flip(bool flip)
    {
        // /m_Rigidbody2D.velocity.x < 0 &&    
        bool shouldFlip = spriteRenderer.sprite == rightSprite || spriteRenderer.sprite == backSprite;
            base.Flip(shouldFlip);
    }
}
