using Enum;
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
        private GameObject objectSelected = null;

        // keep track of all objects added into the scene
        List<GameObject> objectsInScene = new List<GameObject>();
        private int numberOfObjectsInScene = 0;

        private Dictionary<Object, int> numberOfObjectsInSceneByType = new Dictionary<Object, int>();

        [Header("Interaction Manager")]
        [SerializeField]
        private XRInteractionManager interactionManager;

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

            // set number of objects in scene by type
            numberOfObjectsInSceneByType[Object.Drill] = 0;
            numberOfObjectsInSceneByType[Object.Toolbox] = 0;

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
        public void ObjectAdded(GameObject ARObject, Object objectType)
        {
            // add object to objects in scene
            objectsInScene.Add(ARObject);

            numberOfObjectsInScene++;

            numberOfObjectsInSceneByType[objectType] += 1;
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

            numberOfObjectsInSceneByType[Object.Drill] = 0;
            numberOfObjectsInSceneByType[Object.Toolbox] = 0;
        }

        public void SetObjectToInstantiate(GameObject otherObject)
        {
            objectToAR = otherObject;
            
            // add Object to instantiate into ARPlacementinteractable
            aRPlacementInteractable.placementPrefab = objectToAR;

        }

        /// <summary>
        /// Interact with the object on rotation, scale and moving forward
        /// </summary>
        /// <param name="interactionVoice"></param>
        public void InteractWithObject(InteractionVoice? interactionVoice)
        {
            if (interactionVoice == null) return;

            switch (interactionVoice)
            {
                case InteractionVoice.Rotate:
                    objectSelected.transform.Rotate(0, 45, 0);
                    break;
                case InteractionVoice.Scale:
                    objectSelected.transform.localScale *= 1.2f;
                    break;
                case InteractionVoice.Forward:
                default:
                    objectSelected.transform.Translate(Vector3.forward);
                    break;
            }
        }

        /// <summary>
        /// Handle when object was selected.
        /// Define object selected.
        /// Change App state to Object Selection.
        /// </summary>
        /// <param name="obj"></param>
        public void ObjectWasSelected(GameObject obj)
        {
            objectSelected = obj;

            // communicate obejct selected stae
            AppManager.Instance.SetAppState(AppState.ObjectSelected);

            // reset previous text speech
            VoiceCommandsManager.Instance.CleanTextSpeech();
        }

        /// <summary>
        /// Handle when Object was unselected.
        /// Remove object selection.
        /// Change app state to Idle.
        /// </summary>
        /// <param name="obj"></param>
        public void ObjectWasExited (GameObject obj)
        {
            objectSelected = null;

            //communicate obejct selected stae
            AppManager.Instance.SetAppState(AppState.Idle);
        }

        /// <summary>
        /// deletes the object selected
        /// </summary>
        public void DeleteObject()
        {        
            if (objectSelected != null)
            {
                // count less one
                numberOfObjectsInScene--;

                Object objectType = objectSelected.GetComponent<ARObject>().ObjectType;
                numberOfObjectsInSceneByType[objectType] -= 1;

                // desotry object causes conflits. Deactivation simulates it
                IXRSelectInteractable interactor = objectSelected.GetComponent<ARSelectionInteractable>();

                objectSelected.SetActive(false);
                interactionManager.UnregisterInteractable(interactor);

                objectSelected = null;
            }

            // set app state
            AppManager.Instance.SetAppState(AppState.Idle);
        }

        /// <summary>
        /// Returns the number of objects based on given objectType
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public int NumberOfObjectsByType(Object objectType)
        {
            return numberOfObjectsInSceneByType[objectType];
        }

        #endregion

    }
}