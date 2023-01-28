using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkullEnemy_MagicBarrier : SkullEnemy, I_BarrierdEnemy
{
    [Header("Magic Barrier")]
    [SerializeField] MagicBarrier magicBarrier;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Physics2D.IgnoreCollision(collider2D, magicBarrier.CircleCollider);
        // Physics2D.IgnoreCollision(collider, magicBarrier.CircleCollider);
        magicBarrier.SpriteRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateMagicBarrier();
    }

    protected void UpdateMagicBarrier(){
        magicBarrier.SpriteRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
    }

    public override void ChangeHP(int deltaHP)
    {
        if(magicBarrier.Active){
            return;
        }

        base.ChangeHP(deltaHP);
    }

    public virtual void ShieldBlockedBarrier(){
        hitShield = true;

        DOVirtual.DelayedCall(.15f, ()=>{
            hitShield = false;
        });
    }
}
