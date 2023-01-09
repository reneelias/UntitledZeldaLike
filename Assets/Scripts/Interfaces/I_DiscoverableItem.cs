using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_DiscoverableItem
{
    void Discover();
    void Finish();
    public string[] DiscoverMessages{
        get;
    }
    public float FontScaling{
        get;
    }
    public GameObject SpriteObject{
        get;
    }
}
