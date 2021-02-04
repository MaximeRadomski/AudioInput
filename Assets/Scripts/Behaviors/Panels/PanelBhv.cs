using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelBhv : MonoBehaviour
{
    public Instantiator Instantiator;
    protected bool _hasInit;

    public virtual void Init()
    {
        Instantiator = GameObject.Find(Constants.AbjectAudioInputs).GetComponent<Instantiator>();
    }
}
