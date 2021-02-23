using System.Collections.Generic;
using UnityEngine;
using Lasp;
using WindowsInput;
using WindowsInput.Native;
using System.Linq;
using System.Collections;

public class AbjectAudioInputs : MonoBehaviour
{
    public float DynRangeDB { get; set; }
    public Sprite TabBigOn;
    public Sprite TabBigOff;
    public Sprite TabSmallOn;
    public Sprite TabSmallOff;

    private float _pitchValue;
    private int _binSize = 2048;
    private float _threshold = 0.05f;
    private int _maxPeak = 5;
    private int _maxPeakCheck = 1;

    private SpectrumAnalyzer _spectrumAnalyzer;
    private AudioLevelTracker _audioLevelTracker;
    private InputSimulator _inputSimulator;

    private List<Peak> _peaks = new List<Peak>();
    private int _peaksCount;
    private float[] _spectrum;
    private int _samplerate;
    private int _currentPanel = -1;

    private Instantiator _instantiator;
    private TMPro.TextMeshPro _dbData;
    private TMPro.TextMeshPro _hzData;
    private TMPro.TextMeshPro _peaksData;
    private LevelDrawerBhv _levelDrawer;
    private SpectrumDrawerBhv _spectrumDrawer;
    private CheckBoxBhv _onOff;
    private ParticleSystem _notesThrower;
    private float _lastNoteThrow;

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

        _instantiator = GetComponent<Instantiator>();
        _dbData = Helper.GetFieldData("Db");
        _hzData = Helper.GetFieldData("Hz");
        _peaksData = Helper.GetFieldData("Peaks");
        _levelDrawer = GameObject.Find("LevelDrawer").GetComponent<LevelDrawerBhv>();
        _spectrumDrawer = GameObject.Find("SpectrumDrawer").GetComponent<SpectrumDrawerBhv>();
        _onOff = GameObject.Find("OnOff").GetComponent<CheckBoxBhv>();
        _notesThrower = GameObject.Find("NotesThrower").GetComponent<ParticleSystem>();
        _lastNoteThrow = Time.time;

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
        _onOff.GetComponent<ButtonBhv>().EndActionDelegate = () => { OnOff(); };
    }

    private void LoadData()
    {
        _panel00.Init();
        _panel01.Init();
        _panel02.Init();
        _panel03.Init();
        SetPanel(0);
        OnOff(Constants.IsOn);
        StartCoroutine(Helper.ExecuteAfterDelay(0.1f, () =>
        {
            if (!Constants.HasInit)
                _instantiator.NewPopupYesNo(_panel00.transform.position, "hover help", "this tool use a hover system to explain its features. if you are not sure what a feature does, hover its label to get its description.", null, "understood", null);
            Constants.HasInit = true;
            return true;
        }, lockInputWhile: true));
    }

    private void SetPanel(int id)
    {
        if (_currentPanel == id)
            return;
        _currentPanel = id;
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
        if (Constants.IsOn)
            MoveMouseOutOfSync();
        if (Time.time < Constants.LastFrame + Constants.Frame)
            return;
        Constants.LastFrame = Time.time;
        AnalyzeSound();
        _dbData.text = DynRangeDB.ToString("0");
        _hzData.text = DynRangeDB > 0 ? _pitchValue.ToString("F2") : "0.00";
        _peaksData.text = DynRangeDB > 0 ? _peaksCount.ToString() : "0";
        var isUnderHeldReset = _levelDrawer.Draw(_audioLevelTracker, _panel00.HeldReset, (int)_currentSingleFrameReset);
        _spectrumDrawer.Draw(_spectrum, _threshold, _peaks);

        if (DynRangeDB > 0)
            AnalyseAudioInputs();
        if (isUnderHeldReset)
            HeldReset();
        if (DynRangeDB <= _currentSingleFrameReset)
            SingleTapReset();
        
        if(_heldInputs != null && _heldInputs.Count > 0)
            HandleHeld();
        if (_timeHeldInputs != null && _timeHeldInputs.Count > 0)
            HandleTimeHeld();

        _currentValidAudioInput = null;
    }    

    private void HeldReset()
    {
        if (_heldInputs != null && _heldInputs.Count > 0)
        {
            foreach (var audioInput in _heldInputs)
            {
                if (audioInput.MouseInput == MouseInput.None)
                    _inputSimulator.Keyboard.KeyUp(audioInput.Key);
                else
                    TriggerMouseInput(audioInput.MouseInput, down: false);
            }
            _heldInputs.Clear();
        }
        UpdatePanelVisual(false);
    }

    private void SingleTapReset()
    {
        var limitOverDynRangeDB = DynRangeDB + (DynRangeDB + 10);
        if (_currentSingleFrameReset > limitOverDynRangeDB)
        {
            _currentSingleFrameReset = limitOverDynRangeDB;
        }
        if (DynRangeDB == 0)
            _currentSingleFrameReset -= 0.5f;
        if (_currentSingleFrameReset < 0)
            _currentSingleFrameReset = 0;
        _lastMaxBeforeNewInput = limitOverDynRangeDB;
        _hasToWaitResetBeforeNewSingle = false;
    }

    private void HandleHeld()
    {
        for (int i = _heldInputs.Count - 1; i >= 0; --i)
        {
            if (_currentValidAudioInput != null && _heldInputs[i] != null
                && (int)_heldInputs[i].Param == Constants.HeldUntilReleased
                && !(_heldInputs[i].Key == _currentValidAudioInput.Key && _heldInputs[i].MouseInput == _currentValidAudioInput.MouseInput && _heldInputs[i].Hz == _currentValidAudioInput.Hz))
            {
                if (_heldInputs[i].MouseInput == MouseInput.None)
                    _inputSimulator.Keyboard.KeyUp(_heldInputs[i].Key);
                else
                    TriggerMouseInput(_heldInputs[i].MouseInput, down: false);
                _heldInputs.RemoveAt(i);
                continue;
            }
            UpdatePanelVisual(true, i == 0 ? "_" : string.Empty, InputType.Held, _heldInputs[i].Id);
        }
    }

    private void HandleTimeHeld()
    {
        for (int i = _timeHeldInputs.Count - 1; i >= 0; --i)
        {
            if (_timeHeldInputs[i].HiddenParam > Time.time)
                UpdatePanelVisual(true, i == 0 ? "_" : string.Empty, InputType.TimeHeld, _timeHeldInputs[i].Id);
            else
            {
                if (_timeHeldInputs[i].MouseInput == MouseInput.None)
                    _inputSimulator.Keyboard.KeyUp(_timeHeldInputs[i].Key);
                else
                    TriggerMouseInput(_timeHeldInputs[i].MouseInput, down: false);
                _timeHeldInputs.RemoveAt(i);
            }
        }
        if (_timeHeldInputs.Count == 0)
            UpdatePanelVisual(false);
    }

    private float _lastFrameFrequency = 0.00f;
    private float _currentSingleFrameReset = 0.0f;
    private int _nbConsecutiveFrames = 1;
    private int _nbConsecutiveFramesDefault = 1;
    private bool _hasToWaitResetBeforeNewSingle = false;
    private float _lastMaxBeforeNewInput = 0.0f;
    private AudioInput _currentValidAudioInput;

    private List<AudioInput> _heldInputs;
    private List<AudioInput> _timeHeldInputs;

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
            //var isSet = (_panel01.AudioInputs[i].InputType != InputType.Mouse && _panel01.AudioInputs[i].Key != VirtualKeyCode.NONAME) || (_panel01.AudioInputs[i].InputType == InputType.Mouse && _panel01.AudioInputs[i].MouseInput != MouseInput.None);
            var isSet = _panel01.AudioInputs[i].Hz > 0.0f;
            if (_panel01.AudioInputs[i].Enabled && isSet && Helper.FloatEqualsPrecision(_pitchValue, _panel01.AudioInputs[i].Hz, _panel00.HzOffset))
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
        var inputStr = audioInput.MouseInput == MouseInput.None ? audioInput.Key.ToString() : audioInput.MouseInput.ToString();
        if (audioInput.InputType == InputType.SingleTap && !_hasToWaitResetBeforeNewSingle)
        {
            if (Constants.IsOn)
            {
                if (audioInput.MouseInput == MouseInput.None)
                    _inputSimulator.Keyboard.KeyDown(audioInput.Key);
                else
                    TriggerMouseInput(audioInput.MouseInput, down: true, (int)audioInput.Param);
                StartCoroutine(InputUpAfterDelay(audioInput.Key, audioInput.MouseInput, Constants.SingleTapDelay));
            }
            UpdatePanelVisual(true, inputStr, InputType.SingleTap, audioInput.Id);
            _hasToWaitResetBeforeNewSingle = true;
        }
        else if (audioInput.InputType == InputType.Held)
        {
            if (_heldInputs == null)
                _heldInputs = new List<AudioInput>();
            if (_heldInputs.Find(a => a.Key == audioInput.Key && a.MouseInput == audioInput.MouseInput && a.Hz == audioInput.Hz) == null)
            {
                if (Constants.IsOn)
                {
                    if (audioInput.MouseInput == MouseInput.None)
                        _inputSimulator.Keyboard.KeyDown(audioInput.Key);
                    else
                        TriggerMouseInput(audioInput.MouseInput, down: true, (int)audioInput.Param);
                }
                UpdatePanelVisual(true, inputStr, InputType.Held, audioInput.Id);
                _heldInputs.Add(audioInput);
            }
            _currentValidAudioInput = audioInput.Clone();
        }
        else if (audioInput.InputType == InputType.CustomTap && !_hasToWaitResetBeforeNewSingle)
        {
            StartCoroutine(CustomTapSend(audioInput.Key, audioInput.MouseInput, (int)audioInput.Param, audioInput.Id));
            _hasToWaitResetBeforeNewSingle = true;
        }
        else if (audioInput.InputType == InputType.TimeHeld && !_hasToWaitResetBeforeNewSingle)
        {
            if (_timeHeldInputs == null)
                _timeHeldInputs = new List<AudioInput>();
            var alreadyContained = _timeHeldInputs.Find(a => a.Key == audioInput.Key && a.Hz == audioInput.Hz);
            if (alreadyContained == null)
            {
                if (Constants.IsOn)
                {
                    if (audioInput.MouseInput == MouseInput.None)
                        _inputSimulator.Keyboard.KeyDown(audioInput.Key);
                    else
                        TriggerMouseInput(audioInput.MouseInput, down: true, (int)audioInput.Param);
                }
                UpdatePanelVisual(true, inputStr, InputType.TimeHeld, audioInput.Id);
                var tmpAudioInput = audioInput.Clone();
                tmpAudioInput.HiddenParam = Time.time + tmpAudioInput.Param;
                _timeHeldInputs.Add(tmpAudioInput);
            }
            else
            {
                for (int i = 0; i < _timeHeldInputs.Count; ++i)
                {
                    if (_timeHeldInputs[i].Key == audioInput.Key && _timeHeldInputs[i].Hz == audioInput.Hz)
                    {
                        _timeHeldInputs[i].HiddenParam = Time.time + _timeHeldInputs[i].Param;
                        break;
                    }
                }
            }
            _hasToWaitResetBeforeNewSingle = true;
        }
        else
            UpdatePanelVisual(false);
    }

    private void UpdatePanelVisual(bool down, string key = null, InputType type = InputType.SingleTap, int id = -1)
    {
        if (Constants.IsOn && down && Time.time > _lastNoteThrow + Constants.SingleTapDelay)
        {
            _lastNoteThrow = Time.time;
            _notesThrower.Play();
        }
        if (Constants.IsOn && _panel00.enabled)
        {
            _panel00.UpdateIcon(down, key, type);
        }
        else if (down == true)
        {
            if (_panel01.enabled && id != -1)
                _panel01.UpdateTrigger(id);
        }
    }

    private IEnumerator CustomTapSend(VirtualKeyCode key, MouseInput mouseInput, int count, int id)
    {
        if (count > 0)
        {
            if (Constants.IsOn)
            {
                if (mouseInput == MouseInput.None)
                    _inputSimulator.Keyboard.KeyDown(key);
                else
                    TriggerMouseInput(mouseInput, down: true);
                StartCoroutine(InputUpAfterDelay(key, mouseInput, Constants.SingleTapDelay));
            }
            UpdatePanelVisual(true, key.ToString(), InputType.CustomTap, id);
            --count;
            yield return new WaitForSeconds(_panel03.CustomTapDelay);
            StartCoroutine(CustomTapSend(key, mouseInput, count, id));
        }
    }

    private void TriggerMouseInput(MouseInput mouseInput, bool down = true, int? cursorOffset = null)
    {
        if (down)
        {
            if (mouseInput == MouseInput.LeftButton)
                _inputSimulator.Mouse.LeftButtonDown();
            else if (mouseInput == MouseInput.RightButton)
                _inputSimulator.Mouse.RightButtonDown();
            else if (mouseInput.GetHashCode() >= MouseInput.Button0.GetHashCode())
                _inputSimulator.Mouse.XButtonDown(mouseInput.GetHashCode() - MouseInput.Button0.GetHashCode());
            else if (Helper.IsMouseDirection(mouseInput))
            {
                if (cursorOffset == null)
                    cursorOffset = _panel03.MouseSensitivity;
                MoveCursor(mouseInput, cursorOffset);
            }
        }
        else
        {
            if (mouseInput == MouseInput.LeftButton)
                _inputSimulator.Mouse.LeftButtonUp();
            else if (mouseInput == MouseInput.RightButton)
                _inputSimulator.Mouse.RightButtonUp();
            else if (mouseInput.GetHashCode() >= MouseInput.Button0.GetHashCode())
                _inputSimulator.Mouse.XButtonUp(mouseInput.GetHashCode() - MouseInput.Button0.GetHashCode());
        }
    }

    private void MoveMouseOutOfSync()
    {
        CycleThroughAudioInputs(_heldInputs);
        CycleThroughAudioInputs(_timeHeldInputs);

        void CycleThroughAudioInputs(List<AudioInput> audioInputs)
        {
            if (audioInputs == null || audioInputs.Count == 0)
                return;
            foreach (var audioInput in audioInputs)
            {
                if ((audioInput.InputType == InputType.Held || audioInput.InputType == InputType.TimeHeld)
                    && Helper.IsMouseDirection(audioInput.MouseInput))
                    MoveCursor(audioInput.MouseInput);
            }
        }
    }

    private void MoveCursor(MouseInput input, int? cursorOffset = null)
    {
        var x = 0;
        var y = 0;
        if (cursorOffset == null)
            cursorOffset = _panel03.MouseSensitivity;
        if (input == MouseInput.Up)
            y = -cursorOffset.Value;
        else if (input == MouseInput.Down)
            y = cursorOffset.Value;
        else if (input == MouseInput.Left)
            x = -cursorOffset.Value;
        else if (input == MouseInput.Right)
            x = cursorOffset.Value;

        _inputSimulator.Mouse.MoveMouseBy(x, y);
    }

    private IEnumerator InputUpAfterDelay(VirtualKeyCode key, MouseInput mouseInput, float timeHeld)
    {
        yield return new WaitForSeconds(timeHeld);
        if (Constants.IsOn)
        {
            if (mouseInput == MouseInput.None)
                _inputSimulator.Keyboard.KeyUp(key);
            else
                TriggerMouseInput(mouseInput, down: false);
        }
    }

    void AnalyzeSound()
    {
        _spectrum = _spectrumAnalyzer.logSpectrumArray.ToArray();
        _peaksCount = 0;
        _peaks.Clear();
        if (_spectrum != null && _spectrum.Count() > 0)
        {
            int currentNbPeak = 0;
            float currentPeakMax = 0.0f;
            var hundredth = _spectrum.Length / 100;
            var twoHundredth = _spectrum.Length / 200;
            for (int i = 0; i < _binSize; i += 1)
            {
                if (_spectrum[i] > currentPeakMax && _spectrum[i] > _threshold)
                {
                    currentPeakMax = _spectrum[i];
                    if (_peaks.Count < currentNbPeak + 1)
                        _peaks.Add(new Peak(_spectrum[i], i));
                    else
                    {
                        _peaks[currentNbPeak].amplitude = _spectrum[i];
                        _peaks[currentNbPeak].index = i;
                    }
                }
                if (i > 0 && _spectrum[i - 1] > _threshold && _spectrum[i] <= _threshold)
                {
                    ++currentNbPeak;
                    currentPeakMax = 0.0f;
                    _peaks.Sort(new AmpComparer());
                    if (_peaks.Count > _maxPeak)
                        _peaks.RemoveAt(_maxPeak);
                }
                if (i > 0 && i % hundredth == 0 && _spectrum[i - hundredth] > _threshold && _spectrum[i] < _threshold)
                    ++_peaksCount;
            }
            _peaks.Sort(new AmpComparer());
        }
        float freqN = 0f;
        var total = 0.0f;
        if (_peaks.Count > 0)
        {
            int maxN = _peaks[0].index;
            freqN = maxN;

            int i = 0;
            while (i < _peaks.Count && i < _maxPeakCheck)
            {
                total += _peaks[i].index;
                ++i;
            }
            total = total / i;
            //var _freqNMultiplier = 0.5f;
            //if (maxN > 0 && maxN < _binSize - 1)
            //{
            //    var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            //    var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            //    freqN += _freqNMultiplier * (dR * dR - dL * dL);
            //}
        }
        _pitchValue = total * (_samplerate / 2f) / _binSize;
        _pitchValue /= 100.0f;

        if (DynRangeDB > _lastMaxBeforeNewInput)
        {
            _lastMaxBeforeNewInput = DynRangeDB;
            _currentSingleFrameReset = DynRangeDB + _panel00.SingleTapReset;
            if (_currentSingleFrameReset < 0.0f)
                _currentSingleFrameReset = 0.0f;
        }
    }

    public void OnOff(bool? isOnParam = null)
    {
        if (isOnParam != null)
            Constants.IsOn = isOnParam.Value;
        else
            Constants.IsOn = !Constants.IsOn;
        if (Constants.IsOn)
            _onOff.Check();
        else
            _onOff.Uncheck();
    }
}