using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Utils;


namespace Managers 
{
    /// <summary>
    /// Gets and handles the information about ARPlanes generated
    /// </summary>
    public class PlaneGeneratedManager : MonoBehaviour
    {
        #region fields
        private static PlaneGeneratedManager instance;

        // track if device can classify planes
        private bool deviceSupportsClassification = false;

        // track if device supports occlusion
        private bool deviceSupportsOcclusion = false;

        // track of floor hight
        private float floorHight = float.MaxValue;
        private float thresholdHight = 0.2f;

        // track all planes 
        private List<ARPlane> allARPlanes = new List<ARPlane>();

        [Header("AR Managers")]
        [SerializeField]
        private ARPlaneManager aRPlaneManager;
        [SerializeField]
        private AROcclusionManager aROcclusionManager;

        [Header("Materials to distinguish between different plane classification")]
        [SerializeField]
        private Material wallMaterial;
        [SerializeField]
        private Material tableMaterial;
        [SerializeField]
        private Material floorMaterial;
        [SerializeField]
        private Material defaultSurfaceMaterial;

        #endregion

        #region properties

        /// <summary>
        /// Property to acess the Singletion instance of PlaneGeneratedManager
        /// </summary>
        public static PlaneGeneratedManager Instance
        {
            get {  return instance; }
        }

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

        #endregion

        #region private methods

        // Start is called before the first frame update
        private void Start()
        {

            if (aRPlaneManager != null)
            {
                // listen to ARPlanes have been added, updated or removed
                aRPlaneManager.planesChanged += PlanesChanged;
            } 
            else
            {
                Debug.LogError("ARPlaneManager is not assigned.");
            }

            // Define if device can classify ARPlanes automatically
            deviceSupportsClassification = DeviceProperties.Instance.SupportsPlaneClassification;

            // Define id device supports occlusion
            deviceSupportsOcclusion = DeviceProperties.Instance.SupportsOcclusion;

            // check if device has AR support
            bool? supportsAR = null;
            DeviceProperties.Instance.SupportsAR((support) => { 
                supportsAR = support; 
                Debug.Log("SupportsAR: " + supportsAR.ToString());
                AppManager.Instance.SetAppState(AppState.Initializing);
            });

            
        }


        /// <summary>
        /// Handles the logic when AR planes have changed (been added, updated or removed)
        /// </summary>
        /// <param name="arfs"></param>
        private void PlanesChanged(ARPlanesChangedEventArgs args)
        {
            // get menu see/hide planes state
            bool showARPlanes = MenuManager.Instance.ShowingARPlanesMenu;

            // handle added new AR planes
            allARPlanes = new List<ARPlane> ();

            // track all ARplane
            allARPlanes.AddRange(args.added);
            allARPlanes.AddRange(args.updated);

            // loop every ARPlane
            foreach (ARPlane plane in allARPlanes)
            {
                PlaneClassification planeClassifiction; 

                // case device supports automatic detection. If not, try to classify
                if (deviceSupportsClassification)
                {
                    planeClassifiction = plane.classification;
                }
                else
                {
                    // try to classify the plan
                    planeClassifiction = ClassifyThePlan(plane);
                }

                // change material on ARPlane based on classification
                switch (planeClassifiction)
                {
                    case PlaneClassification.Wall:
                        UpdateARPlaneMaterial(plane, wallMaterial);
                        break;
                    case PlaneClassification.Floor:
                        UpdateARPlaneMaterial(plane, floorMaterial);
                        break;
                    case PlaneClassification.Table:
                        UpdateARPlaneMaterial(plane, tableMaterial);
                        break;
                    default:
                        UpdateARPlaneMaterial(plane, defaultSurfaceMaterial);
                        break;
                }

                // set active state based on menu see/hide planes
                plane.gameObject.SetActive(showARPlanes);
            }
        }

        /// <summary>
        /// Method to try to classify an ARPlane based on its normal and height
        /// </summary>
        /// <param name="aRPlane"></param>
        /// <returns></returns>
        private PlaneClassification ClassifyThePlan(ARPlane aRPlane)
        {
            PlaneAlignment planeAlignment = aRPlane.alignment;

            switch (planeAlignment)
            {
                case PlaneAlignment.Vertical:
                    return PlaneClassification.Wall;
                case PlaneAlignment.HorizontalUp:
                case PlaneAlignment.HorizontalDown:
                    // get plane position and hight as y component
                    Vector3 planePosition = aRPlane.transform.position;

                    // redefine floorHight if a lower plane is found 
                    if (planePosition.y < floorHight)
                    {
                        floorHight = planePosition.y;
                        return PlaneClassification.Floor;
                    }

                    // if the plane is inside floor hight threshold, assume that is also a floor 
                    if (Mathf.Abs(planePosition.y - floorHight) < thresholdHight)
                    {
                        return PlaneClassification.Floor;
                    }

                    // assume all horizontal planes above floor level as table
                    return PlaneClassification.Table;
                default:
                    return PlaneClassification.None;
            }
        }

        /// <summary>
        /// Updates the given material in the given plane
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="material"></param>
        private void UpdateARPlaneMaterial(ARPlane plane, Material material)
        {
            MeshRenderer meshRenderer = plane.GetComponent<MeshRenderer>();

            // change material array in MeshRender with an array with the given material 
            if (meshRenderer != null)
            {
                Material[] mats = new Material[1];
                mats[0] = material;

                meshRenderer.materials = mats;
            }
            else
            {
                Debug.LogError("The ARPlane does not have a Renderer Component");
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Shows or hides all ARPlanes in scene
        /// </summary>
        /// <param name="state"></param>
        public void ShowHideARPlanes(bool state)
        {
            foreach(ARPlane aRPlane in allARPlanes)
            {
                aRPlane.gameObject.SetActive(state);
            }
        }

        #endregion
    }
}