using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Trap
{
    public Trap_Type TrapType{
        get;
    }

    public bool TrapActivated{
        get;
    }
}

public enum Trap_Type{
    Spikes
}