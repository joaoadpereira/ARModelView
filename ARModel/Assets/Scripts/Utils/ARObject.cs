using Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        // arNote of the object
        [SerializeField]
        private GameObject aRNote;

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
            // inform this object was created
            ARInteractionsManager.Instance.ObjectAdded(this.gameObject);

            // activate ARNotes if Menu showARNotes is activated
            ShowHideARNote(InstructionsManager.Instance.ShowingARNotes);

            // add rigidBody if Menu physics is activated
            if (InstructionsManager.Instance.PhysicsActivated)
            {
                transform.AddComponent<Rigidbody>();
            }

            // set active or not if Menu showing objects is activated
            this.gameObject.SetActive(InstructionsManager.Instance.ShowingAllObjectsMenu);
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

        /// <summary>
        /// Shows or hides the ARNote of this object
        /// </summary>
        /// <param name="state"></param>
        public void ShowHideARNote(bool state)
        {
            aRAnnotationInteractable.enabled = state;
            aRNote.SetActive(state);
        }

        #endregion
    }
}
