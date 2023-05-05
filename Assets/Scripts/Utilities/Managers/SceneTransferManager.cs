using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransferManager : PersistantSingleton<SceneTransferManager>
{
    EventTrigger onLoadEventPrefab;
    EventTrigger onLoadEvent;
    public EventTrigger OnLoadEvent{
        get => onLoadEvent;
        protected set => onLoadEvent = value;
    }

    protected override void Awake() {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadNewScene(string sceneName, EventTrigger onLoadEventPrefab, string settingsStringJSON) {
        SceneManager.LoadScene(sceneName);
        this.onLoadEventPrefab = onLoadEventPrefab;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
        if (onLoadEventPrefab != null) {
            // SetBlackOverlay(false);
            OnLoadEvent = GameObject.Instantiate(onLoadEventPrefab);
            OnLoadEvent.Trigger();
        }
    }

    public void ResetScene() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);


        // if(Application.platform == RuntimePlatform.Android) {
        //     Camera.main.orthographicSize = 5f;
        // } else {
        //     GameObject.Find("JoystickCanvas").SetActive(false);
        // }
    }
}
