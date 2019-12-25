using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CanvasScaleSet : MonoBehaviour
{
    private void Awake()
    {
        #if UNITY_EDITOR
        #else
        GetComponent<CanvasScaler>().referenceResolution = new Vector2(Screen.width,Screen.height);
        #endif
    }
}
