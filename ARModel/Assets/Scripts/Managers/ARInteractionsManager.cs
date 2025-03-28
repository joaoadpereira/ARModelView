using Enums;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

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

        // selected object
        private GameObject selectedObject;
        private Material originalSelectedObjectMaterial;

        [Header("AR Raycast Manager in scene")]
        [SerializeField]
        private ARRaycastManager aRRaycastManager;

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

        private void OnEnable()
        {
            // enable enhanced touch support
            UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            // disable enhanced touch support
            UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Disable();
        }

        private void Start()
        {
            AppManager.Instance.StateChanged += EnableObjectPlacing;
        }

        private void Update()
        {
            /*// gets the touches in screen
            var activeTouches = Touch.activeTouches;
            if (activeTouches.Count == 0) return;

            // only enable interaction with AR objects if canARTouch
            if (canARTouch)
            {
                // only check for one time touch in screen
                if (activeTouches[0].phase == TouchPhase.Began)
                {
                    // check if touched an object
                    bool clickedInObject = CheckForObjectTouch(activeTouches[0]);

                    // if didnt touch any object
                    if (!clickedInObject)
                    {
                        // but an object is selected
                        if (objectHasBeenSelected)
                        {
                            // reset the object selection to non
                            ResetObjectSelection();
                            return;
                        }

                        // tries to hit an AR Plane based on screen touch
                        CheckForARPlaneTouch(activeTouches[0]);
                    }
                    return;
                }
            }*/
        }

        /// <summary>
        /// Checks is a touch in screen and raycasts any hit with an object
        /// </summary>
        /// <param name="touch"></param>
        /// <returns></returns>
        private bool CheckForObjectTouch(Touch touch)
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hittedObject = hit.collider.gameObject;
                if (hittedObject.transform.parent.CompareTag("ObjectToManipulate"))
                {
                    // user touched same object then it should disable its selection
                    if (hittedObject == selectedObject)
                    {
                       ResetObjectSelection();
                        return true;
                    }

                    // reset previous selected object material
                    if (selectedObject != null)
                    {
                        selectedObject.GetComponent<MeshRenderer>().material = originalSelectedObjectMaterial;
                    }

                    // define object selected
                    selectedObject = hittedObject;

                    // save the original material of selected object
                    originalSelectedObjectMaterial = selectedObject.GetComponent<MeshRenderer>().material;

                    // change the material of the selected object
                    selectedObject.GetComponent<MeshRenderer>().material = selectedMaterial;

                    // change state of objectHasBeenSelected
                    objectHasBeenSelected = true;

                    // change app state to object selected
                    AppManager.Instance.SetAppState(AppState.ObjectSelected);

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Puts the selected object material to its original one
        /// Resets the selected object
        /// Changes App state to idle
        /// </summary>
        private void ResetObjectSelection()
        {
            selectedObject.GetComponent<MeshRenderer>().material = originalSelectedObjectMaterial;
            selectedObject = null;
            originalSelectedObjectMaterial = null;

            // change app state to idle
            AppManager.Instance.SetAppState(AppState.Idle);
        }

        /// <summary>
        /// Gets a screen touch and raycasts until it hits in an GameObject
        /// Based on the hit point position, Returns the hit object
        /// </summary>
        private void CheckForARPlaneTouch(Touch touch)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if (aRRaycastManager.Raycast(touch.screenPosition, hits))
            {
                GameObject hittedGameObject = hits[0].trackable.gameObject;

                // it can hit a plane but it can also hit an instantiated object
                if (hittedGameObject.GetComponent<ARPlane>() != null)
                {
                    Vector3 hitPosePosition = hits[0].pose.position;

                    GameObject newObjectAdded = Instantiate(objectToAR);

                    newObjectAdded.transform.position = hitPosePosition;

                    objectsInScene.Add(newObjectAdded);
                }
            }
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
        }

        #endregion

    }
}