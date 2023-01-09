using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FillBar : MonoBehaviour
{
    public float FillPercent{
        get;
        set;
    }

    [SerializeField] GameObject innerBar;
    [SerializeField] GameObject outerBar;
    [SerializeField] bool showText;
    [SerializeField] bool deactivateOnZeroPercent;
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject parentObject;
    [SerializeField] bool alwaysStayUpright = true;
    float distanceFromUprightObject;

    GameObject UI_Camera;

    public bool isUI = false;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        FillPercent = 1f;

        canvas.worldCamera = Camera.main;
        canvas.gameObject.SetActive(showText);
        
        if(isUI){
            UI_Camera = GameObject.Find("UI Camera");
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        if(alwaysStayUpright){
            distanceFromUprightObject = (transform.position - parentObject.transform.position).magnitude;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCanvasOrder();
        if(isUI){
            // transform.position = UI_Camera.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(-4.72f, 9.72f, 0f));
        }

        UpdateStayUpright();
    }

    void FixedUpdate()
    {
        UpdateBarWidth();
    }

    public void UpdateBarWidth()
    {
        float xScale = innerBar.transform.localScale.x - (innerBar.transform.localScale.x - FillPercent) * .5f;
        innerBar.transform.localScale = new Vector3(xScale, innerBar.transform.localScale.y, innerBar.transform.localScale.z);

        if(deactivateOnZeroPercent && innerBar.transform.localScale.x <= .005f)
        {
            // canvas.active = false;
            gameObject.SetActive(false);
        }
    }

    public void UpdateStayUpright(){
        if(!alwaysStayUpright){
            return;
        }

        transform.localEulerAngles = new Vector3(0f, 0f, -parentObject.transform.eulerAngles.z);
        Vector3 uprightPosition = new Vector3();
        uprightPosition.x = Mathf.Cos(Mathf.Deg2Rad * (transform.eulerAngles.z + 90)) * distanceFromUprightObject;
        uprightPosition.y = Mathf.Sin(Mathf.Deg2Rad * (transform.eulerAngles.z + 90)) * distanceFromUprightObject;
        uprightPosition.z = 0f;

        transform.localPosition = uprightPosition;
    }

    public void UpdateText(string newText)
    {
        text.text = newText;
    }

    private void UpdateCanvasOrder()
    {
        if(!parentObject){
            // canvas.GetComponent<Canvas>().sortingOrder = innerBar.GetComponent<SpriteRenderer>().sortingOrder + 1;
            return;
        }
        
        canvas.sortingOrder = parentObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        outerBar.GetComponent<SpriteRenderer>().sortingOrder = parentObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        innerBar.GetComponent<SpriteRenderer>().sortingOrder = parentObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }
}
