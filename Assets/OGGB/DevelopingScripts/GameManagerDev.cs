using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

//The Game Manager keeps track of which scenes to load, handles loading scenes, fading between scenes and also video playing/pausing

namespace Interactive360
{

    public class GameManagerDev : MonoBehaviour
    {

        public static GameManagerDev instance = null;

        Scene scene;
        VideoPlayer video;
        Animator anim;
        Image fadeImage;

        AsyncOperation operation;


        [Header("Scene Management")]
        public string[] scenesToLoad;
        public string activeScene;

        [Space]
        [Header("UI Settings")]

        public bool useFade;
        public GameObject fadeOverlay;
        public GameObject ControlUI;
        //public GameObject LoadingUI;



        //make sure that we only have a single instance of the game manager
        
        void Awake()
        {
            if (instance == null)
            {
                
                DontDestroyOnLoad(gameObject); //(we want to recreate temporarily)
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject); //gameObject is the object this is attached to
            }
            
        }

        
        //set the initial active scene
        void Start()
        {
            scene = SceneManager.GetActiveScene();
            activeScene = scene.buildIndex + " - " + scene.name;
            BindHotSpotsInScenes();
        }


        //Select scene is called from either the menu manager or hotspot manager, and is used to load the desired scene
        public void SelectScene(string sceneToLoad)
        {
            //if we want to use the fading between scenes, start the coroutine here
            if (useFade)
            {
                StartCoroutine(FadeOutAndIn(sceneToLoad));
                
            }
            //if we dont want to use fading, just load the next scene
            else
            {
                SceneManager.LoadScene(sceneToLoad);
            }
            //set the active scene to the next scene
            activeScene = sceneToLoad;
            
        }

        //make method that adds the scenes linked to the new scene (by default it should have the last active scene added )

        /*
        // Each button will match up to a respective scene. Button 1 in the Menu Manager will match up to Scene 1 in the Scenes Manager
        public void BindHotSpotsToScenes()
        {

            if (m_hotSpotsInScene.Length != GameManagerDev.instance.scenesToLoad.Length)
            {
                Debug.Log("Amount of hotspots and scenes do not match!");
                return;
            }
            else
            {
                for (int i = 0; i < m_hotSpotsInScene.Length; i++)
                {
                    string sceneName = GameManagerDev.instance.scenesToLoad[i];
                    m_hotSpotsInScene[i].onClick.AddListener(() => GameManagerDev.instance.SelectScene(sceneName));

                }
            }

        }

        */

        public void BindHotSpotsInScenes()
        {
            ControlUI = HotspotManagerDev.instance.gameObject;
            for (int i = 0; i < HotspotManagerDev.instance.m_hotSpotButtons.Length; i++)
            {
                Debug.Log(HotspotManagerDev.instance.m_hotSpotButtons.Length);
                //remember to add get and set methods for the hotSpotButtons class
                int n = i;
                HotspotManagerDev.instance.m_hotSpotButtons[i].hotSpot.onClick.AddListener(() => GameManagerDev.instance.SelectScene(HotspotManagerDev.instance.m_hotSpotButtons[n].sceneObject));
            } 
            
        }


        IEnumerator FadeOutAndIn(string sceneToLoad)
        {
            //get references to animatior and image component 
            anim = fadeOverlay.GetComponent<Animator>();
            fadeImage = fadeOverlay.GetComponent<Image>();

            //turn control UI off and loading UI on
            ControlUI.SetActive(false);
            //LoadingUI.SetActive(true);

            //set FadeOut to true on the animator so our image will fade out
            anim.SetBool("FadeOut", true);

            //wait until the fade image is entirely black (alpha=1) then load next scene
            yield return new WaitUntil(() => fadeImage.color.a == 1);
            SceneManager.LoadScene(sceneToLoad);
            Scene scene = SceneManager.GetSceneByName(sceneToLoad);
            Debug.Log("loading scene:" + scene.name);
            yield return new WaitUntil(() => scene.isLoaded);

            // grab video and wait until it is loaded and prepared before starting the fade out
            video = FindObjectOfType<VideoPlayer>();
            yield return new WaitUntil(() => video.isPrepared);

            //set FadeOUt to false on the animator so our image will fade back in 
            anim.SetBool("FadeOut", false);

            //wait until the fade image is completely transparent (alpha = 0) and then turn loading UI off and control UI back on
            yield return new WaitUntil(() => fadeImage.color.a == 0);
            //LoadingUI.SetActive(false);

            //if we have not destroyed the control UI, set it to active
            //if (ControlUI)
            // 

            BindHotSpotsInScenes();
            ControlUI.SetActive(true);
        }

        //Find the video in the scene and pause it
        public void PauseVideo()
        {
            if (!video)
            {
                video = FindObjectOfType<VideoPlayer>();
            }
            video.Pause();
        }

        //Find the video in the scene and play it
        public void PlayVideo()
        {
            if (!video)
            {
                video = FindObjectOfType<VideoPlayer>();
            }
            video.Play();
        }
    }
}

