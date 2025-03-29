using JetBrains.Annotations;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Utils
{
    /// <summary>
    /// Checks and returns properties concerning the device properties
    /// </summary>
    public class DeviceProperties : MonoBehaviour
    {
        #region fields
        private static DeviceProperties instance;
        private bool supportsAR = false;

        #endregion

        #region properties

        /// <summary>
        /// Property to acess the Singletion instance of DeviceProperties
        /// </summary>
        public static DeviceProperties Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Checks and returns if device supports automatic plane classification
        /// </summary>
        public bool SupportsPlaneClassification
        {
            get
            {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)

                ARPlaneManager aRPlaneManager = GameObject.FindObjectOfType<ARPlaneManager>();

                if (aRPlaneManager != null)
                {
                    return aRPlaneManager.descriptor.supportsClassification;
                }
                else
                {
                    Debug.LogError("ARPlaneManager was not found.");
                }

#endif
                return false;
            }
        }

        /// <summary>
        /// Checks and returns if device supports Occlusion
        /// </summary>
        public bool SupportsOcclusion
        {
            get
            {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)

                AROcclusionManager aROcclusionManager = GameObject.FindObjectOfType<AROcclusionManager>();

                if (aROcclusionManager != null)
                {
                    return aROcclusionManager.descriptor.environmentDepthImageSupported == Supported.Supported;
                }
                else
                {
                    Debug.LogError("AROclusionManager was not found.");
                }
#endif
                return false;
            }
        }

        #endregion

        #region private methods

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

        /// <summary>
        /// Checks ARavailabilty and sets supportsAR
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckARAvailability(Action<bool> callback)
        {
            yield return ARSession.CheckAvailability();
            supportsAR = ARSession.state != ARSessionState.Unsupported;
            callback.Invoke(supportsAR);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Returns if the current device supports AR features
        /// </summary>
        public void SupportsAR(Action<bool> callback)
        {
            StartCoroutine(CheckARAvailability(callback));
        }

        #endregion
    }
}