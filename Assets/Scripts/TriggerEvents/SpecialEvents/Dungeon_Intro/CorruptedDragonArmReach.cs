using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CorruptedDragonArmReach : EventTrigger
{
    [SerializeField] CorruptedDragonPuppet corruptedDragonPuppet;
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

    public override void Trigger(){
        float bodyMoveDuration = 1f;
        float bodyTranslateY = .125f;

        corruptedDragonPuppet.AnimateBody(new Vector3(0f, bodyTranslateY, 0f), bodyMoveDuration, 0f);
        corruptedDragonPuppet.AnimateHead(new Vector3(0f, bodyTranslateY, 0f), bodyMoveDuration, 0f);

        float armMovementDuration = 2f;
        float headTranslateY = -.25f;
        corruptedDragonPuppet.AnimateArmMovement(ArmSide.Right, -100f, corruptedDragonPuppet.MaxHandDistance, armMovementDuration, bodyMoveDuration);
        corruptedDragonPuppet.AnimateArmMovement(ArmSide.Left, -80f, corruptedDragonPuppet.MaxHandDistance, armMovementDuration, bodyMoveDuration);
        DOVirtual.DelayedCall(bodyMoveDuration, ()=>{
            corruptedDragonPuppet.LeftArm.SetArmLayerRelativeHand();
        });
        DOVirtual.DelayedCall(bodyMoveDuration, ()=>{
            corruptedDragonPuppet.RightArm.SetArmLayerRelativeHand();
        });
        corruptedDragonPuppet.AnimateHead(new Vector3(0f, headTranslateY, 0f), armMovementDuration, bodyMoveDuration)
            .OnComplete(Finish);
        DOVirtual.DelayedCall(bodyMoveDuration, ()=>{
            corruptedDragonPuppet.SetHeadLayer("Default", 4);
            corruptedDragonPuppet.NeckPiece_Long.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
            corruptedDragonPuppet.NeckPiece_Long.GetComponent<SpriteRenderer>().sortingLayerID = 0;
            // corruptedDragonPuppet.SetHeadLayer("Default", 1);
        });
        
        // corruptedDragonPuppet.LeftArm.SetArmLayerRelativeHand();
        // corruptedDragonPuppet.RightArm.SetArmLayerRelativeHand();
    }
}
