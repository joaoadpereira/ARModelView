using Enums;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the placing, moving, rotation and scaling of AR objects with AR planes
/// </summary>
public class ARInteractionsManager : MonoBehaviour
{
    #region fields
    private static ARInteractionsManager instance;

    // condition to enable objet placing
    private bool canPlaceObject = false;

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
        AppManager.Instance.StateChanged += EnableObjectPlacing;
    }

    private void Update()
    {
        // only enable object placing case canPlaceObject
        if (canPlaceObject)
        {
            CheckForTouchToAddObject();
        }
    }

    /// <summary>
    /// Controls if user can Place objects or not
    /// </summary>
    /// <param name="status"></param>
    private void SetObjectPlacing(bool status)
    {
        canPlaceObject = status;
    }

    /// <summary>
    /// Gets a screen touch and raycasts until it hits in an ARPlane
    /// 
    /// </summary>
    private void CheckForTouchToAddObject()
    {
        Debug.Log("Objects can now be placed");
    }

    #endregion

    #region public methods

    /// <summary>
    /// Enables the user interaction to add objects into planes
    /// </summary>
    public void EnableObjectPlacing(AppState appState)
    {
        SetObjectPlacing(appState == AppState.Idle);
    }

    #endregion


}
