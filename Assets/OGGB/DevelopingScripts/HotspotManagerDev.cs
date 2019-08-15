using UnityEngine;
using UnityEngine.UI;

namespace Interactive360
{

    //This script should be attached to an empty game object in any scene utilizing hotspots. It binds all hotspots in the scene to the scenes that they are supposed to load.
    public class HotspotManagerDev : MonoBehaviour
    {
        public static HotspotManagerDev instance = null;
        //public Button[] m_hotSpotsInScene; //A reference to all the hotspots in the scene that would load new scenes
        public SceneButton[] m_hotSpotButtons;
        //private readonly string[] m_Scenes;
        public GameObject[] gameObjectsss;
        public GameObject m_playButton; //A reference to the button that toggles the video content to play
        public GameObject m_pauseButton; //A reference to the button that toggle the video content to pause

        [System.Serializable]
        public class SceneButton
        {
            public Button hotSpot;
            public string sceneObject;
        }


        void Awake()
        {
            if (instance == null)
            {
                //DontDestroyOnLoad(gameObject); //(we want to recreate temporarily)
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject); //gameObject is the object this is attached to
            }
        }

    

        
        // on Start, bind all buttons to their respective scenes and call DontDestroyOnLoad to keep the Main Menu in every scene
        void Start()
        {
            //BindHotSpotsToScenes();
             
        }
        void Update()
        {
            checkForInput();
        }

        
        //If we detect input, call the toggleMenu method 
        private void checkForInput()
        {
            
        }

        //Toggle between showing play and pause button when once is pressed
        public void toggleButton()
        {

            m_pauseButton.SetActive(!m_pauseButton.activeInHierarchy);
            m_playButton.SetActive(!m_playButton.activeInHierarchy);

        }

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


    }
}

