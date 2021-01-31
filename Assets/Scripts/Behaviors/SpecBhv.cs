using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecBhv : MonoBehaviour
{
    private GameObject _back;

    private bool _hasInit;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _back = transform.GetChild(0).gameObject;
        _hasInit = true;
    }

    public void UpdateBack(float scale)
    {
        if (!_hasInit)
            Init();
        _back.transform.localPosition = new Vector3(transform.localPosition.x / 2, 0.0f, 0.0f);
        _back.transform.localScale = new Vector3(scale + 1, 1.0f, 1.0f);
    }
}
