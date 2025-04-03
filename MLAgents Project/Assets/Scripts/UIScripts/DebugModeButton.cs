using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugModeButton : MonoBehaviour
{

    public GameObject debugPanel;

    // Start is called before the first frame update
    void Start()
    {
        debugPanel.SetActive(false);
    }


    public void debugButtonClick()
    {
        if (debugPanel.activeSelf == false)
        {
            debugPanel.SetActive(true);
        }
        else
        {
            debugPanel.SetActive(false);
        }
    
        
    }


}
