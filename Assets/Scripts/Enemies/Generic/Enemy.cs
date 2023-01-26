using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;
using DG.Tweening;

public class Enemy : MonoBehaviour, IDefeatable
{
    protected int hp;
    [SerializeField] int maxHP;

    [SerializeField] private bool engaged;
    [SerializeField] public bool Engaged{
        get {return engaged;}
        protected set {engaged = value;}
    }

    protected Vector3 spawnPosition;

    [SerializeField] protected FillBar HPBar;

    public bool Alive{
        get;
        protected set;
    }

    [SerializeField] protected float engagementRange;
    [SerializeField] protected FollowScript followScript;
    [SerializeField] protected int damageAmount;
    protected bool countingDownEngaged;
    protected float engagedDT;
    [SerializeField] protected float engagedTime;
    // Start is called before the first frame update
    protected bool suspendEngageUpdate;
    public bool shouldFollowTarget = true;

    [SerializeField] protected Rigidbody2D m_Rigidbody2D;
    [SerializeField] protected Collider2D collider2D;
	protected Vector3 m_Velocity = Vector3.zero;
    protected Vector3 deathPosition;
    public bool shouldResetHealth = true;
    public bool shouldRevive = false;
    public bool shouldReturnToOrigin = true;
    [SerializeField] protected float dieAnimationTime = 1f;
    float initialAngle;
    [SerializeField] private bool showHPBar = true;
    [SerializeField] AudioClip deathAudioClip;
    [SerializeField] protected Animator animator;
    [SerializeField] protected bool hasWalkingAnimation = false;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    public float referenceMovementSpeed;
    public bool Defeated{
        protected set; get;
    }
    [SerializeField] LightCollider2D lightCollider2D;
    [SerializeField] GameObject[] spawnableItemTypes;
    [Range(0, 1f)][SerializeField] float dropPercent = 1f;
    [Range(0, 5f)][SerializeField] int itemSpawnAmount = 1;
    int swordSlashNum = -1;
    [SerializeField] protected bool enableCollisionDamage = true;
    [SerializeField] protected bool useTriggerDamageCheck = false;
    [SerializeField] protected float pushForceMagnitude = 30f;
    [SerializeField] int floorLevel = 1;
    public int FloorLevel{
        get => floorLevel;
        protected set => floorLevel = value;
    }
    protected bool hitShield = false;
    protected bool beingMovedByTile = false;
    protected MovementTile currentMovementTile;
    [SerializeField] SortingOrderByY sortingOrderByY;
    protected bool movementPaused = false;
    protected float movementPauseDT = 0f;
    [Tooltip("Amount of time enemy pauses movement after being hit by sword or running into player shield.")]
    [SerializeField] protected float movementPauseTime = .5f;
    float lastSwordHitPauseDT = 0f;
    [Tooltip("Amount of time between sword/shield hits being able to stun the enemy.")]
    [SerializeField] float lastSwordAndShieldPauseDuration = 3f;
    bool swordShieldHitPaused = false;
    [Tooltip("Force magnitude of being knocked by player sword.")]
    [SerializeField] float swordHitKnockForceMagnitude = 40f;
    public float SwordHitKnockForceMagnitude{
        protected set => swordHitKnockForceMagnitude = value;
        get => swordHitKnockForceMagnitude;
    }
    [Tooltip("Force magnitude of being knocked by player shield.")]
    [SerializeField] float shieldHitKnockForceMagnitude = 20f;
    public float ShieldHitKnockForceMagnitude{
        protected set => shieldHitKnockForceMagnitude = value;
        get => shieldHitKnockForceMagnitude;
    }
    [SerializeField] int playerShieldHitStaminaCost = 20;
    public int PlayerShieldHitStaminaCost{
        protected set => playerShieldHitStaminaCost = value;
        get => playerShieldHitStaminaCost;
    }
    [SerializeField] int projectileShieldHitStaminaCost = 10;
    public int ProjectileShieldHitStaminaCost{
        protected set => projectileShieldHitStaminaCost = value;
        get => projectileShieldHitStaminaCost;
    }
    [SerializeField] int playerSwordHitStaminaCost = 10;
    public int PlayerSwordHitStaminaCost{
        protected set => playerSwordHitStaminaCost = value;
        get => playerSwordHitStaminaCost;
    }
    [SerializeField] bool explodeOnDeath = true;
    [SerializeField] SmokeExplosion smokeExplosion;
    public CharacterDirection CharacterDirection{
        protected set;
        get;
    }
    [SerializeField] CharacterDirection spawnCharacterDirection = CharacterDirection.Down;
    [SerializeField] bool hasCharacterDirections = true;
    protected Dictionary<CharacterDirection, Vector2> directionVectorsDictionary;
    public bool suspendAnimationUpdate = false;

    protected virtual void Start()
    {
        Alive = true;
        hp = maxHP;

        HPBar.UpdateText($"{hp}/{maxHP}");
        
        HPBar.gameObject.SetActive(showHPBar);

        // spawnPosition = followScript.originPosition;
        spawnPosition = transform.position;

        if(followScript != null){
            followScript.originPosition = spawnPosition;
        }

        suspendEngageUpdate = false;

        initialAngle = transform.localEulerAngles.z;
        
        if(followScript != null){
            followScript.shouldFollow = shouldFollowTarget;
        }

        directionVectorsDictionary = new Dictionary<CharacterDirection, Vector2>();
        directionVectorsDictionary.Add(CharacterDirection.Up, Vector2.up);
        directionVectorsDictionary.Add(CharacterDirection.Down, Vector2.down);
        directionVectorsDictionary.Add(CharacterDirection.Left, Vector2.left);
        directionVectorsDictionary.Add(CharacterDirection.Right, Vector2.right);

        SetCharacterDirection(spawnCharacterDirection, true);
        

        // Debug.Log($"SpawnPosition:")
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateSwordHitPause();
    }
    protected virtual void FixedUpdate()
    {
        UpdateEngaged();
        UpdateWalkAnimation();
        UpdateMovementPause();

    }    
    
    // protected virtual void UpdateCharacterDirection(){

    //     if(spriteRenderer.sprite == frontSprite){
    //         CharacterDirection = CharacterDirection.Down;
    //     } else if(spriteRenderer.sprite == backSprite){
    //         CharacterDirection = CharacterDirection.Up;
    //     } else if(spriteRenderer.sprite == rightSprite){
    //         CharacterDirection = CharacterDirection.Right;
    //     } else if(spriteRenderer.sprite == leftSprite){
    //         CharacterDirection = CharacterDirection.Left;
    //     }
    // }
    protected virtual void UpdateWalkAnimation(){
        if(!Alive || !hasWalkingAnimation || movementPaused || suspendAnimationUpdate){
            return;
        }
        
        animator.speed = m_Rigidbody2D.velocity.magnitude / referenceMovementSpeed;
        animator.SetFloat("Horizontal", m_Rigidbody2D.velocity.normalized.x);
        animator.SetFloat("Vertical", m_Rigidbody2D.velocity.normalized.y);
        animator.SetFloat("Speed", m_Rigidbody2D.velocity.magnitude);
    }
    protected virtual void UpdateEngaged()
    {
        if(followScript == null || suspendEngageUpdate || !shouldFollowTarget){
            return;
        }

        if(!Engaged && followScript.shouldFollow || Engaged && !followScript.shouldFollow){
            followScript.shouldFollow = Engaged;
        }
        bool wasEngaged = Engaged;

        float distanceToTarget = Vector3.Distance(followScript.objectToFollow.transform.position, gameObject.transform.position);
        
        bool inEngageRange = distanceToTarget <= engagementRange && followScript.objectToFollow.GetComponent<CharacterControls>().FloorLevel == FloorLevel;

        if(!inEngageRange && Engaged){
            if(!countingDownEngaged){
                countingDownEngaged = true;
                engagedDT = 0;
            } else {
                engagedDT += Time.deltaTime;
                
                if(engagedTime != -1 && engagedDT >= engagedTime){
                    Engaged = false;
                    countingDownEngaged = false;
                }
            }
        } else if(inEngageRange){
            countingDownEngaged = false;
            Engaged = true;
        }
        
        if(Engaged){
            // Debug.Log($"distanceToTarget: {distanceToTarget}");
            // Debug.Log($"engagedDT: {engagedDT}");
        }

        followScript.shouldFollow = Engaged;
    }

    void UpdateMovementPause(){
        if(!movementPaused){
            return;
        }

        movementPauseDT += Time.deltaTime;
		m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, Vector2.zero, ref m_Velocity, .05f);

        if(movementPauseDT >= movementPauseTime){
            movementPauseDT = 0f;
            movementPaused = false;

            if(followScript != null && followScript.enabled){
                suspendEngageUpdate = false;
                followScript.shouldFollow = true;
            }
        }
    }

    protected virtual void UpdateSwordHitPause(){
        if(!swordShieldHitPaused){
            return;
        }

        lastSwordHitPauseDT += Time.deltaTime;

        if(lastSwordHitPauseDT >= lastSwordAndShieldPauseDuration){
            lastSwordHitPauseDT = 0f;
            swordShieldHitPaused = false;
        }
    }

    public virtual void ChangeHP(int deltaHP){
        hp += deltaHP;
        HPBar.FillPercent = (float)hp/maxHP;
        HPBar.UpdateText($"{hp}/wa{maxHP}");
    }

    public virtual void SetCharacterDirection(CharacterDirection characterDirection, bool setVelocityToZero = false){
        if(!hasCharacterDirections){
            return;
        }

        if(setVelocityToZero){
            m_Rigidbody2D.velocity = Vector2.zero;
        }

        animator.SetFloat("PrevHorizontal", directionVectorsDictionary[characterDirection].x);
        animator.SetFloat("PrevVertical", directionVectorsDictionary[characterDirection].y);
        animator.SetFloat("Horizontal", directionVectorsDictionary[characterDirection].x);
        animator.SetFloat("Vertical", directionVectorsDictionary[characterDirection].y);
        animator.SetFloat("Speed", 0f);
        animator.Update(0f);
        CharacterDirection = characterDirection;
    }

    public virtual bool HitWithSword(int swordSlashNum, Vector3 swordForceVector, int deltaHP){
        if(swordSlashNum == this.swordSlashNum){
            return false;
        }

        // Debug.Log($"Hit enemy with sword, force magnitude: {swordForceVector.magnitude}");
        bool pauseMovement = lastSwordHitPauseDT < lastSwordAndShieldPauseDuration && !swordShieldHitPaused;
        if(pauseMovement){
            swordShieldHitPaused = true;
        }
        ApplyForce(swordForceVector, ForceMode2D.Impulse, false, pauseMovement);
        // m_Rigidbody2D.AddForce(swordForceVector, ForceMode2D.Impulse);
        this.swordSlashNum = swordSlashNum;
        ChangeHP(deltaHP);

        return true;
    }

    public void EnableRenderer(bool enable){
        GetComponent<Renderer>().enabled = enable;
    }

    public void UpdateVelocity(Vector3 targetVelocity, float movementSmoothing){
		// m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, movementSmoothing);
        m_Rigidbody2D.velocity = targetVelocity;
    }



    private void OnDrawGizmosSelected(){
		UnityEngine.Gizmos.color = new Color(1f, 0.25f, 0.25f, .25f);
        UnityEngine.Gizmos.DrawWireSphere(transform.position, engagementRange);
    }

    protected virtual void Die(){
        Alive = false;
        Engaged = false;

        m_Rigidbody2D.simulated = false;
        GetComponent<SortingOrderByY>().enabled = false;
        GameMaster.Instance.audioSource.PlayOneShot(deathAudioClip);
        sortingOrderByY.enabled = false;
        spriteRenderer.sortingLayerName = "Walls";
        spriteRenderer.sortingOrder = 1000;

        Defeated = true;

        if(lightCollider2D != null){
            lightCollider2D.enabled = false;
        }

        DOVirtual.DelayedCall(dieAnimationTime, SpawnItems);
        if(explodeOnDeath){
            DOVirtual.DelayedCall(dieAnimationTime, ()=>{smokeExplosion.SpawnExplosion(transform.position, spriteRenderer.sortingLayerName);});
        }
        suspendAnimationUpdate = true;
    }

    public virtual void Revive(bool setOriginPosition = true){
        Alive = true;
        Engaged = false;
        m_Rigidbody2D.simulated = true;
        GetComponent<SortingOrderByY>().enabled = true;
        if(setOriginPosition){
            if(followScript == null){
                transform.position = spawnPosition;
            } else {
                transform.position = followScript.originPosition;
            }
        }
        transform.localEulerAngles = new Vector3(0f, 0f, initialAngle);
        if(lightCollider2D != null){
            lightCollider2D.enabled = false;
        }
        Defeated = false;
        sortingOrderByY.enabled = true;
        spriteRenderer.sortingLayerName = "Default";
        hp = maxHP;
        if(showHPBar){
            HPBar.gameObject.SetActive(true);
            HPBar.FillPercent = (float)hp/maxHP;
            HPBar.UpdateText($"{hp}/{maxHP}");
        }
        swordSlashNum = -1;
        gameObject.SetActive(true);
        EnableRenderer(true);
        SetCharacterDirection(spawnCharacterDirection);
    }

    public virtual void SetSpawnItems(GameObject[] spawnableItemTypes, float dropPercent = 1f, int itemSpawnAmount = 1){
        this.spawnableItemTypes = spawnableItemTypes;
        this.dropPercent = dropPercent;
        this.itemSpawnAmount = itemSpawnAmount;
    }

    protected virtual void SpawnItems(){
        for(int i = 0; i < itemSpawnAmount; i++){
            float randNum = Random.Range(0, 1f);

            if(randNum < dropPercent && spawnableItemTypes.Length > 0){
                GameObject dropItem = Instantiate(spawnableItemTypes[(int)Random.Range(0, spawnableItemTypes.Length)]);
                if(itemSpawnAmount == 1){
                    dropItem.transform.position = transform.position;
                } else {
                    float randAngle = Random.Range(0f, Mathf.PI);
                    float randLength = Random.Range(0f, 1f);
                    dropItem.transform.position = transform.position + new Vector3(Mathf.Cos(randAngle), Mathf.Sin(randAngle), 0f) * randLength;
                }

                if(dropItem.tag == "PickUp"){
                    dropItem.GetComponent<IPickup>().Activate();
                    GameMaster.Instance.dungeon.CurrentRoom.AddObjToDesapwn(dropItem);
                }
            }
        }   
    }
 //
        // Summary:
        //     The Transform attached to this GameObject.
    public void SetEngagement(bool engaged, GameObject objectToFollow, float engagedTime = -1){
        followScript.objectToFollow = objectToFollow;
        this.engagedTime = engagedTime;
        Engaged = engaged;
    }


    public void SetEngagement(bool engaged, GameObject objectToFollow, float engagedTime = -1, float engagementRange = 5f){
        SetEngagement(engaged, objectToFollow, engagedTime);
        this.engagementRange = engagementRange;
    }

    public virtual void ResetOnRoomLeave(){
        if(Alive){
            if(shouldResetHealth){
                hp = maxHP;
                HPBar.FillPercent = (float)hp/maxHP;
                HPBar.UpdateText($"{hp}/{maxHP}");
            }
            m_Rigidbody2D.velocity = Vector2.zero;
            Engaged = false;

            if(shouldReturnToOrigin){
                transform.position = spawnPosition;
            }
        } else if(shouldRevive){
            Revive();
        }
        
        suspendAnimationUpdate = true;
        gameObject.SetActive(false);
    }

    protected virtual bool CheckShieldCollision(){
        Collider2D[] collider2Ds = new Collider2D[10];
        collider2D.OverlapCollider(new ContactFilter2D().NoFilter(), collider2Ds);

        for(int i = 0; i < collider2Ds.Length; i++){
            Collider2D currentCollider2D = collider2Ds[i];
            if(currentCollider2D == null){
                hitShield = false;
                break;
            }
            if(currentCollider2D.tag == "Shield"){
                hitShield = true;
                return true;
            }
        }

        return false;
    }

    
    public virtual void ApplyForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Impulse, bool ignoreInvinsible = false, bool pauseMovement = true){

        // Debug.Log($"Applying force to character, force strength: {force.magnitude}");
        // Debug.Log($"Character velocity before force: {m_Rigidbody2D.velocity.magnitude}");
        m_Rigidbody2D.AddForce(force, forceMode);
        // Debug.Log($"Character velocity after force: {m_Rigidbody2D.velocity.magnitude}");

        if(pauseMovement){
            movementPaused = true;
            movementPauseDT = 0f;
            
            if(followScript != null && followScript.enabled){
                suspendEngageUpdate = true;
                followScript.shouldFollow = false;
            }
        }
    }

    public virtual void ApplyForceFromMovementTile(Vector2 force, MovementTile movementTile){
        if(beingMovedByTile && !movementTile.Equals(currentMovementTile)){
            return;
        }
        // Debug.Log($"Force being applied by {movementTile.name}");

        if(!beingMovedByTile){
            currentMovementTile = movementTile;
            beingMovedByTile = true;
        }

        ApplyForce(force, ForceMode2D.Force, true, false);
    }

    public virtual void SetAsHiveEnemy(){
        followScript.enabled = false;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Shield"){
            hitShield = true;
        }

        if(collision.gameObject.tag == "Player" && enableCollisionDamage && !useTriggerDamageCheck && !hitShield){
            
            if(!CheckShieldCollision()){
                Vector2 force = (collision.gameObject.transform.position - transform.position).normalized * pushForceMagnitude;
                collision.gameObject.GetComponent<CharacterControls>().ApplyForce(force);
                collision.gameObject.GetComponent<PlayableCharacter>().ChangeHP(-damageAmount);
            }
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D other){
        if(other.gameObject.tag == "Shield"){
            hitShield = true;
        }

        if(other.gameObject.tag == "Player" && enableCollisionDamage && !useTriggerDamageCheck && !hitShield){
            if(!CheckShieldCollision()){
                Vector2 force = (other.gameObject.transform.position - transform.position).normalized * pushForceMagnitude;
                other.gameObject.GetComponent<CharacterControls>().ApplyForce(force);
                other.gameObject.GetComponent<PlayableCharacter>().ChangeHP(-damageAmount);
            }
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player"){
            hitShield = false;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Shield"){
            hitShield = true;
        }

        if(other.tag == "Player" && useTriggerDamageCheck && !hitShield){

            if(!CheckShieldCollision()){
                Vector2 force = (collider2D.gameObject.transform.position - transform.position).normalized * pushForceMagnitude;
                collider2D.gameObject.GetComponent<CharacterControls>().ApplyForce(force);
                collider2D.gameObject.GetComponent<PlayableCharacter>().ChangeHP(-damageAmount);
            }
            // collider2D.gameObject.GetComponent<CharacterControls>()
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collider2D)
    {
        if(collider2D.tag == "Shield"){
            hitShield = true;
        }

        if(collider2D.tag == "Player" && useTriggerDamageCheck && !hitShield){

            if(!CheckShieldCollision()){
                Vector2 force = (collider2D.gameObject.transform.position - transform.position).normalized * pushForceMagnitude;
                collider2D.gameObject.GetComponent<CharacterControls>().ApplyForce(force);
                collider2D.gameObject.GetComponent<PlayableCharacter>().ChangeHP(-damageAmount);
            }
        }
    }

    
    void OnTriggerExit2D(Collider2D other)
    {
        switch(other.tag){
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
