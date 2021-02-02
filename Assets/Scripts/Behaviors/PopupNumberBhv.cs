using Lasp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupNumberBhv : PopupBhv
{
    private System.Func<float, object> _resultAction;
    private TMPro.TextMeshPro _text;

    private int _maxLengthUnite;
    private int _maxLengthDecimal = 2;
    private float _currentFloat;
    private string _currentString;

    public void Init(string title, string content, float current, int maxLength, System.Func<float, object> resultAction)
    {
        _resultAction = resultAction;
        _maxLengthUnite = maxLength;
        _currentFloat = current;

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
        if (e.isKey && e.rawType == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.Backspace && _currentString.Length > 0)
            {
                _currentString = _currentString = _currentString.Remove(_currentString.Length - 1, 1);
                UpdateText();
            }
            else if ((e.keyCode >= KeyCode.Alpha0 && e.keyCode <= KeyCode.Alpha9)
                  || (e.keyCode >= KeyCode.Keypad0 && e.keyCode <= KeyCode.Keypad9))
            {
                if ((!_currentString.Contains(".") && HasReachedMaxLengthUnite())
                    ||(_currentString.Contains(".") && HasReachedMaxLengthDecimal()))
                    return;
                int numberToAdd = int.Parse(e.keyCode.ToString().Substring(e.keyCode.ToString().Length - 1));
                _currentString = _currentString.Insert(_currentString.Length, numberToAdd.ToString());
                UpdateText();
            }
            else if (e.keyCode == KeyCode.KeypadPeriod || e.keyCode == KeyCode.Period || e.keyCode == KeyCode.P)
            {
                if (_currentString.Contains("."))
                    return;
                _currentString = _currentString.Insert(_currentString.Length, ".");
                UpdateText();
            }
            else if (e.keyCode == KeyCode.KeypadMinus || e.keyCode == KeyCode.Minus || e.keyCode == KeyCode.M)
            {
                if (_currentString.Length != 0)
                    return;
                _currentString = _currentString.Insert(_currentString.Length, "-");
                UpdateText();
            }
        }
    }

    private int LengthUnite()
    {
        var length = _currentString.Length;
        if (_currentString.Contains("."))
            length = _currentString.IndexOf(".");
        if (_currentString.Contains("-"))
            length -= 1;
        return length;
    }

    private int LengthDecimal()
    {
        var length = 0;
        if (_currentString.Contains(".") && _currentString.Length > _currentString.IndexOf(".") + 1)
            length = _currentString.Length - (_currentString.IndexOf(".") + 1);
        return length;
    }

    private bool HasReachedMaxLengthUnite()
    {
        return LengthUnite() >= _maxLengthUnite;
    }

    private bool HasReachedMaxLengthDecimal()
    {
        return LengthDecimal() >= _maxLengthDecimal;
    }

    private void UpdateText()
    {
        if (_currentString.Length == 0)
            _text.text = "...";
        else
            _text.text = _currentString;
    }

    private void PositiveDelegate()
    {
        Constants.DecreaseInputLayer();
        try
        {
            if (_currentString[_currentString.Length - 1] == '.')
                _currentString += "0";
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
        _resultAction?.Invoke(_currentFloat);
        Destroy(gameObject);
    }

    public override void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(_currentFloat);
        Destroy(gameObject);
    }

    public override void ValidatePopup()
    {
        PositiveDelegate();
    }
}
