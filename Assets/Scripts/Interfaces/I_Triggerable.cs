using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Triggerable
{
    public bool Triggered{
        get;
    }
    public EventTrigger NextEventTrigger{
        get;
    }
    public float PauseBeforeNextAction{
        get;
    }

    public void Finish();

    public void Trigger();
}
