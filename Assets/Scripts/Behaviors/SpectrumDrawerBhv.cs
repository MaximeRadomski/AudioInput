using System.Collections.Generic;
using UnityEngine;

public class SpectrumDrawerBhv : MonoBehaviour
{
    public GameObject Spec;

    private List<SpecBhv> _specs;

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
        _hasInit = true;
    }

    public void Draw(float[] spectrum)
    {
        if (spectrum == null || spectrum.Length <= 0)
            return;
        if (!_hasInit)
            Init();
        int increment = spectrum.Length / 100;
        int iS = 0;
        for (int i = 0; iS < spectrum.Length; ++i)
        {
            if (i >= _specs.Count)
                return;
            _specs[i].transform.localPosition = new Vector3(spectrum[iS] * 49.0f * Constants.Pixel, _specs[i].transform.localPosition.y, 0.0f);
            _specs[i].UpdateBack(spectrum[iS] * 49.0f);
            iS += increment;
        }
    }
}
