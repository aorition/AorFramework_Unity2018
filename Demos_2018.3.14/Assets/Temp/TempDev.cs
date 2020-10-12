using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AorBaseUtility;

public class TempDev : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VersionInfo ve1 = VersionInfo.Create("1.0.5.1035_alpha_保重版");
        VersionInfo ve2 = VersionInfo.Create("1.1.0.0");

        Debug.Log(ve1.ToJSONString());
        Debug.Log(ve2.ToShortString());
        Debug.Log(ve2.ToJSONString());

        Debug.Log("ve1 > ve2 = " + (ve1 > ve2).ToString());

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
