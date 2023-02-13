using Lasp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindowsInput;
using WindowsInput.Native;

public class PopupInputBhv : PopupBhv
{
    private Action<int, bool> _resultAction;
    private TMPro.TextMeshPro _recordedText;
    private TMPro.TextMeshPro _sendableText;

    private string _currentString;
    private VirtualKeyCode _currentKeyCode;
    private VirtualKeyCode _newKeyCode = VirtualKeyCode.NONAME;
    private KeyCode _currentTypedKeyCode;
    private KeyCode _newTypedKeyCode = KeyCode.None;
    private bool _returnVirtualKeyCode;
    private bool _isSecond;

    //For Test Mapping
    //private InputSimulator _inputSimulator;

    public void Init(int currentId, string title, Action<int, bool> resultAction, bool returnVirtualKeyCode)
    {
        //For Test Mapping
        //_inputSimulator = new InputSimulator();

        transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = title;
        _isSecond = title.Contains("2nd");
        Constants.EscapeOrEnterLocked = true;
        _resultAction = resultAction;
        _currentKeyCode = (VirtualKeyCode)currentId;
        _currentTypedKeyCode = (KeyCode)currentId;
        _returnVirtualKeyCode = returnVirtualKeyCode;

        var buttonPositive = transform.Find("ButtonPositive");
        buttonPositive.GetComponent<ButtonBhv>().EndActionDelegate = PositiveDelegate;

        var buttonNegative = transform.Find("ButtonNegative");
        buttonNegative.GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;

        _recordedText = transform.Find("RecordedInput").GetChild(1).GetComponent<TMPro.TextMeshPro>();
        _sendableText = transform.Find("SendableInput").GetChild(1).GetComponent<TMPro.TextMeshPro>();
        _currentString = "";
        UpdateText();
    }

    //for test Mapping
    //int _id = 0;
    //string _debugMapping = "";

    //private void Update()
    //{
    //    if (_id < 256)
    //    {
    //        var test = (VirtualKeyCode)_id;
    //        _inputSimulator.Keyboard.KeyPress(test);
    //        _debugMapping = test.ToString();
    //        ++_id;
    //    }
    //}

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.rawType == EventType.KeyDown && e.keyCode != KeyCode.None)
        {
            //For Test Mapping
            //_debugMapping = $"{e.keyCode} -> {_debugMapping}";
            //Debug.Log(_debugMapping);
            _currentString = e.keyCode.ToString().ToLower();
            _newTypedKeyCode = e.keyCode;
            _newKeyCode = MapKeyCodeToVirtualKeyCode(e.keyCode);
            UpdateText();
        }
    }

    private void UpdateText()
    {
        if (_currentString.Length == 0)
        {
            _recordedText.text = "...";
            _sendableText.text = "none";
        }
        else
        {
            _recordedText.text = _currentString;
            _sendableText.text = _newKeyCode == VirtualKeyCode.NONAME ? "unbindable" : _newKeyCode.ToString().ToLower();
        }
    }

    private void PositiveDelegate()
    {
        Constants.EscapeOrEnterLocked = false;
        Constants.DecreaseInputLayer();
        if (_returnVirtualKeyCode)
            _resultAction?.Invoke(_newKeyCode.GetHashCode(), _isSecond);
        else
            _resultAction?.Invoke(_newTypedKeyCode.GetHashCode(), _isSecond);
        Destroy(gameObject);
    }

    private void NegativeDelegate()
    {
        Constants.EscapeOrEnterLocked = false;
        Constants.DecreaseInputLayer();
        if (_returnVirtualKeyCode)
            _resultAction?.Invoke(_currentKeyCode.GetHashCode(), _isSecond);
        else
            _resultAction?.Invoke(_currentTypedKeyCode.GetHashCode(), _isSecond);
        Destroy(gameObject);
    }

    public override void ExitPopup()
    {
        Constants.EscapeOrEnterLocked = false;
        Constants.DecreaseInputLayer();
        if (_returnVirtualKeyCode)
            _resultAction?.Invoke(_currentKeyCode.GetHashCode(), _isSecond);
        else
            _resultAction?.Invoke(_currentTypedKeyCode.GetHashCode(), _isSecond);
        Destroy(gameObject);
    }

    public override void ValidatePopup()
    {
        PositiveDelegate();
    }

    private VirtualKeyCode MapKeyCodeToVirtualKeyCode(KeyCode keyCode)
    {
        var vkc = VirtualKeyCode.NONAME;
        switch (keyCode)
        {
            case KeyCode.Backspace: vkc = VirtualKeyCode.BACK; break;
            case KeyCode.Tab: vkc = VirtualKeyCode.TAB; break;
            case KeyCode.Clear: vkc = VirtualKeyCode.CLEAR; break;
            case KeyCode.Return: vkc = VirtualKeyCode.RETURN; break;
            case KeyCode.LeftShift: vkc = VirtualKeyCode.LSHIFT; break;
            case KeyCode.RightShift: vkc = VirtualKeyCode.RSHIFT; break;
            case KeyCode.LeftControl: vkc = VirtualKeyCode.LCONTROL; break;
            case KeyCode.RightControl: vkc = VirtualKeyCode.RCONTROL; break;
            case KeyCode.Menu: vkc = VirtualKeyCode.MENU; break;
            case KeyCode.Pause: vkc = VirtualKeyCode.PAUSE; break;
            case KeyCode.PageUp: vkc = VirtualKeyCode.PRIOR; break;
            case KeyCode.PageDown: vkc = VirtualKeyCode.NEXT; break;
            case KeyCode.CapsLock: vkc = VirtualKeyCode.CAPITAL; break;
            case KeyCode.Escape: vkc = VirtualKeyCode.ESCAPE; break;
            case KeyCode.Space: vkc = VirtualKeyCode.SPACE; break;
            case KeyCode.End: vkc = VirtualKeyCode.END; break;
            case KeyCode.Home: vkc = VirtualKeyCode.HOME; break;
            case KeyCode.LeftArrow: vkc = VirtualKeyCode.LEFT; break;
            case KeyCode.UpArrow: vkc = VirtualKeyCode.UP; break;
            case KeyCode.DownArrow: vkc = VirtualKeyCode.DOWN; break;
            case KeyCode.RightArrow: vkc = VirtualKeyCode.RIGHT; break;
            case KeyCode.Print: vkc = VirtualKeyCode.PRINT; break;
            case KeyCode.Insert: vkc = VirtualKeyCode.INSERT; break;
            case KeyCode.Delete: vkc = VirtualKeyCode.DELETE; break;
            case KeyCode.Help: vkc = VirtualKeyCode.HELP; break;
            case KeyCode.Alpha0: vkc = VirtualKeyCode.VK_0; break;
            case KeyCode.Alpha1: vkc = VirtualKeyCode.VK_1; break;
            case KeyCode.Alpha2: vkc = VirtualKeyCode.VK_2; break;
            case KeyCode.Alpha3: vkc = VirtualKeyCode.VK_3; break;
            case KeyCode.Alpha4: vkc = VirtualKeyCode.VK_4; break;
            case KeyCode.Alpha5: vkc = VirtualKeyCode.VK_5; break;
            case KeyCode.Alpha6: vkc = VirtualKeyCode.VK_6; break;
            case KeyCode.Alpha7: vkc = VirtualKeyCode.VK_7; break;
            case KeyCode.Alpha8: vkc = VirtualKeyCode.VK_8; break;
            case KeyCode.Alpha9: vkc = VirtualKeyCode.VK_9; break;
            case KeyCode.A: vkc = VirtualKeyCode.VK_A; break;
            case KeyCode.B: vkc = VirtualKeyCode.VK_B; break;
            case KeyCode.C: vkc = VirtualKeyCode.VK_C; break;
            case KeyCode.D: vkc = VirtualKeyCode.VK_D; break;
            case KeyCode.E: vkc = VirtualKeyCode.VK_E; break;
            case KeyCode.F: vkc = VirtualKeyCode.VK_F; break;
            case KeyCode.G: vkc = VirtualKeyCode.VK_G; break;
            case KeyCode.H: vkc = VirtualKeyCode.VK_H; break;
            case KeyCode.I: vkc = VirtualKeyCode.VK_I; break;
            case KeyCode.J: vkc = VirtualKeyCode.VK_J; break;
            case KeyCode.K: vkc = VirtualKeyCode.VK_K; break;
            case KeyCode.L: vkc = VirtualKeyCode.VK_L; break;
            case KeyCode.M: vkc = VirtualKeyCode.VK_M; break;
            case KeyCode.N: vkc = VirtualKeyCode.VK_N; break;
            case KeyCode.O: vkc = VirtualKeyCode.VK_O; break;
            case KeyCode.P: vkc = VirtualKeyCode.VK_P; break;
            case KeyCode.Q: vkc = VirtualKeyCode.VK_Q; break;
            case KeyCode.R: vkc = VirtualKeyCode.VK_R; break;
            case KeyCode.S: vkc = VirtualKeyCode.VK_S; break;
            case KeyCode.T: vkc = VirtualKeyCode.VK_T; break;
            case KeyCode.U: vkc = VirtualKeyCode.VK_U; break;
            case KeyCode.V: vkc = VirtualKeyCode.VK_V; break;
            case KeyCode.W: vkc = VirtualKeyCode.VK_W; break;
            case KeyCode.X: vkc = VirtualKeyCode.VK_X; break;
            case KeyCode.Y: vkc = VirtualKeyCode.VK_Y; break;
            case KeyCode.Z: vkc = VirtualKeyCode.VK_Z; break;
            case KeyCode.LeftWindows: vkc = VirtualKeyCode.LWIN; break;
            case KeyCode.RightWindows: vkc = VirtualKeyCode.RWIN; break;
            case KeyCode.LeftCommand: vkc = VirtualKeyCode.LWIN; break;
            case KeyCode.RightCommand: vkc = VirtualKeyCode.RWIN; break;
            case KeyCode.Keypad0: vkc = VirtualKeyCode.NUMPAD0; break;
            case KeyCode.Keypad1: vkc = VirtualKeyCode.NUMPAD1; break;
            case KeyCode.Keypad2: vkc = VirtualKeyCode.NUMPAD2; break;
            case KeyCode.Keypad3: vkc = VirtualKeyCode.NUMPAD3; break;
            case KeyCode.Keypad4: vkc = VirtualKeyCode.NUMPAD4; break;
            case KeyCode.Keypad5: vkc = VirtualKeyCode.NUMPAD5; break;
            case KeyCode.Keypad6: vkc = VirtualKeyCode.NUMPAD6; break;
            case KeyCode.Keypad7: vkc = VirtualKeyCode.NUMPAD7; break;
            case KeyCode.Keypad8: vkc = VirtualKeyCode.NUMPAD8; break;
            case KeyCode.Keypad9: vkc = VirtualKeyCode.NUMPAD9; break;
            case KeyCode.KeypadMultiply: vkc = VirtualKeyCode.MULTIPLY; break;
            case KeyCode.KeypadPlus: vkc = VirtualKeyCode.ADD; break;
            case KeyCode.KeypadMinus: vkc = VirtualKeyCode.SUBTRACT; break;
            case KeyCode.Numlock: vkc = VirtualKeyCode.NUMLOCK; break;
            case KeyCode.ScrollLock: vkc = VirtualKeyCode.SCROLL; break;
            case KeyCode.KeypadDivide: vkc = VirtualKeyCode.DIVIDE; break;
            case KeyCode.F1: vkc = VirtualKeyCode.F1; break;
            case KeyCode.F2: vkc = VirtualKeyCode.F2; break;
            case KeyCode.F3: vkc = VirtualKeyCode.F3; break;
            case KeyCode.F4: vkc = VirtualKeyCode.F4; break;
            case KeyCode.F5: vkc = VirtualKeyCode.F5; break;
            case KeyCode.F6: vkc = VirtualKeyCode.F6; break;
            case KeyCode.F7: vkc = VirtualKeyCode.F7; break;
            case KeyCode.F8: vkc = VirtualKeyCode.F8; break;
            case KeyCode.F9: vkc = VirtualKeyCode.F9; break;
            case KeyCode.F10: vkc = VirtualKeyCode.F10; break;
            case KeyCode.F11: vkc = VirtualKeyCode.F11; break;
            case KeyCode.F12: vkc = VirtualKeyCode.F12; break;
            case KeyCode.F13: vkc = VirtualKeyCode.F13; break;
            case KeyCode.F14: vkc = VirtualKeyCode.F14; break;
            case KeyCode.F15: vkc = VirtualKeyCode.F15; break;
            case KeyCode.Plus: vkc = VirtualKeyCode.OEM_PLUS; break;
            case KeyCode.Comma: vkc = VirtualKeyCode.OEM_COMMA; break;
            case KeyCode.Minus: vkc = VirtualKeyCode.OEM_MINUS; break;
            case KeyCode.Period: vkc = VirtualKeyCode.OEM_PERIOD; break;
            case KeyCode.Semicolon: vkc = VirtualKeyCode.OEM_1; break;
            case KeyCode.Equals: vkc = VirtualKeyCode.OEM_PLUS; break;
            case KeyCode.Slash: vkc = VirtualKeyCode.OEM_2; break;
            case KeyCode.BackQuote: vkc = VirtualKeyCode.OEM_3; break;
            case KeyCode.LeftBracket: vkc = VirtualKeyCode.OEM_4; break;
            case KeyCode.Backslash: vkc = VirtualKeyCode.OEM_5; break;
            case KeyCode.RightBracket: vkc = VirtualKeyCode.OEM_6; break;
            case KeyCode.Quote: vkc = VirtualKeyCode.OEM_7; break;
            case KeyCode.LeftAlt: vkc = VirtualKeyCode.LMENU; break;
            case KeyCode.RightAlt: vkc = VirtualKeyCode.RMENU; break;
            case KeyCode.AltGr: vkc = VirtualKeyCode.MENU; break;
        }
        return vkc;
    }
}
