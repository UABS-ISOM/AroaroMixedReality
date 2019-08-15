/*namespace Aroaro

{
    using Photon.Pun;
    using Photon.Realtime;
    using UnityEngine;
    using VRTK;

    /// <summary>
    /// Defines the <see cref="InteractableObjectOwnershipManager" />
    /// </summary>
    public class InteractableObjectOwnershipManager : MonoBehaviour, IPunOwnershipCallbacks
    {
        /// <summary>
        /// The OnOwnershipRequest
        /// </summary>
        /// <param name="targetView">The targetView<see cref="PhotonView"/></param>
        /// <param name="requestingPlayer">The requestingPlayer<see cref="Player"/></param>
        public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
        {
            if (targetView.gameObject.GetComponent<VRTK_InteractableObject>() != null)
                targetView.TransferOwnership(requestingPlayer);
        }

        /// <summary>
        /// The OnOwnershipTransfered
        /// </summary>
        /// <param name="targetView">The targetView<see cref="PhotonView"/></param>
        /// <param name="previousOwner">The previousOwner<see cref="Player"/></param>
        public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
        {
            VRTK_InteractableObject targetInteractableObject = targetView.gameObject.GetComponent<VRTK_InteractableObject>();
            if (targetInteractableObject == null) return;
            if (targetView.OwnerActorNr == 0 || targetView.IsMine)
            {
                targetInteractableObject.isGrabbable = true;
            }
            else
            {
                targetInteractableObject.isGrabbable = false;
                targetInteractableObject.ForceStopInteracting();
            }
        }

        /// <summary>
        /// The OnEnable
        /// </summary>
        internal void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        /// <summary>
        /// The OnDisable
        /// </summary>
        internal void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }
}

*/
