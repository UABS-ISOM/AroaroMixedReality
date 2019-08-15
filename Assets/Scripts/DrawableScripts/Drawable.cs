namespace AroaroMixedReality
{
    using Photon.Pun;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Defines the <see cref="Drawable" />
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class Drawable : MonoBehaviour
    {
        /// <summary>
        /// Defines the textureWidth
        /// </summary>
        private int textureWidth;

        /// <summary>
        /// Defines the textureHeight
        /// </summary>
        private int textureHeight;

        /// <summary>
        /// Defines the texture
        /// </summary>
        private Texture2D texture;
        private PhotonView photonView;
        /// <summary>
        /// Defines the previousPositions
        /// </summary>
        private Dictionary<int, Vector2> previousPositions = new Dictionary<int, Vector2>();

        /// <summary>
        /// The Draw
        /// </summary>
        /// <param name="penId">The penId<see cref="int"/></param>
        /// <param name="position">The position<see cref="Vector2"/></param>
        /// <param name="penSize">The penSize<see cref="int"/></param>
        /// <param name="r">The r<see cref="byte"/></param>
        /// <param name="g">The g<see cref="byte"/></param>
        /// <param name="b">The b<see cref="byte"/></param>
        /// <param name="a">The a<see cref="byte"/></param>


        [PunRPC]
        public void Draw(int penId, Vector2 position, int penSize, byte r, byte g, byte b, byte a)
        {
            Color32 penColor = new Color32(r, g, b, a);
            int x = Mathf.RoundToInt(position.x * texture.width - (penSize / 2));
            int y = Mathf.RoundToInt(position.y * texture.height - (penSize / 2));
            Color32[] colors = Enumerable.Repeat<Color32>(penColor, penSize * penSize).ToArray<Color32>();

            texture.SetPixels32(x, y, penSize, penSize, colors);

            Vector2 previousPosition;
            if (previousPositions.TryGetValue(penId, out previousPosition))
            {
                for (float t = 0.01f; t < 1.00f; t += 0.01f)
                {
                    int lerpX = Mathf.RoundToInt(Mathf.Lerp(previousPosition.x, (float)x, t));
                    int lerpY = Mathf.RoundToInt(Mathf.Lerp(previousPosition.y, (float)y, t));
                    texture.SetPixels32(lerpX, lerpY, penSize, penSize, colors);
                }
            }
            previousPositions[penId] = new Vector2(x, y);
        }

        /// <summary>
        /// The EndStroke
        /// </summary>
        /// <param name="penId">The penId<see cref="int"/></param>
        [PunRPC]
        public void EndStroke(int penId)
        {
            previousPositions.Remove(penId);
        }

        private void Awake()
        {
            photonView = gameObject.GetComponent<PhotonView>();
        }

        /// <summary>
        /// The OnCollisionExit
        /// </summary>
        /// <param name="collision">The collision<see cref="Collision"/></param>
        internal void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.GetComponent<WriteTool>() != null) gameObject.GetComponent<PhotonView>().RPC("EndStroke", RpcTarget.AllBufferedViaServer, collision.gameObject.GetInstanceID());
        }

        /// <summary>
        /// The Start
        /// </summary>
        internal void Start()
        {
            float localScaleX = transform.localScale.x;
            float localScaleY = transform.localScale.y;
            if (localScaleY > localScaleX)
            {
                textureWidth = 1024;
                textureHeight = Mathf.RoundToInt(textureWidth * localScaleY / localScaleX);
            }
            else
            {
                textureHeight = 1024;
                textureWidth = Mathf.RoundToInt(textureWidth * localScaleX / localScaleY);
            }
            texture = new Texture2D(textureWidth, textureHeight);
            GetComponent<Renderer>().material.mainTexture = (Texture)texture;
            texture.Apply();
        }

        /// <summary>
        /// The Update
        /// </summary>
        internal void Update()
        {
            if (previousPositions.Count() != 0) texture.Apply();
        }
    }
}
