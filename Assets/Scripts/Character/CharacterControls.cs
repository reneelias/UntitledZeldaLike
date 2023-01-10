using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.InputSystem;

public class CharacterControls : MonoBehaviour
{ 
    [Header("Character Controls")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected PlayableCharacter playableCharacter;
    protected PlayerControls controls;
    public PlayerControls Controls{
        protected set => controls = value;
        get => controls;
    }
    [SerializeField] protected PlayerInput playerInput;
    public PlayerInput PlayerInput{
        protected set => playerInput = value;
        get => playerInput;
    }
    protected bool flipped = false;
    public bool Flipped{
        get{return flipped;}
    }
    [SerializeField] protected float speedModifier = .05f;
    [SerializeField] protected float walkSpeed = 1f;
    [SerializeField] protected Rigidbody2D m_Rigidbody2D;
	protected Vector3 m_Velocity = Vector3.zero;
    public Vector2 Velocity{
        get{ return m_Rigidbody2D.velocity; }
    }
    [Range(0, .3f)] [SerializeField] protected float m_MovementSmoothing = .05f;
    protected bool translatingToPosition;
    protected Vector2 translationDirection;
    public bool NormalControlsSuspended{
        get; set;
    }
    [SerializeField] protected VariableJoystick movementJoystick;

    protected bool movementInputDetected_Y = false;
    [SerializeField] protected float normalDrag = 10f;
    [SerializeField] protected float slipperyDrag = 7f;
    [SerializeField] PlayableCharacter character;
    Vector2 lastOnGroundPosition;
    [SerializeField] GameObject previousDoorResetPositionObject;
    Vector3 previousDoorResetPosition;
    public bool Falling{
        private set;
        get;
    } = false;
    [SerializeField] protected float fallTimeLength = .5f;
    protected Vector3 originalScale;
    [SerializeField] AudioClip fallSound;
    [SerializeField] protected Animator animator;
    protected bool interactionPause = false;
    protected bool justFinishedInteraction = false;
    [SerializeField] KeyCode interactionKey = KeyCode.E;
    Interactable_Type interactionType;
    GameObject interactableObject;
    bool withinInteractRange = false;
    float interactionPauseDT = 0f;
    [SerializeField] float interactionPauseTime = 1f;
    protected GameObject discoveredItem;
    [SerializeField] protected GameObject discoveredItemPositionObject;
    protected bool walking = false;

    public bool FacingForward{
        protected set; get;
    }
    public bool swordSlashing = false;
    protected GameObject weapon;
    protected bool animatingWeaponSwitch = false;
    public enum WeaponType{
        Staff,
        Sword
    }
    [SerializeField] protected WeaponType currentWeaponType = WeaponType.Staff;
    [SerializeField] public WeaponType CurrentWeaponType{
        get{return currentWeaponType;}
    }
    [SerializeField] protected bool swordObtained = false;
    public CharacterDirection CharacterDirection{
        protected set;
        get;
    }
    [SerializeField] CharacterDirection startCharacterDirection = CharacterDirection.Up;
    [SerializeField] protected Sprite frontSprite;
    [SerializeField] protected Sprite backSprite;
    [SerializeField] protected Sprite leftSprite;
    [SerializeField] protected Sprite rightSprite;
        protected Sprite prevSprite;
    [SerializeField] protected AudioClip swordSound;
    [SerializeField] protected GameObject swordHitboxes;
    protected GameObject currentSwordColliderParent;
    protected GameObject currentSwordHitbox;
    protected int currentSwordHitboxNum;
    public int SlashNum {
        get;
        protected set;
    } = 0;
    [SerializeField] int swordDamage = 10;
    public int SwordDamage{
        protected set => swordDamage = value;
        get => swordDamage;
    }
    [SerializeField] int floorLevel = 1;
    public int FloorLevel{
        get => floorLevel;
        protected set => floorLevel = value;
    }
    bool onStairCase = false;
    [SerializeField] float stairMovementSlowMultiplier = .5f;
    protected Vector2 moveInput;
    [SerializeField] protected StaffLightSprite staffLightSprite;
    protected bool attackHeldDown = false;
    [SerializeField] protected Staff staff;
    [SerializeField] protected Sword sword;
    [SerializeField] protected WizardFloorLight wizardFloorLight;
    [SerializeField] protected float dodgeMagnitude = 2.5f;
    protected bool dodging = false;
    [SerializeField] protected float dodgeTime = .2f;
    [SerializeField] protected int dodgeStaminaCost = 20;
    protected float dodgeDT = 0f;
    [SerializeField] protected float dodgeTrailSpawnTime = .04f;
    protected float dodgeTrailSpawnDT = 0f;
    [SerializeField] protected float dodgeTrailFadeDur = .04f;
    [SerializeField] protected float dodgeSpriteStartingAlpha = .5f;
    [SerializeField] protected DodgeTrailSprite dodgeTrailSpritePrefab;
    protected DodgeTrailSprite[] dodgeTrailSprites;
    protected int dodgeTrailIndex = -1;
    [SerializeField] protected Color dodgeSpriteColor = Color.white;
    [SerializeField] protected Collider2D boxCollider;
    [SerializeField] protected Collider2D boxColliderTrigger;
    protected Tween physicalDodgeTween;
    protected Tween timerDodgeTween;
    protected Dictionary<CharacterDirection, Vector2> directionVectorsDictionary;
    [SerializeField] Shield shield;
    protected bool shielding = false;
    [SerializeField] float shieldingMovementMultiplier = .5f;
    [SerializeField] float shieldingMass = 5f;
    protected bool defensiveButtonHeldDown = false;
    public int SortingOrder{
        get => spriteRenderer.sortingOrder;
    }
    [SerializeField] BoxCollider_Trigger boxCollider_Trigger;
    bool teleporting = false;
    bool justFinishedTeleporting = false;
    bool beingMovedByTile = false;
    [SerializeField] float movementTileShieldForceMulti = 5f;
    MovementTile currentMovementTile;
    bool onFloatingPlatform = false;
    public bool OnFloatingPlatform {
        get => onFloatingPlatform;
        protected set => onFloatingPlatform = value;
    }
    Vector2 floatingPlatformVelocity;
    FloatingPlatform currentFloatingPlatform;
    List<(GameObject movePosition, float moveSpeed)> eventMoveActions;
    int eventMoveIndex = 0;
    protected bool eventMoveActive = false;
    EventTrigger currentEventTrigger;
    protected bool eventControlsSuspended = false;
    public Vector2 AimingInput{
        protected set;
        get;
    }

    void Awake()
    {
        // Debug.Log(playerInput.action);
        // controls = new PlayerControls();
        // // controls.Gameplay.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        // // controls.Gameplay.Aiming.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        // // controls.Gameplay.Movement.canceled += ctx => moveInput = Vector2.zero;
        
        // controls.Gameplay.PrimaryAttack.performed += ctx => PrimaryAttackTriggered();
        // controls.Gameplay.PrimaryAttack.canceled += ctx => PrimaryAttackReleased();

        // controls.Gameplay.WeaponSwitch.performed += ctx => TriggerWeaponSwitch();

        // controls.Gameplay.DefensiveAbility.performed += ctx => TriggerDefensiveAbility();
        // controls.Gameplay.DefenisveAbilityReleased.performed += ctx => DefensiveButtonReleased();

        // controls.Gameplay.Aiming.performed += ctx => AimingInput = ctx.ReadValue<Vector2>();
        // controls.Gameplay.Aiming.canceled += ctx => AimingInput = Vector2.zero;

        // Controls.Gameplay.Pause.performed += ctx => SetPause();

        directionVectorsDictionary = new Dictionary<CharacterDirection, Vector2>();
        directionVectorsDictionary.Add(CharacterDirection.Up, Vector2.up);
        directionVectorsDictionary.Add(CharacterDirection.Down, Vector2.down);
        directionVectorsDictionary.Add(CharacterDirection.Left, Vector2.left);
        directionVectorsDictionary.Add(CharacterDirection.Right, Vector2.right);

        SetCharacterDirection(startCharacterDirection, true);
        
        // controls.Gameplay.PrimaryAttackHeldDown.performed += ctx => PrimaryAttackHeldDown();
        // controls.Gameplay.PrimaryAttackHeldDown.canceled += ctx => PrimaryAttackReleased();
        // controls.Gameplay.PrimaryAttack.performed += ctx => ClickTriggered();
    }
   
    // Start is called before the first frame update
    protected virtual void Start()
    {
        NormalControlsSuspended = false;
        if(previousDoorResetPositionObject != null){
            previousDoorResetPosition = previousDoorResetPositionObject.transform.position;
        }
        originalScale = transform.localScale;
        if(swordObtained){
            GameMaster.Instance.Controls_UI.ActivateWeaponSwitchUI();
        }

        InitializeDodgeTrailSprites();

        GameMaster.Instance.crosshair.controls = controls;
        // lastOnGroundPosition = Vector2.negativeInfinity;
        // m_Rigidbody2D.drag = normalDrag;

        // controls.controlSchemes[0].
    }
    
    void OnEnable()
    {
        // controls.Enable();
    }

    void OnDisable()
    {
        // controls.Disable();
    }

    protected virtual void InitializeWeapon(){
        SetWeaponType();
        weapon.SetActive(true);
        if(currentWeaponType == WeaponType.Staff){
            sword.gameObject.SetActive(false);
        } else if(currentWeaponType == WeaponType.Sword){
            staff.gameObject.SetActive(false);
        }
    }

    protected virtual void InitializeDodgeTrailSprites(){
        dodgeTrailSprites = new DodgeTrailSprite[15];
        
        for(int i = 0; i < dodgeTrailSprites.Length; i++){
            dodgeTrailSprites[i] = GameObject.Instantiate(dodgeTrailSpritePrefab);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateWalkAnim();
        UpdateCharacterDirection();
        UpdateWeapon();
        UpdateShielding();
        UpdateInteractionPause();
        UpdateInput();
        UpdateEventMove();
    }

    protected virtual void FixedUpdate()
    {
        ControlUpdate();

    }

    protected virtual void UpdateInput(){
        // if(Input.GetKeyDown(KeyCode.Space)){
        // if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetButtonDown("PrimaryAttack")){
        
        // if(controls.Gameplay.PrimaryAttack.triggered){
        //     if(currentWeaponType == WeaponType.Staff){
                
        //     }

            
        // }
    }

    protected void UpdateWeapon(){
        switch(CurrentWeaponType){
            case WeaponType.Sword:
                // SlashWeapon();
            break;
            case WeaponType.Staff:
                if(attackHeldDown){
                    staffLightSprite.ShootBeam();
                }
            break;
        }
    }

    protected void UpdateShielding(){
        if(!shielding){
            return;
        }

        if(playableCharacter.Stamina <= 0){
            UnShield();
        }
    }

    protected virtual void ControlUpdate(){
        if(GameMaster.Instance.GameOver){
            m_Rigidbody2D.velocity = Vector2.zero;
        }

        if(translatingToPosition || NormalControlsSuspended || !character.Alive || dodging){
            return;
        }

        // if(swordSlashing){
		//     m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, Vector2.zero, ref m_Velocity, m_MovementSmoothing);
        //     return; 
        // }
        // Vector3 speedVector = new Vector3(Input.GetAxisRaw("Horizontal") * speedModifier, Input.GetAxisRaw("Vertical") * speedModifier, 0);
        // speedVector = speedVector.normalized * walkSpeed;
        // this.transform.position += speedVector;
        Vector2 targetVelocity;
        
        // targetVelocity = new Vector2(Input.GetAxisRaw("Horizontal") * speedModifier, Input.GetAxisRaw("Vertical") * speedModifier);
        // targetVelocity = new Vector2(moveInput.x * speedModifier, moveInput.y * speedModifier);
        targetVelocity = new Vector2(moveInput.x, moveInput.y);

        // if(movementJoystick != null && (Math.Abs(movementJoystick.Horizontal) > 0 || Math.Abs(movementJoystick.Vertical) > 0)){
        //     targetVelocity = new Vector2(movementJoystick.Horizontal * walkSpeed, movementJoystick.Vertical * walkSpeed);
        // } else {
        //     targetVelocity = targetVelocity.normalized * walkSpeed;
        // }

        if(targetVelocity.magnitude > 1f){
            targetVelocity = targetVelocity.normalized;
        }

        targetVelocity *= walkSpeed;

        if(swordSlashing){
            targetVelocity = Vector2.zero;
        }
        
        if(onStairCase){
            targetVelocity *= stairMovementSlowMultiplier;
        }
         
        if(shielding){
            targetVelocity *= shieldingMovementMultiplier;
        }
        

        if(Math.Abs(targetVelocity.y) > .05f){
            movementInputDetected_Y = true;
        } else {
            movementInputDetected_Y = false;
        }
        
        // if(targetVelocity.magnitude != 0){
		    m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
            // m_Rigidbody2D.AddForce(targetVelocity.normalized, ForceMode2D.Force);
        // }

        if(onFloatingPlatform){
            m_Rigidbody2D.velocity += (Vector2)currentFloatingPlatform.Velocity * .33f * Time.deltaTime / Time.fixedDeltaTime;
        }

        if(targetVelocity.x != 0){
            // Flip(targetVelocity.x > 0);
        }
    }

    protected virtual void UpdateCharacterDirection(){

        if(spriteRenderer.sprite == frontSprite){
            CharacterDirection = CharacterDirection.Down;
        } else if(spriteRenderer.sprite == backSprite){
            CharacterDirection = CharacterDirection.Up;
        } else if(spriteRenderer.sprite == rightSprite){
            CharacterDirection = CharacterDirection.Right;
        } else if(spriteRenderer.sprite == leftSprite){
            CharacterDirection = CharacterDirection.Left;
        }

        
    }

    protected virtual void UpdateWalkAnim(){
        if(swordSlashing || shielding || teleporting || !playableCharacter.Alive || eventControlsSuspended || MenuScript.GamePaused){
            return;
        }
        // if(swordSlashing || shielding || teleporting || (moveInput.magnitude <= .05f && !eventControlsSuspended) || !playableCharacter.Alive){
        //     return;
        // }

        // if(m_Rigidbody2D.velocity.magnitude <= .01f && walking){
        //     walking = false;
        // }

        if(moveInput.magnitude <= .05f && walking){
            walking = false;
        }

        // if(moveInput.magnitude > .05f){
        if(moveInput.magnitude > .05f){
            walking = true;
            // Debug.Log("Updating Prev Velocity");
            // Debug.Log($"CurrentHoriz: {m_Rigidbody2D.velocity.normalized.x}, CurrentVert: {m_Rigidbody2D.velocity.normalized.y}");
            // animator.SetFloat("PrevHorizontal", m_Rigidbody2D.velocity.normalized.x);
            // animator.SetFloat("PrevVertical", m_Rigidbody2D.velocity.normalized.y);
            animator.SetFloat("PrevHorizontal", moveInput.normalized.x);
            animator.SetFloat("PrevVertical", moveInput.normalized.y);
        }

        // animator.SetFloat("Speed", m_Rigidbody2D.velocity.magnitude);
        // animator.SetFloat("Horizontal", m_Rigidbody2D.velocity.normalized.x);
        // animator.SetFloat("Vertical", m_Rigidbody2D.velocity.normalized.y);
        animator.SetFloat("Speed", moveInput.magnitude);
        animator.SetFloat("Horizontal", moveInput.normalized.x);
        animator.SetFloat("Vertical", moveInput.normalized.y);

        // Debug.Log($"PrevHoriz: {animator.GetFloat("PrevHorizontal")}, PrevVert: {animator.GetFloat("PrevVertical")}");
    }

    public virtual void InteractionTriggered(InputAction.CallbackContext context){
        if(context.performed){
            if(interactionPause){
                NextInteractionPhase();
            } else if(!interactionPause && withinInteractRange){
                InteractWithObject();
            }
        }
    }

    protected virtual void InteractWithObject(){
        if(interactableObject.GetComponent<I_Interactable>().Interact()){
            PauseForInteraction(interactableObject);
            I_DiscoverableItem discoverableItem;
            List<(GameObject character, string text)> textTupleList = new List<(GameObject character, string text)>();

            switch(interactableObject.GetComponent<I_Interactable>().InteractableType){
                case Interactable_Type.Chest:
                    discoverableItem = interactableObject.GetComponent<Chest>().ContainedItem.GetComponent<I_DiscoverableItem>();
                    foreach(string text in discoverableItem.DiscoverMessages){
                        textTupleList.Add((null, text));
                    }
                    break;
                default:
                    discoverableItem = interactableObject.GetComponent<I_DiscoverableItem>();
                    foreach(string text in discoverableItem.DiscoverMessages){
                        textTupleList.Add((null, text));
                    }
                    break;
            }
            

            GameMaster.Instance.mainTextPrompt.ShowPrompt(textTupleList, discoverableItem.FontScaling);
            UnShield();

            if(interactionType == Interactable_Type.Sword){
                discoverableItem.SpriteObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }   
    }

    protected virtual void UpdateEventMove(){
        if(!eventMoveActive){
            return;
        }
        
        if((gameObject.transform.position - eventMoveActions[eventMoveIndex].movePosition.transform.position).magnitude <= .05f){
            if(++eventMoveIndex >= eventMoveActions.Count){
                currentEventTrigger.Finish();
                eventMoveActive = false;
                if(currentEventTrigger.GiveControlBackToPlayer){
                    DOVirtual.DelayedCall(currentEventTrigger.PauseBeforeNextAction, ()=>{
                            NormalControlsSuspended = false;
                            eventControlsSuspended = false;
                            staffLightSprite.SuspendShooting = false;
                        });
                }

                SetCharacterDirection(currentEventTrigger.CharacterDirectionWhenFinished, true);
                return;
            }

            m_Rigidbody2D.velocity = (eventMoveActions[eventMoveIndex].movePosition.transform.position - gameObject.transform.position).normalized * eventMoveActions[eventMoveIndex].moveSpeed;
        }
    }


    protected virtual void UpdateInteractionPause(){
        if(!interactionPause){
            return;
        }
        
        interactionPauseDT += Time.deltaTime;
    }

    protected virtual void NextInteractionPhase(){
        if(interactionPauseDT >= interactionPauseTime){
            if(GameMaster.Instance.mainTextPrompt.Active && GameMaster.Instance.mainTextPrompt.NextText()){
                interactionPauseDT = 0f;
                return;
            }
            interactionPause = false;
            Camera.main.GetComponent<CameraBehavior>().ZoomOutOriginalSize();
            if(eventControlsSuspended){
                if(currentEventTrigger.GiveControlBackToPlayer){
                    eventControlsSuspended = false;
                    NormalControlsSuspended = false;
                    if(currentWeaponType == WeaponType.Staff){
                        staffLightSprite.SuspendShooting = false;
                    }
                }
                currentEventTrigger.Finish();
            } else {
                NormalControlsSuspended = false;
                if(currentWeaponType == WeaponType.Staff){
                    staffLightSprite.SuspendShooting = false;
                }
            }

            if(interactionType == Interactable_Type.Chest || interactionType == Interactable_Type.Sword){
                animator.SetBool("ItemDiscovered", false);
                discoveredItem.GetComponent<I_DiscoverableItem>().Finish();
                discoveredItem.SetActive(false);

                if(interactionType == Interactable_Type.Sword){
                    swordObtained = true;
                    GameMaster.Instance.Controls_UI.ActivateWeaponSwitchUI();
                }
            }

            justFinishedInteraction = true;
            GameMaster.Instance.mainTextPrompt.HidePrompt();
        }
    }

    public void MovementTriggered(InputAction.CallbackContext context){
        if(context.performed){
            moveInput = context.ReadValue<Vector2>();
        } else if(context.canceled){
            moveInput = Vector2.zero;
        }

        // Debug.Log("MovementTriggered");
    }

    public void PrimaryAttackTriggered(InputAction.CallbackContext context){
        if(GameMaster.Instance.GameOver || MenuScript.GamePaused){
            return;
        }
        
        if(context.performed){
            switch(CurrentWeaponType){
                case WeaponType.Sword:
                    SlashWeapon();
                break;
                case WeaponType.Staff:
                    staffLightSprite.ShootBeam();
                    attackHeldDown = true;
                break;
            }
        } else {
            PrimaryAttackReleased(context);
        }

        // Debug.Log("PrimaryAttack Triggered");
    }

    public void PrimaryAttackHeldDown(InputAction.CallbackContext context){
        return;
        if(GameMaster.Instance.GameOver || MenuScript.GamePaused){
            return;
        }

        switch(CurrentWeaponType){
            case WeaponType.Sword:
            break;
            case WeaponType.Staff:
                staffLightSprite.ShootBeam();
            break;
        }
    }

    public void PrimaryAttackReleased(InputAction.CallbackContext context){ 
        if(GameMaster.Instance.GameOver || MenuScript.GamePaused || !context.canceled){
            return;
        }

        switch(CurrentWeaponType){
            case WeaponType.Sword:
            break;
            case WeaponType.Staff:
                staffLightSprite.ResetTouch();
                attackHeldDown = false;
            break;
        }
    }

    public void AimingTriggered(InputAction.CallbackContext context){
        if(context.performed){
            AimingInput = context.ReadValue<Vector2>();
        } else if(context.canceled){
            AimingInput = Vector2.zero;
        }

        GameMaster.Instance.crosshair.SetAimingInput();
    }

    public virtual void TriggerWeaponSwitch(InputAction.CallbackContext context){
        if(animatingWeaponSwitch || Falling || NormalControlsSuspended || interactionPause || !swordObtained 
            || swordSlashing || dodging || shielding || GameMaster.Instance.GameOver || MenuScript.GamePaused || !context.performed){
            return;
        }
        // switch(currentWeapon){
        //     case WeaponType.Staff:
        //         currentWeapon = WeaponType.Sword;
        //         break;
        // }

        /*SWORD ACIVATING*/
        if(currentWeaponType == WeaponType.Staff){
            staffLightSprite.TransitionStaffActive(false);
            staffLightSprite.DetachFromStaff();
            wizardFloorLight.DetachFromStaff();
            GameMaster.Instance.Controls_UI.SetSwordUiActive();
            GameMaster.Instance.crosshair.SetCrosshairVisible(false);
        }

        weapon.GetComponent<I_Weapon>().Disappear();
        currentWeaponType = (WeaponType)Math.Abs(((int)currentWeaponType) - 1);
        SetWeaponType();
        weapon.SetActive(true);
        weapon.GetComponent<I_Weapon>().Appear();
        /*STAFF ACIVATING*/
        if(currentWeaponType == WeaponType.Staff){
            staffLightSprite.TransitionStaffActive(true);
            staffLightSprite.AttachToStaff();
            wizardFloorLight.AttachToStaff();
            GameMaster.Instance.Controls_UI.SetStaffUiActive();
            GameMaster.Instance.crosshair.SetCrosshairVisible(true);
        }

        animatingWeaponSwitch = true;

        // animator.enabled = true;
        // animator.SetBool("StaffDisappearing", currentWeapon == WeaponType.Sword);
        // animator.SetBool("SwordDisappearing", currentWeapon == WeaponType.Staff);
        // animator.SetBool("StaffAppearing", false);
        // animator.SetBool("SwordAppearing", false);
    }
    protected void SlashWeapon(){
        if(swordSlashing || NormalControlsSuspended || interactionPause || Falling || currentWeaponType != WeaponType.Sword || animatingWeaponSwitch || playableCharacter.Stamina <= 0 || eventControlsSuspended){
            return;       
        }

        if(shielding){
            UnShield();
        }
        
        swordSlashing = true;
        // animator.SetFloat("PrevHorizontal", m_Rigidbody2D.velocity.normalized.x);
        // animator.SetFloat("PrevVertical", m_Rigidbody2D.velocity.normalized.y);
        prevSprite = spriteRenderer.sprite;
    
        if(prevSprite == leftSprite){
            animator.SetFloat("PrevHorizontal", -1f);
            animator.SetFloat("PrevVertical", 0);
        } else if(prevSprite == rightSprite){
            animator.SetFloat("PrevHorizontal", 1f);
            animator.SetFloat("PrevVertical", 0);
        } else if(prevSprite == frontSprite){
            animator.SetFloat("PrevHorizontal", 0);
            animator.SetFloat("PrevVertical", -1f);
        } else if(prevSprite == backSprite){
            animator.SetFloat("PrevHorizontal", 0);
            animator.SetFloat("PrevVertical", 1f);
        }

        if(++SlashNum >= int.MaxValue){
            SlashNum = 0;
        }

        animator.SetBool("SwordSlashing", swordSlashing);
        // m_Rigidbody2D.velocity = Vector2.zero;
        weapon.gameObject.SetActive(false);
        GameMaster.Instance.audioSource.PlayOneShot(swordSound);

        swordHitboxes.SetActive(true);
        currentSwordColliderParent = swordHitboxes.transform.Find($"SlashHitboxes_{CharacterDirection.ToString()}").gameObject;
        currentSwordColliderParent.SetActive(true);
        currentSwordHitboxNum = -1;
        ActivateNextSwordHitbox();

        // Debug.Log($"SlashHitbox_{CharacterDirection.ToString()}");
    }

    public virtual void TriggerDefensiveAbility(InputAction.CallbackContext context){
        if(interactionPause || NormalControlsSuspended || Falling || animatingWeaponSwitch || GameMaster.Instance.GameOver || MenuScript.GamePaused || !context.performed){
            return;       
        }

        switch(currentWeaponType){
            case WeaponType.Staff:
                if(dodging){
                    return;
                }
                Dodge();
            break;
            case WeaponType.Sword:
                if(shielding){
                    return;
                }
                Shield();
            break;
        }

        defensiveButtonHeldDown = true;
    }
    protected virtual void Dodge(){
        if(playableCharacter.Stamina < dodgeStaminaCost){
            return;
        }

        Vector2 dodgeVector;
        if(walking){
            // dodgeVector = new Vector2(animator.GetFloat("PrevHorizontal"),  animator.GetFloat("PrevVertical")).normalized;
            // dodgeVector = new Vector2(m_Rigidbody2D.velocity.x,  m_Rigidbody2D.velocity.y).normalized;
            dodgeVector = moveInput.normalized;
        } else {
            dodgeVector = directionVectorsDictionary[CharacterDirection];
        }
        float currentDodgeMagnitude = dodgeMagnitude;
        LayerMask layerMask = LayerMask.GetMask(new string[]{"Scenery"});
        RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[10];
        boxCollider.Raycast(dodgeVector, raycastHit2Ds, dodgeMagnitude);
        List<string> tagsToIgnore = new List<string>{"Enemy", "Player", "PlayerBoxCollider", "UnCollidable", "Trap", "BelowGround", "SlipperyFloor", "EnemyBarrier", "MovementTile"};


        for(int i = 0; i < raycastHit2Ds.Length; i++){
            RaycastHit2D raycastHit = raycastHit2Ds[i];
            if(!raycastHit){
                break;
            }
            if(raycastHit.collider.tag == "Trap"){
                I_Trap trap = raycastHit.collider.gameObject.GetComponent<I_Trap>();
                bool shouldBreak = false;

                switch(trap.TrapType){
                    case Trap_Type.Spikes:
                        if(trap.TrapActivated){
                            currentDodgeMagnitude = raycastHit.distance - .25f;
                            shouldBreak = true;
                            break;
                        }
                    break;
                }

                if(shouldBreak){
                    break;
                }
            }
            if(!tagsToIgnore.Contains(raycastHit.collider.tag) && raycastHit.distance < dodgeMagnitude){
            // if(raycastHit.distance < dodgeMagnitude){
                currentDodgeMagnitude = raycastHit.distance - .25f;
                break;
            }
        }

        if(beingMovedByTile){
            currentDodgeMagnitude *= .5f;
        }
        dodgeVector *= currentDodgeMagnitude;
        m_Rigidbody2D.simulated = false;
        dodging = true;
        lastOnGroundPosition = m_Rigidbody2D.position - m_Rigidbody2D.velocity.normalized * .5f;
        playableCharacter.InitiateDodge();
        
        dodgeTrailSpawnDT = 0f;
        physicalDodgeTween = transform.DOMove(new Vector3(transform.position.x + dodgeVector.x, transform.position.y + dodgeVector.y, 0f), dodgeTime * currentDodgeMagnitude / dodgeMagnitude).SetEase(Ease.Linear)
        .OnUpdate(()=>{
            dodgeTrailSpawnDT += Time.deltaTime;
            if(dodgeTrailSpawnDT >= dodgeTrailSpawnTime){
                SpawnDodgeTrailSprite();
                dodgeTrailSpawnDT = 0f;
            }
            // Debug.Log($"Dodge tween update {dodgeUpdateNum}");
            });

        timerDodgeTween = DOTween.To(()=> dodgeDT, x => dodgeDT = x, dodgeTime, dodgeTime)
            .OnComplete(()=>{
                dodging = false;
                m_Rigidbody2D.simulated = true;
                playableCharacter.EndDodge();
                boxCollider_Trigger.DodgeEnded();
            });

        playableCharacter.ChangeStamina(-dodgeStaminaCost);
    }

    void SpawnDodgeTrailSprite(){
        {
            if(++dodgeTrailIndex >= dodgeTrailSprites.Length){
                dodgeTrailIndex = 0;
            }
        }while(dodgeTrailSprites[dodgeTrailIndex].Active)

        // Debug.Log($"dodgeTrailIndex: {dodgeTrailIndex}");
        dodgeTrailSprites[dodgeTrailIndex].Activate(spriteRenderer.sprite, transform.position, dodgeSpriteStartingAlpha, dodgeTrailFadeDur, spriteRenderer.sortingLayerName, spriteRenderer.sortingOrder, transform.localScale, dodgeSpriteColor);
    }

    public void ControlsChanged(PlayerInput playerInput){
        GameMaster.Instance.InputSchemeChange(playerInput.currentControlScheme);
        // if(Gamepad.current != null){
        //     Debug.Log($"Gamepad name: {Gamepad.current.name}");
        // }

        string controlType = playerInput.currentControlScheme == "Gamepad" ? Gamepad.current.name : "Keyboard_Mouse";
        ControlsManager.Instance.SetCurrentControlType(controlType);
    }

    public virtual void SetCharacterDirection(CharacterDirection characterDirection, bool setVelocityToZero = false){
        if(setVelocityToZero){
            m_Rigidbody2D.velocity = Vector2.zero;
        }

        animator.SetFloat("PrevHorizontal", directionVectorsDictionary[characterDirection].x);
        animator.SetFloat("PrevVertical", directionVectorsDictionary[characterDirection].y);
        animator.SetFloat("Horizontal", directionVectorsDictionary[characterDirection].x);
        animator.SetFloat("Vertical", directionVectorsDictionary[characterDirection].y);
        CharacterDirection = characterDirection;
    }

    protected virtual void Shield(){
        if(playableCharacter.Stamina == 0){
            return;
        }

        if(moveInput.magnitude > 0f){
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
            animator.Update(Time.deltaTime);
        // Debug.Log($"moveInput: {moveInput}");
            UpdateCharacterDirection();
        // Debug.Log($"CharacterDirection: {CharacterDirection}");
        }

        shield.EquipShield(CharacterDirection);
        shielding = true;
        animator.SetBool("Shielding", true);
        animator.SetFloat("PrevHorizontal", directionVectorsDictionary[CharacterDirection].x);
        animator.SetFloat("PrevVertical", directionVectorsDictionary[CharacterDirection].y);
        m_Rigidbody2D.mass = shieldingMass;
    }

    protected virtual void UnShield(){
        if(!shielding){
            return;
        }

        shielding = false;
        animator.SetBool("Shielding", false);
        // Debug.Log($"Velocity magnitude after shielding: {m_Rigidbody2D.velocity.magnitude}");
        if(m_Rigidbody2D.velocity.magnitude < .25f){
            m_Rigidbody2D.velocity = Vector2.zero;
            animator.SetFloat("PrevHorizontal", directionVectorsDictionary[CharacterDirection].x);
            animator.SetFloat("PrevVertical", directionVectorsDictionary[CharacterDirection].y);
        }
        m_Rigidbody2D.mass = 1f;
        shield.UnEquipShield();
    }

    public virtual void DefensiveButtonReleased(InputAction.CallbackContext context){
        if(!context.performed){
            return;
        }

        defensiveButtonHeldDown = false;

        UnShield();
    }

    protected virtual void Flip(bool flip){
        Vector3 newScale = transform.localScale;

        if(flip){
            newScale.x = -Math.Abs(newScale.x);
        } else {
            newScale.x = Math.Abs(newScale.x);
        }

        transform.localScale = newScale;
        
        flipped = flip;
    }

    public virtual bool InitiateFall(Collider2D other){
        if(Falling){
            return false;
        }
        // Debug.Log("Falling");
        Falling = true;
        NormalControlsSuspended = true;
        GameMaster.Instance.audioSource.PlayOneShot(fallSound);
        boxCollider_Trigger.characterFalling = true;

        transform.DOScale(0f, .5f)
            .OnUpdate(()=>{transform.localEulerAngles += new Vector3(0f, 0f, 1f);})
            .OnComplete(()=>{
                transform.localScale = originalScale;
                transform.localEulerAngles = Vector3.zero;
                // m_Rigidbody2D.position = previousDoorResetPosition;
                if(GameMaster.Instance.dungeon.CurrentRoom.UseDefaultLastOnGroundPosition){
                    m_Rigidbody2D.position = GameMaster.Instance.dungeon.CurrentRoom.DefaultLastOnGroundPosition;
                } else {
                    m_Rigidbody2D.position = lastOnGroundPosition;
                }
                ResetLastOnGroundPosition();
                m_Rigidbody2D.velocity = Vector2.zero;
                NormalControlsSuspended = false;
                m_Rigidbody2D.drag = 0f;
                character.ChangeHP(-10, true);
                Falling = false;
                boxCollider_Trigger.characterFalling = false;
            });

        m_Rigidbody2D.drag = 5f;
        float fallVelocity = m_Rigidbody2D.velocity.y < 0f ? 4f : 2f;
        
        // fallVelocity = 0f;
        Vector2 closestPoint = other.ClosestPoint(transform.position);
        Vector2 position2D = transform.position;
        m_Rigidbody2D.velocity = (closestPoint - position2D).normalized * fallVelocity;

        return true;
                // m_Rigidbody2D.velocity = m_Rigidbody2D.velocity.normalized * fallVelocity;
    }

    public void TranslateToPosition(Vector3 newPosition, float transitionTime, bool turnOffPhysics = false){
        m_Rigidbody2D.velocity = Vector2.zero;
        if(turnOffPhysics){
            m_Rigidbody2D.simulated = false;
        }
        
        translatingToPosition = true;
        previousDoorResetPosition = newPosition;
        translationDirection = (newPosition - transform.position).normalized;
        transform.DOMove(newPosition, transitionTime)
            .OnComplete(()=> {
                translatingToPosition = false; 
                m_Rigidbody2D.simulated = true;
            })
            .OnUpdate(()=>{
                animator.SetFloat("Horizontal", translationDirection.x);
                animator.SetFloat("Vertical", translationDirection.y);
                animator.SetFloat("Speed", translationDirection.magnitude);
            });
    }
    protected virtual void PauseForInteraction(GameObject interactableObject){
        interactionPause = true;
        NormalControlsSuspended = true;
        Interactable_Type interactableType = interactableObject.GetComponent<I_Interactable>().InteractableType;
        // CharacterDirection = CharacterDirection.Down;
        SetCharacterDirection(CharacterDirection.Down);
        // transform.localScale = originalScale;

        if(interactableType == Interactable_Type.Chest || interactableType == Interactable_Type.Sword){
            animator.SetBool("ItemDiscovered", true);
            animator.Update(0f);
            GameObject containedItem;
            if(interactableType == Interactable_Type.Chest){
                containedItem = interactableObject.GetComponent<Chest>().ContainedItem;
            } else {
                containedItem = interactableObject;
            }
            
            DiscoveredItemInteraction(containedItem);
        }

        Camera.main.GetComponent<CameraBehavior>().ZoomInFocusObject();

        m_Rigidbody2D.velocity = Vector2.zero;
        interactionType = interactableType;
        interactionPauseDT = 0f;

        // Flip(false);
    }

    public virtual void PauseForInteractionEvent(EventTrigger eventTrigger, bool selfTriggered = false){
        
        if(!selfTriggered){
            currentEventTrigger = eventTrigger;
            NormalControlsSuspended = true;
            eventControlsSuspended = true;
            staffLightSprite.SuspendShooting = true;
        }

        switch(eventTrigger.TriggerType){
            case TriggerType.Dialogue:
                interactionPause = true;
                GameMaster.Instance.mainTextPrompt.ShowPrompt(eventTrigger.DialogueTupleList);
                break;
            default:
                break;
        }

        m_Rigidbody2D.velocity = Vector2.zero;
        interactionPauseDT = 0f;
    }

    protected virtual void DiscoveredItemInteraction(GameObject discoveredItemPrefab){
        discoveredItem = Instantiate(discoveredItemPrefab);
        discoveredItem.transform.position = discoveredItemPositionObject.transform.position;
        discoveredItem.transform.DOMoveY(discoveredItem.transform.position.y + .125f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        I_DiscoverableItem discoverableItem = discoveredItem.GetComponent<I_DiscoverableItem>();
        if(discoverableItem.SpriteObject.GetComponent<SortingOrderByY>() != null){
            discoverableItem.SpriteObject.GetComponent<SortingOrderByY>().enabled = false;
        }
        discoverableItem.SpriteObject.GetComponent<SpriteRenderer>().sortingLayerName = spriteRenderer.sortingLayerName;
        discoverableItem.SpriteObject.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 1000;
        discoverableItem.Discover();
    }

    protected virtual void ActivateNextSwordHitbox(){
        if(currentSwordHitboxNum > 1){
            return;
        }
        if(currentSwordHitboxNum >= 0){
            currentSwordHitbox.SetActive(false);
        }
        playableCharacter.ChangeStamina(0);

        // Debug.Log($"animation name: {animator.GetCurrentAnimatorClipInfo(0)[0].clip.name}");

        currentSwordHitboxNum++;
        if(currentSwordColliderParent.name.Split('_')[1] != CharacterDirection.ToString()){
            currentSwordColliderParent.SetActive(false);
            currentSwordColliderParent = swordHitboxes.transform.Find($"SlashHitboxes_{CharacterDirection.ToString()}").gameObject;
            currentSwordColliderParent.SetActive(true);
        }
        // Debug.Log(currentSwordColliderParent.name);
        // AnimatorClipInfo[] animatorinfo;
        // animatorinfo = animator.GetCurrentAnimatorClipInfo(0);
        // string currentAnimation = animatorinfo[0].clip.name;
        // Debug.Log($"Current anim name: {currentAnimation}");
        // Debug.Log($"SlashHitbox_{CharacterDirection.ToString()}_0{currentSwordHitboxNum}");
        // Debug.Log($"currentSwordHitboxNum: {currentSwordHitboxNum}");
        // Debug.Log($"CharacterDirection: {CharacterDirection}");
        currentSwordHitbox = currentSwordColliderParent.transform.Find($"SlashHitbox_{CharacterDirection.ToString()}_0{currentSwordHitboxNum}").gameObject;
        currentSwordHitbox.SetActive(true);
        // currentSwordHitbox.GetComponent<SwordHitbox>().SetLocalPosition();
    }

    protected virtual void DeactivateSwordHitboxes(){
        currentSwordHitbox.SetActive(false);
        currentSwordColliderParent.SetActive(false);
        swordHitboxes.SetActive(false);
    }

    protected virtual void EndSwordAnimation(){
        swordSlashing = false;
        animator.SetBool("SwordSlashing", false);
        weapon.gameObject.SetActive(true);

        // if(prevSprite == leftSprite){
        //     animator.SetFloat("PrevHorizontal", -1f);
        //     animator.SetFloat("PrevVertical", 0);
        // } else if(prevSprite == rightSprite){
        //     animator.SetFloat("PrevHorizontal", 1f);
        //     animator.SetFloat("PrevVertical", 0);
        // } else if(prevSprite == frontSprite){
        //     animator.SetFloat("PrevHorizontal", 0);
        //     animator.SetFloat("PrevVertical", -1f);
        // } else if(prevSprite == backSprite){
        //     animator.SetFloat("PrevHorizontal", 0);
        //     animator.SetFloat("PrevVertical", 1f);
        // }

        // m_Rigidbody2D.velocity = Vector2.zero;
        m_Velocity = Vector3.zero;
        // Debug.Log("Slashing finished");
        // Debug.Log($"PrevHoriz: {animator.GetFloat("PrevHorizontal")}, PrevVert: {animator.GetFloat("PrevVertical")}");

        if(defensiveButtonHeldDown){
            Shield();
        }
    }

    protected virtual void SetWeaponType(){
        switch(currentWeaponType){
            case WeaponType.Staff:
                weapon = staff.gameObject;
                break;
            case WeaponType.Sword:
                weapon = sword.gameObject;
                break;
        }
    }

    public virtual void CompleteWeaponSwitch(){
        animatingWeaponSwitch = false;
    }

    public void WeaponHitTarget(GameObject targetObject){
        switch(targetObject.tag){
            case "Enemy":
                Enemy targetEnemy = targetObject.GetComponent<Enemy>();
                Vector3 swordForceVector = (targetObject.transform.position - transform.position).normalized * targetEnemy.SwordHitKnockForceMagnitude;
                targetEnemy.HitWithSword(SlashNum, swordForceVector, -swordDamage);
                break;
            case "PickUp":
                IPickup pickUpItem = targetObject.GetComponent<IPickup>();
                if(pickUpItem.Pickup()){
                    switch(pickUpItem.PickupType){
                        case Pickup_Type.Heart:
                            character.ChangeHP(targetObject.GetComponent<Heart>().HealAmount, true);
                            break;
                        case Pickup_Type.Coin:
                            targetObject.GetComponent<Coin>().Pickup();
                            break;
                    }
                }
                break;
            case "Breakable":
                // Skull skull = targetObject.GetComponent<Skull>();
                // IBreakable breakable = targetObject.GetComponent<IBreakable>();
                targetObject.GetComponent<IBreakable>().Break();

                break;
            case "Scenery":
                // if(targetObject.name.Split('_')[0] == "pillar"){
                //     EndSwordAnimation();
                //     DeactivateSwordHitboxes();
                // }
                break;
        }
    }

    public void ChangeFloorLevel(int floorLevel){
        this.floorLevel = floorLevel;
    }
    
    public void StaircaseInteraction(bool entering){
        onStairCase = entering;
    }

    public void ApplyForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Impulse, bool ignoreInvinsible = false){
        if(playableCharacter.Invinsible && !ignoreInvinsible){
            return;
        }
        // Debug.Log($"Applying force to character, force strength: {force.magnitude}");
        // Debug.Log($"Character velocity before force: {m_Rigidbody2D.velocity.magnitude}");
        m_Rigidbody2D.AddForce(force, forceMode);
        // Debug.Log($"Character velocity after force: {m_Rigidbody2D.velocity.magnitude}");
    }

    public void ApplyForceFromMovementTile(Vector2 force, MovementTile movementTile){
        if(beingMovedByTile && !movementTile.Equals(currentMovementTile)){
            return;
        }
        // Debug.Log($"Force being applied by {movementTile.name}");

        if(!beingMovedByTile){
            currentMovementTile = movementTile;
            beingMovedByTile = true;
        }

        float forceMultiplier = shielding ? movementTileShieldForceMulti : 1f;
        ApplyForce(force * forceMultiplier, ForceMode2D.Force, true);
    }
    
    public void SlipperyFloorContact(){
        // if(!lastOnGroundPosition.Equals(Vector2.negativeInfinity) || Falling){
        if(Falling){
            return;
        }
        
        // Debug.Log("Slippery Floor contact");
        lastOnGroundPosition = m_Rigidbody2D.position - m_Rigidbody2D.velocity.normalized * .5f;
    }

    public void ExitSlipperyFloor(){
        if(Falling){
            return;
        }

        // ResetLastOnGroundPosition();
    }

    public void ResetLastOnGroundPosition(){
        lastOnGroundPosition = Vector2.negativeInfinity;
    }

    public void InitiateTeleportation(TeleporterPad teleporterPad){
        float tweenDuration = 1f;
        float directionChangeDT = 0f;
        float directionChangeTime = .2f;
        NormalControlsSuspended = true;
        teleporting = true;
        m_Rigidbody2D.velocity = Vector2.zero;
        m_Rigidbody2D.simulated = false;
        // teleporterPad.Room.gameObject.SetActive(true);

        transform.DOMove(teleporterPad.transform.position + new Vector3(0f, .35f, 0f), tweenDuration)
            .OnComplete(()=>{
                Teleport(teleporterPad, directionChangeTime);
            });
            // .OnUpdate(()=>{
            //     directionChangeDT += Time.deltaTime;
            //     if(directionChangeDT >= directionChangeTime){
            //         directionChangeDT = 0f;
            //         if(++CharacterDirection > CharacterDirection.Right){
            //             CharacterDirection = CharacterDirection.Down;
            //         }
            //         directionChangeTime -= .05f;
            //     }
            // });
    }

    public void Teleport(TeleporterPad teleporterPad, float directionChangeTime){
        teleporterPad.ConnectedTeleporterPad.Room.gameObject.SetActive(true);
        float directionChangeDT = 0f;
        Tween overlayFadeInTween = GameMaster.Instance.FadeInSpriteOverlay(.5f)
            .OnUpdate(()=>{
                GameMaster.Instance.dungeon.Camera.skipTransitionPositionTween = true;
                GameMaster.Instance.dungeon.Camera.SetCameraOnPlayerPosition();
            //     directionChangeDT += Time.deltaTime;
            //     if(directionChangeDT >= directionChangeTime){
            //         directionChangeDT = 0f;
            //         if(++CharacterDirection > CharacterDirection.Right){
            //             CharacterDirection = CharacterDirection.Down;
            //         }
            //         directionChangeTime -= .05f;
            //     }
            })
            .OnComplete(()=>{
                
                teleporterPad.Room.DeactivateRoom();
                teleporterPad.ConnectedTeleporterPad.Room.ActivateRoom();
                CharacterDirection = CharacterDirection.Down;
                transform.position = teleporterPad.ConnectedTeleporterPad.transform.position + new Vector3(0f, .35f, 0f);
                
                GameMaster.Instance.dungeon.Camera.SetCameraOnPlayerPosition();
                // justFinishedTeleporting = true;
                GameMaster.Instance.FadeOutSpriteOverlay(.5f)
                    .OnComplete(()=>{
                        teleporting = false;
                        justFinishedTeleporting = true;
                        NormalControlsSuspended = false;
                        m_Rigidbody2D.simulated = true;
                    }); 
                // ControlsSuspended = false;
            });
    }

    public void EnterFloatingPlatform(FloatingPlatform floatingPlatform){
        OnFloatingPlatform = true;
        currentFloatingPlatform = floatingPlatform;
    }

    public void ExitFloatingPlatform(FloatingPlatform floatingPlatform){
        if(floatingPlatform != currentFloatingPlatform) {
            return;
        }

        OnFloatingPlatform = false;
    }
    
    public void MovementEventTriggered(EventTrigger eventTrigger, bool selfTriggered = false){
        if(selfTriggered){
            NormalControlsSuspended = true;
            eventControlsSuspended = true;
            staffLightSprite.SuspendShooting = true;
            currentEventTrigger = eventTrigger;
        }

        eventMoveActions = eventTrigger.MoveActions;
        eventMoveIndex = 0;
        eventMoveActive = true;
        m_Rigidbody2D.velocity = (eventMoveActions[eventMoveIndex].movePosition.transform.position - gameObject.transform.position).normalized * eventMoveActions[eventMoveIndex].moveSpeed;
    }

    public void Respawn(int floorLevel){
        FloorLevel = floorLevel;
        switch(CurrentWeaponType){
            case WeaponType.Sword:
            break;
            case WeaponType.Staff:
                staffLightSprite.ResetTouch();
                attackHeldDown = false;
            break;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Interactable"){

            
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch(other.tag){
            // case "BelowGround":
            //     if(Falling){
            //         break;
            //     }
            //     InitiateFall();
            //     m_Rigidbody2D.drag = 5f;
            //     float fallVelocity = m_Rigidbody2D.velocity.y < 0f ? 4f : 2f;
                
            //     // fallVelocity = 0f;
            //     Vector2 closestPoint = other.ClosestPoint(transform.position);
            //     Vector2 position2D = transform.position;
            //     m_Rigidbody2D.velocity = (closestPoint - position2D).normalized * fallVelocity;
            //     // m_Rigidbody2D.velocity = m_Rigidbody2D.velocity.normalized * fallVelocity;
            //     break;
            case "PickUp":
                IPickup pickUpItem = other.GetComponent<IPickup>();
                if(pickUpItem.Pickup()){
                    switch(pickUpItem.PickupType){
                        case Pickup_Type.Heart:
                            character.ChangeHP(other.GetComponent<Heart>().HealAmount, true);
                            break;
                        case Pickup_Type.Coin:
                            other.GetComponent<Coin>().Pickup();
                            break;
                    }
                }
                break;

            case "TeleporterPad":
                TeleporterPad teleporterPad = other.gameObject.GetComponent<TeleporterPad>();

                if(teleporting || justFinishedTeleporting || !teleporterPad.Active){
                    break;
                }
                InitiateTeleportation(teleporterPad);
                break;
            case "EventTrigger":
                EventTrigger eventTrigger = other.gameObject.GetComponent<EventTrigger>();
                if(eventTrigger.Triggered){
                    return;
                }

                NormalControlsSuspended = true;
                eventControlsSuspended = true;
                staffLightSprite.SuspendShooting = true;
                eventTrigger.Trigger();
                currentEventTrigger = eventTrigger;

                switch(eventTrigger.TriggerType){
                    case TriggerType.Move:
                        MovementEventTriggered(eventTrigger);
                        
                    break;
                    case TriggerType.Dialogue:
                        PauseForInteractionEvent(eventTrigger, true);
                    break;
                    case TriggerType.CameraAnimation:
                    break;
                }

                
                break;
            
        }
        if(other.tag == "BelowGround"){

            
            // m_Rigidbody2D.velocity = Vector2.zero;
        } 
        // if(other.tag == "SlipperyFloor" && lastOnGroundPosition.Equals(Vector2.negativeInfinity)){
        // // if(other.tag == "SlipperyFloor"){
        //     lastOnGroundPosition = m_Rigidbody2D.position - m_Rigidbody2D.velocity.normalized * .5f;// - m_Rigidbody2D.velocity;
        // }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        switch(other.tag){
            // case "BelowGround":
            //     if(Falling){
            //         break;
            //     }
            //     InitiateFall();
            //     m_Rigidbody2D.drag = 5f;
            //     float fallVelocity = m_Rigidbody2D.velocity.y < 0f ? 4f : 2f;
                
            //     // fallVelocity = 0f;
            //     Vector2 closestPoint = other.ClosestPoint(transform.position);
            //     Vector2 position2D = transform.position;
            //     m_Rigidbody2D.velocity = (closestPoint - position2D).normalized * fallVelocity;
            //     // m_Rigidbody2D.velocity = m_Rigidbody2D.velocity.normalized * fallVelocity;
            //     break;
            case "Interactable":
                interactableObject = other.gameObject;
                if(withinInteractRange || interactableObject.GetComponent<I_Interactable>() == null || !interactableObject.GetComponent<I_Interactable>().Interactable){
                   break; 
                }
                withinInteractRange = true;
                interactableObject.GetComponent<I_Interactable>().DisplayInteractionPrompt();
                // if(interactableObject.Interact()){
                //     // PauseForInteraction(other.gameObject);
                // }
                break;
        } 
    }

    void OnTriggerExit2D(Collider2D other)
    {
       switch(other.tag){
            case "BelowGround":
                break;
            case "Interactable":
                interactableObject = other.gameObject;
                withinInteractRange = false;
                // if(!withinInteractRange || !interactableObject.GetComponent<I_Interactable>().Interactable){
                if(interactableObject == null || interactableObject.GetComponent<I_Interactable>() == null || !interactableObject.GetComponent<I_Interactable>().Interactable){
                   break;
                }
                interactableObject.GetComponent<I_Interactable>().HideInteractionPrompt();
                // if(interactableObject.Interact()){
                //     // PauseForInteraction(other.gameObject);
                // }
                break;
            case "TeleporterPad":
                if(justFinishedTeleporting){
                    justFinishedTeleporting = false;
                }
                break;
            case "MovementTile":
                MovementTile movementTile = other.gameObject.GetComponent<MovementTile>();
                if(movementTile.Equals(currentMovementTile)){
                    currentMovementTile = null;
                    beingMovedByTile = false;
                }
                break;
        } 
    }
}

public enum CharacterDirection{
    Down,
    Up,
    Left,
    Right
}

