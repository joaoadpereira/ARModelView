using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;

namespace Managers 
{ 
    /// <summary>
    /// Manages the voice command controllers
    /// </summary>
    public class VoiceCommandsManager : MonoBehaviour
    {
        #region fields
        private static VoiceCommandsManager instance;

        // handle speech Recognizion
        

        #endregion

        #region properties

        /// <summary>
        /// Property to access the Singleton instance of VoiceCommandsManager
        /// </summary>
        public static VoiceCommandsManager Instance
        {
            get { return instance; }
        }

        #endregion

        private void Awake()
        {
            // Set the current instance as the singleton or destroy case duplicated
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        #region private methods
        // Start is called before the first frame update
        void Start()
        {
            // Check if permission is granted
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                // Request permission
                Permission.RequestUserPermission(Permission.Microphone);
            }

           
        }

        #endregion
    }
}
