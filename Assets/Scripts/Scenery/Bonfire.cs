using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonfire : MonoBehaviour
{
    [SerializeField] ParticleEmitter smokeEmitter;
    [SerializeField] bool emitSmoke = false;
    [SerializeField] GameObject flameObject;
    [SerializeField] bool flameOn = true;
    // Start is called before the first frame update
    void Start()
    {
        if(!flameOn){
            flameObject.SetActive(false);
        }

        if(emitSmoke){
            smokeEmitter.activelySpawning = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
