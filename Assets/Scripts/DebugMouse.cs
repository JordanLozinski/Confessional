
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMouse : MonoBehaviour
{
    private Text txt;
    void Awake()
    {
        txt = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        txt.text = Dialog.MouseNearNotebook().ToString();
    }
}
