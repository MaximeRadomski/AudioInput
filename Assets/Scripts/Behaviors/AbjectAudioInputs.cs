using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lasp;
using WindowsInput;
using WindowsInput.Native;
using System.Linq;

public class AbjectAudioInputs : MonoBehaviour
{
    public float DynRangeDB { get; set; }
    public Sprite TabBigOn;
    public Sprite TabBigOff;
    public Sprite TabSmallOn;
    public Sprite TabSmallOff;

    private float _pitchValue;
    private float _highestPeak;
    private int _binSize = 2048;
    private float _threshold = 0.01f;

    private SpectrumAnalyzer _spectrumAnalyzer;
    private AudioLevelTracker _audioLevelTracker;

    private List<Peak> _peaks = new List<Peak>();
    private int _peaksCount;
    private float[] _spectrum;
    int _samplerate;

    private TMPro.TextMeshPro _dbData;
    private TMPro.TextMeshPro _hzData;
    private TMPro.TextMeshPro _peaksData;
    private LevelDrawerBhv _levelDrawer;
    private SpectrumDrawerBhv _spectrumDrawer;

    private List<PanelBhv> _panels;
    private Vector3 _resetPanelPosition = new Vector3(-45.0f, -25.0f, 0.0f);

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

        _dbData = Helper.GetFieldData("Db");
        _hzData = Helper.GetFieldData("Hz");
        _peaksData = Helper.GetFieldData("Peaks");
        _levelDrawer = GameObject.Find("LevelDrawer").GetComponent<LevelDrawerBhv>();
        _spectrumDrawer = GameObject.Find("SpectrumDrawer").GetComponent<SpectrumDrawerBhv>();

        _panels = new List<PanelBhv>();
        _panels.Add(GameObject.Find("Panel00").GetComponent<PanelBhv>());
        _panels.Add(GameObject.Find("Panel01").GetComponent<PanelBhv>());
        _panels.Add(GameObject.Find("Panel02").GetComponent<PanelBhv>());
        _panels.Add(GameObject.Find("Panel03").GetComponent<PanelBhv>());
    }

    private void SetButtons()
    {
        GameObject.Find("Tab00").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetPanel(0); };
        GameObject.Find("Tab01").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetPanel(1); };
        GameObject.Find("Tab02").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetPanel(2); };
        GameObject.Find("Tab03").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetPanel(3); };
    }

    private void LoadData()
    {
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
        _dbData.text = DynRangeDB.ToString("F1");
        _hzData.text = DynRangeDB > 0 ? _pitchValue.ToString("F2") : "0.0";
        _peaksData.text = DynRangeDB > 0 ? _peaksCount.ToString() : "0";
        if (DynRangeDB > 0)
            PlayCorrespondingInputs();
        _levelDrawer.Draw(_audioLevelTracker);
        _spectrumDrawer.Draw(_spectrum);
    }

    void PlayCorrespondingInputs()
    {
        //var test = new InputSimulator();
        //test.Keyboard.KeyPress(VirtualKeyCode.SPACE);
    }

    void OnGUI()
    {
        //Event e = Event.current;
        //if (e.isKey && e.rawType == EventType.KeyUp)
        //{
        //    KeyCode test = KeyCode.B;
        //    _inputText.text = $"Detected key code: {e.keyCode}";
        //}
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
                if (_peaks.Count > 5)
                {
                    _peaks.Sort(new AmpComparer());
                    _peaks.Remove (_peaks [5]);
                }
            }
            if (i > _threshold * 5 && _spectrum[i - 1] > 0 && _spectrum[i] == 0)
                ++_peaksCount;
        }
        float freqN = 0f;
        if (_peaks.Count > 0)
        {
            maxV = _peaks[0].amplitude;
            int maxN = _peaks[0].index;
            freqN = maxN;
            if (maxN > 0 && maxN < _binSize - 1)
            {
                var dL = _spectrum[maxN - 1] / _spectrum[maxN];
                var dR = _spectrum[maxN + 1] / _spectrum[maxN];
                freqN += 0.5f * (dR * dR - dL * dL);
            }
            _highestPeak = maxN;
        }
        else
            _highestPeak = 0.0f;
        _pitchValue = freqN * (_samplerate / 2f) / _binSize;
        _pitchValue /= 100.0f;
        _peaks.Clear();
    }
}