namespace AroaroMixedReality
{
    using Photon.Pun;
    using Photon.Realtime;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Defines the <see cref="PUNNetworkManager" />
    /// </summary>
    public class PUNNetworkManager : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Defines the gameVersion
        /// </summary>
        private string gameVersion = "2.0";

        /// <summary>
        /// Defines the avatar
        /// </summary>
        public GameObject avatar;
        public GameObject cameraToFollow;

   

        /// <summary>
        /// The OnConnectedToMaster
        /// </summary>
        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to master and now join " + SceneManager.GetActiveScene().name);
            PhotonNetwork.JoinRoom(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// The OnJoinRoomFailed
        /// </summary>
        /// <param name="returnCode">The returnCode<see cref="short"/></param>
        /// <param name="message">The message<see cref="string"/></param>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Joined room failed and now will create and join " + SceneManager.GetActiveScene().name);
            PhotonNetwork.CreateRoom(SceneManager.GetActiveScene().name, new RoomOptions(), TypedLobby.Default);
        }

        /// <summary>
        /// The OnJoinedRoom
        /// </summary>
        public override void OnJoinedRoom()
        {
            
            Debug.Log("Joined room success");
            Color randomColor = Random.ColorHSV();
            GameObject player = PhotonNetwork.Instantiate(avatar.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0, new object[] { randomColor.r, randomColor.g, randomColor.b, randomColor.a });
            player.GetComponent<AttachPlayerModelScript>().playerCameraToFollow = cameraToFollow;
            
        }

        /// <summary>
        /// The Awake
        /// </summary>
        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// <summary>
        /// The Start
        /// </summary>
        internal void Start()
        {
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnectedAndReady)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                Debug.Log("Is connected and now join " + SceneManager.GetActiveScene().name);
                PhotonNetwork.JoinRoom(SceneManager.GetActiveScene().name);
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }
}
