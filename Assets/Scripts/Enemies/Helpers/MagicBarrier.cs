using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MagicBarrier : MonoBehaviour
{
    [SerializeField] CircleCollider2D circleCollider;
    public CircleCollider2D CircleCollider{
        get => circleCollider;
    }
    public bool Active{
        protected set;
        get;
    } = true;
    [SerializeField] SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer{
        get => spriteRenderer;
    }
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] Enemy parentEnemy;
    public Enemy ParentEnemy{
        get => parentEnemy;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(!Active){
            return;
        }

        if(other.collider.gameObject.tag == "Shield"){
            Active = false;

            circleCollider.enabled = false;
            parentEnemy.GetComponent<I_BarrierdEnemy>().ShieldBlockedBarrier();

            spriteRenderer.DOFade(0f, fadeDuration)
                .OnComplete(()=>{
                    gameObject.SetActive(false);
                });
        }
    }
}
