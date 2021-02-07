using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lasp;
using WindowsInput;
using WindowsInput.Native;
using System.Linq;
using System.Collections;

public class AbjectAudioInputs : MonoBehaviour
{
    public bool On = false;

    public float DynRangeDB { get; set; }
    public Sprite TabBigOn;
    public Sprite TabBigOff;
    public Sprite TabSmallOn;
    public Sprite TabSmallOff;

    private float _pitchValue;
    private int _binSize = 2048;
    private float _threshold = 0.01f;
    private int _idTimeHolding = -1;
    private int MaxPeak = 5;
    private float freqNMultiplier = 0.5f;

    private SpectrumAnalyzer _spectrumAnalyzer;
    private AudioLevelTracker _audioLevelTracker;
    private InputSimulator _inputSimulator;

    private List<Peak> _peaks = new List<Peak>();
    private int _peaksCount;
    private float[] _spectrum;
    int _samplerate;

    private TMPro.TextMeshPro _dbData;
    private TMPro.TextMeshPro _hzData;
    private TMPro.TextMeshPro _peaksData;
    private LevelDrawerBhv _levelDrawer;
    private SpectrumDrawerBhv _spectrumDrawer;
    private CheckBoxBhv _onOff;

    private List<PanelBhv> _panels;
    private Panel00Bhv _panel00;
    private Panel01Bhv _panel01;
    private Panel02Bhv _panel02;
    private Panel03Bhv _panel03;
    private Vector3 _resetPanelPosition = new Vector3(-450.0f, -250.0f, 0.0f);

    void Start()
    {
        Init();
        SetButtons();
        LoadData();
    }

    private void Init()
    {
        Application.targetFrameRate = 60;
        _samplerate = AudioSettings.outputSampleRate;
        _spectrumAnalyzer = this.GetComponent<SpectrumAnalyzer>();
        _audioLevelTracker = this.GetComponent<AudioLevelTracker>();
        _inputSimulator = new InputSimulator();

        _dbData = Helper.GetFieldData("Db");
        _hzData = Helper.GetFieldData("Hz");
        _peaksData = Helper.GetFieldData("Peaks");
        _levelDrawer = GameObject.Find("LevelDrawer").GetComponent<LevelDrawerBhv>();
        _spectrumDrawer = GameObject.Find("SpectrumDrawer").GetComponent<SpectrumDrawerBhv>();
        _onOff = GameObject.Find("OnOff").GetComponent<CheckBoxBhv>();

        _panels = new List<PanelBhv>();
        _panels.Add(GameObject.Find("Panel00").GetComponent<PanelBhv>());
        _panels.Add(GameObject.Find("Panel01").GetComponent<PanelBhv>());
        _panels.Add(GameObject.Find("Panel02").GetComponent<PanelBhv>());
        _panels.Add(GameObject.Find("Panel03").GetComponent<PanelBhv>());
        _panel00 = GameObject.Find("Panel00").GetComponent<Panel00Bhv>();
        _panel01 = GameObject.Find("Panel01").GetComponent<Panel01Bhv>();
        _panel02 = GameObject.Find("Panel02").GetComponent<Panel02Bhv>();
        _panel03 = GameObject.Find("Panel03").GetComponent<Panel03Bhv>();
    }

    private void SetButtons()
    {
        GameObject.Find("Tab00").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetPanel(0); };
        GameObject.Find("Tab01").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetPanel(1); };
        GameObject.Find("Tab02").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetPanel(2); };
        GameObject.Find("Tab03").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetPanel(3); };
        _onOff.GetComponent<ButtonBhv>().EndActionDelegate = OnOff;
    }

    private void LoadData()
    {
        _panel00.Init();
        _panel01.Init();
        _panel02.Init();
        _panel03.Init();
        SetPanel(0);
    }

    private void SetPanel(int id)
    {
        var referencePosition = GameObject.Find("---------- Panels").transform.position;
        for (int i = 0; i < _panels.Count; ++i)
        {
            var tabSpriteRenderer = GameObject.Find($"Tab0{i}").GetComponent<SpriteRenderer>();
            var textMesh = tabSpriteRenderer.gameObject.transform.childCount > 0
                ? tabSpriteRenderer.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>()
                : null;
            if (id == i)
            {
                _panels[i].transform.position = referencePosition;
                _panels[i].enabled = true;
                tabSpriteRenderer.sprite = i < 3 ? TabBigOn : TabSmallOn;
                tabSpriteRenderer.sortingOrder = 10;
                if (textMesh != null)
                    textMesh.text = $"<material=\"3x5.1.2.3\">{textMesh.text}";
            }
            else
            {
                _panels[i].transform.position = _resetPanelPosition;
                _panels[i].enabled = false;
                tabSpriteRenderer.sprite = i < 3 ? TabBigOff : TabSmallOff;
                tabSpriteRenderer.sortingOrder = 0;
                if (textMesh != null && textMesh.text.Contains("material"))
                    textMesh.text = textMesh.text.Substring(22);
            }
        }
    }

    void Update()
    {
        AnalyzeSound();
        _dbData.text = DynRangeDB.ToString("0");
        _hzData.text = DynRangeDB > 0 ? _pitchValue.ToString("F2") : "0.00";
        _peaksData.text = DynRangeDB > 0 ? _peaksCount.ToString() : "0";
        if (DynRangeDB > _panel00.LevelReset)
            AnalyseAudioInputs();
        else if (DynRangeDB <= _panel00.LevelReset && _lastFrameLevel <= _panel00.LevelReset)
            LevelReset();
        _levelDrawer.Draw(_audioLevelTracker);
        _spectrumDrawer.Draw(_spectrum);
        _lastFrameLevel = DynRangeDB;
        if (_idTimeHolding != -1)
            UpdatePanelVisual(true, "_", InputType.TimeHolded, _idTimeHolding);
    }

    private void LevelReset()
    {
        _hasToWaitResetBeforeNewInput = false;
        if (_holdedInputs != null && _holdedInputs.Count > 0)
        {
            foreach (var audioInput in _holdedInputs)
            {
                _inputSimulator.Keyboard.KeyUp(audioInput.Key);
            }
            _holdedInputs.Clear();
        }
        UpdatePanelVisual(false);
    }

    private float _lastFrameLevel = 0.0f;
    private float _lastFrameFrequency = 0.00f;
    private int _nbConsecutiveFrames = 1;
    private int _nbConsecutiveFramesDefault = 1;
    private bool _hasToWaitResetBeforeNewInput = false;

    private List<AudioInput> _holdedInputs;

    void AnalyseAudioInputs()
    {
        if (Helper.FloatEqualsPrecision(_pitchValue, _lastFrameFrequency, _panel00.HzOffset))
        {
            if (_nbConsecutiveFrames >= _panel00.RequiredFrames)
            {
                PrepareAudioInputsFromFrequency(_pitchValue);
            }
            ++_nbConsecutiveFrames;
        }
        else
            _nbConsecutiveFrames = _nbConsecutiveFramesDefault;
        _lastFrameFrequency = _pitchValue;
    }

    private void PrepareAudioInputsFromFrequency(float frequency)
    {
        var validFrequencies = new List<AudioInput>();
        for (int i = 0; i < _panel01.AudioInputs.Count; ++i)
        {
            if (_panel01.AudioInputs[i].Enabled && _panel01.AudioInputs[i].Key != VirtualKeyCode.NONAME && Helper.FloatEqualsPrecision(_pitchValue, _panel01.AudioInputs[i].Hz, _panel00.HzOffset))
            {
                validFrequencies.Add(_panel01.AudioInputs[i]);
            }
        }
        if (validFrequencies.Count == 1)
            SendAudioInput(validFrequencies[0]);
        else if (validFrequencies.Count > 1)
        {
            var lowestPeakDifference = 1080;
            var lowestId = -1;
            for (int i = 0; i < validFrequencies.Count; ++i)
            {
                var difference = Mathf.Abs(validFrequencies[i].Peaks - _peaksCount);
                if (difference < lowestPeakDifference)
                {
                    lowestPeakDifference = difference;
                    lowestId = i;
                }
                else if (difference == lowestPeakDifference
                    && ((_panel00.PeaksPriority == PeaksPriority.Higher && validFrequencies[i].Peaks > validFrequencies[lowestId].Peaks)
                    || (_panel00.PeaksPriority == PeaksPriority.Lower && validFrequencies[i].Peaks < validFrequencies[lowestId].Peaks)))
                {
                    lowestPeakDifference = difference;
                    lowestId = i;
                }
            }
            if (lowestId != -1)
                SendAudioInput(validFrequencies[lowestId]);
        }

    }

    private void SendAudioInput(AudioInput audioInput)
    {
        if (audioInput.InputType == InputType.SingleTap && !_hasToWaitResetBeforeNewInput)
        {
            if (On)
                _inputSimulator.Keyboard.KeyPress(audioInput.Key);
            UpdatePanelVisual(true, audioInput.Key.ToString(), InputType.SingleTap, audioInput.IdInScene);
            _hasToWaitResetBeforeNewInput = true;
        }
        else if (audioInput.InputType == InputType.Holded)
        {
            if (_holdedInputs == null)
                _holdedInputs = new List<AudioInput>();
            if (_holdedInputs.Find(a => a.Key == audioInput.Key) == null)
            {
                if (On)
                    _inputSimulator.Keyboard.KeyDown(audioInput.Key);
                UpdatePanelVisual(true, audioInput.Key.ToString(), InputType.Holded, audioInput.IdInScene);
                _holdedInputs.Add(audioInput);
            }
            else
                UpdatePanelVisual(true, "_", InputType.Holded, audioInput.IdInScene);
        }
        else if (audioInput.InputType == InputType.CustomTap && !_hasToWaitResetBeforeNewInput)
        {
            StartCoroutine(CustomTapSend(audioInput.Key, audioInput.Param, audioInput.IdInScene));
            _hasToWaitResetBeforeNewInput = true;
        }
        else if (audioInput.InputType == InputType.TimeHolded && !_hasToWaitResetBeforeNewInput)
        {
            if (On)
                _inputSimulator.Keyboard.KeyDown(audioInput.Key);
            UpdatePanelVisual(true, audioInput.Key.ToString(), InputType.TimeHolded, audioInput.IdInScene);
            StartCoroutine(TimeHolded(audioInput.Key, audioInput.Param));
            _idTimeHolding = audioInput.IdInScene;
            _hasToWaitResetBeforeNewInput = true;
        }
        else
            UpdatePanelVisual(false);
    }

    private void UpdatePanelVisual(bool down, string key = null, InputType type = InputType.SingleTap, int id = -1)
    {
        if (On && _panel00.enabled)
        {
            _panel00.UpdateIcon(down, key, type);
        }
        else if (down == true)
        {
            if (_panel01.enabled && id != -1)
                _panel01.UpdateTrigger(id);
        }
    }

    private IEnumerator CustomTapSend(VirtualKeyCode key, int count, int id)
    {
        if (On)
            _inputSimulator.Keyboard.KeyPress(key);
        UpdatePanelVisual(true, key.ToString(), InputType.CustomTap, id);
        --count;
        if (count > 0)
        {
            yield return new WaitForSeconds(_panel00.CustomTapDelay);
            StartCoroutine(CustomTapSend(key, count, id));
        }
    }

    private IEnumerator TimeHolded(VirtualKeyCode key, int timeHolded)
    {
        yield return new WaitForSeconds(timeHolded);
        if (On)
            _inputSimulator.Keyboard.KeyUp(key);
        _idTimeHolding = -1;
    }

    void AnalyzeSound()
    {
        _spectrum = _spectrumAnalyzer.logSpectrumArray.ToArray();
        float maxV = 0f;
        _peaksCount = 0;
        if (_spectrum != null && _spectrum.Count() > 0)
        for (int i = 0; i < _binSize; i++)
        {
            if (_spectrum[i] > maxV && _spectrum[i] > _threshold)
            {
                _peaks.Add(new Peak(_spectrum[i], i));
                if (_peaks.Count > MaxPeak)
                {
                    _peaks.Sort(new AmpComparer());
                    _peaks.Remove (_peaks [MaxPeak]);
                }
            }
            if (i > _threshold * MaxPeak && _spectrum[i - 1] > 0 && _spectrum[i] == 0)
                ++_peaksCount;
        }
        float freqN = 0f;
        if (_peaks.Count > 0)
        {
            int maxN = _peaks[0].index;
            freqN = maxN;
            if (maxN > 0 && maxN < _binSize - 1)
            {
                var dL = _spectrum[maxN - 1] / _spectrum[maxN];
                var dR = _spectrum[maxN + 1] / _spectrum[maxN];
                freqN += freqNMultiplier * (dR * dR - dL * dL);
            }
        }
        _pitchValue = freqN * (_samplerate / 2f) / _binSize;
        _pitchValue /= 100.0f;
        _peaks.Clear();
    }

    private void OnOff()
    {
        On = !On;
        if (On)
            _onOff.Check();
        else
            _onOff.Uncheck();
    }
}