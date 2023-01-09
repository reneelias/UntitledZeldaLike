using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBreakable
{
    public bool Broken{
        get;
    }
    public bool Breakable{
        get;
    }
    public Breakable_Type BreakableType{
        get;
    }
    public bool Break();
}

public enum Breakable_Type{
    Skull,
    Mushroom
}