namespace AroaroMixedReality
{
    
    using UnityEngine;
    using Photon.Pun;
    using ExitGames.Client.Photon;
    using Photon.Realtime;
    [RequireComponent(typeof(PhotonView))]
    public class AttachPlayerModelScript : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Attaches the player model objects to the players VR camera rig. Check for offline use.
        /// </summary>

        public GameObject playerCameraToFollow;
        public GameObject playerHead;
        public GameObject playerLeftHand;
        public GameObject playerRightHand;

        private object[] instantiationData;


        internal void Awake()
        {
            instantiationData = gameObject.GetComponent<PhotonView>().InstantiationData;
            
        }

        // Start is called before the first frame update
        internal void Start()
        {
            /// exit logic if event does not belong to this player
            SetupAvatarColor();
            if (photonView.IsMine)
            {
                // Hide gaze pointer for local avatar by default
                

                // Make avatar invisible as we don't need to see our own avatar
                //playerHead.GetComponent<MeshRenderer>().enabled = false;
                
                //foreach (MeshRenderer r in playerHead.GetComponentsInChildren<MeshRenderer>())
                //{
                   // r.enabled = false;
                //}

                // Disable menu & menu activation collider as it's only used for remote player
                
                gameObject.GetComponent<Collider>().enabled = false;
            }
            else
            {
                // Get current gaze pointer state of remote user
                //ToggleGazePointer((bool)photonView.Owner.CustomProperties[CustomPlayerProperties.GazePointerState]);
            }


        }
        private void SetupAvatarColor()
        {
            Color avatarColor = new Color((float)instantiationData[0], (float)instantiationData[1], (float)instantiationData[2], (float)instantiationData[3]);
            playerHead.GetComponent<Renderer>().material.color = avatarColor;
            
        }
        

        // Update is called once per frame
        void Update()
        {
            if (!photonView.IsMine) return;
            playerHead.transform.position = playerCameraToFollow.transform.position;
            playerHead.transform.rotation = playerCameraToFollow.transform.rotation;

        }
    }
}

