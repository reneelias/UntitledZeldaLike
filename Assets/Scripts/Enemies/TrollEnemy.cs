using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FunkyCode;

public class TrollEnemy : Enemy
{
    Tween idleScaleTween;
    // Start is called before the first frame update
    
    [Header("Troll")]

    public bool canCharge = true;
    [SerializeField] float chargeStartTime;
    float chargeDT;
    int prevChargeDT;
    float chargeProbability;
    bool charging;
    [SerializeField] float chargingTime;
    float chargingDT;
    Vector2 chargingVelocity;
    [SerializeField] float chargeSpeed;
    float postChargePauseDT;
    [SerializeField] float postChargePauseTime;
    bool postChargePausing;
    bool collidedOnCharge;
    [SerializeField] protected float postChargeDrag = 75f;
    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip chargeRoar;
    [SerializeField] AudioClip chargeStep;
    [SerializeField] float volume = 1.25f;
    [SerializeField] float chargeStepPlaybackSpeed = 1.25f;

    protected override void Start()
    {
        base.Start();

        // idleScaleTween = transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetDelay(Random.Range(0f, .5f));
        
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.Find("WizardBoxCollider").GetComponent<Collider2D>());
    
        charging = false;
        chargingDT = 0f;
        chargeDT = 0f;
        prevChargeDT = 0;
        postChargePauseDT = 0f;
        postChargePausing = false;
        collidedOnCharge = false;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.pitch = chargeStepPlaybackSpeed;
        audioSource.Stop();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateCharge();

        // Debug.Log($"Curr Velocity: {m_Rigidbody2D.velocity}");
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
        animator.SetBool("IsDead", true);
        audioSource.Stop();

        DieAnimation();
    }

    private void DieAnimation(){
        // float targetY = transform.position.y - .25f;
        // Tween yTween = transform.DOMoveY(targetY, .5f).SetEase(Ease.OutBounce);
        // transform.DORotate(new Vector3(0, 0, -80), .35f);



        // yTween.OnComplete();
        DOVirtual.DelayedCall(dieAnimationTime, Deactivate);
        // transform.DOScaleY(1.475f, .275f)
    }

    public void Deactivate(){
        EnableRenderer(false);
        // HPBar.active = false;
        // HPBar.GetComponent<Renderer>().enabled = false;
        // GetComponent<Rigidbody2D>().Sleep();
    }

    protected override void UpdateWalkAnimation()
    {
        if(postChargePausing){
            return;
        }

        base.UpdateWalkAnimation();
    }

    private void SetCharge(bool chargeStatus)
    {
        if(chargeStatus){
            suspendEngageUpdate = true;
            chargingDT = 0f;
            chargeDT = 0f;
            followScript.shouldFollow = false;
            chargingVelocity = (Vector2)(followScript.objectToFollow.transform.position - transform.position).normalized * chargeSpeed;
            GameMaster.Instance.audioSource.PlayOneShot(chargeRoar, volume * GameMaster.Instance.MasterVolume);
            audioSource.volume = volume  * GameMaster.Instance.MasterVolume;
            audioSource.Play();
            // Debug.Log("Charging");
        } else {
            if(Engaged){
                postChargePausing = true;
                postChargePauseDT = 0f;
                m_Rigidbody2D.velocity = Vector2.zero;
            } else {
                suspendEngageUpdate = false;
            }
            audioSource.Stop();
        }

        charging = chargeStatus;
    }

    private void UpdateCharge(){
        if(!Engaged || !canCharge || !Alive){
            return;
        }

        if(postChargePausing){
            
            // animator.SetBool("AnimateDown", false);
            // animator.SetBool("AnimateUp", false);
            // animator.SetBool("AnimateLeft", false);
            // animator.SetBool("AnimateRight", false);
            animator.speed = 0f;
            
            if(!collidedOnCharge){
                m_Rigidbody2D.velocity = Vector2.zero;
            }

            postChargePauseDT += Time.deltaTime;
            if(postChargePauseDT >= postChargePauseTime){
                postChargePausing = false;
                suspendEngageUpdate = false;
                m_Rigidbody2D.drag = 0f;
            }
            return;
        }

        if(charging){
            m_Rigidbody2D.velocity = chargingVelocity;

            chargingDT += Time.deltaTime;

            if(chargingDT >= chargingTime){
                SetCharge(false);
                collidedOnCharge = false;
            }
        } else {
            chargeDT += Time.deltaTime;
            if((int)chargeDT > prevChargeDT){
                prevChargeDT = (int)chargeDT;
                chargeProbability = chargeDT / chargeStartTime;
                float randValue = Random.value;
                    // Debug.Log($"Random Value: {randValue}");

                if(chargeProbability >= randValue){
                    if(followScript.hasLOS){
                        SetCharge(true);
                        prevChargeDT = 0;
                        // chargeDT = 0f;
                    } else {
                        chargeDT = 0f;
                    }

                    // Debug.Log($"Charge Probability: {chargeProbability}, Random Value: {randValue}");
                }
            }
        }
    }

    public override void Revive(bool setOriginPosition = true)
    {
        base.Revive(setOriginPosition);

        Engaged = false;
        SetCharge(false);
        postChargePausing = false;
    }

    protected override void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "PlayerProjectile"){
            Debug.Log("Collision with Player Projectile");
            return;
        }

        base.OnCollisionEnter2D(collision);

        if(charging){
            SetCharge(false);
            // m_Rigidbody2D.AddForce(chargingVelocity * -.25f, ForceMode2D.Impulse);
            m_Rigidbody2D.AddForce(chargingVelocity.normalized * -ShieldHitKnockForceMagnitude, ForceMode2D.Impulse);
            m_Rigidbody2D.drag = postChargeDrag;
            collidedOnCharge = true;
            // Debug.Log("Collided on charge.");
        }
    }
    protected override void OnCollisionStay2D(Collision2D collision){
        base.OnCollisionStay2D(collision);
    }

    public override void ResetOnRoomLeave()
    {
        chargeDT = 0f;
        charging = false;
        
        base.ResetOnRoomLeave();
    }
}
