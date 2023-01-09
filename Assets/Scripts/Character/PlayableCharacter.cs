using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayableCharacter : MonoBehaviour
{
    protected int hp;
    public int HP{
        protected set => hp = value;
        get => hp;
    }
    [SerializeField] int maxHP;
    public int MaxHP{
        protected set => maxHP = value;
        get => maxHP;
    }
    public int HpRatio {
        get => hp / maxHP;
    }
    [SerializeField] FillBar HPBar;
    public bool Invinsible{
        get;
        protected set;
    }

    private float invinsibleDT;
    private float invinsibleTime;

    private float alphaChangeDT;
    private float alphaChangeTime;

    protected SpriteRenderer spriteRenderer;
    [SerializeField] CharacterControls characterControls;
    [SerializeField] AudioClip damageAudioClip;
    [SerializeField] AudioClip deathAudioClip;
    [SerializeField] Animator animator;
    [SerializeField] int coinCount = 0;
    [SerializeField] GameObject centerPoint;
    protected bool dodging = false;
    public GameObject CenterPoint{
        get {return centerPoint;}
    }
    protected int stamina;
    public int Stamina{
        get => stamina;
        protected set => stamina = value;
    }
    [SerializeField] FillBar staminaBar;
    [SerializeField] protected int maxStamina = 100;
    float staminaRechargeBeginDT;
    [SerializeField] float staminaRechargeTime = 1f;
    [SerializeField] int staminaRechargeRate = 2;
    bool staminaRecharging = false;

    public bool Alive{
        get;
        protected set;
    }
    // Start is called before the first frame update
    void Start()
    {
        Alive = true;
        hp = maxHP;
        // Debug.Log(HPBar);
        HPBar.UpdateText($"{hp}/{maxHP}");

        stamina = maxStamina;
        staminaBar.UpdateText($"{stamina}/{maxStamina}");

        invinsibleDT = 0f;
        invinsibleTime = 1f;
        alphaChangeDT = 0f;
        alphaChangeTime = .05f;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        UpdateInvinsible();
        UpdateStamina();
    }

    private void UpdateInvinsible(){
        if(!Invinsible){
            return;
        }

        Color currColor = spriteRenderer.color;
        if(dodging){
            currColor.a = .25f;
            spriteRenderer.color = currColor;
            return;
        }

        alphaChangeDT += Time.deltaTime;
        if(alphaChangeDT >= alphaChangeTime){
            currColor.a = currColor.a == 1f ? .5f : 1f;
            alphaChangeDT = 0f;
        }
        spriteRenderer.color = currColor;

        invinsibleDT += Time.deltaTime;
        if(invinsibleDT >= invinsibleTime){
            Invinsible = false;
            currColor.a = 1f;
            spriteRenderer.color = currColor;
        }
    }

    protected void UpdateStamina(){
        if(!staminaRecharging && stamina < maxStamina){
            staminaRechargeBeginDT += Time.deltaTime;

            if(staminaRechargeBeginDT >= staminaRechargeTime){
                staminaRecharging = true;
                staminaRechargeBeginDT = 0f;
            }
        }

        if(staminaRecharging){
            stamina += staminaRechargeRate;
        

            if(stamina >= maxStamina){
                staminaRecharging = false;
                stamina = maxStamina;
            }
            
            staminaBar.FillPercent = (float)stamina/maxStamina;
            staminaBar.UpdateText($"{stamina}/{maxStamina}");
        }
    }

    public virtual void ChangeHP(int deltaHP, bool overrideInvinsibility = false){
        if((!overrideInvinsibility && Invinsible) || !Alive){
            return;
        }

        hp += deltaHP;
        hp = (int)Mathf.Clamp(hp, 0, maxHP);
        HPBar.FillPercent = (float)hp/maxHP;
        HPBar.UpdateText($"{hp}/{maxHP}");

        if(deltaHP < 0){
            invinsibleDT = 0f;
            alphaChangeDT = 0f;
            Invinsible = true;

            GameMaster.Instance.audioSource.PlayOneShot(damageAudioClip);

            if(hp <= 0 && Alive){
                GameMaster.Instance.SetGameOver();
                GameMaster.Instance.audioSource.PlayOneShot(deathAudioClip);
                Alive = false;
                // GameMaster.Instance.dungeon.CurrentRoom.DeathRoomReset();
                characterControls.NormalControlsSuspended = true;
            }
        }
    }

    public void Respawn(Vector3 respawnPosition, int floorLevel, CharacterDirection respawnPlayerDirection){
        spriteRenderer.sortingLayerName = "Default";
        gameObject.GetComponent<SortingOrderByY>().enabled = true;
        hp = maxHP;
        HPBar.FillPercent = (float)hp/maxHP;
        HPBar.UpdateText($"{hp}/{maxHP}");
        characterControls.NormalControlsSuspended = false;
        Alive = true;
        transform.position = respawnPosition;
        characterControls.Respawn(floorLevel);
        characterControls.SetCharacterDirection(respawnPlayerDirection, true);
    }

    public virtual void ChangeStamina(int deltaStamina, bool resetRechargeTimer = true){
        if(!Alive){
            return;
        }

        stamina += deltaStamina;
        stamina = (int)Mathf.Clamp(stamina, 0, maxStamina);
        staminaBar.FillPercent = (float)stamina/maxStamina;
        staminaBar.UpdateText($"{stamina}/{maxStamina}");

        if(resetRechargeTimer){
            staminaRechargeBeginDT = 0f;
            staminaRecharging = false;
        }
    }

    public void InitiateDodge(){
        Invinsible = true;
        dodging = true;
    }

    public void EndDodge(){
        Invinsible = false;
        Color currColor = spriteRenderer.color;
        currColor.a = 1f;
        spriteRenderer.color = currColor;
        dodging = false;
    }

    public void EnableRenderer(bool enable){
        GetComponent<Renderer>().enabled = enable;
    }

    void OnCollisionEnter2D(Collision2D other)
    {

    }    
    
    void OnTriggerEnter2D(Collider2D collider2D){

    }
}
