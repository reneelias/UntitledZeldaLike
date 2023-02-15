using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FunkyCode;

public class Skull : A_ItemDropper, IBreakable, I_Respawnable
{
    public bool Broken{
        protected set; get;
    }
    public bool Breakable{
        protected set; get;
    } = true;
    public Breakable_Type BreakableType{
        protected set; get;
    }
    Vector3 originPosition;
    Vector3 originalScale;
    Vector3 originalRotation;
    bool falling = false;
    public bool Falling{
        get{ return falling; }
    }
    [SerializeField] AudioClip breakSound;
    [SerializeField] AudioClip fallSound;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject leftEye;
    [SerializeField] GameObject rightEye;
    [SerializeField] Collider2D collider;
    [SerializeField] Rigidbody2D rigidbody;
    protected bool beingMovedByTile = false;
    protected MovementTile currentMovementTile;
    float originalDrag;
    [Tooltip("MovementTile Drag Modifier")]
    [SerializeField] float dragModifier_MT = 10;
    [SerializeField] Collider2D[] collidersToIgnore;
    [SerializeField] Collider2D groundCheckHitbox;
    [SerializeField] bool immediateRespawn = false;
    [SerializeField] float immediateRespawnDelay = 1f;
    float immedateRespawnDT = 0f;
    bool immediateRespawningActive = false;
    [SerializeField] LightCollider2D lightCollider2D;
    [SerializeField] float originalEyeLightsSize = .59f;
    [SerializeField] float fallDuration = .5f;
    public bool Active{
        get;
        protected set;
    }
    
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        Initialize();
    }

    void Start()
    {

    }

    void Initialize(){
        originPosition = transform.position;
        originalScale = transform.localScale;
        originalDrag = rigidbody.drag;
        originalRotation = transform.localEulerAngles;
        Active = true;
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        foreach(Collider2D other in collidersToIgnore){
            Physics2D.IgnoreCollision(collider, other);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateImmediateRespawn();   
    }

    void UpdateImmediateRespawn(){
        if(!immediateRespawningActive){
            return;
        }

        immedateRespawnDT += Time.deltaTime;
        if(immedateRespawnDT >= immediateRespawnDelay){
            immediateRespawningActive = false;
            Respawn();
        }
    }

    public void Respawn(){
        gameObject.SetActive(true);
        Broken = false;
        Breakable = true;

        transform.position = originPosition;
        transform.localScale = originalScale;
        transform.localEulerAngles = originalRotation;
        leftEye.SetActive(true);
        rightEye.SetActive(true);
        spriteRenderer.enabled = true;
        rigidbody.simulated = true;
        beingMovedByTile = false;
        lightCollider2D.enabled = true;
        leftEye.GetComponent<Light2D>().size = originalEyeLightsSize;
        rightEye.GetComponent<Light2D>().size = originalEyeLightsSize;
        Active = true;
    }

    public bool Break(){
        if(!Breakable){
            return false;
        }

        Broken = true;
        Breakable = false;
        Active = false;
        GameMaster.Instance.audioSource.PlayOneShot(breakSound, GameMaster.Instance.MasterVolume);

        if(immediateRespawn){
            immediateRespawningActive = true;
            immedateRespawnDT = 0f;
            leftEye.SetActive(false);
            rightEye.SetActive(false);
            spriteRenderer.enabled = false;
            rigidbody.velocity = Vector2.zero;
            rigidbody.angularVelocity = 0f;
            rigidbody.simulated = false;
            lightCollider2D.enabled = false;
        } else {
            gameObject.SetActive(false);
        }

        DropItem();
        return true;    
    }

    protected virtual void InitiateFall(){
        if(falling){
            return;
        }

        Breakable = false;
        falling = true;
        GameMaster.Instance.audioSource.PlayOneShot(fallSound, GameMaster.Instance.MasterVolume);

        transform.DOScale(0f, fallDuration)
            .OnUpdate(()=>{transform.localEulerAngles += new Vector3(0f, 0f, 1f);})
            .OnComplete(()=>{
                
                falling = false;
                Active = false;
                if(immediateRespawn){
                    immediateRespawningActive = true;
                    immedateRespawnDT = 0f;
                    rigidbody.velocity = Vector2.zero;
                    rigidbody.angularVelocity = 0f;
                    rigidbody.simulated = false;
                    lightCollider2D.enabled = false;
                } else{
                    transform.localScale = originalScale;
                    transform.localEulerAngles = originalRotation;
                    gameObject.SetActive(false);
                }
        });

        DOTween.To(()=> leftEye.GetComponent<Light2D>().size, x => leftEye.GetComponent<Light2D>().size = x, 0, fallDuration);
        DOTween.To(()=> rightEye.GetComponent<Light2D>().size, x => rightEye.GetComponent<Light2D>().size = x, 0, fallDuration);
        // leftEye.GetComponent<Light2D>()
        // rightEye.GetComponent<Light2D>().transform.DOScale(Vector3.zero, fallDuration);
    }

    public virtual void ApplyForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Impulse){

        // Debug.Log($"Applying force to character, force strength: {force.magnitude}");
        // Debug.Log($"Character velocity before force: {m_Rigidbody2D.velocity.magnitude}");
        rigidbody.AddForce(force, forceMode);
        // Debug.Log($"Character velocity after force: {m_Rigidbody2D.velocity.magnitude}");

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

        rigidbody.drag = dragModifier_MT;
        ApplyForce(force, ForceMode2D.Force);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(Broken){
            return;
        }

        LayerMask groundLayerMask = LayerMask.GetMask(new string[]{"Ground"});

        switch(other.tag){
            case "BelowGround":
                // InitiateFall();
                // if(!collider.IsTouchingLayers(groundLayerMask)){
                if(!groundCheckHitbox.IsTouchingLayers(groundLayerMask)){
                    InitiateFall();
                }
                break;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(Broken){
            return;
        }

        LayerMask groundLayerMask = LayerMask.GetMask(new string[]{"Ground"});

        switch(other.tag){
            case "BelowGround":
                // InitiateFall();
                // if(!collider.IsTouchingLayers(groundLayerMask)){
                if(!groundCheckHitbox.IsTouchingLayers(groundLayerMask)){
                    InitiateFall();
                    MechanicalHatch mechanicalHatch = other.gameObject.transform.parent.gameObject.GetComponent<MechanicalHatch>();

                    if(mechanicalHatch == null){
                        break;
                    }

                    mechanicalHatch.UnlockObjects();
                }
                break;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(Broken){
            return;
        }

        switch(other.tag){
            case "MovementTile":
                MovementTile movementTile = other.gameObject.GetComponent<MovementTile>();
                if(movementTile.Equals(currentMovementTile)){
                    currentMovementTile = null;
                    beingMovedByTile = false;
                }
                rigidbody.drag = originalDrag;
                break;
        } 
    }
}
