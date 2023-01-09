using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Respawnable
{
    void Respawn();
    bool Active{ 
        get;
    }
}
