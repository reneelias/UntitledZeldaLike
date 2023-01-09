using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FunkyCode;

public class TeleporterPad : MonoBehaviour, IUnlocklableObject
{
    [SerializeField] GameObject teleporterBase;
    [SerializeField] TeleporterPadRing teleporterPadRingPrefab;
    [SerializeField] float shrinkTime;
    [SerializeField] float ringSpawnTime;
    [SerializeField] float ringSpawnDT = 0f;
    TeleporterPadRing[] rings;
    [SerializeField] int numOfRings = 3;
    [SerializeField] Color padColor;
    [SerializeField] Color ringColor;
    int ringIndex = 0;
    [SerializeField] float alphaStepMultiplier = 4f;
    [SerializeField] ParticleEmitter particleEmitter;
    [SerializeField] TeleporterPad connectedTeleporterPad;
    public TeleporterPad ConnectedTeleporterPad{
        get => connectedTeleporterPad;
    }
    [SerializeField] bool unlocked = false;
    public bool Unlocked{
        get => unlocked;
        protected set => unlocked = value;
    }
    [SerializeField] Room room;
    public Room Room{
        get => room;
    }
    [SerializeField] Light2D light;
    public bool Active{
        protected set;
        get;
    } = true;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }
    
    void Initialize(){ 
        rings = new TeleporterPadRing[numOfRings];
        for(int i = 0; i < numOfRings; i++){
            rings[i] = GameObject.Instantiate(teleporterPadRingPrefab, transform);
            rings[i].Initialize();
            rings[i].GetComponent<SpriteRenderer>().sortingLayerName = teleporterBase.GetComponent<SpriteRenderer>().sortingLayerName;
        }

        teleporterBase.GetComponent<SpriteRenderer>().color = padColor;
        ringSpawnDT = ringSpawnTime; 

        particleEmitter.color = padColor;

        light.color = padColor;

        if(!Unlocked){
            gameObject.SetActive(false);
            Active = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        UpdateRings();
    }

    void UpdateRings(){
        ringSpawnDT += Time.deltaTime;

        if(ringSpawnDT >= ringSpawnTime){
            while(!rings[ringIndex].Activate(shrinkTime, teleporterBase.GetComponent<SpriteRenderer>().sortingOrder + 1, ringColor, alphaStepMultiplier)){
                if(++ringIndex >= numOfRings){
                    ringIndex = 0;
                }
            }

            if(++ringIndex >= numOfRings){
                    ringIndex = 0;
            }

            ringSpawnDT = 0f;
        }
    }

    public void Unlock(){
        gameObject.SetActive(true);
        Unlocked = true;
        
        if(room.active){
            Color newColor;
            SpriteRenderer teleporterBaseSpriteRenderer = teleporterBase.GetComponent<SpriteRenderer>();
            newColor = teleporterBaseSpriteRenderer.color;
            newColor.a = 0f;
            float fadeInDuration = 1f;
            teleporterBaseSpriteRenderer.DOFade(1f, fadeInDuration);
            float ogLightAlpha = light.color.a;
            light.color.a = 0f;
                DOTween.To(() => light.color.a, x => light.color.a = x, ogLightAlpha, fadeInDuration)
                .OnComplete(()=>{
                    Active = true;
                });
        } else {
            Active = true;
        }

        if(!connectedTeleporterPad.Unlocked){
            connectedTeleporterPad.Unlock();
        }
    }
}
