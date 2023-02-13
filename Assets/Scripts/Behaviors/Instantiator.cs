using Lasp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindowsInput.Native;

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

    public GameObject NewPopupNumber(Vector3 position, string title, string content, float current, int maxLength, System.Func<float, object> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupNumber");
        var tmpPopupInstance = Instantiate(tmpPopupObject, position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupNumberBhv>().Init(title, content, current, maxLength, resultAction);
        return tmpPopupInstance;
    }

    public GameObject NewPopupText(Vector3 position, string title, string content, string current, int maxLength, System.Func<string, object> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupText");
        var tmpPopupInstance = Instantiate(tmpPopupObject, position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupTextBhv>().Init(title, content, current, maxLength, resultAction);
        return tmpPopupInstance;
    }

    public GameObject NewPopupFrequencies(Vector3 position, List<float> defaultFrequencies, int defaultPeaksNumber, System.Func<List<float>, int, object> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupFrequencies");
        var tmpPopupInstance = Instantiate(tmpPopupObject, position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupFrequencies>().Init(defaultFrequencies, defaultPeaksNumber, resultAction, this);
        return tmpPopupInstance;
    }

    public GameObject NewPopupInput(Vector3 position, string title, int currentId, System.Action<int, bool> resultAction, bool returnVirtualKeyCode = true)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupInput");
        var tmpPopupInstance = Instantiate(tmpPopupObject, position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupInputBhv>().Init(currentId, title, resultAction, returnVirtualKeyCode);
        return tmpPopupInstance;
    }

    public GameObject NewPopupEnum<EnumType>(Vector3 position, string title, int currentId, System.Func<int, object> resultAction) where EnumType : System.Enum
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupEnum");
        var tmpPopupInstance = Instantiate(tmpPopupObject, position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupEnumBhv>().Init<EnumType>(title, currentId, resultAction);
        return tmpPopupInstance;
    }

    public GameObject NewPopupList(Vector3 position, string title, int currentId, List<string> list, System.Func<int, object> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupList");
        var tmpPopupInstance = Instantiate(tmpPopupObject, position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupListBhv>().Init(title, currentId, list, resultAction);
        return tmpPopupInstance;
    }

    public GameObject NewPopupMultiInput(Vector3 position, System.Action<bool, bool?> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupMultiInput");
        var tmpPopupInstance = Instantiate(tmpPopupObject, position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupMultiInputBhv>().Init(resultAction);
        return tmpPopupInstance;
    }

    public GameObject NewAudioInput(Transform listSource, Vector3 offset, AudioInput audioInput, int id, Panel01Bhv panelBhv)
    {
        var tmpAudioInputObject = Resources.Load<GameObject>("Prefabs/AudioInput");
        var tmpAudioInputInstance = Instantiate(tmpAudioInputObject, listSource.position + offset, tmpAudioInputObject.transform.rotation);
        tmpAudioInputInstance.name = $"AudioInputs[{id}]";
        tmpAudioInputInstance.transform.SetParent(listSource);
        tmpAudioInputInstance.GetComponent<AudioInputBhv>().Init(audioInput, panelBhv, id);
        return tmpAudioInputInstance;
    }

    public GameObject PopText(string text, Vector2 position, Transform parent, float floatingTime = 0.0f, float speed = 0.05f, float distance = 0.25f, float startFadingDistancePercent = 0.04f, float fadingSpeed = 0.1f)
    {
        var tmpPoppingTextObject = Resources.Load<GameObject>("Prefabs/PoppingText");
        var tmpPoppingTextInstance = Instantiate(tmpPoppingTextObject, position, tmpPoppingTextObject.transform.rotation, parent);
        tmpPoppingTextInstance.GetComponent<PoppingTextBhv>().Init(text, position, floatingTime, speed, distance, startFadingDistancePercent, fadingSpeed);
        return tmpPoppingTextInstance;
    }

    public GameObject PopNoShadowText(string text, Vector2 position, Transform parent, float floatingTime = 0.0f, float speed = 0.05f, float distance = 0.25f, float startFadingDistancePercent = 0.04f, float fadingSpeed = 0.1f)
    {
        var tmpPoppingTextObject = Resources.Load<GameObject>("Prefabs/PoppingTextNoShadow");
        var tmpPoppingTextInstance = Instantiate(tmpPoppingTextObject, position, tmpPoppingTextObject.transform.rotation, parent);
        tmpPoppingTextInstance.GetComponent<PoppingTextBhv>().Init(text, position, floatingTime, speed, distance, startFadingDistancePercent, fadingSpeed);
        return tmpPoppingTextInstance;
    }
}
