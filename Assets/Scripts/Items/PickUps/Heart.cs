using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FunkyCode;

public class Heart : MonoBehaviour, IPickup
{
    public bool Pickupable{
        protected set;
        get;
    } = false;

    public Pickup_Type PickupType{
        protected set;
        get;
    }

    [SerializeField] AudioClip pickUpSound;
    Sequence heartFallSequence;
    [SerializeField] bool lightOn = true;
    [SerializeField] float maxLightBrightness = .5f;
    [SerializeField] LightSprite2D raysLight;
    [SerializeField] int healAmount = 10;
    public int HealAmount{
        get {return healAmount;}
    }
 
    // Start is called before the first frame update
    void Start()
    {
        InitializeFallAppearAnim();
        PickupType = Pickup_Type.Heart;
        
        if(Pickupable){
            Activate();
        }
    }

    // Update is called once per frame
    void Update()
    {
    
    }
    void FixedUpdate()
    {
        RotateLight();
    }

    void RotateLight(){
        if(!lightOn){
            return;
        }

        raysLight.lightSpriteTransform.rotation += 1f;
    }

    public void Activate(bool specialBehavior = true){
        Pickupable = true;
        gameObject.SetActive(true);

        if(specialBehavior){
            heartFallSequence.Restart();
        }

        if(lightOn){
            raysLight.color.a = maxLightBrightness / 2f;
            DOTween.To(()=> raysLight.color.a, x=> raysLight.color.a = x, maxLightBrightness, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        } else {
            raysLight.enabled = false;
        }
    }

    public bool Pickup(){
        if(Pickupable){
            Deactivate();

            GameMaster.Instance.audioSource.PlayOneShot(pickUpSound);
            return true;
        }

        return false;
    }

    void Deactivate(bool destroy = true){
        gameObject.SetActive(false);
        Pickupable = false;

        if(destroy){
            DOVirtual.DelayedCall(.25f, DestroyObject);
        }
    }

    void InitializeFallAppearAnim(){
        Vector3 shakeStrengthVector = new Vector3(.075f, 0f, 0f);
        Vector3 originalPosition = transform.position;
        transform.position = originalPosition + new Vector3(0f, .35f, 0f);
        
        // transform.DOShakePosition(1f);
        float heartFallTime = .75f;
        heartFallSequence = DOTween.Sequence()
            .Append(transform.DOShakePosition(heartFallTime, shakeStrengthVector, 5, 0f).SetEase(Ease.OutQuad))
            .Join(transform.DOMoveY(originalPosition.y, heartFallTime).SetEase(Ease.OutQuad))
            .OnUpdate(()=>{
                    // Debug.Log("Updating heart fall sequence");
                })
            .OnComplete(()=>{
                    // openable = true;
                    // Interactable = true;
                })
            .Pause();
    }

    void DestroyObject(){
        GameObject.Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {

    }
}
