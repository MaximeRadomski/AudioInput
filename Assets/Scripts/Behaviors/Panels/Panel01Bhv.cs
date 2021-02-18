using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class Panel01Bhv : PanelBhv
{
    public List<AudioInput> AudioInputs;

    private CheckBoxBhv _allEnabled;
    private Transform _listSource;
    private TMPro.TextMeshPro _pageNumber;
    private ButtonBhv _pagePrevious;
    private ButtonBhv _pageNext;
    private ButtonBhv _newButton;
    private ButtonBhv _importButton;
    private ButtonBhv _exportButton;


    private float _spaceBetween = Constants.Pixel * 12;
    private int _currentPage = 0;

    void Start()
    {
        Init();
        SetButtons();
        SetFolders();
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
        
        (_importButton = GameObject.Find("ImportButton").GetComponent<ButtonBhv>()).EndActionDelegate = Import;
        (_exportButton = GameObject.Find("ExportButton").GetComponent<ButtonBhv>()).EndActionDelegate = Export;
    }

    private void SetFolders()
    {
        var folderPath = $"{Application.dataPath}/../{Constants.ExportsFolderName}";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
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
        foreach (var audioInput in AudioInputs)
            audioInput.Id = -1;
        for (int i = _currentPage * 10; i < (_currentPage + 1) * 10; ++i)
        {
            if (i >= AudioInputs.Count)
            {
                _pageNext.DisableButton();
                break;
            }
            Instantiator.NewAudioInput(_listSource, new Vector3(0.0f, y * -_spaceBetween, 0.0f), AudioInputs[i], i, this);
            AudioInputs[i].Id = y;
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
        ActualizeList();
    }

    private void ActualizeList()
    {
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
            SetCurrentPage(_currentPage);
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

    private void Import()
    {
        SetFolders();

        var folderPath = $"{Application.dataPath}/../{Constants.ExportsFolderName}";
        var filesNames = Directory.GetFiles(folderPath);
        if (filesNames == null || filesNames.Length == 0)
        {
            this.Instantiator.NewPopupYesNo(transform.position, "file missing", "there is no available file in your exports folder", null, "damn...", null);
            return;
        }

        var listFilesNames = new List<string>();
        foreach (var name in filesNames)
            listFilesNames.Add(name.Substring(folderPath.Length).Replace("\\", string.Empty).Replace("/", string.Empty));

        this.Instantiator.NewPopupList(transform.position, "import", -1, listFilesNames, OnImportSelected);

        object OnImportSelected(int id)
        {
            if (id == -1)
                return false;
            var fileText = File.ReadAllText(filesNames[id]);

            var tmpList = new List<AudioInput>();
            for (int i = 0; i < Constants.MaxAudioInputs; ++i)
            {
                var bracketOpenId = fileText.IndexOf('{');
                if (bracketOpenId == -1)
                    break;
                var bracketClosedId = fileText.Substring(bracketOpenId).IndexOf('}');
                if (bracketClosedId == -1)
                    break;
                var isolatedStr = fileText.Substring(bracketOpenId, bracketClosedId + 1);
                var tmpAudioInputJson = JsonUtility.FromJson<AudioInputJson>(isolatedStr);
                tmpList.Add(tmpAudioInputJson.ToAudioInput());
                var nextStartId = fileText.IndexOf("},");
                if (nextStartId == -1)
                    break;
                fileText = fileText.Substring(nextStartId + 2);
            }

            AudioInputs.Clear();
            AudioInputs = tmpList;
            ActualizeList();

            return true;
        }
    }

    private void Export()
    {
        if (AudioInputs == null || AudioInputs.Count == 0)
        {
            this.Instantiator.PopText("no inputs", _exportButton.transform.position + new Vector3(0.0f, 2.0f, 0.0f), distance: 2.0f, startFadingDistancePercent: 0.50f);
            return;
        }

        SetFolders();

        string path = "";
        string fileName = "";
        int i = 1;
        while (i <= 100)
        {
            fileName = $"Export-{i.ToString("00")}.json";
            path = $"{Application.dataPath}/../{Constants.ExportsFolderName}/{fileName}";
            if (!File.Exists(path))
            {
                File.WriteAllText(path, string.Empty);
                break;
            }
            ++i;
        }

        string content = $"\"{Constants.AudioInputsJson}\": [\n";
        for (int id = 0; id < AudioInputs.Count; ++id)
        {
            if (id != 0)
                content += ",\n";
            var json = AudioInputs[id].ToAudioInputJson();
            content += $"{JsonUtility.ToJson(json)}";
        }
        content += "]";

        File.AppendAllText(path, content);
        this.Instantiator.PopText(fileName.ToLower(), _exportButton.transform.position + new Vector3(6.0f, 0.0f, 0.0f), distance: 2.0f, startFadingDistancePercent: 0.50f);
        //openInExplorer();
    }

    private void openInExplorer()
    {
        var upperFolder = $"{Application.dataPath}/../".Replace("Assets/../", string.Empty);
        if (Directory.Exists(upperFolder))
        {
            var folders = Directory.GetDirectories(upperFolder);
            foreach (var folder in folders)
            {
                if (folder.Contains("Exports"))
                {
                    string cmd = "explorer.exe";
                    string arg = "/select, " + folder;
                    Process.Start(cmd, $"\"{arg}\"");
                    return;
                }
            }
        }
        
    }
}
