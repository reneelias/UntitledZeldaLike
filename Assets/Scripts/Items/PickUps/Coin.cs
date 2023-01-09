using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FunkyCode;

public class Coin : MonoBehaviour, IPickup, I_DiscoverableItem
{
    [SerializeField] int value = 1;
    [SerializeField] CoinValue coinValue = CoinValue.One;
    [SerializeField] Color[] coinColors;
    [SerializeField] AudioClip pickUpSound;
    [SerializeField] bool lightOn = true;
    [SerializeField] float maxLightBrightness = .5f;
    [SerializeField] LightSprite2D raysLight;
    Vector3 originPosition;
    Sequence bounceSequence;
    [SerializeField] bool pickupable = false;
    public bool Pickupable{
        protected set; get;
    }
    public Pickup_Type PickupType{
        protected set; get;
    }
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
    [SerializeField] SpriteRenderer spriteRenderer;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        Initialize();
        PickupType = Pickup_Type.Coin;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeBounceAnim();

        // Pickupable = pickupable;
        if(Pickupable){
            Activate();
        }

    }

    void Initialize(){
        value = (int)coinValue;
        int colorIndex = 0;

        switch(coinValue){
            case CoinValue.One:
                break;
            case CoinValue.Five:
                colorIndex = 1;
                break;
            case CoinValue.Twenty:
                colorIndex = 2;
                break;
            case CoinValue.OneHundred:
                colorIndex = 3;
                break;
        }

        spriteRenderer.color = coinColors[colorIndex];

        raysLight.color = coinColors[colorIndex];
        raysLight.color.a = .5f;

        // discoverMessages = $"You found a turtle coin! This has a value of {value}!";
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
            bounceSequence.Restart();
        }
        
        if(lightOn){
            raysLight.color.a = maxLightBrightness / 2f;
            DOTween.To(()=> raysLight.color.a, x=> raysLight.color.a = x, maxLightBrightness, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        } else {
            raysLight.enabled = false;
        }
    }

    void Deactivate(bool destroy = true){
        gameObject.SetActive(false);
        Pickupable = false;

        if(destroy){
            DOVirtual.DelayedCall(.25f, DestroyObject);
        }
    }    

    void DestroyObject(){
        GameObject.Destroy(gameObject);
    }

    public bool Pickup(){
        if(Pickupable){
            Deactivate();

            GameMaster.Instance.audioSource.PlayOneShot(pickUpSound);
            GameMaster.Instance.UpdateCoinCount(value);
            return true;
        }

        return false;
    }

    void InitializeBounceAnim(){
        originPosition = transform.position;
        float yTranslation = .25f;
        transform.position += new Vector3(0f, yTranslation, 0f);
        Vector3 bounceStrengthVector = new Vector3(0f, .1f, 0f);
        float bounceDiffVectorAngle = Mathf.Atan2(bounceStrengthVector.y, bounceStrengthVector.x);
        bounceDiffVectorAngle += Mathf.Deg2Rad * transform.localEulerAngles.z;
        bounceStrengthVector = new Vector3(Mathf.Cos(bounceDiffVectorAngle), Mathf.Sin(bounceDiffVectorAngle), 0f) * bounceStrengthVector.magnitude;


        float bounceTime = .5f;
        bounceSequence = DOTween.Sequence()
            .Append(transform.DOMoveY(transform.position.y - yTranslation, .25f)).SetEase(Ease.Linear)
            .Append(transform.DOPunchPosition(bounceStrengthVector, bounceTime, 4, 0f)).SetEase(Ease.OutQuad)
            .OnUpdate(()=>{
                // Debug.Log("Coin bouncing");
            })
            .OnComplete(()=>{
                })
            .Pause();
    }

    
    public void Discover(){
        GameMaster.Instance.UpdateCoinCount(value);
    }

    public void Finish(){
    }
}

public enum CoinValue{
    One = 1,
    Five = 5,
    Twenty = 20,
    OneHundred = 100
}