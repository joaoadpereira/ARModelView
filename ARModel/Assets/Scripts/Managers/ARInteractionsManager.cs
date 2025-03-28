using Enums;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AR;
using utils;

namespace Managers 
{ 
    /// <summary>
    /// Handles the placing, moving, rotation and scaling of AR objects with AR planes
    /// </summary>
    public class ARInteractionsManager : MonoBehaviour
    {
        #region fields
        private static ARInteractionsManager instance;

        // condition to enable AR interaction 
        private bool canARTouch = false;

        // condition case an object has been selected
        private bool objectHasBeenSelected = false;

        // keep track of all objects added into the scene
        List<GameObject> objectsInScene = new List<GameObject>();
        
        [Header("AR Placement Interactablein scene")]
        [SerializeField]
        private ARPlacementInteractable aRPlacementInteractable;

        [Header("Object to Add in AR")]
        [SerializeField]
        private GameObject objectToAR;
        [SerializeField]
        private Material selectedMaterial;

        #endregion

        #region properties

        /// <summary>
        /// Property to access the Singletion instance of ARInteractionsManager
        /// </summary>
        public static ARInteractionsManager Instance
        {
            get { return instance; }
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

        private void Start()
        {
            // add method to listen to app state change
            AppManager.Instance.StateChanged += EnableObjectPlacing;

            // add Object to instantiate into ARPlacementinteractable
            aRPlacementInteractable.placementPrefab = objectToAR;
        }

        /// <summary>
        /// Enables the user interaction to add objects into planes
        /// Enables user to select objects
        /// </summary>
        private void EnableObjectPlacing(AppState appState)
        {
            bool canAddAndSelectObjects = appState == AppState.Idle;
            bool userSelectedAnObject = appState == AppState.ObjectSelected;

            canARTouch = canAddAndSelectObjects || userSelectedAnObject;
            objectHasBeenSelected = userSelectedAnObject;

            // enable or disable interaction with objects 
            aRPlacementInteractable.enabled = canARTouch;

            // change interaction in every object in scene 
            foreach(GameObject ARObject in objectsInScene)
            {
                ARObject.GetComponent<ARObject>().ChangeObjectInteraction(canARTouch);
            }

        }

        #endregion

        #region pubblic methods

        /// <summary>
        /// Handles when an AR object was added in the scene
        /// </summary>
        /// <param name="ARObject"></param>
        public void ObjectAdded(GameObject ARObject)
        {
            // add object to objects in scene
            objectsInScene.Add(ARObject);
        }

        #endregion

    }
}