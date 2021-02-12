using Lasp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupDeviceIdBhv : PopupBhv
{
    private System.Func<DeviceDescriptor, object> _resultAction;
    private Vector3 _listStartPosition;
    private DeviceDescriptor _currentDevice;
    private IEnumerable<DeviceDescriptor> _devices;

    public void Init(DeviceDescriptor currentDevice, System.Func<DeviceDescriptor, object> resultAction)
    {
        _currentDevice = currentDevice;
        _resultAction = resultAction;

        var buttonNegative = transform.Find("ButtonNegative");
        buttonNegative.GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;

        _listStartPosition = transform.Find("ListStartPosition").position;
        var spaceBetween = 12 * Constants.Pixel;
        _devices = AudioSystem.InputDevices;
        int i = 0;
        foreach (var device in _devices)
        {
            var tmpButtonObject = Resources.Load<GameObject>("Prefabs/DevicePopupButton");
            var tmpButtonInstance = Instantiate(tmpButtonObject, _listStartPosition + new Vector3(-0.8557f, -spaceBetween * i, 0.0f), tmpButtonObject.transform.rotation);
            tmpButtonInstance.name = $"DeviceChoice{i}";
            string tmpName = device.Name;
            tmpButtonInstance.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = tmpName.ToLower();
            tmpButtonInstance.GetComponent<ButtonBhv>().EndActionDelegate = SelectDevice;
            tmpButtonInstance.transform.SetParent(transform);
            ++i;
        }
    }

    private void SelectDevice()
    {
        var subString = Constants.LastEndActionClickedName.Substring(12);
        int id = int.Parse(subString);
        int i = 0;
        DeviceDescriptor resultDevice = new DeviceDescriptor();
        foreach (var device in _devices)
        {
            if (i == id)
            {
                resultDevice = device;
                break;
            }
            ++i;
        }
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(resultDevice);
        Destroy(gameObject);
    }

    private void NegativeDelegate()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(_currentDevice);
        Destroy(gameObject);
    }

    public override void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(_currentDevice);
        Destroy(gameObject);
    }
}
