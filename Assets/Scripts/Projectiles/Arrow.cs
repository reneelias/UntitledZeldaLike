using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public bool Active{
        protected set; get;
    } = false;
    Vector2 velocity;
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] Collider2D collider2D;
    int damageAmount;
    float deactivateTime = .5f;
    float deactivateDT = 0f;
    bool tickDeactivationTimer = false;
    bool hitSurface = false;
    public bool shouldAttachToTargets = true;
    GameObject attachedBody;
    Vector3 relativePositionToAttachedBody;
    [SerializeField] List<string> nonCollidingTags;
    [SerializeField] int floorLevel = 1;
    public int FloorLevel{
        get => floorLevel;
        protected set => floorLevel = value;
    }
    protected bool hitShield = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetColliderToIgnore(Collider2D other){
        Physics2D.IgnoreCollision(collider2D, other);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        UpdateShot();
        UpdateDeactivateTimer();
        UpdateAttached();
    }

    void UpdateShot(){
        if(!Active){
            return;
        }

        rigidbody2D.velocity = velocity;
    }

    void UpdateAttached(){
        if(!shouldAttachToTargets || !hitSurface || !Active){
            return;
        }

        transform.position = attachedBody.transform.position - relativePositionToAttachedBody;
    }

    void UpdateDeactivateTimer(){
        if(!tickDeactivationTimer){
            return;
        }

        deactivateDT += Time.deltaTime;

        if(deactivateDT >= deactivateTime){
            Activate(false);
        }
    }

    public void Shoot(Vector2 startingPosition, float rotationAngle, Vector2 velocity, int damageAmount, int floorLevel){
        Activate(true);

        transform.position = startingPosition;
        transform.eulerAngles = new Vector3(0f, 0f, rotationAngle);
        this.velocity = velocity;
        this.damageAmount = damageAmount;
        hitSurface = false;
        tickDeactivationTimer = false;
        deactivateDT = 0f;
        FloorLevel = floorLevel;
        hitShield = false;
    }

    public void Activate(bool activate){
        Active = activate;
        EnableRenderer(activate);
    }

    public void EnableRenderer(bool enable){
        GetComponent<Renderer>().enabled = enable;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(hitSurface || !Active){
            return;
        }

        if(other.tag == "Shield"){
            hitShield = true;
            ControlsManager.Instance.PlayControllerHaptics(.1f, .3f, .1f);
        } else if(other.tag == "Player"){
            Collider2D[] collider2Ds = new Collider2D[10];
            collider2D.OverlapCollider(new ContactFilter2D().NoFilter(), collider2Ds);
            for(int i = 0; i < collider2Ds.Length; i++){
                Collider2D currentCollider2D = collider2Ds[i];
                if(currentCollider2D == null){
                    break;
                }

                // Debug.Log($"Arrow hit: {currentCollider2D.name}");

                if(currentCollider2D.tag == "Shield"){
                    hitShield = true;
                    ControlsManager.Instance.PlayControllerHaptics(.1f, .3f, .1f);
                    // Debug.Log("Arrow hit shield");
                }
            }
            if(other.gameObject.GetComponent<CharacterControls>().FloorLevel == floorLevel && !hitShield){
                other.GetComponent<PlayableCharacter>().ChangeHP(-damageAmount);
            }
        } else if(other.tag == "Enemy"){
            other.GetComponent<Enemy>().ChangeHP(-damageAmount);
        }
        
        if(!nonCollidingTags.Contains(other.tag)){
            velocity = Vector2.zero;
            tickDeactivationTimer = true;
            hitSurface = true;

            attachedBody = other.gameObject;
            relativePositionToAttachedBody = attachedBody.transform.position - transform.position;
        }
    }
}
