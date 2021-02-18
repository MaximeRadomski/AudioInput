using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupListBhv : PopupBhv
{
    private List<string> _list;
    private System.Func<int, object> _resultAction;
    private Transform _listStartPosition;
    private int _currentId;
    private int _startId;
    private int _currentPageFirst;
    private int _listLength;

    private ButtonBhv _pageUp;
    private ButtonBhv _pageDown;

    public void Init(string title, int currentId, List<string> list, System.Func<int, object> resultAction)
    {
        _list = list;
        _currentId = currentId;
        _resultAction = resultAction;

        transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = title;

        var buttonNegative = transform.Find("ButtonNegative");
        buttonNegative.GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;

        (_pageUp = transform.Find("PageUp").GetComponent<ButtonBhv>()).EndActionDelegate = PageUp;
        (_pageDown = transform.Find("PageDown").GetComponent<ButtonBhv>()).EndActionDelegate = PageDown;

        _listStartPosition = transform.Find("ListStartPosition");
        _startId = 0;
        _currentPageFirst = 0;
        LoadList(_currentPageFirst);
    }

    private void SelectEnum()
    {
        var subString = Constants.LastEndActionClickedName.Substring(10);
        int id = int.Parse(subString);
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(id);
        Destroy(gameObject);
    }

    private void NegativeDelegate()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(_currentId);
        Destroy(gameObject);
    }

    public override void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(_currentId);
        Destroy(gameObject);
    }

    private void LoadList(int start)
    {
        var spaceBetween = 12 * Constants.Pixel;
        _listLength = _list.Count;
        var alreadyCount = _listStartPosition.transform.childCount;
        if (alreadyCount > 0)
        {
            for (int i = alreadyCount - 1; i >= 0; --i)
                Destroy(_listStartPosition.GetChild(i).gameObject);
        }
        int y = 0;
        for (int i = start; i < _listLength && y < 5; ++i)
        {
            var tmpButtonObject = Resources.Load<GameObject>("Prefabs/DevicePopupButton");
            var tmpButtonInstance = Instantiate(tmpButtonObject, _listStartPosition.position + new Vector3(-0.8557f, -spaceBetween * y, 0.0f), tmpButtonObject.transform.rotation);
            tmpButtonInstance.name = $"EnumChoice{i}";
            string tmpName = _list[i];
            tmpButtonInstance.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = tmpName.ToLower();
            tmpButtonInstance.GetComponent<ButtonBhv>().EndActionDelegate = SelectEnum;
            tmpButtonInstance.transform.SetParent(_listStartPosition);
            ++y;
        }
        if (start <= _startId)
            _pageUp.DisableButton();
        else
            _pageUp.EnableButton();
        if (start + y >= _listLength)
            _pageDown.DisableButton();
        else
            _pageDown.EnableButton();
    }
    private void PageUp()
    {
        if (_currentPageFirst - 5 >= _startId)
            _currentPageFirst -= 5;
        LoadList(_currentPageFirst);
    }

    private void PageDown()
    {
        if (_currentPageFirst + 5 < _listLength)
            _currentPageFirst += 5;
        LoadList(_currentPageFirst);
    }
}
