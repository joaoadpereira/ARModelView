using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enums 
{
    /// <summary>
    /// States of the App
    /// </summary>
    public enum AppState
    {
        Initializing,
        ShowingInstructions,
        Idle,
        PlacingObject,
        ObjectSelected,
        ManipulatingObject,
    }
}
