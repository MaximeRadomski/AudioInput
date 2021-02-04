using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel02Bhv : PanelBhv
{

    void Start()
    {
        Init();
        SetButtons();
    }

    private void SetButtons()
    {
        transform.Find("DarkSoulsButton").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://youtu.be/haevbe2UXQA"); };
        transform.Find("DoomEternalButton").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://youtu.be/VRMzU4o57-c"); };
        transform.Find("TwitterButton").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://twitter.com/Abject_Sama"); };
        transform.Find("OnlyFanButton").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://youtu.be/dQw4w9WgXcQ"); };
    }
}
