using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace utils
{
    [RequireComponent(typeof(ARRotationInteractable))]
    [RequireComponent(typeof(ARScaleInteractable))]
    [RequireComponent(typeof(ARSelectionInteractable))]
    [RequireComponent(typeof(ARTranslationInteractable))]
    [RequireComponent(typeof(ARAnnotationInteractable))]
    /// <summary>
    /// Handles the AR object added into the scene
    /// </summary>
    public class ARObject : MonoBehaviour
    {
        #region fields

        // define all manipulative components 
        private ARRotationInteractable aRRotationInteractable;
        private ARScaleInteractable aRScaleInteractable;
        private ARSelectionInteractable aRSelectionInteractable;
        private ARTranslationInteractable aRTranslationInteractable;
        private ARAnnotationInteractable aRAnnotationInteractable;

        #endregion

        #region private methods

        private void Awake()
        {
            aRRotationInteractable = GetComponent<ARRotationInteractable>();
            aRScaleInteractable = GetComponent <ARScaleInteractable>();
            aRSelectionInteractable = GetComponent<ARSelectionInteractable>();
            aRTranslationInteractable = GetComponent<ARTranslationInteractable>();   
            aRAnnotationInteractable = GetComponent<ARAnnotationInteractable>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            // communicates that this object was added into the scene
            ObjectWasInstantiated();
        }

        /// <summary>
        /// Commmunicates with ARInteractionsManager that this object was added into the scene
        /// </summary>
        private void ObjectWasInstantiated()
        {
            ARInteractionsManager.Instance.ObjectAdded(this.gameObject);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Enables or disables the object interaction
        /// </summary>
        /// <param name="state"></param>
        public void ChangeObjectInteraction(bool state)
        {
            aRRotationInteractable.enabled = state;
            aRScaleInteractable.enabled = state;
            aRSelectionInteractable.enabled = state;
            aRTranslationInteractable.enabled = state;
            aRAnnotationInteractable.enabled = state;   
        }

        #endregion
    }
}
