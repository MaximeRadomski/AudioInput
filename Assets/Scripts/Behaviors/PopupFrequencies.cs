using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupFrequencies : PopupBhv
{
    public Sprite Selected;
    public Sprite Unselected;

    private System.Func<List<float>, int, object> _resultAction;
    private Instantiator _instantiator;

    private AbjectAudioInputs _abjectAudioInputs;
    private List<TMPro.TextMeshPro> _peaksData;
    private List<SpriteRenderer> _selected;
    private Transform _midBracket;
    private Transform _botBracket;
    private CheckBoxBhv _lockedBhv;

    private List<List<FrequencyCount>> _currentFrequencies;
    private int _currentPeaksNumber;
    private bool _isLocked;

    public void Init(List<float> defaultFrequencies, int defaultPeaksNumber, System.Func<List<float>, int, object> resultAction, Instantiator instantiator)
    {
        _abjectAudioInputs = GameObject.Find(Constants.AbjectAudioInputs).GetComponent<AbjectAudioInputs>();
        _abjectAudioInputs.OnRecordingFrequencies = SetFrequencies;
        _resultAction = resultAction;
        _instantiator = instantiator;
        _peaksData = new List<TMPro.TextMeshPro>();
        _selected = new List<SpriteRenderer>();
        _currentFrequencies = new List<List<FrequencyCount>>();
        for (int i = 0; i < 5; ++i)
        {
            _currentFrequencies.Add(new List<FrequencyCount>() { new FrequencyCount(defaultFrequencies[i], 1) });
        }
        _isLocked = false;

        transform.Find("Peak0").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetFrequencyPopup(0); };
        transform.Find("Peak0").transform.GetChild(1).GetComponent<ButtonBhv>().DoActionDelegate = () => { SetPeaksNumber(0); };
        _peaksData.Add(transform.Find("Peak0").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>());
        _selected.Add(transform.Find("Peak0").transform.GetChild(1).GetComponent<SpriteRenderer>());

        transform.Find("Peak1").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetFrequencyPopup(1); };
        transform.Find("Peak1").transform.GetChild(1).GetComponent<ButtonBhv>().DoActionDelegate = () => { SetPeaksNumber(1); };
        _peaksData.Add(transform.Find("Peak1").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>());
        _selected.Add(transform.Find("Peak1").transform.GetChild(1).GetComponent<SpriteRenderer>());

        transform.Find("Peak2").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetFrequencyPopup(2); };
        transform.Find("Peak2").transform.GetChild(1).GetComponent<ButtonBhv>().DoActionDelegate = () => { SetPeaksNumber(2); };
        _peaksData.Add(transform.Find("Peak2").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>());
        _selected.Add(transform.Find("Peak2").transform.GetChild(1).GetComponent<SpriteRenderer>());

        transform.Find("Peak3").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetFrequencyPopup(3); };
        transform.Find("Peak3").transform.GetChild(1).GetComponent<ButtonBhv>().DoActionDelegate = () => { SetPeaksNumber(3); };
        _peaksData.Add(transform.Find("Peak3").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>());
        _selected.Add(transform.Find("Peak3").transform.GetChild(1).GetComponent<SpriteRenderer>());

        transform.Find("Peak4").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetFrequencyPopup(4); };
        transform.Find("Peak4").transform.GetChild(1).GetComponent<ButtonBhv>().DoActionDelegate = () => { SetPeaksNumber(4); };
        _peaksData.Add(transform.Find("Peak4").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>());
        _selected.Add(transform.Find("Peak4").transform.GetChild(1).GetComponent<SpriteRenderer>());

        _midBracket = transform.Find("MidBracket");
        _botBracket = transform.Find("BotBracket");

        transform.Find("Refresh").GetComponent<ButtonBhv>().EndActionDelegate = Refresh;
        _lockedBhv = transform.Find("Lock").GetComponent<CheckBoxBhv>();
        _lockedBhv.GetComponent<ButtonBhv>().EndActionDelegate = () => { Lock(); };

        var buttonPositive = transform.Find("ButtonPositive");
        buttonPositive.GetComponent<ButtonBhv>().EndActionDelegate = PositiveDelegate;

        var buttonNegative = transform.Find("ButtonNegative");
        buttonNegative.GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;

        SetFrequencies(defaultFrequencies);
        SetPeaksNumber(defaultPeaksNumber - 1);
    }

    private object SetFrequencies(List<float> frequencies)
    {
        if (_isLocked)
            return false;

        for (int i = 0; i < 5; ++i)
        {
            var alreadyId = _currentFrequencies[i].FindIndex(fc => Helper.FloatEqualsPrecision(fc.Frequency, frequencies[i], 0.01f));
            if (alreadyId == -1)
                _currentFrequencies[i].Add(new FrequencyCount(frequencies[i], 1));
            else
                _currentFrequencies[i][alreadyId].Count += 1;
            _currentFrequencies[i].Sort((fc1, fc2) => fc1.Count.CompareTo(fc2.Count));
            _currentFrequencies[i].Reverse();
        }

        for (int i = 0; i < 5; ++i)
        {
            _peaksData[i].text = _currentFrequencies[i][0].Frequency.ToString("F2");
        }
        return true;
    }

    private void SetPeaksNumber(int number)
    {
        if (number < 0)
            number = 0;
        else if (number > 4)
            number = 4;
        _currentPeaksNumber = number + 1;
        for (int i = 0; i < 5; ++i)
        {
            if (i <= number)
                _selected[i].sprite = Selected;
            else
                _selected[i].sprite = Unselected;
        }
        _botBracket.position = new Vector3(_botBracket.position.x, _peaksData[number].transform.position.y, 0.0f);
        _midBracket.position = new Vector3(_midBracket.position.x, Mathf.Lerp(_peaksData[0].transform.position.y, _botBracket.position.y, 0.5f));
        _midBracket.localScale = new Vector3(1.0f, (11.0f * number) + 3f, 1.0f);
    }

    private void SetFrequencyPopup(int id)
    {
        var content = $"pick a stable one";
        _instantiator.NewPopupNumber(transform.position, "Frequency in Hz", content, _currentFrequencies[id][0].Frequency, 4, SetFrequency);

        object SetFrequency(float value)
        {
            if (value < 0.0f)
                value = 0.0f;
            _currentFrequencies[id].Clear();
            _currentFrequencies[id].Add(new FrequencyCount(value, 1));
            _peaksData[id].text = value.ToString("F2");
            return true;
        }
    }

    private void Refresh()
    {
        for (int i = 0; i < 5; ++i)
        {
            _currentFrequencies[i].Clear();
            _currentFrequencies[i].Add(new FrequencyCount(0.0f, 1));
        }
        var tmpIsLocked = _isLocked;
        _isLocked = false;
        var tmpList = new List<float>();
        for (int i = 0; i < 5; ++i)
        {
            tmpList.Add(_currentFrequencies[i][0].Frequency);
        }
        SetFrequencies(tmpList);
        _isLocked = tmpIsLocked;
    }

    private void Lock(bool? isLocked = null)
    {
        if (isLocked != null)
            _isLocked = isLocked.Value;
        else
            _isLocked = !_isLocked;
        if (_isLocked)
            _lockedBhv.Check();
        else
            _lockedBhv.Uncheck();
    }

    private void PositiveDelegate()
    {
        Constants.DecreaseInputLayer();
        var tmpList = new List<float>();
        for (int i = 0; i < 5; ++i)
        {
            tmpList.Add(_currentFrequencies[i][0].Frequency);
        }
        _resultAction.Invoke(tmpList, _currentPeaksNumber);
        Destroy(gameObject);
    }

    private void NegativeDelegate()
    {
        _abjectAudioInputs.OnRecordingFrequencies = null;
        Constants.DecreaseInputLayer();
        Destroy(gameObject);
    }

    public override void ExitPopup()
    {
        _abjectAudioInputs.OnRecordingFrequencies = null;
        base.ExitPopup();
    }

    public override void ValidatePopup()
    {
        PositiveDelegate();
    }
}
