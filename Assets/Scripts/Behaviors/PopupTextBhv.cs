using System;
using System.Globalization;
using UnityEngine;

public class PopupTextBhv : PopupBhv
{
    private System.Func<string, object> _resultAction;
    private TMPro.TextMeshPro _text;

    private int _maxLength;
    private string _currentText;
    private string _currentString;

    private string _forbiddenCharacters = "\t\f\n\0";

    private NumberFormatInfo _nfi;

    public void Init(string title, string content, string current, int maxLength, System.Func<string, object> resultAction)
    {
        _resultAction = resultAction;
        _maxLength = maxLength;
        _currentText = current;

        var cultureInfo = CultureInfo.CurrentCulture;
        _nfi = cultureInfo.NumberFormat;

        transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = title.ToLower();
        transform.Find("Content").GetComponent<TMPro.TextMeshPro>().text = content.ToLower();

        var buttonPositive = transform.Find("ButtonPositive");
        buttonPositive.GetComponent<ButtonBhv>().EndActionDelegate = PositiveDelegate;

        var buttonNegative = transform.Find("ButtonNegative");
        buttonNegative.GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;

        transform.Find("DeleteButton").GetComponent<ButtonBhv>().EndActionDelegate = DeleteText;

        _text = transform.Find("TextBackground").GetChild(0).GetComponent<TMPro.TextMeshPro>();
        _currentString = _currentText == null ? "" : _currentText ;
        UpdateText();
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.rawType == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.Backspace && _currentString.Length > 0)
            {
                _currentString = _currentString.Remove(_currentString.Length - 1, 1);
                UpdateText();
            }
            else if (_currentString.Length < _maxLength && !_forbiddenCharacters.Contains(e.character.ToString()))
            {
                _currentString += e.character.ToString().ToLower();
                UpdateText();
            }
        }
    }

    private void DeleteText()
    {
        _currentString = "";
        UpdateText();
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
            _resultAction?.Invoke(_currentString);
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
        _resultAction?.Invoke(_currentText);
        Destroy(gameObject);
    }

    public override void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(_currentText);
        Destroy(gameObject);
    }

    public override void ValidatePopup()
    {
        PositiveDelegate();
    }
}
