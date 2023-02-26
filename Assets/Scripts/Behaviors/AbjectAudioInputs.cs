using System.Collections.Generic;
using UnityEngine;
using Lasp;
using WindowsInput;
using WindowsInput.Native;
using System.Linq;
using System.Collections;

public class AbjectAudioInputs : FrameRateBehavior
{
    public float DynRangeDB { get; set; }
    public Sprite TabBigOn;
    public Sprite TabBigOff;
    public Sprite TabSmallOn;
    public Sprite TabSmallOff;
    public System.Func<List<float>, object> OnRecordingFrequencies;

    private int _binSize = 2048;

    private SpectrumAnalyzer _spectrumAnalyzer;
    private AudioLevelTracker _audioLevelTracker;
    private InputSimulator _inputSimulator;

    private List<Peak> _peaks = new List<Peak>();
    public List<float> CurrentFrequencies = new List<float>();
    //private int _peaksCount;
    private float[] _spectrum;
    private int _samplerate;

    private Instantiator _instantiator;
    private TMPro.TextMeshPro _dbData;
    private TMPro.TextMeshPro _hzData;
    private TMPro.TextMeshPro _peaksData;
    private LevelDrawerBhv _levelDrawer;
    private SpectrumDrawerBhv _spectrumDrawer;
    private CheckBoxBhv _onOff;
    private ParticleSystem _notesThrower;
    private float _lastNoteThrow;
    private float _mainPanelY = -0.06115063f;

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

        _panels = new List<PanelBhv>
        {
            GameObject.Find("Panel00").GetComponent<PanelBhv>(),
            GameObject.Find("Panel01").GetComponent<PanelBhv>(),
            GameObject.Find("Panel02").GetComponent<PanelBhv>(),
            GameObject.Find("Panel03").GetComponent<PanelBhv>()
        };
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
            if (!Constants.HasInit && Constants.HoverHelpStatus == OnOffStatus.On)
                _instantiator.NewPopupYesNo(_panel00.transform.position, "hover help", "this tool use a hover system to explain its features. if you are not sure what a feature does, hover its label to get its description.", null, "understood", null);
            Constants.HasInit = true;
            return true;
        }, lockInputWhile: true));
    }

    private void SetPanel(int id)
    {
        if (Constants.CurrentPanel == id)
            return;
        Constants.CurrentPanel = id;
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
                {
                    textMesh.fontMaterial = Resources.Load<Material>($"Fonts & Materials/{PlayerPrefHelper.GetFont()}.1.2.3");
                    textMesh.transform.position += new Vector3(0.0f, Constants.Pixel, 0.0f);
                }
            }
            else
            {
                _panels[i].transform.position = _resetPanelPosition;
                _panels[i].enabled = false;
                tabSpriteRenderer.sprite = i < 3 ? TabBigOff : TabSmallOff;
                tabSpriteRenderer.sortingOrder = 0;
                if (textMesh != null)
                {
                    textMesh.fontMaterial = Resources.Load<Material>($"Fonts & Materials/{PlayerPrefHelper.GetFont()}.2.3.4");
                    textMesh.transform.localPosition = new Vector3(textMesh.transform.localPosition.x, _mainPanelY, 0.0f);
                }
            }
        }
    }

    protected override void NormalUpdate()
    {
        if (Constants.IsOn)
            MoveMouseOutOfSync();
        if (Time.time < Constants.LastFrame + Constants.Frame)
            return;
        Constants.LastFrame = Time.time;
        AnalyzeSound();
        _dbData.text = DynRangeDB.ToString("0");
        _hzData.text = DynRangeDB > 0 && CurrentFrequencies != null && CurrentFrequencies.Count > 0 ? CurrentFrequencies[0].ToString("F2") : "0.00";
        _peaksData.text = DynRangeDB > 0 ? _peaks.Count.ToString() : "0";
        var isUnderHeldReset = _levelDrawer.Draw(_audioLevelTracker, _panel00.HeldReset, (int)_currentSingleFrameReset);
        _spectrumDrawer.Draw(_spectrum, _panel00.SpectrumThreshold, _peaks);

        if (DynRangeDB > 0)
            AnalyseAudioInputs();
        if (isUnderHeldReset)
            HeldReset();
        if (DynRangeDB <= _currentSingleFrameReset)
            SingleTapReset();

        if (_heldInputs != null && _heldInputs.Count > 0)
            HandleHeld();
        if (_timeHeldInputs != null && _timeHeldInputs.Count > 0)
            HandleTimeHeld();

        _tickHeldAudioInput = null;
    }

    private void HeldReset()
    {
        if (_heldInputs != null && _heldInputs.Count > 0)
        {
            foreach (var audioInput in _heldInputs)
            {
                if (audioInput.Mouse == MouseInput.None)
                    _inputSimulator.Keyboard.KeyUp(audioInput.Key);
                else
                    TriggerMouseInput(audioInput.Mouse, down: false);
            }
            _heldInputs.Clear();
        }
        UpdatePanelVisual(false);
    }

    private void SingleTapReset()
    {
        var limitOverDynRangeDB = DynRangeDB + Mathf.Abs(_panel00.SingleTapReset);
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
            var heldUntilNextReset = _tickHeldAudioInput != null && _heldInputs[i] != null
                && (int)_heldInputs[i].Param == Constants.HeldUntilNext
                && !(_heldInputs[i].Key == _tickHeldAudioInput.Key && _heldInputs[i].Mouse == _tickHeldAudioInput.Mouse && IsAudioInputValid(_heldInputs[i], _tickHeldAudioInput.Frequencies));
            var heldOnlyListenedReset = _tickHeldAudioInput == null && _heldInputs[i] != null
                && (int)_heldInputs[i].Param == Constants.HeldOnlyListened;
            if (heldUntilNextReset || heldOnlyListenedReset)
            {
                if (_heldInputs[i].Mouse == MouseInput.None)
                    _inputSimulator.Keyboard.KeyUp(_heldInputs[i].Key);
                else
                    TriggerMouseInput(_heldInputs[i].Mouse, down: false);
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
            if (_timeHeldInputs[i].TimeHeldParam > Time.time)
                UpdatePanelVisual(true, i == 0 ? "_" : string.Empty, InputType.CustomHeld, _timeHeldInputs[i].Id);
            else
            {
                if (_timeHeldInputs[i].Mouse == MouseInput.None)
                    _inputSimulator.Keyboard.KeyUp(_timeHeldInputs[i].Key);
                else
                    TriggerMouseInput(_timeHeldInputs[i].Mouse, down: false);
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
    private AudioInput _tickHeldAudioInput;

    private List<AudioInput> _heldInputs;
    private List<AudioInput> _timeHeldInputs;

    void AnalyseAudioInputs()
    {
        if (CurrentFrequencies == null || CurrentFrequencies.Count <= 0)
            return;
        if (Helper.FloatEqualsPrecision(CurrentFrequencies[0], _lastFrameFrequency, _panel00.HzOffset))
        {
            if (_nbConsecutiveFrames >= _panel00.RequiredFrames)
            {
                PrepareAudioInputsFromFrequency();
                OnRecordingFrequencies?.Invoke(CurrentFrequencies);
            }
            ++_nbConsecutiveFrames;
        }
        else
            _nbConsecutiveFrames = _nbConsecutiveFramesDefault;
        _lastFrameFrequency = CurrentFrequencies[0];
    }

    private void PrepareAudioInputsFromFrequency()
    {
        var validFrequencies = new List<AudioInput>();
        for (int i = 0; i < _panel01.AudioInputs.Count; ++i)
        {
            if (IsAudioInputValid(_panel01.AudioInputs[i]))
            {
                validFrequencies.Add(_panel01.AudioInputs[i]);
            }
        }
        if (validFrequencies.Count == 1)
            SendAudioInput(validFrequencies[0]);
        else if (validFrequencies.Count > 1)
        {
            validFrequencies.Sort((ai1, ai2) => ai1.Peaks.CompareTo(ai2.Peaks)); //Croissant
            if (_panel00.PeaksPriority == PeaksPriority.Higher)
                SendAudioInput(validFrequencies[validFrequencies.Count - 1]);
            else
                SendAudioInput(validFrequencies[0]);
        }
    }

    private bool IsAudioInputValid(AudioInput audioInput, List<float> currentFrequencies = null)
    {
        if (currentFrequencies == null)
            currentFrequencies = CurrentFrequencies;
        var isSet = audioInput.Frequencies[0] > 0.0f;
        if (!isSet || !audioInput.Enabled)
            return false;
        var nbValidFrequencies = 0;
        for (int i = 0; i < audioInput.Peaks; ++i)
        {
            //pour chaque peak de audioinput, vérifier si un équivalent à peu près existe dans currentFrequencies
            var validList = currentFrequencies.GetRange(0, audioInput.Peaks).Where(f => Helper.FloatEqualsPrecision(f, audioInput.Frequencies[i], _panel00.HzOffset));
            if (validList != null && validList.Count() > 0)
                ++nbValidFrequencies;
        }
        return nbValidFrequencies >= audioInput.Peaks;
    }

    private void SendAudioInput(AudioInput audioInput)
    {
        //MouseInput mouseInput;
        //VirtualKeyCode keyboardInput;
        //if (isSecond)
        //{
        //    mouseInput = audioInput.MouseInput2;
        //    keyboardInput = audioInput.Key2;
        //}
        //else
        //{
        //    mouseInput = audioInput.MouseInput;
        //    keyboardInput = audioInput.Key;
        //}
        if (audioInput.InputType == InputType.Tap && !_hasToWaitResetBeforeNewSingle && DynRangeDB > _currentSingleFrameReset)
        {
            if (Constants.IsOn)
            {
                if (audioInput.GetMainDevice() == DeviceType.Keyboard)
                    _inputSimulator.Keyboard.KeyDown(audioInput.Key);
                else
                    TriggerMouseInput(audioInput.Mouse, down: true, (int)audioInput.Param);
                StartCoroutine(InputUpAfterDelay(audioInput.Key, audioInput.Mouse, audioInput.GetMainDevice(), Constants.SingleTapDelay));
                if (audioInput.HasSecond)
                {
                    if (audioInput.GetSecondDevice() == DeviceType.Keyboard)
                        _inputSimulator.Keyboard.KeyDown(audioInput.Key2);
                    else
                        TriggerMouseInput(audioInput.Mouse2, down: true, (int)audioInput.Param);
                    StartCoroutine(InputUpAfterDelay(audioInput.Key2, audioInput.Mouse2, audioInput.GetSecondDevice(), Constants.SingleTapDelay));
                }
            }
            UpdatePanelVisual(true, audioInput.KeyToString(), InputType.Tap, audioInput.Id);
            _hasToWaitResetBeforeNewSingle = true;
        }
        else if (audioInput.InputType == InputType.Held)
        {
            if (_heldInputs == null)
                _heldInputs = new List<AudioInput>();
            if (_heldInputs.Find(a => a.Key == audioInput.Key && a.Mouse == audioInput.Mouse
                                   && a.Key2 == audioInput.Key2 && a.Mouse2 == audioInput.Mouse2
                                   && a.Frequencies[0] == audioInput.Frequencies[0]) == null)
            {
                if (Constants.IsOn)
                {
                    if (audioInput.GetMainDevice() == DeviceType.Keyboard)
                        _inputSimulator.Keyboard.KeyDown(audioInput.Key);
                    else
                        TriggerMouseInput(audioInput.Mouse, down: true, (int)audioInput.Param);
                    if (audioInput.HasSecond)
                    {
                        if (audioInput.GetSecondDevice() == DeviceType.Keyboard)
                            _inputSimulator.Keyboard.KeyDown(audioInput.Key2);
                        else
                            TriggerMouseInput(audioInput.Mouse2, down: true, (int)audioInput.Param);
                    }
                }
                UpdatePanelVisual(true, audioInput.KeyToString(), InputType.Held, audioInput.Id);
                _heldInputs.Add(audioInput);
            }
            _tickHeldAudioInput = audioInput.CloneFrequenciesParam();
        }
        else if (audioInput.InputType == InputType.CustomTap && !_hasToWaitResetBeforeNewSingle && DynRangeDB > _currentSingleFrameReset)
        {
            StartCoroutine(CustomTapSend(audioInput.CloneKeys(), (int)audioInput.Param));
            _hasToWaitResetBeforeNewSingle = true;
        }
        else if (audioInput.InputType == InputType.CustomHeld && !_hasToWaitResetBeforeNewSingle && DynRangeDB > _currentSingleFrameReset)
        {
            if (_timeHeldInputs == null)
                _timeHeldInputs = new List<AudioInput>();
            var existing = _timeHeldInputs.Find(a => a.Key == audioInput.Key && a.Mouse == audioInput.Mouse
                                       && a.Key2 == audioInput.Key2 && a.Mouse2 == audioInput.Mouse2
                                       && a.Frequencies[0] == audioInput.Frequencies[0]);
            if (existing == null)
            {
                if (Constants.IsOn)
                {
                    if (audioInput.GetMainDevice() == DeviceType.Keyboard)
                        _inputSimulator.Keyboard.KeyDown(audioInput.Key);
                    else
                        TriggerMouseInput(audioInput.Mouse, down: true, (int)audioInput.Param);
                    if (audioInput.HasSecond)
                    {
                        if (audioInput.GetSecondDevice() == DeviceType.Keyboard)
                            _inputSimulator.Keyboard.KeyDown(audioInput.Key2);
                        else
                            TriggerMouseInput(audioInput.Mouse2, down: true, (int)audioInput.Param);
                    }
                }
                UpdatePanelVisual(true, audioInput.KeyToString(), InputType.CustomHeld, audioInput.Id);
                var tmpAudioInput = audioInput.CloneFrequenciesParam();
                var timeHeld = tmpAudioInput.Param;
                if (timeHeld == 0.00f)
                    timeHeld = 3600.0f;
                tmpAudioInput.TimeHeldParam = Time.time + timeHeld;
                _timeHeldInputs.Add(tmpAudioInput);
            }
            else
            {
                if (audioInput.Param > 0.00f)
                    existing.TimeHeldParam = Time.time + existing.Param;
                else
                {
                    if (existing.GetMainDevice() == DeviceType.Keyboard)
                        _inputSimulator.Keyboard.KeyUp(existing.Key);
                    else
                        TriggerMouseInput(existing.Mouse, down: false);
                    if (existing.HasSecond)
                    {
                        if (existing.GetSecondDevice() == DeviceType.Keyboard)
                            _inputSimulator.Keyboard.KeyUp(existing.Key2);
                        else
                            TriggerMouseInput(existing.Mouse2, down: false);
                    }
                    _timeHeldInputs.Remove(existing);
                }
            }
            _hasToWaitResetBeforeNewSingle = true;
        }
        else
            UpdatePanelVisual(false);
        //if (audioInput.HasSecond && !isSecond)
        //    SendAudioInput(audioInput, true);
        //this.InvokeNextFrame(() =>
        //{

        //});
    }

    private void UpdatePanelVisual(bool down, string key = null, InputType type = InputType.Tap, int id = -1)
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

    private IEnumerator CustomTapSend(AudioInput audioInput, int count)
    {
        if (count > 0)
        {
            if (Constants.IsOn)
            {
                if (audioInput.GetMainDevice() == DeviceType.Keyboard)
                    _inputSimulator.Keyboard.KeyDown(audioInput.Key);
                else
                    TriggerMouseInput(audioInput.Mouse, down: true);
                StartCoroutine(InputUpAfterDelay(audioInput.Key, audioInput.Mouse, audioInput.GetMainDevice(), Constants.SingleTapDelay));
                if (audioInput.HasSecond)
                {
                    if (audioInput.GetSecondDevice() == DeviceType.Keyboard)
                        _inputSimulator.Keyboard.KeyDown(audioInput.Key2);
                    else
                        TriggerMouseInput(audioInput.Mouse2, down: true);
                    StartCoroutine(InputUpAfterDelay(audioInput.Key2, audioInput.Mouse2, audioInput.GetSecondDevice(), Constants.SingleTapDelay));
                }
            }
            UpdatePanelVisual(true, audioInput.KeyToString(), InputType.CustomTap, audioInput.Id);
            --count;
            yield return new WaitForSeconds(_panel03.CustomTapDelay);
            StartCoroutine(CustomTapSend(audioInput, count));
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
                if ((audioInput.InputType == InputType.Held || audioInput.InputType == InputType.CustomHeld)
                    && Helper.IsMouseDirection(audioInput.Mouse))
                    MoveCursor(audioInput.Mouse);
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

    private IEnumerator InputUpAfterDelay(VirtualKeyCode key, MouseInput mouseInput, DeviceType deviceType, float timeHeld)
    {
        yield return new WaitForSeconds(timeHeld);
        if (Constants.IsOn)
        {
            if (deviceType == DeviceType.Keyboard)
                _inputSimulator.Keyboard.KeyUp(key);
            else
                TriggerMouseInput(mouseInput, down: false);
        }
    }

    void AnalyzeSound()
    {
        _spectrum = _spectrumAnalyzer.logSpectrumArray.ToArray();
        //_peaksCount = 0;
        _peaks.Clear();
        CurrentFrequencies.Clear();
        if (_spectrum != null && _spectrum.Count() > 0)
        {
            int currentNbPeak = 0;
            float currentPeakMax = 0.0f;
            var hundredth = _spectrum.Length / 100;
            var twoHundredth = _spectrum.Length / 200;
            for (int i = 0; i < _binSize; i += 1)
            {
                if (_spectrum[i] > currentPeakMax && _spectrum[i] > _panel00.SpectrumThreshold)
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
                if (i > 0 && _spectrum[i - 1] > _panel00.SpectrumThreshold && _spectrum[i] <= _panel00.SpectrumThreshold)
                {
                    ++currentNbPeak;
                    currentPeakMax = 0.0f;
                    _peaks.Sort(new AmpComparer());
                    if (_peaks.Count > 5)
                        _peaks.RemoveAt(5);
                }
            }
            _peaks.Sort(new AmpComparer());
        }
        if (_peaks.Count > 0)
        {
            for (int i = 0; i < 5; ++i)
            {
                if (i < _peaks.Count)
                    CurrentFrequencies.Add((_peaks[i].index * (_samplerate / 2f) / _binSize) / 100);
                else
                    CurrentFrequencies.Add(0.0f);
            }
        }

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