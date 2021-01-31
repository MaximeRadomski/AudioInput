using Lasp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
    public GameObject NewPopupYesNo(Vector3 position, string title, string content, string negative, string positive,
        System.Func<bool, object> resultAction, Sprite sprite = null)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupYesNo");
        var tmpPopupInstance = Instantiate(tmpPopupObject, position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupYesNoBhv>().Init(title, content, negative, positive, resultAction, sprite);
        return tmpPopupInstance;
    }

    public GameObject NewPopupDeviceId(Vector3 position, DeviceDescriptor currentDevice, System.Func<DeviceDescriptor, object> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupDeviceId");
        var tmpPopupInstance = Instantiate(tmpPopupObject, position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupDeviceIdBhv>().Init(currentDevice, resultAction);
        return tmpPopupInstance;
    }
}
