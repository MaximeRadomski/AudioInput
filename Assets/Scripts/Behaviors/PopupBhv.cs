using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopupBhv : MonoBehaviour
{
    void Start()
    {
        transform.Find("ExitButton").GetComponent<ButtonBhv>().EndActionDelegate = ExitPopup;
    }

    public virtual void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        Destroy(gameObject);
    }

    public virtual void ValidatePopup()
    {

    }
}
