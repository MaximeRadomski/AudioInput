using Lasp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupDeviceIdBhv : PopupBhv
{
    private System.Func<DeviceDescriptor, object> _resultAction;
    private Transform _listStartPosition;
    private DeviceDescriptor _currentDevice;
    private IEnumerable<DeviceDescriptor> _devices;
    private int _currentPageFirst;
    private int _devicesLength;

    private ButtonBhv _pageUp;
    private ButtonBhv _pageDown;

    public void Init(DeviceDescriptor currentDevice, System.Func<DeviceDescriptor, object> resultAction)
    {
        _currentDevice = currentDevice;
        _resultAction = resultAction;

        var buttonNegative = transform.Find("ButtonNegative");
        buttonNegative.GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;

        (_pageUp = transform.Find("PageUp").GetComponent<ButtonBhv>()).EndActionDelegate = PageUp;
        (_pageDown = transform.Find("PageDown").GetComponent<ButtonBhv>()).EndActionDelegate = PageDown;

        _listStartPosition = transform.Find("ListStartPosition");
        _devices = AudioSystem.InputDevices;
        int length = 0;
        foreach (var device in _devices)
            ++length;
        _devicesLength = length;
        _currentPageFirst = 0;
        LoadList(_currentPageFirst);
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

    private void LoadList(int start)
    {
        var spaceBetween = 12 * Constants.Pixel;
        
        var alreadyCount = _listStartPosition.transform.childCount;
        if (alreadyCount > 0)
        {
            for (int deadId = alreadyCount - 1; deadId >= 0; --deadId)
                Destroy(_listStartPosition.GetChild(deadId).gameObject);
        }
        int i = 0;
        int y = 0;
        foreach (var device in _devices)
        {
            if (i < start)
            {
                ++i;
                continue;
            }
            if (y >= 5)
                break;
            var tmpButtonObject = Resources.Load<GameObject>("Prefabs/DevicePopupButton");
            var tmpButtonInstance = Instantiate(tmpButtonObject, _listStartPosition.position + new Vector3(-0.8557f, -spaceBetween * y, 0.0f), tmpButtonObject.transform.rotation);
            tmpButtonInstance.name = $"DeviceChoice{i}";
            string tmpName = device.Name;
            tmpButtonInstance.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = tmpName.ToLower();
            tmpButtonInstance.GetComponent<ButtonBhv>().EndActionDelegate = SelectDevice;
            tmpButtonInstance.transform.SetParent(_listStartPosition);
            ++i;
            ++y;
        }

        if (start <= 0)
            _pageUp.DisableButton();
        else
            _pageUp.EnableButton();
        if (start + y >= _devicesLength)
            _pageDown.DisableButton();
        else
            _pageDown.EnableButton();
    }

    private void PageDown()
    {
        if (_currentPageFirst + 5 < _devicesLength)
            _currentPageFirst += 5;
        LoadList(_currentPageFirst);
    }

    private void PageUp()
    {
        if (_currentPageFirst - 5 >= 0)
            _currentPageFirst -= 5;
        LoadList(_currentPageFirst);
    }
}
