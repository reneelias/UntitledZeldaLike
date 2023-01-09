using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Weapon
{
    public bool Active{
        get;
    }

    public void Appear();
    public void Disappear();
}
