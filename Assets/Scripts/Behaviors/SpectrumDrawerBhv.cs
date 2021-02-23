using System.Collections.Generic;
using UnityEngine;

public class SpectrumDrawerBhv : MonoBehaviour
{
    public GameObject Spec;

    private List<SpecBhv> _specs;
    private Transform _threshold;
    private List<SpecBhv> _peaks;

    private bool _hasInit;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        _specs = new List<SpecBhv>();
        for (int i = 0; i < 100; ++i)
        {
            var tmpSpec = Instantiate(Spec, new Vector3(transform.position.x, transform.position.y + (i * Constants.Pixel), 0.0f), Spec.transform.rotation);
            tmpSpec.name = $"Spec{i}";
            tmpSpec.transform.SetParent(transform);
            _specs.Add(tmpSpec.GetComponent<SpecBhv>());
        }
        _threshold = transform.Find("Threshold");
        _peaks = new List<SpecBhv>();
        for (int i = 0; i < 5; ++i)
            _peaks.Add(transform.Find($"Peak{i}").GetComponent<SpecBhv>());
        _hasInit = true;
    }

    public void Draw(float[] spectrum, float threshold, List<Peak> peaks)
    {
        var width = 49.0f;
        _threshold.localPosition = new Vector3(threshold * width * Constants.Pixel, _threshold.localPosition.y, 0.0f);
        for (int i = 0; i < 5; ++i)
        {
            if (i < peaks.Count)
            {
                _peaks[i].transform.localPosition = new Vector3(peaks[i].amplitude * width * Constants.Pixel, ((int)((float)peaks[i].index / spectrum.Length * 100.0f) + 1) * Constants.Pixel, 0.0f);
                _peaks[i].UpdateBack(peaks[i].amplitude * 49.0f);
            }
            else
                _peaks[i].transform.localPosition = new Vector3(30.0f, -30.0f, 0.0f);
        }
        if (spectrum == null || spectrum.Length <= 0)
            return;
        if (!_hasInit)
            Init();
        int increment = spectrum.Length / 100;
        int idSpectrum = 0;
        for (int i = 0; idSpectrum < spectrum.Length; ++i)
        {
            if (i >= _specs.Count)
                return;
            _specs[i].transform.localPosition = new Vector3(spectrum[idSpectrum] * width * Constants.Pixel, _specs[i].transform.localPosition.y, 0.0f);
            _specs[i].UpdateBack(spectrum[idSpectrum] * 49.0f);
            idSpectrum += increment;
        }
    }
}
