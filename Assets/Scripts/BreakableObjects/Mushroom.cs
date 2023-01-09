using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FunkyCode;

public class Mushroom : A_ItemDropper, IBreakable
{
    bool jiggling = false;
    Tween jiggleTween;
    Vector3 originalScale;
    public bool Broken{
        get;
        protected set;
    } = false;

    public bool Breakable{
        get;
        protected set;
    } = true;

    public Breakable_Type BreakableType{
        get;
        protected set;
    } = Breakable_Type.Mushroom;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject bottomHalf;
    [SerializeField] GameObject topHalf;
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] LightCollider2D lightCollider2D;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Jiggle(){
        if(jiggleTween != null && jiggleTween.IsPlaying()){
            jiggleTween.Kill();
            transform.localScale = originalScale;
        }

        jiggling = true;
        jiggleTween = transform.DOShakeScale(.14f, .3f, 10, 90f);
        jiggleTween.OnComplete(
            ()=>{
                    transform.localScale = originalScale;
                    jiggling = false;
                }
        );
    }

    public bool Break(){
        if(Broken || !Breakable){
            return false;
        }
        
        lightCollider2D.enabled = false;
        rigidbody2D.simulated = false;
        spriteRenderer.enabled = false;
        bottomHalf.SetActive(true);
        topHalf.SetActive(true);
        // bottomHalf.GetComponent<SpriteRenderer>().enabled = true;
        // topHalf.GetComponent<SpriteRenderer>().enabled = true;

        bottomHalf.GetComponent<SpriteRenderer>().DOFade(0f, 1f)
            .OnComplete(()=>{gameObject.SetActive(false);});
            
        topHalf.GetComponent<SpriteRenderer>().DOFade(0f, 1f)
            .OnComplete(()=>{gameObject.SetActive(false);});
        
        float slideMagnitude = .25f;
        float slideAngle = (-18f + topHalf.transform.eulerAngles.z) * Mathf.Deg2Rad;
        topHalf.transform.DOMove(topHalf.transform.position + new Vector3(slideMagnitude * Mathf.Cos(slideAngle), slideMagnitude * Mathf.Sin(slideAngle), 0f), 1f);

        Breakable = false;
        Broken = true;

        DropItem();
        return true;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log($"current object: {gameObject.name}, colliding with: {other.otherCollider.name}");
        if(other.gameObject.tag == "PlayerProjectile"){
            Jiggle();
        }
    }
}
