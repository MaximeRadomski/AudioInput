using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelBhv : MonoBehaviour
{
    protected Instantiator _instantiator;
    protected bool _hasInit;

    public virtual void Init()
    {
        _instantiator = GameObject.Find(Constants.AbjectAudioInputs).GetComponent<Instantiator>();
    }
}
