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

    public GameObject NewPopupNumber(Vector3 position, string title, string content, bool castInt, int maxLength, System.Func<float, object> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupNumber");
        var tmpPopupInstance = Instantiate(tmpPopupObject, position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupNumberBhv>().Init(title, content, castInt, maxLength, resultAction);
        return tmpPopupInstance;
    }

    public GameObject NewPopupEnum<EnumType>(Vector3 position, string title, string content, int currentId, System.Func<int, object> resultAction) where EnumType : System.Enum
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupEnum");
        var tmpPopupInstance = Instantiate(tmpPopupObject, position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupEnumBhv>().Init<EnumType>(title, content, currentId, resultAction);
        return tmpPopupInstance;
    }
}
