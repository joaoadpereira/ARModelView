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

        private int numberOfObjectsInScene = 0;
        
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

        /// <summary>
        /// Returns the number of objects in scene
        /// </summary>
        public int NumberOfObjectsInScene
        {
            get { return numberOfObjectsInScene; }  
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

            numberOfObjectsInScene++;
        }

        /// <summary>
        /// Shows or hides each ARNote of each Object added into the scene.
        /// </summary>
        /// <param name="state"></param>
        public void ShowHideARNotes(bool state)
        {
            foreach(GameObject aRObject in objectsInScene)
            {
                aRObject.GetComponent<ARObject>().ShowHideARNote(state);
            }
        }

        /// <summary>
        /// Shows or hides all objects added into the scene.
        /// </summary>
        /// <param name="state"></param>
        public void ShowHideAllObjects(bool state)
        {
            foreach (GameObject aRObject in objectsInScene)
            {
                aRObject.SetActive(state);
            }
        }

        /// <summary>
        /// Adds or removes a RigidBody to each object
        /// </summary>
        /// <param name="state"></param>
        public void AddRemovePhysicsInObjects(bool state)
        {
            foreach (GameObject aRObject in objectsInScene)
            {
                if (state)
                {
                    aRObject.AddComponent<Rigidbody>();
                }
                else
                {
                    if (aRObject.GetComponent<Rigidbody>() != null)
                    {
                        Destroy(aRObject.GetComponent<Rigidbody>());
                    }
                }
            }
        }

        /// <summary>
        /// Destroy all objects in scene and clear its references
        /// </summary>
        public void DeleteAllObjects()
        {
            foreach( GameObject aRObject in objectsInScene)
            {
                Destroy(aRObject);
            }

            // reset counting
            objectsInScene.Clear();
            numberOfObjectsInScene = 0;
        }

        public void SetObjectToInstantiate(GameObject otherObject)
        {
            objectToAR = otherObject;
            
            // add Object to instantiate into ARPlacementinteractable
            aRPlacementInteractable.placementPrefab = objectToAR;

        }

        #endregion

    }
}