using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBoxBhv : MonoBehaviour
{
    public Sprite CheckSprite;
    public Sprite UncheckSprite;
    public Sprite QuanticSprite;

    private SpriteRenderer _spriteRenderer;
    private bool _hasInit;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (_hasInit)
            return;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _hasInit = true;
    }

    public void Check()
    {
        if (!_hasInit)
            Init();
        _spriteRenderer.sprite = CheckSprite;
    }

    public void Uncheck()
    {
        if (!_hasInit)
            Init();
        _spriteRenderer.sprite = UncheckSprite;
    }

    public void Quantic()
    {
        if (!_hasInit)
            Init();
        _spriteRenderer.sprite = QuanticSprite;
    }
}
