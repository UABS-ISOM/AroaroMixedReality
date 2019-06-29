namespace AroaroMixedReality
{
    using ExitGames.Client.Photon;
    using Photon.Pun;
    using Photon.Realtime;
    using UnityEngine;

    /// <summary>
    /// Defines the <see cref="PUNEventsHandler" />
    /// </summary>
    public class PUNEventsHandler : MonoBehaviour, IOnEventCallback
    {
        /// <summary>
        /// Defines the EventCode
        /// </summary>
        public enum EventCode : byte
        {
            /// <summary>
            /// Defines the InstantiateSceneObject
            /// </summary>
            InstantiateSceneObject
        }

        /// <summary>
        /// The OnEnable
        /// </summary>
        public void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        /// <summary>
        /// The OnDisable
        /// </summary>
        public void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        /// <summary>
        /// The OnEvent
        /// </summary>
        /// <param name="photonEvent">The photonEvent<see cref="EventData"/></param>
        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            if (eventCode == (byte)EventCode.InstantiateSceneObject)
            {
                object[] data = (object[])photonEvent.CustomData;

                string name = (string)data[0];
                Vector3 position = (Vector3)data[1];
                Quaternion rotation = (Quaternion)data[2];

                PhotonNetwork.InstantiateSceneObject(name, position, rotation);
            }
        }


    }
}
