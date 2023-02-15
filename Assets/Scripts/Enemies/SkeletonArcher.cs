using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkeletonArcher : Enemy
{
    public bool IsShooting{
        protected set; get;
    } = false;
    Vector2 directionToPlayer;
    
    [Header("SkeletonArcher")]
    [SerializeField] float shootTime = 3f;
    [SerializeField] bool isStationary = false;
    float shootDT = 0f;
    float shootRandomCheckTime = 1f;
    float shootRandomCheckDT = 0f;
    [SerializeField] GameObject objectToAttack;
    const float BASE_ANIMATION_LENGTH = 1f;
    float animationLengthTime;
    float animationDT;
    float shootingAnimAngleOffset = 0f;
    bool shootingOffsetSet = false;
    bool hasShot = false;
    [SerializeField] Arrow arrowPrefab;
    Arrow[] arrows;
    [SerializeField] int numberOfArrows = 4;
    int arrowIndex = 0;
    [SerializeField] GameObject arrowStartingPositions;
    Vector2 arrowStartingPosition;
    [SerializeField] float arrowSpeed = 10f;
    [SerializeField] AudioClip arrowSound;
    [SerializeField] RadialProximityChecker radialProximityChecker;
    bool dodging = false;
    float dodgeTime = .5f;
    float dodgeDT = 0f;
    Collider2D colliderToDodge;
    [SerializeField] float dodgeForceMagnitude = 5f;
    [Range(0, .3f)] [SerializeField] protected float m_MovementSmoothing = .05f;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        InitializeArrows();
        Random.seed = System.Environment.TickCount;
        Renderer renderer = GetComponent<Renderer>();
        Material material = renderer.material;
        // Debug.Log("material 'Lit': " + material.GetFloat("_Lit"));

        material.SetFloat("_Lit", .85f);
        // GetComponent<Material>().SetFloat("Lit", .975f);
    }

    void InitializeArrows(){
        arrows = new Arrow[numberOfArrows];
        for(int i = 0; i < arrows.Length; i++){
            arrows[i] = Instantiate(arrowPrefab);
            arrows[i].gameObject.layer = LayerMask.NameToLayer("Enemy Projectiles");
            arrows[i].tag = "EnemyProjectile";
            arrows[i].transform.localScale = new Vector3(2f, 2f, 1f);
            arrows[i].SetColliderToIgnore(collider2D);
            arrows[i].Activate(false);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        UpdateShootTime();
        UpdateShoot();
        UpdateRadialProximityChecker();
        UpdateDodge();
        UpdateStationaryPosition();
    }

    void UpdateShoot(){
        if(!IsShooting || !Alive){
            return;
        }
        
        if(!shootingOffsetSet && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Substring(0, 5) == "Shoot"){
            // Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            float angleToPlayer = Mathf.Rad2Deg * Mathf.Atan2(directionToPlayer.y, directionToPlayer.x);

            if(angleToPlayer < 0){
                angleToPlayer += 360;
            }

            if(angleToPlayer >= 315 || angleToPlayer < 45){
                shootingAnimAngleOffset = 0f;
                arrowStartingPosition = arrowStartingPositions.transform.Find("ShootRightPosition").transform.position;
            } else if(angleToPlayer >= 45 && angleToPlayer < 135){
                shootingAnimAngleOffset = 90f;
                arrowStartingPosition = arrowStartingPositions.transform.Find("ShootUpPosition").transform.position;
            } else if(angleToPlayer >= 135 && angleToPlayer < 225){
                shootingAnimAngleOffset = 180f;
                arrowStartingPosition = arrowStartingPositions.transform.Find("ShootLeftPosition").transform.position;
            } else {
                shootingAnimAngleOffset = 270f;
                arrowStartingPosition = arrowStartingPositions.transform.Find("ShootDownPosition").transform.position;
            }

            shootingOffsetSet = true;
        }

        if(shootingOffsetSet){
            directionToPlayer = (objectToAttack.transform.position - transform.position).normalized;
            float angleToPlayer = Mathf.Rad2Deg * Mathf.Atan2(directionToPlayer.y, directionToPlayer.x);
            transform.eulerAngles = new Vector3(0f, 0f, angleToPlayer - shootingAnimAngleOffset);
            HPBar.transform.eulerAngles = new Vector3(0f, 0f, -transform.eulerAngles.z);
        }

        // Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        // animationLengthTime = animator.GetCurrentAnimatorStateInfo(0).length;
        // Debug.Log($"Current anim length: {animationLengthTime}");

        animationDT += Time.deltaTime;
        // Debug.Log($"Current anim length: {animator.GetCurrentAnimatorStateInfo(0).length}");

        if(animationDT >= animationLengthTime * .9f && !hasShot){
            float angleToPlayer = Mathf.Rad2Deg * Mathf.Atan2(directionToPlayer.y, directionToPlayer.x);

            if(angleToPlayer < 0){
                angleToPlayer += 360;
            }

            if(angleToPlayer >= 315 || angleToPlayer < 45){
                shootingAnimAngleOffset = 0f;
                arrowStartingPosition = arrowStartingPositions.transform.Find("ShootRightPosition").transform.position;
            } else if(angleToPlayer >= 45 && angleToPlayer < 135){
                shootingAnimAngleOffset = 90f;
                arrowStartingPosition = arrowStartingPositions.transform.Find("ShootUpPosition").transform.position;
            } else if(angleToPlayer >= 135 && angleToPlayer < 225){
                shootingAnimAngleOffset = 180f;
                arrowStartingPosition = arrowStartingPositions.transform.Find("ShootLeftPosition").transform.position;
            } else {
                shootingAnimAngleOffset = 270f;
                arrowStartingPosition = arrowStartingPositions.transform.Find("ShootDownPosition").transform.position;
            }

            // arrows[arrowIndex].Shoot(arrowStartingPosition, shootingAnimAngleOffset + transform.eulerAngles.z, directionToPlayer.normalized * arrowSpeed, 10, FloorLevel);
            arrows[arrowIndex].Shoot(arrowStartingPosition, angleToPlayer, directionToPlayer.normalized * arrowSpeed, 10, FloorLevel);
            if(++arrowIndex >= arrows.Length){
                arrowIndex = 0;
            }

            GameMaster.Instance.audioSource.PlayOneShot(arrowSound, GameMaster.Instance.MasterVolume);
            hasShot = true;

            
        }

        if(animationDT >= animationLengthTime){
            IsShooting = false;
            animator.SetBool("IsShooting", IsShooting);

            if(followScript != null){
                suspendEngageUpdate = false;
                followScript.shouldFollow = shouldFollowTarget;
            }
            animationDT = 0f;
            transform.eulerAngles = Vector3.zero;
            HPBar.transform.eulerAngles = Vector3.zero;
        }

        // directionToPlayer = (objectToAttack.transform.position - transform.position).normalized;
        // animator.SetFloat("Horizontal", directionToPlayer.x);
        // animator.SetFloat("Vertical", directionToPlayer.y);
    }

    void UpdateShootTime(){
        if(IsShooting || !Alive || dodging){
            return;
        }

        shootRandomCheckDT += Time.deltaTime;
        shootDT += Time.deltaTime;

        if(shootRandomCheckDT >= shootRandomCheckTime){
            float randNum = Random.Range(0, 1f);

            if(randNum <= (shootDT / shootTime) && HasLOS()){
                Shoot();

                shootDT = 0f;
            }

            shootRandomCheckDT = 0f;
        }
    }

    protected void UpdateRadialProximityChecker(){
        if(!Alive || dodging || IsShooting || isStationary){
            return;
        }

        if(radialProximityChecker.CollidersToDodge.Count > 0){
            dodging = true;
            if(followScript != null){
                suspendEngageUpdate = true;
                followScript.shouldFollow = shouldFollowTarget;
            }

            colliderToDodge = radialProximityChecker.CollidersToDodge[0];
            float objectVelocityAngle = Mathf.Atan2(colliderToDodge.attachedRigidbody.velocity.normalized.y, colliderToDodge.attachedRigidbody.velocity.normalized.x);
            Vector2 positionDifference = transform.position - colliderToDodge.gameObject.transform.position;
            float positionDifferenceAngle =  Mathf.Atan2(positionDifference.normalized.y, positionDifference.normalized.x);
            Vector2 perpindicularDodgeForce;
            float forceAngleOffset = Mathf.PI /2f;
            if(positionDifferenceAngle - objectVelocityAngle < 0f){
                forceAngleOffset *= -1f;
            }

            perpindicularDodgeForce = new Vector2(Mathf.Cos(positionDifferenceAngle + forceAngleOffset), Mathf.Sin(positionDifferenceAngle + forceAngleOffset)) * dodgeForceMagnitude;
            m_Rigidbody2D.AddForce(perpindicularDodgeForce, ForceMode2D.Impulse);

            radialProximityChecker.Reset();
        }
    }

    protected void UpdateDodge(){
        if(!dodging || !Alive){
            return;
        }

        dodgeDT += Time.deltaTime;

        if(dodgeDT >= dodgeTime){
            dodging = false;
            suspendEngageUpdate = false;
            dodgeDT = 0f;
        }
    }

    protected override void UpdateWalkAnimation()
    {
        if(IsShooting){
            return;
        }

        base.UpdateWalkAnimation();
    }

    protected virtual void UpdateStationaryPosition(){
        if(!isStationary){
            return;
        }

        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, Vector2.zero, ref m_Velocity, m_MovementSmoothing);
    }

    void Shoot(){
        IsShooting = true;
        if(!isStationary){
            m_Rigidbody2D.velocity = Vector2.zero;
        }

        animator.SetBool("IsShooting", IsShooting);
        animator.SetFloat("Speed", 0f);
        animator.SetFloat("Horizontal", directionToPlayer.x);
        animator.SetFloat("Vertical", directionToPlayer.y);
        animator.speed = 1.25f;

         if(followScript != null){
            suspendEngageUpdate = true;
            followScript.shouldFollow = false;
        }
        shootingOffsetSet = false;
        hasShot = false;

        Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        animationLengthTime = (1f/animator.speed) * BASE_ANIMATION_LENGTH;
        Debug.Log($"Current anim length: {animationLengthTime}");
        // animationLengthTime = animator
    }

    bool HasLOS(){
        RaycastHit2D[] raycastResults = new RaycastHit2D[1];
        Vector2 raycastDirection;
        string[] layersToCheck = new string[]{"Player", "Default", "Enemy"};
        LayerMask layerMask  = LayerMask.GetMask(layersToCheck);

        raycastDirection = (objectToAttack.transform.position - transform.position).normalized;
        collider2D.Raycast(raycastDirection, raycastResults, Mathf.Infinity, layerMask);
        RaycastHit2D raycastObject = raycastResults[0];

        directionToPlayer = raycastDirection;

        return raycastObject.rigidbody != null && raycastObject.rigidbody.tag == "Player"
            && (transform.position - raycastObject.transform.position).magnitude <= engagementRange
            && FloorLevel == objectToAttack.GetComponent<CharacterControls>().FloorLevel;
    }

    public override void ChangeHP(int deltaHP)
    {
        base.ChangeHP(deltaHP);
        if(deltaHP < 0){
            Engaged = true;
            engagedDT = 0;
        }

        // Debug.Log($"Taking Damage: {hp}");
        if(hp <= 0){
            Die();
        }
    }
    protected override void Die(){
        base.Die();

        float velocityAngle = Mathf.Rad2Deg * Mathf.Atan2(GetComponent<Rigidbody2D>().velocity.y, GetComponent<Rigidbody2D>().velocity.x);
        if(velocityAngle < 0){
            velocityAngle += 360f;
        }

        velocityAngle -= 270;

        // Debug.Log(transform.eulerAngles);
        transform.eulerAngles = new Vector3(0, 0, velocityAngle);
        // HPBar.transform.Rotate(0, 0, -velocityAngle);
        // transform.Rotate(0, 0, velocityAngle);
        // Debug.Log(velocityAngle);
        
        animator.speed = 1f;
        animator.SetFloat("Speed", 0);
        animator.SetBool("IsShooting", false);
        animator.SetBool("IsDead", true);

        DieAnimation();
    }

    public override void Revive(bool setOriginPosition = true){
        base.Revive(setOriginPosition);

        animator.SetBool("IsDead", false);
    }

    private void DieAnimation(){
        DOVirtual.DelayedCall(dieAnimationTime, Deactivate);
    }

    public void Deactivate(){
        EnableRenderer(false);
    }
}
