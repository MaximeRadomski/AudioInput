using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupEnumBhv : PopupBhv
{
    private System.Func<int, object> _resultAction;
    private Transform _listStartPosition;
    private int _currentId;
    private int _startId;
    private int _currentPageFirst;
    private int _enumLength;

    private ButtonBhv _pageUp;
    private ButtonBhv _pageDown;

    public void Init<EnumType>(string title, int currentId, System.Func<int, object> resultAction) where EnumType : System.Enum
    {
        _currentId = currentId;
        _resultAction = resultAction;

        transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = title;

        var buttonNegative = transform.Find("ButtonNegative");
        buttonNegative.GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;

        (_pageUp = transform.Find("PageUp").GetComponent<ButtonBhv>()).EndActionDelegate = () => { PageUp<EnumType>(); };
        (_pageDown = transform.Find("PageDown").GetComponent<ButtonBhv>()).EndActionDelegate = () => { PageDown<EnumType>(); };

        _listStartPosition = transform.Find("ListStartPosition");
        _startId = 0;
        _currentPageFirst = 0;
        LoadList<EnumType>(_currentPageFirst);
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

    private void LoadList<EnumType>(int start) where EnumType : System.Enum
    {
        var spaceBetween = 12 * Constants.Pixel;
        var values = (EnumType[])System.Enum.GetValues(typeof(EnumType));
        _enumLength = values.Length;
        var alreadyCount = _listStartPosition.transform.childCount;
        if (alreadyCount > 0)
        {
            for (int i = alreadyCount - 1; i >= 0; --i)
                Destroy(_listStartPosition.GetChild(i).gameObject);
        }
        int y = 0;
        var reachedTheEnd = false;
        for (int i = start; i < _enumLength && y < 5; ++i)
        {
            if (values[i].GetHashCode() == -1)
            {
                reachedTheEnd = true;
                continue;
            }
            var tmpButtonObject = Resources.Load<GameObject>("Prefabs/EnumButton");
            var tmpButtonInstance = Instantiate(tmpButtonObject, _listStartPosition.position + new Vector3(0.0f, -spaceBetween * y, 0.0f), tmpButtonObject.transform.rotation);
            tmpButtonInstance.name = $"EnumChoice{i}";
            string tmpName = values[i].GetDescription();
            tmpButtonInstance.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = tmpName.ToLower();
            tmpButtonInstance.GetComponent<ButtonBhv>().EndActionDelegate = SelectEnum;
            tmpButtonInstance.transform.SetParent(_listStartPosition);
            ++y;
        }
        if (start <= _startId)
            _pageUp.DisableButton();
        else
            _pageUp.EnableButton();
        if (start + y >= _enumLength || reachedTheEnd)
            _pageDown.DisableButton();
        else
            _pageDown.EnableButton();
    }

    private void PageDown<EnumType>() where EnumType : System.Enum
    {
        if (_currentPageFirst + 5 < _enumLength)
            _currentPageFirst += 5;
        LoadList<EnumType>(_currentPageFirst);
    }

    private void PageUp<EnumType>() where EnumType : System.Enum
    {
        if (_currentPageFirst - 5 >= _startId)
            _currentPageFirst -= 5;
        LoadList<EnumType>(_currentPageFirst);
    }
}
