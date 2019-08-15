namespace AroaroMixedReality
{
    using System.Collections;
    using System.Collections.Generic;
    using Photon.Pun;
    using UnityEngine;
    

    public class NetworkControllableObject : MonoBehaviour
    {
        private PhotonView photonView;
        public bool isControllable;

        public void DestroyObject()
        {
            if (photonView != null)
            {
                photonView.RPC("DestroyNetworkedObject", RpcTarget.All);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        [PunRPC]
        private void DestroyNetworkedObject()
        {
            if (photonView.IsMine) PhotonNetwork.Destroy(gameObject);
        }

        public void IsInteractedOn()
        {
            if (photonView != null)
                photonView.RequestOwnership();
        }

        public void NotInteractedOn()
        {
            if (photonView != null)
            {
                photonView.TransferOwnership(0); //transfer ownership back to scene
            }
                
        }

        
        private void Awake()
        {
            photonView = gameObject.GetComponent<PhotonView>();
        }

        // Start is called before the first frame update
        void Start()
        {
            

           
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
