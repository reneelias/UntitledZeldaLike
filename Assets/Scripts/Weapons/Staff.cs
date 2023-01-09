using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : A_Weapon
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        UpdatePosition();
        UpdateLayerOrder();
    }
    
    void FixedUpdate()
    {
        // UpdateFlipped();
    }

}
