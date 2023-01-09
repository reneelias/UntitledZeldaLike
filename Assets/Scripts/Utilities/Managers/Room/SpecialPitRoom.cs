using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPitRoom : Room
{
    [SerializeField] GameObject initialLastGroundPositionObject;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        UseDefaultLastOnGroundPosition = true;
        DefaultLastOnGroundPosition = initialLastGroundPositionObject.transform.position;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate(){
        base.FixedUpdate();
    }

    public void GroundColliderTouched(Vector3 lastGroundPosition){
        DefaultLastOnGroundPosition = lastGroundPosition;
    }
}
