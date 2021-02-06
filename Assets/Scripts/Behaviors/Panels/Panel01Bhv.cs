using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel01Bhv : PanelBhv
{
    public List<AudioInput> AudioInputs;

    private List<AudioInputBhv> _audioInputBhvs;

    private CheckBoxBhv _allEnabled;
    private Transform _listSource;
    private TMPro.TextMeshPro _pageNumber;
    private ButtonBhv _pagePrevious;
    private ButtonBhv _pageNext;
    private ButtonBhv _newButton;


    private float _spaceBetween = Constants.Pixel * 12;
    private int _currentPage = 0;

    void Start()
    {
        Init();
        SetButtons();
        LoadData();
    }

    public override void Init()
    {
        if (_hasInit)
            return;
        base.Init();

        AudioInputs = PlayerPrefHelper.GetAudioInputs();
        _allEnabled = GameObject.Find("AllEnabled").GetComponent<CheckBoxBhv>();
        _listSource = GameObject.Find("ListSource").transform;
        _pageNumber = GameObject.Find("PageNumber").GetComponent<TMPro.TextMeshPro>();

        _hasInit = true;
    }

    private void SetButtons()
    {
        _allEnabled.GetComponent<ButtonBhv>().EndActionDelegate = SetAllEnable;
        (_pagePrevious = GameObject.Find("PagePrevious").GetComponent<ButtonBhv>()).EndActionDelegate = () => { SetCurrentPage(_currentPage - 1); };
        (_pageNext = GameObject.Find("PageNext").GetComponent<ButtonBhv>()).EndActionDelegate = () => { SetCurrentPage(_currentPage + 1); };
        (_newButton = GameObject.Find("NewButton").GetComponent<ButtonBhv>()).EndActionDelegate = () => { NewAudioInput(); };
    }

    private void LoadData()
    {
        SetCurrentPage(_currentPage);
    }

    private void UpdateList()
    {
        var oldAudioInputs = GameObject.FindGameObjectsWithTag(Constants.TagAudioInputs);
        for (int dead = oldAudioInputs.Length - 1; dead >= 0; --dead)
            Destroy(oldAudioInputs[dead]);
        int y = 0;
        for (int i = _currentPage * 10; i < (_currentPage + 1) * 10; ++i)
        {
            if (i >= AudioInputs.Count)
            {
                _pageNext.DisableButton();
                break;
            }
            Instantiator.NewAudioInput(_listSource, new Vector3(0.0f, y * -_spaceBetween, 0.0f), AudioInputs[i], i, this);
            AudioInputs[i].IdInScene = y;
            ++y;
        }
        if ((_currentPage + 1) * 10 >= AudioInputs.Count)
            _pageNext.DisableButton();
        if (AudioInputs.Count == Constants.MaxAudioInputs)
            _newButton.DisableButton();
        else if (_newButton.Disabled)
            _newButton.EnableButton();
    }

    private void SetCurrentPage(float pageNumber)
    {
        var intValue = (int)pageNumber;
        var maxPage = Constants.MaxAudioInputs / 10;
        if (intValue < 0)
            intValue = 0;
        else if (intValue > maxPage)
            intValue = maxPage;
        if (intValue == 0)
            _pagePrevious.DisableButton();
        else if (_pagePrevious.Disabled)
            _pagePrevious.EnableButton();
        if (intValue == maxPage)
            _pageNext.DisableButton();
        else if (_pageNext.Disabled)
            _pageNext.EnableButton();
        _currentPage = intValue;
        _pageNumber.text = (intValue + 1).ToString();
        UpdateList();
    }

    private void NewAudioInput()
    {
        if (AudioInputs.Count >= Constants.MaxAudioInputs)
            return;
        AudioInputs.Add(new AudioInput());
        var id = AudioInputs.Count - 1;
        PlayerPrefHelper.SetAudioInput(AudioInputs[id], id);
        if (AudioInputs.Count > (_currentPage + 1) * 10)
        {
            SetCurrentPage((AudioInputs.Count / 10) + (AudioInputs.Count % 10 == 0 ? -1 : 0));
        }
        else
        {
            UpdateList();
        }
    }

    public void OnDelete(int id)
    {
        AudioInputs.RemoveAt(id);
        PlayerPrefHelper.SetAudioInputs(AudioInputs);
        AudioInputs.Clear();
        AudioInputs = PlayerPrefHelper.GetAudioInputs();
        if (AudioInputs.Count <= (_currentPage) * 10)
        {
            SetCurrentPage(_currentPage - 1);
            _pageNext.DisableButton();
        }
        else
        {
            UpdateList();
        }
    }

    public void SetAllEnable()
    {
        var currentAllEnabled = GetAllEnabled();
        var fullEnable = true;
        if (currentAllEnabled == 1)
        {
            fullEnable = false;
        }
        foreach (var input in AudioInputs)
        {
            input.Enabled = fullEnable;
        }
        UpdateAllEnabled();
        PlayerPrefHelper.SetAudioInputs(AudioInputs);
        UpdateList();
    }

    public void UpdateAllEnabled()
    {
        var currentAllEnabled = GetAllEnabled();
        if (currentAllEnabled == 0)
        {
            _allEnabled.Uncheck();
        }
        else if (currentAllEnabled == 1)
        {
            _allEnabled.Check();
        }
        else
        {
            _allEnabled.Quantic();
        }
    }

    public int GetAllEnabled()
    {
        int enabled = 0;
        int disabled = 0;
        foreach (var input in AudioInputs)
        {
            if (input.Enabled)
                ++enabled;
            else
                ++disabled;
        }
        if (enabled == 0)
            return 0;
        else if (disabled == 0)
            return 1;
        else
            return 2;
    }

    public void UpdateTrigger(int id)
    {
        if (id < _listSource.childCount)
            _listSource.GetChild(id).GetComponent<AudioInputBhv>().Tilt();
    }
}
