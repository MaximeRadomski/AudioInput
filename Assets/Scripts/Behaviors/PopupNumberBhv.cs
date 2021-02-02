using Lasp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupNumberBhv : PopupBhv
{
    private System.Func<float, object> _resultAction;
    private Vector3 _listStartPosition;
    private TMPro.TextMeshPro _text;

    private int _maxLength;
    private bool _castInt;
    //private float _currentCast;
    private string _currentString;

    public void Init(string title, string content, bool castInt, int maxLength, System.Func<float, object> resultAction)
    {
        _castInt = castInt;
        _resultAction = resultAction;
        _maxLength = maxLength;

        transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = title.ToLower();
        transform.Find("Content").GetComponent<TMPro.TextMeshPro>().text = content.ToLower();

        var buttonPositive = transform.Find("ButtonPositive");
        buttonPositive.GetComponent<ButtonBhv>().EndActionDelegate = PositiveDelegate;

        var buttonNegative = transform.Find("ButtonNegative");
        buttonNegative.GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;

        _text = transform.Find("NumberBackground").GetChild(0).GetComponent<TMPro.TextMeshPro>();
        _currentString = "";
        UpdateText();
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.rawType == EventType.KeyUp)
        {
            if (e.keyCode == KeyCode.Backspace && _currentString.Length > 0)
            {
                _currentString = _currentString = _currentString.Remove(_currentString.Length - 1, 1);
                UpdateText();
            }
            else if ((e.keyCode >= KeyCode.Alpha0 && e.keyCode <= KeyCode.Alpha9)
                  || (e.keyCode >= KeyCode.Keypad0 && e.keyCode <= KeyCode.Keypad9))
            {
                if (_currentString.Length == _maxLength)
                    return;
                int numberToAdd = int.Parse(e.keyCode.ToString().Substring(e.keyCode.ToString().Length - 1));
                _currentString = _currentString.Insert(_currentString.Length, numberToAdd.ToString());
                UpdateText();
            }
            else if (!_castInt && (e.keyCode == KeyCode.KeypadPeriod || e.keyCode == KeyCode.Period))
            {
                if (_currentString.Length == _maxLength)
                    return;
                _currentString = _currentString.Insert(_currentString.Length, ".");
                UpdateText();
            }
            else if (e.keyCode == KeyCode.KeypadMinus || e.keyCode == KeyCode.Minus)
            {
                if (_currentString.Length == _maxLength)
                    return;
                _currentString = _currentString.Insert(_currentString.Length, "-");
                UpdateText();
            }
        }
    }

    private void UpdateText()
    {
        _text.text = _currentString;
    }

    private void PositiveDelegate()
    {
        Constants.DecreaseInputLayer();
        try
        {
            float _cast = float.Parse(_currentString);
            _resultAction?.Invoke(_cast);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            ExitPopup();
        }
        Destroy(gameObject);
    }

    private void NegativeDelegate()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(-1.0f);
        Destroy(gameObject);
    }

    public override void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(-1.0f);
        Destroy(gameObject);
    }

    public override void ValidatePopup()
    {
        PositiveDelegate();
    }
}
