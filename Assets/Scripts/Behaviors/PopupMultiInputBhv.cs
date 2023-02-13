using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMultiInputBhv : PopupBhv
{    
    private System.Action<bool, bool?> _resultAction;

    public void Init(System.Action<bool, bool?> resultAction)
    {
        _resultAction = resultAction;

        transform.Find("ButtonKeyboard").GetComponent<ButtonBhv>().EndActionDelegate = () => { KeyboardDelegate(false); };
        transform.Find("ButtonMouse").GetComponent<ButtonBhv>().EndActionDelegate = () => { MouseDelegate(false); };
        transform.Find("ButtonKeyboard2").GetComponent<ButtonBhv>().EndActionDelegate = () => { KeyboardDelegate(true); };
        transform.Find("ButtonMouse2").GetComponent<ButtonBhv>().EndActionDelegate = () => { MouseDelegate(true); };
        transform.Find("DeleteButton").GetComponent<ButtonBhv>().EndActionDelegate = DeleteSecond;
    }

    private void KeyboardDelegate(bool isSecond)
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(false, isSecond);
        Destroy(gameObject);
    }

    private void MouseDelegate(bool isSecond)
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(true, isSecond);
        Destroy(gameObject);
    }

    private void DeleteSecond()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(true, null);
        Destroy(gameObject);
    }

    public override void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        Destroy(gameObject);
    }

    public override void ValidatePopup()
    {
        ExitPopup();
    }
}
