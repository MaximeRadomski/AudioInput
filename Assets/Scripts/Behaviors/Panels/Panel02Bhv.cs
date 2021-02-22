using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel02Bhv : PanelBhv
{
    private ButtonBhv _pageUp;
    private ButtonBhv _pageDown;

    private TMPro.TextMeshPro _toolVersion;

    private int _currentPage;
    private int _maxPage;
    private Vector3 _resetPagePosition;

    void Start()
    {
        Init();
        SetButtons();
        LoadData();
    }

    public override void Init()
    {
        base.Init();

        (_pageUp = transform.Find("PageUp").GetComponent<ButtonBhv>()).EndActionDelegate = PageUp;
        (_pageDown = transform.Find("PageDown").GetComponent<ButtonBhv>()).EndActionDelegate = PageDown;

        _currentPage = 0;
        _maxPage = 0;
        while (_maxPage < 30)
        {
            if (transform.Find($"Page{_maxPage.ToString("00")}") == null)
                break;
            ++_maxPage;
        }
        _resetPagePosition = new Vector3(-200.0f, -200.0f, 0.0f);
    }

    private void SetButtons()
    {
        transform.Find("Page01").transform.Find("DarkSoulsButton").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://youtu.be/haevbe2UXQA"); };
        transform.Find("Page01").transform.Find("DoomEternalButton").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://youtu.be/VRMzU4o57-c"); };
        transform.Find("Page01").transform.Find("TwitterButton").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://twitter.com/Abject_Sama"); };
        transform.Find("Page01").transform.Find("OnlyFanButton").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://youtu.be/dQw4w9WgXcQ"); };

        _toolVersion = transform.Find("Page02").transform.Find("ToolVersion").GetComponent<TMPro.TextMeshPro>();

        transform.Find("Page02").transform.Find("DigitalSensei").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://www.twitch.tv/digitalsenseigaming"); };
        transform.Find("Page02").transform.Find("LobosJr").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://www.twitch.tv/lobosjr"); };
        transform.Find("Page02").transform.Find("KeijiroTakahashi").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://github.com/keijiro/Lasp"); };
        transform.Find("Page02").transform.Find("Devs").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://forum.unity.com/threads/detecting-musical-notes-from-vocal-input.316698/"); };
        transform.Find("Page02").transform.Find("MichaelNoonan").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://github.com/michaelnoonan/inputsimulator"); };
        transform.Find("Page02").transform.Find("Elringus").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://github.com/Elringus/UnityRawInput"); };
        transform.Find("Page02").transform.Find("GrafxKid").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://lospec.com/palette-list/oil-6"); };
    }

    private void LoadData()
    {
        _toolVersion.text = $"version {Application.version}";
        LoadPage(_currentPage);
    }

    private void LoadPage(int pageId)
    {
        for (int i = 0; i < _maxPage; ++i)
        {
            var page = transform.Find($"Page{i.ToString("00")}");
            if (i == pageId)
                page.transform.position = transform.position;
            else
                page.transform.position = _resetPagePosition;
        }
        if (pageId <= 0)
            _pageUp.DisableButton();
        else
            _pageUp.EnableButton();
        if (pageId + 1 >= _maxPage)
            _pageDown.DisableButton();
        else
            _pageDown.EnableButton();
    }

    private void PageUp()
    {
        if (_currentPage - 1 >= 0)
            --_currentPage;
        LoadPage(_currentPage);
    }

    private void PageDown()
    {
        if (_currentPage + 1 < _maxPage)
            ++_currentPage;
        LoadPage(_currentPage);
    }
}
