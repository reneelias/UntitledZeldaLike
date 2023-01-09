using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsibleBlock : MonoBehaviour, ISwitchable
{
    [SerializeField] GameObject fullBlock;
    [SerializeField] GameObject collapsedBlock;
    public bool Activated{
        protected set; get;
    } = false;
    bool activatePermanently = false;
    public bool ActivatePermanently{
        get{
            return activatePermanently;
        }
        set{
            activatePermanently = value;
            if(activatePermanently){
                Activate();
            } else {
                Deactivate();
            }
        }
    }
    [SerializeField] GameObject extraHitbox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Activate(){
        Activated = true;
        fullBlock.SetActive(!Activated);
        collapsedBlock.SetActive(Activated);

        if(extraHitbox != null){
            extraHitbox.SetActive(false);
        }
    }

    public void Deactivate(){
        Activated = false;
        fullBlock.SetActive(!Activated);
        collapsedBlock.SetActive(Activated);
        
        if(extraHitbox != null){
            extraHitbox.SetActive(false);
        }
    }
}
