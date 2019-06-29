namespace AroaroMixedReality
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Photon.Pun;
    using System;
    
    using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

    public class WriteTool : MonoBehaviour
    {

        [SerializeField]
        private Color paintColor;

        [Range(1, 10)]
        public int lineSize;

        private Drawable previousTouchingCanvas;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Transform penTipTransform;
        private ControllerFinder controllerLocator;
        private bool isHeld;

        public bool IsHeld
        {
            get { return isHeld; }
            set
            {
                isHeld = value;

            }
        }

        // declare PenColor property accessors
        // note that the tip of the paint tool will reflect the current color chosen
        public Color PaintColor
        {
            get {
                return paintColor; } //returns the private paintColor properties
            set
            {
                paintColor = value; //contextual keyword used in set accessor - similar to input parameter on a method
                penTipTransform.gameObject.GetComponent<Renderer>().material.color = paintColor;
                // penEndTransform.gameObject.GetComponent<Renderer>().material.color = penColor; probably don't need this
            }
        }



        internal void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<Drawable>() != null)
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }



        internal void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.GetComponent<Drawable>() != null)
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        // Awake is called when the script instance is being loaded - use to initialise variables and game state before the game starts, called once after all objects are intialised
        // this is where you should speak to other objects, i.e. using GameObject.FindWithTag - use to set up references between scripts
        private void Awake()
        {
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            penTipTransform = transform.Find("Tip");
            PaintColor = paintColor;

        }

        // Start is called before the first frame update
        void Start()
        {
            isHeld = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (InUse() && Physics.Raycast(penTipTransform.position, transform.up, out RaycastHit hit))
            {
                Drawable canvas = hit.collider.gameObject.GetComponent<Drawable>();

                // Return if pen is not touching a drawable element
                if (canvas == null) return;

                previousTouchingCanvas = canvas;

                // Convert Color to Color32 as it can be more efficiently sent over the network (apparently)
                Color32 penColor32 = (Color32)PaintColor;
                byte r = penColor32.r;
                byte g = penColor32.g;
                byte b = penColor32.b;
                byte a = penColor32.a;

                PhotonView.Get(canvas).RPC(nameof(canvas.Draw), RpcTarget.AllBufferedViaServer, gameObject.GetInstanceID(), new Vector2(hit.textureCoord.x, hit.textureCoord.y), lineSize, r, g, b, a);
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


        public void PenHeldManipulatedStart()
        {
            Debug.Log("penheld");
            IsHeld = true;
        }

        public void PenDroppedManipulation()
        {
            IsHeld = false;
            Debug.Log("pendroppped");
        }

        public bool InUse()
        {
            return IsHeld;
        }


    }
}

