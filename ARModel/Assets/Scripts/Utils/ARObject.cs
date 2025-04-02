using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
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

        private string objectName;
        private DateTime instantiateTime;

        private bool objectIsSelected = false;

        [Header("ARNtes components")]
        // arNote of the object
        [SerializeField]
        private GameObject aRNote;
        [SerializeField]
        private TMP_Text textTitle;
        [SerializeField]
        private TMP_Text textContent;

        [SerializeField]
        private Object objectType;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the type of this object
        /// </summary>
        public Object ObjectType
        {
            get { return objectType; }
        }

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
            // inform this object was created
            ARInteractionsManager.Instance.ObjectAdded(this.gameObject, objectType);

            // activate ARNotes if Menu showARNotes is activated
            ShowHideARNote(MenuManager.Instance.ShowingARNotes);

            // add rigidBody if Menu physics is activated
            if (MenuManager.Instance.PhysicsActivated)
            {
                Rigidbody rg =transform.AddComponent<Rigidbody>();
                rg.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }

            // set name and time that was created
            int numberOfObjectsOfThisType = ARInteractionsManager.Instance.NumberOfObjectsByType(this.ObjectType);

            string originalPrefabName = gameObject.name.Replace("(Clone)", "").Trim();
            objectName = originalPrefabName + " " + numberOfObjectsOfThisType.ToString();
            instantiateTime = DateTime.Now;

            //Set into title and textContent in ARNote
            textTitle.text = objectName;
            textContent.text = "The " + objectName + " object was added to the world at " + instantiateTime.ToString("HH:mm:ss") + ".";

            // set active or not if Menu showing objects is activated
            this.gameObject.SetActive(MenuManager.Instance.ShowingARPlanesMenu);

            // listen to when the object is selected
            aRSelectionInteractable.selectEntered.AddListener(OnObjectSelected);

            // listen to when the object is unselected
            aRSelectionInteractable.selectExited.AddListener(OnObjectSelecteExited);

        }

        /// <summary>
        /// Handle when this object is selected
        /// </summary>
        /// <param name="args"></param>
        private void OnObjectSelected(SelectEnterEventArgs args)
        {
            objectIsSelected = !objectIsSelected;

            ARInteractionsManager.Instance.ObjectWasSelected(this.gameObject);

            // to fix the bug, manually set arnote based on instructionsManager.showingARNotesMenu
            bool showingARNotesMenu = MenuManager.Instance.ShowingARNotes;
            ARInteractionsManager.Instance.ShowHideARNotes(showingARNotesMenu);
            //aRNote.SetActive(showingARNotesMenu);
        }

        /// <summary>
        /// Handle when this object is unselected
        /// </summary>
        /// <param name="args"></param>
        private void OnObjectSelecteExited(SelectExitEventArgs args)
        {
            ARInteractionsManager.Instance.ObjectWasExited(this.gameObject);

            // to fix the bug, manually set arnote based on instructionsManager.showingARNotesMenu
            bool showingARNotesMenu = MenuManager.Instance.ShowingARNotes;
            ARInteractionsManager.Instance.ShowHideARNotes(showingARNotesMenu);
            //aRNote.SetActive(showingARNotesMenu);
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
