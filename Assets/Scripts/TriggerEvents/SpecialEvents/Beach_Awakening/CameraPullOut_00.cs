using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraPullOut_00 : EventTrigger
{

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    public override void Trigger() {
        base.Trigger();

        GameMaster.Instance.SetBlackOverlay(true);
        GameMaster.Instance.FadeOutSpriteOverlay(2f, 2f);
    }
}
