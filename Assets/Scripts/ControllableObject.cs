 /*namespace Aroaro
{
    using Photon.Pun;
    using UnityEngine;
    //using VRTK;
    //using VRTK.GrabAttachMechanics;

    /// <summary>
    /// Defines the <see cref="ControllableObject" />
    /// </summary>
    ///
   
    public class ControllableObject : VRTK_InteractableObject
    {
        /// <summary>
        /// Defines the displayObjectMenu
        /// </summary>
        [Header("Additional Options")]
        public bool displayObjectMenu = false;

        /// <summary>
        /// Defines the photonView
        /// </summary>
        private PhotonView photonView;

        /// <summary>
        /// The DestroyObject
        /// </summary>
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

        /// <summary>
        /// The DestroyNetworkedObject
        /// </summary>
        [PunRPC]
        private void DestroyNetworkedObject()
        {
            if (photonView.IsMine) PhotonNetwork.Destroy(gameObject);
        }

        /// <summary>
        /// The SetupDefaultGrabMechanic
        /// </summary>
        private void SetupDefaultGrabMechanic()
        {
            VRTK_ChildOfControllerGrabAttach primaryGrabMechanic = gameObject.AddComponent<VRTK_ChildOfControllerGrabAttach>();
            primaryGrabMechanic.precisionGrab = true;
            grabAttachMechanicScript = primaryGrabMechanic;
        }

        /// <summary>
        /// The Grabbed
        /// </summary>
        /// <param name="currentGrabbingObject">The currentGrabbingObject<see cref="VRTK_InteractGrab"/></param>
        public override void Grabbed(VRTK_InteractGrab currentGrabbingObject = null)
        {
            base.Grabbed(currentGrabbingObject);
            if (photonView != null)
                photonView.RequestOwnership();
        }

        /// <summary>
        /// The Ungrabbed
        /// </summary>
        /// <param name="previousGrabbingObject">The previousGrabbingObject<see cref="VRTK_InteractGrab"/></param>
        public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject = null)
        {
            base.Ungrabbed(previousGrabbingObject);
            if (photonView != null)
                photonView.TransferOwnership(0);
        }

        /// <summary>
        /// The Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            photonView = gameObject.GetComponent<PhotonView>();
        }

        /// <summary>
        /// The Start
        /// </summary>
        internal void Start()
        {
            if (displayObjectMenu)
            {
                GameObject objectMenu = Resources.Load<GameObject>("ObjectMenu");
                ObjectMenu objectMenuScript = objectMenu.GetComponent<ObjectMenu>();
                objectMenuScript.targetGameObject = transform.gameObject;
                Instantiate(objectMenu, transform.position + new Vector3(0, transform.localScale.z, 0), transform.rotation, transform);
            }

            // Setup default grab mechanic if none is specified
            if (grabAttachMechanicScript == null)
                SetupDefaultGrabMechanic();
        }
    }
}
*/
