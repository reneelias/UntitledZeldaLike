using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterPointLightBehavior : MonoBehaviour
{

	public GameObject character;
    // private float elapsedDT = 0f;

    private UnityEngine.Rendering.Universal.Light2D light2D;

    private float shadowModifier = .0025f;
    public bool active = true;

    // Start is called before the first frame update
    void Start()
    {
        character = GameObject.Find("Mario");
        if(character != null){
            // Debug.Log("Mario found");
            // Debug.Log(this.transform.position.x);

            // this.transform.position = new Vector3(this.transform.position.x + 5, this.transform.position.y, this.transform.position.z);
        }

        light2D = GetComponent<UnityEngine.Rendering.Universal.Light2D>();

        if(light2D != null){
            Debug.Log("light2D found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // this.transform.position = new Vector3(character.transform.position.x, character.transform.position.y + 4, character.transform.position.z);

        // elapsedDT += Time.deltaTime;

        // if(elapsedDT >= 4){
        //     Debug.Log("4 seconds have passed.");
        //     elapsedDT = 0;
        // }

        if(active){
            light2D.intensity += shadowModifier;
            if(light2D.intensity >= 1.5 || light2D.intensity <= .5){
                shadowModifier *= -1;
                light2D.intensity += shadowModifier;
            }
        } else if(light2D.intensity != 0){
            Debug.Log("setting intensity to 0");
            light2D.intensity = 0;
        }
    }
}
