namespace AroaroMixedReality
{
    using System;
    using Microsoft.MixedReality.Toolkit.UI; //using this to replace VRTK components
    using Photon.Pun;
    using UnityEngine;
    ///using VRTK; must replace VRTK components with window component

    /// <summary>
    /// Defines the <see cref="Pen" />
    /// </summary>
    public class Pen : Interactable
    {
        /// <summary>
        /// Defines the penColor
        /// </summary>
        [SerializeField]
        private Color penColor;

        /// <summary>
        /// Defines the penSize
        /// </summary>
        [Range(1, 10)]
        public int penSize;

        /// <summary>
        /// Defines the resetTransformOnDrop
        /// </summary>
        public bool resetTransformOnDrop;

        /// <summary>
        /// The parent GameObject or "holder" of this pen
        /// </summary>
        [Tooltip("Default to the GameObject this script is attached to")]
        public Transform originalTransform;

        /// <summary>
        /// Defines the penTipTransform
        /// </summary>
        private Transform penTipTransform;

        /// <summary>
        /// Defines the penEndTransform
        /// </summary>
        private Transform penEndTransform;

        /// <summary>
        /// Defines the originalPosition
        /// </summary>
        private Vector3 originalPosition;

        /// <summary>
        /// Defines the originalRotation
        /// </summary>
        private Quaternion originalRotation;

        /// <summary>
        /// Defines the previousTouchingCanvas
        /// </summary>
        private Drawable previousTouchingCanvas;

        /// <summary>
        /// Gets or sets the PenColor
        /// </summary>
        public Color PenColor
        {
            get { return penColor; }
            set
            {
                penColor = value;
                penTipTransform.gameObject.GetComponent<Renderer>().material.color = penColor;
                penEndTransform.gameObject.GetComponent<Renderer>().material.color = penColor;
            }
        }

        /// <summary>
        /// The Grabbed
        /// </summary>
        /// <param name="currentGrabbingObject">The currentGrabbingObject<see cref="VRTK_InteractGrab"/></param>
        /// 
        /*
        public override void Grabbed(VRTK_InteractGrab currentGrabbingObject = null)
        {
            base.Grabbed(currentGrabbingObject);
            transform.parent = null;
        }

        */

        /// <summary>
        /// The Ungrabbed
        /// </summary>
        /// <param name="previousGrabbingObject">The previousGrabbingObject<see cref="VRTK_InteractGrab"/></param>

        /*
        public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject = null)
        {
            base.Ungrabbed(previousGrabbingObject);
            if (!resetTransformOnDrop) return;
            if (originalTransform != null)
            {
                transform.position = originalTransform.position;
                transform.rotation = originalTransform.rotation;
                transform.parent = originalTransform;
            }
            else
            {
                transform.position = originalPosition;
                transform.rotation = originalRotation;
            }
        }

        */

        /// <summary>
        /// The OnCollisionEnter
        /// </summary>
        /// <param name="collision">The collision<see cref="Collision"/></param>
        internal void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<Drawable>() != null)
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }

        /// <summary>
        /// The OnCollisionExit
        /// </summary>
        /// <param name="collision">The collision<see cref="Collision"/></param>
        internal void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.GetComponent<Drawable>() != null)
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        /// <summary>
        /// The Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            penTipTransform = transform.Find("Tip");
            penEndTransform = transform.Find("End");
            PenColor = penColor;
        }

        /// <summary>
        /// The Update
        /// </summary>
        
        
        protected override void Update()
        {
            base.Update();
            if (IsUsing() && Physics.Raycast(penTipTransform.position, transform.up, out RaycastHit hit))
            {
                Drawable canvas = hit.collider.gameObject.GetComponent<Drawable>();

                // Return if pen is not touching a drawable element
                if (canvas == null) return;

                previousTouchingCanvas = canvas;

                // Convert Color to Color32 as it can be more efficiently sent over the network
                Color32 penColor32 = (Color32)PenColor;
                byte r = penColor32.r;
                byte g = penColor32.g;
                byte b = penColor32.b;
                byte a = penColor32.a;

                PhotonView.Get(canvas).RPC(nameof(canvas.Draw), RpcTarget.AllBufferedViaServer, gameObject.GetInstanceID(), new Vector2(hit.textureCoord.x, hit.textureCoord.y), penSize, r, g, b, a);
            }
            else
            {
                if (previousTouchingCanvas != null)
                {
                    PhotonView.Get(previousTouchingCanvas).RPC(nameof(previousTouchingCanvas.EndStroke), RpcTarget.AllBufferedViaServer, gameObject.GetInstanceID());
                    previousTouchingCanvas = null;
                }
            }
        }

        private bool IsUsing()
        {
            throw new NotImplementedException();
        }
    }
}
