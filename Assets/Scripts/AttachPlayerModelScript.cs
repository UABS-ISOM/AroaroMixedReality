using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace AroaroMixedReality
{
    public class AttachPlayerModelScript : MonoBehaviourPun
    {
        /// <summary>
        /// Attaches the player model objects to the players VR camera rig. Check for offline use.
        /// </summary>

        public GameObject playerCameraToFollow;
        public GameObject playerHead;
        public GameObject playerLeftHand;
        public GameObject playerRightHand;



        internal void Awake()
        {

        }
        // Start is called before the first frame update
        void Start()
        {
            /// exit logic if event does not belong to this player



        }

        private void OnEnable()
        {

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
