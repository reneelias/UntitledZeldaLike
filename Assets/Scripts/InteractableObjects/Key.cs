using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Key : MonoBehaviour, IUnlocklableObject, I_DiscoverableItem
{
    [SerializeField] bool unlocked = false;
    [SerializeField] AudioClip unlockSound;
    [SerializeField] AudioClip pickupSound;
    // Start is called before the first frame update
    Sequence bounceSequence;
    [SerializeField] bool grabbable = false;
    [SerializeField] GameObject[] objectsToActivate;
    [SerializeField] Rigidbody2D rigidbody;
    protected bool beingMovedByTile = false;
    protected MovementTile currentMovementTile;
    bool falling = false;
    public bool Falling{
        get{ return falling; }
    }
    [SerializeField] AudioClip fallSound;
    [SerializeField] Collider2D collider;
    Vector3 originalScale;
    Vector3 originPosition;
    bool pickedUp = false;
    [SerializeField] bool resets = false;
    [SerializeField] Collider2D smallerCollider;
    [SerializeField] bool isMasterKey = false;
    [TextArea]
    [SerializeField] string[] discoverMessages;
    public string[] DiscoverMessages{
        protected set {discoverMessages = value;}
        get { return discoverMessages;}
    }
    [SerializeField] float fontScaling = 1f;
    public float FontScaling{
        protected set{fontScaling = value;}
        get {return fontScaling;}
    }
    [SerializeField] GameObject spriteObject;
    public GameObject SpriteObject{
        protected set {spriteObject = value;}
        get {return spriteObject;}
    }
    [SerializeField] ParticleEmitter particleEmitter;

    void Start()
    {
        originalScale = transform.localScale;
        originPosition = transform.position;

        // InitializeBounceAnim();
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.Find("WizardBoxCollider").GetComponent<Collider2D>());

        if(!unlocked && !isMasterKey){
            gameObject.SetActive(false);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    // private void OnTriggerEnter2D(Collider2D collider2D){
    //     if(collider2D.tag == "Player" && grabbable){
    //         GameMaster.Instance.dungeon.ModifyKeyCount(1);
    //         // GameMaster.Instance.audioSource.PlayOneShot()
    //         gameObject.SetActive(false);
    //     }
    // }
    public void Unlock(){
        if(pickedUp || unlocked){
            return;
        }
        gameObject.SetActive(true);
        unlocked = true;
        GameMaster.Instance.audioSource.PlayOneShot(unlockSound);

        // float originalScale = transform.localScale.x;
        // transform.localScale = new Vector3(originalScale * 2f, originalScale * 2f, 1f);
        // transform.DOScale(new Vector3(originalScale, originalScale, 1f), .5f);
        // float originalScale = transform.localScale.x;
        transform.localScale = new Vector3(originalScale.x * 3f, originalScale.y * 3f, 1f);
        InitializeBounceAnim();
        bounceSequence.Restart();
        // bounceSequence.OnComplete(()=>{Debug.Log("bouncing complete!");});
    }

    void InitializeBounceAnim(){
        // float originalScale = transform.localScale.x;

        bounceSequence = DOTween.Sequence()
            .Append(transform.DOScale(new Vector3(originalScale.x, originalScale.y, 1f), .25f))
            .Append(transform.DOScale(new Vector3(originalScale.x * 1.25f, originalScale.y * 1.25f, 1f), .1f).SetLoops(1, LoopType.Yoyo))
            .Append(transform.DOScale(new Vector3(originalScale.x * 1.15f, originalScale.y * 1.15f, 1f), .045f).SetLoops(1, LoopType.Yoyo))
            .Append(transform.DOScale(new Vector3(originalScale.x * 1.075f, originalScale.y * 1.075f, 1f), .025f).SetLoops(1, LoopType.Yoyo))
            .OnComplete(()=>{grabbable = true;})
            .Pause();
    }

    protected virtual void InitiateFall(){
        if(falling){
            return;
        }

        grabbable = false;
        falling = true;
        GameMaster.Instance.audioSource.PlayOneShot(fallSound);

        transform.DOScale(0f, .5f)
            .OnUpdate(()=>{transform.localEulerAngles += new Vector3(0f, 0f, 1f);})
            .OnComplete(()=>{
                transform.localScale = originalScale;
                transform.localEulerAngles = Vector3.zero;
                transform.position = originPosition;
                
                falling = false;
                gameObject.SetActive(false);
                if(resets){
                    unlocked = false;
                }
        });
    }

    public virtual void ApplyForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Impulse){

        // Debug.Log($"Applying force to character, force strength: {force.magnitude}");
        // Debug.Log($"Character velocity before force: {m_Rigidbody2D.velocity.magnitude}");
        rigidbody.AddForce(force, forceMode);
        // Debug.Log($"Character velocity after force: {m_Rigidbody2D.velocity.magnitude}");

    }

    public virtual void ApplyForceFromMovementTile(Vector2 force, MovementTile movementTile){
        if(beingMovedByTile && !movementTile.Equals(currentMovementTile) && currentMovementTile.Activated){
            return;
        }
        // Debug.Log($"Force being applied by {movementTile.name}");

        if(!beingMovedByTile || (beingMovedByTile && !currentMovementTile.Activated)){
            currentMovementTile = movementTile;
            beingMovedByTile = true;
        }

        ApplyForce(force, ForceMode2D.Force);
    }
    
    public void Discover(){
        gameObject.SetActive(true);
        GameMaster.Instance.dungeon.ModifyKeyCount(1, isMasterKey);
        GetComponent<SortingOrderByY>().enabled = false;
        // GameMaster.Instance.audioSource.PlayOneShot(pickupSound);
        pickedUp = true;
    }

    public void Finish(){
        gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other){
        if(pickedUp){
            return;
        }
        
        switch(other.tag){
            case "BelowGround":
                LayerMask groundLayerMask = LayerMask.GetMask(new string[]{"Ground"});
                // InitiateFall();
                if(!collider.IsTouchingLayers(groundLayerMask)){
                    InitiateFall();
                }
                break;

            case "PlayerBoxCollider":
                if(!grabbable){
                    break;
                }
            
                GameMaster.Instance.dungeon.ModifyKeyCount(1, isMasterKey);
                GameMaster.Instance.audioSource.PlayOneShot(pickupSound);
                pickedUp = true;
                gameObject.SetActive(false);
                
                if(objectsToActivate != null){
                    foreach(GameObject objectToActivate in objectsToActivate){
                    objectToActivate.GetComponent<ISwitchable>().ActivatePermanently = true;
                }
            }
                break;
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
