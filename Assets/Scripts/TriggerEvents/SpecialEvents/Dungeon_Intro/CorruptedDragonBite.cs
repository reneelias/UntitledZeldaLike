using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CorruptedDragonBite : EventTrigger
{
    [Header("Corrupted Dragon Bite")]
    [SerializeField] CorruptedDragonPuppet corruptedDragonPuppet;
    [SerializeField] float bodyMoveDuration = .5f;
    [SerializeField] float bodyTranslateY = -.125f;
    [SerializeField] float headTranslateY = -1.5f;
    [SerializeField] float headMovementDuration = 1f;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Trigger()
    {
        base.Trigger();
        

        corruptedDragonPuppet.AnimateBody(new Vector3(0f, bodyTranslateY, 0f), bodyMoveDuration, 0f);
        
        corruptedDragonPuppet.AnimateHead(new Vector3(0f, headTranslateY, 0f), headMovementDuration, bodyMoveDuration * .25f).SetEase(Ease.InSine)
            .OnComplete(Finish);

        DOVirtual.DelayedCall(bodyMoveDuration *.5f, ()=>{
            corruptedDragonPuppet.SetHeadState(HeadState.Biting);
        });
    }

    public override void Finish()
    {
        base.Finish();

        GameMaster.Instance.SetBlackOverlay();
        GameMaster.Instance.EndOfDemo();
    }
}
