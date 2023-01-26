using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LightableObject : MonoBehaviour
{
    [SerializeField] protected bool on = true;
    public bool On{
        get {return on;}
    }
    [SerializeField] protected float globalDarknessModifier = .005f;
    public float GlobalDarknessModifier{
        get { return globalDarknessModifier; }
    }
    [SerializeField] bool lightableByBeam = true;
    public bool LightableByBeam{
        protected set => lightableByBeam = value;
        get => lightableByBeam;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void SetOn(bool on, bool flipOthers = true);

    public abstract void SetOff();

    public abstract void FlipSwitch(bool flipOthers = true);
}
