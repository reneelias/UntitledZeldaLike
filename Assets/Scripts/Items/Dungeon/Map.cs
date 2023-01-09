using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour, I_DiscoverableItem
{
    [SerializeField] ParticleEmitter particleEmitter;
    [TextArea]
    [SerializeField] string[] discoverMessages;
    [SerializeField] float fontScaling = 1f;
    public float FontScaling{
        protected set{fontScaling = value;}
        get {return fontScaling;}
    }
    public string[] DiscoverMessages{
        protected set {discoverMessages = value;}
        get { return discoverMessages;}
    }
    [SerializeField] GameObject spriteObject;
    public GameObject SpriteObject{
        protected set {spriteObject = value;}
        get {return spriteObject;}
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Discover(){
        particleEmitter.activelySpawning = true;
    }

    public void Finish(){
        particleEmitter.activelySpawning = false;
    }
}
