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
    protected override void InitializeMaterializeSpawn(){
        base.InitializeMaterializeSpawn();

        // if(defaultMaterial == null){
        //     defaultMaterial = GetComponent<Renderer>().material;
            // magicBarrier.gameObject.GetComponent<Renderer>().material.SetVector("_DistortionVelocity", distortionVelocity);
        // }
    }    

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateMagicBarrier();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected void UpdateMagicBarrier(){
        magicBarrier.SpriteRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
    }

    protected override void UpdateCheckerboardMaterializeSpawn()
    {
        // base.UpdateCheckerboardMaterializeSpawn();
        if(!materializing){
            return;
        }
        currentCheckerSize += new Vector2(checkerChangeRate, checkerChangeRate);
        // Debug.Log($"Current checker size: {currentCheckerSize}");
        GetComponent<Renderer>().material.SetVector("_CheckerBoardFrequency", currentCheckerSize);
        if(currentCheckerSize.x >= checkerSizeTarget){
            materializing = false;
            GetComponent<Renderer>().material = defaultMaterial;
            Material material = GetComponent<Renderer>().material;
            // Debug.Log("Spawning finished");
        }
        // Debug.Log("flames spawn update");

        flames.GetComponent<Renderer>().material.SetVector("_CheckerBoardFrequency", currentCheckerSize);
        if(currentCheckerSize.x >= checkerSizeTarget){
            flames.GetComponent<Renderer>().material = flamesMaterial;
        }

        // Debug.Log("magicBarrier spawn update");
        magicBarrier.gameObject.GetComponent<Renderer>().material.SetVector("_CheckerBoardFrequency", currentCheckerSize);
        if(currentCheckerSize.x >= checkerSizeTarget){
            magicBarrier.gameObject.GetComponent<Renderer>().material = defaultMaterial;
        }
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

    public override void StartCheckerboardMaterializeSpawn()
    {
        base.StartCheckerboardMaterializeSpawn();

        Renderer magicBarrierRenderer = magicBarrier.gameObject.GetComponent<Renderer>();
        magicBarrierRenderer.material = spawnMaterial;
        magicBarrierRenderer.material.SetVector("_CheckerBoardFrequency", startingCheckerSize);
        magicBarrierRenderer.material.SetVector("_DistortionVelocity", distortionVelocity);
    }
}
