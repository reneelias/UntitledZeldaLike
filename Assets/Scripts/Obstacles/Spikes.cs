using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour, ISwitchable, I_Trap
{
    [SerializeField] GameObject spikesFull;
    [SerializeField] GameObject spikesCollased;
    public bool cycling = true;
    [SerializeField] float cycleTime = 1f;
    float cycleDT = 0f;
    [SerializeField] bool collapsed = false;
    public bool Activated{
        protected set; get;
    } = false;
    public bool TrapActivated{
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
                cycling = false;
                Activate();
            } else {
                cycling = true;
                Deactivate();
            }
        }
    }
    public Trap_Type TrapType{
        protected set;
        get;
    } = Trap_Type.Spikes;

    [SerializeField] protected float pushForceMagnitude = 20f;
    // Start is called before the first frame update
    void Start()
    {
        CycleSpikes(collapsed);
        
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.Find("WizardBoxCollider").GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        UpdateCycle();     
    }
    void UpdateCycle(){
        if(!cycling){
            return;
        }

        cycleDT += Time.deltaTime;

        if(cycleDT >= cycleTime){
            cycleDT = 0f;
            if(Activated){
                Deactivate();
            } else {
                Activate();
            }
        }
    }

    public void CycleSpikes(bool collapsed){
    }

    public void Activate(){
        Activated = collapsed = true;
        TrapActivated = false;
        spikesFull.SetActive(!collapsed);
        spikesCollased.SetActive(collapsed);
    }

    public void Deactivate(){
        Activated  = collapsed = false;
        TrapActivated = true;
        spikesFull.SetActive(!collapsed);
        spikesCollased.SetActive(collapsed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && !collapsed){
            Vector2 force = (other.gameObject.transform.position - transform.position).normalized * pushForceMagnitude;
            other.GetComponent<CharacterControls>().ApplyForce(force, ForceMode2D.Impulse);
            other.GetComponent<PlayableCharacter>().ChangeHP(-10);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Player" && !collapsed){
            Vector2 force = (other.gameObject.transform.position - transform.position).normalized * pushForceMagnitude;
            other.GetComponent<CharacterControls>().ApplyForce(force, ForceMode2D.Impulse);
            other.GetComponent<PlayableCharacter>().ChangeHP(-10);
        }
    }
}
