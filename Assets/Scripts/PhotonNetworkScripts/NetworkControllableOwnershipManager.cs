
 namespace AroaroMixedReality
{
    using System.Collections;
    using System.Collections.Generic;
    using Photon.Pun;
    using Photon.Realtime;
    using UnityEngine;

    public class NetworkControllableOwnershipManager : MonoBehaviour, IPunOwnershipCallbacks
    {
        public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
        {
            if (targetView.gameObject.GetComponent<NetworkControllableObject>() != null)
                targetView.TransferOwnership(requestingPlayer);
        }

        public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
        {
            NetworkControllableObject targetInteractableObject = targetView.gameObject.GetComponent<NetworkControllableObject>();
            if (targetInteractableObject == null) return;
            if (targetView.OwnerActorNr == 0 || targetView.IsMine)
            {
                targetInteractableObject.isControllable = true;
            }
            else
            {
                targetInteractableObject.isControllable = false;
                targetInteractableObject.NotInteractedOn();
            }
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        // Update is called once per frame
        void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }
}

