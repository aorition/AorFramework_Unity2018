using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevRootControllor : MonoBehaviour
{

    private void OnEnable()
    {
        runtimeDisabledCheck();
    }

    private void Update()
    {
        runtimeDisabledCheck();
    }

    private void runtimeDisabledCheck()
    {
        if(Application.isPlaying && this.gameObject.activeSelf)
            this.gameObject.SetActive(false);
    }

}
