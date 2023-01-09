using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float speed = .333f;
    public CharacterController2D controller;
    float horizontalAxisVal = 0f;
    public float runSpeed = 40f;
    public GameObject hammer;
    public GameObject hammerLight, marioLightNormal, marioLightHammer;
    public BoxCollider2D hammerCollider, playerCollider;
    public Sprite marioHammerSprite;
    private bool spriteChanged = false;
    private CharacterPointLightBehavior hammerLightScript;

    bool jump = false;

    // Start is called before the first frame update
    void Start()
    {
        hammer = GameObject.Find("MarioHammer");

        if(hammer != null){
            // Debug.Log("FOUND ZE HAMMER");
        }

        hammerCollider = hammer.GetComponent<BoxCollider2D>();

        if(hammerCollider != null){
            Debug.Log("FOUND ZE HAMMER COLLIDER");
        }

        playerCollider = GetComponent<BoxCollider2D>();
        if(playerCollider != null){
            Debug.Log("FOUND ZE PLAYER COLLIDER");
        }

        hammerLightScript = hammerLight.GetComponent<CharacterPointLightBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalAxisVal = Input.GetAxisRaw("Horizontal") * runSpeed;
        // horizontalAxisVal = Input.acceleration.x * runSpeed;

        foreach(Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                jump = true;
                break;
            }
        }

        if(!jump && Input.GetKeyDown(KeyCode.Space)){
            jump = true;
        }

        // if(playerCollider.bounds.Intersects(hammerCollider.bounds)){
        //     Debug.Log("You are on the hammer, MARIIOOOO!!!");
        // }

        if(!spriteChanged && hammerCollider.IsTouching(playerCollider)){
            Debug.Log("You are on the hammer, MARIIOOOO!!!");
            
            spriteChanged = true;
            GetComponent<SpriteRenderer>().sprite = marioHammerSprite;
            hammer.GetComponent<Renderer>().enabled = false;
            hammerCollider.enabled = false;
            // hammerLight.GetComponent<CharacterPointLightBehavior>().active = false;
            hammerLightScript.active = false;

            marioLightNormal.GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = 0;
            marioLightHammer.GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = 1;
        }

        // jump = Input.GetKeyDown(KeyCode.Space);

        // controller2D.Move();

    }

    void FixedUpdate()
    {
        //Move our character

        controller.Move(horizontalAxisVal * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
