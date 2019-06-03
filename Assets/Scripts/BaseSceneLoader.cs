namespace AroaroMixedReality
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Defines the <see cref="ViewLoader" />
    /// </summary>
    public class BaseSceneLoader : MonoBehaviour
    {
        /// <summary>
        /// Defines the BaseScene
        /// </summary>
        private string BaseScene = "BaseScene";

        /// <summary>
        /// The Start
        /// </summary>
        internal void Start()
        {
            if (!GameObject.Find(BaseScene)) SceneManager.LoadScene(BaseScene, LoadSceneMode.Additive);
        }
    }
}
