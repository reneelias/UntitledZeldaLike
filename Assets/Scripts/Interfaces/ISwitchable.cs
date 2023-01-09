using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISwitchable
{
    public bool Activated{
        get;
    }

    public void Activate();
    public void Deactivate();
    
    public bool ActivatePermanently{
        get; set;
    }
}
