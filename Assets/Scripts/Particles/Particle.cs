using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FunkyCode;

public class Particle : MonoBehaviour, IParticle
{
    // private bool active;
    public bool Active {
        get;
        private set;
    }
    public float Speed {
        get;
        set;
    }
    private Tween yTween;
    private Light2D light;
    private Color color;
    private bool fadingIn = false;
    private bool fadingOut = false;
    private bool movingToTargetPosition = false;
    private float movementStepX;
    private float movementStepY;
    private float fadeInTime;
    private float fadeInDT;
    private float fadeInStep;
    private float lifeDuration;
    private float lifeDurationDT;
    private float fadeOutTime;
    private float fadeOutDT;
    private float fadeOutStep;
    [SerializeField] SpriteRenderer spriteRenderer;
    public bool lightEnabled = true;
    float originalLightSize;
    float originalScale;
    [SerializeField] SortingOrderByY sortingOrderByY;

    // Sequence particleLifeSequence;
    // Start is called before the first frame update
    void Start()
    {
        EnableRenderer(false);
        Active = false;
        light = GetComponent<Light2D>();
        light.color.a = 0;
        originalLightSize = light.size;
        originalScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        UpdateLifeTimer();
        UpdatePosition();
        UpdateFadeIn();
        UpdateFadeOut();
    }

    public void Activate(float spawnX, float spawnY, float lifeDuration, float fadeInTime, float fadeOutTime, float xOffset, float yOffset, Color color, float scale, bool lightEnabled, bool useSortingOrderByY = false){
        EnableRenderer(true);
        Color tempColor = color;
        tempColor.a = 0f;
        spriteRenderer.color = tempColor;
        this.color = tempColor;

        transform.localScale = new Vector3(scale, scale, 1f);

        fadingIn = true;
        fadeInStep = (1f - tempColor.a) / fadeInTime * Time.deltaTime;
        fadeInDT = 0f;
        this.fadeInTime = fadeInTime;

        fadingOut = false;
        fadeOutStep = 1f / fadeOutTime * Time.deltaTime;
        fadeOutDT = 0f;
        this.fadeOutTime = fadeOutTime;

        transform.position = new Vector3(spawnX, spawnY, 0);
        movingToTargetPosition = true;
        movementStepX = xOffset / lifeDuration * Time.deltaTime;
        movementStepY = yOffset / lifeDuration * Time.deltaTime;

        this.lifeDuration = lifeDuration;
        this.lifeDurationDT = 0f;

        Active = true;

        this.lightEnabled = lightEnabled;
        if(lightEnabled){
            light.enabled = true;
            light.color = color;
            light.color.a = 0;
            light.size = originalLightSize * (scale / originalScale);
        }
        
        sortingOrderByY.enabled = useSortingOrderByY;
    }

    public void SetSortingOrder(int sortingOrder, string sortingLayer = "Default"){
        spriteRenderer.sortingLayerName = sortingLayer;
        spriteRenderer.sortingOrder = sortingOrder;
    }

    public void UpdateFadeIn(){
        if(!fadingIn){
            return;
        }

        color.a = Mathf.Min(color.a + fadeInStep, 1f);
        spriteRenderer.color = color;
        if(lightEnabled){
            light.color.a += fadeInStep;
        }

        fadeInDT += Time.deltaTime;
        if(fadeInDT >= fadeInTime){
            fadingIn = false;
            fadingOut = true;
        }
    }

    public void UpdateFadeOut(){
        if(!fadingOut){
            return;
        }

        color.a = Mathf.Max(color.a - fadeOutStep, 0f);
        spriteRenderer.color = color;
        if(lightEnabled){
            light.color.a -= fadeOutStep;
        }

        fadeOutDT += Time.deltaTime;
        if(fadeOutDT >= fadeOutTime){
            fadingOut = false;
        }
    }

    public void UpdateLifeTimer(){
        if(!Active){
            return;
        }

        lifeDurationDT += Time.deltaTime;
        if(lifeDurationDT >= lifeDuration){
            Deactivate();
        }
    }
    public void UpdatePosition(){
        if(!movingToTargetPosition){
            return;
        }

        transform.Translate(Vector3.right * movementStepX);
        transform.Translate(Vector3.up * movementStepY);
    }

    public void FadeOut(){
        spriteRenderer.DOFade(0, .2f).OnComplete(Deactivate);
        // DOTween.To(()=> lightComp.color.a, x=> lightComp.color.a = x, 0, .2f);
    }

    public void Deactivate(){
        movingToTargetPosition = false;
        Active = false;
        light.color.a = 0f;
        light.enabled = false;
        EnableRenderer(false);
    }

    public void EnableRenderer(bool enable){
        GetComponent<Renderer>().enabled = enable;
    }
}
//FF8A5A