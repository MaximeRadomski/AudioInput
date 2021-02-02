using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverBhv : InputBhv
{
    [TextArea]
    public string Content;
    private GameObject _hoverWindow;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        _hoverWindow = GameObject.Find("HoverWindow");
    }

    public override void BeginAction(Vector2 initialTouchPosition)
    {
        return;
    }

    public override void CancelAction()
    {
        return;
    }

    public override void DoAction(Vector2 touchPosition)
    {
        var x = 11;
        var y = -6;
        if (touchPosition.x > 0)
            x = -x;
        if (touchPosition.y < 0)
            y = -y;
        if (_hoverWindow != null)
        {
            _hoverWindow.transform.position = new Vector3(touchPosition.x + x, touchPosition.y + y, 0.0f);
            _hoverWindow.transform.Find("Content").GetComponent<TMPro.TextMeshPro>().text = Content;
        }

    }

    public override void EndAction(Vector2 lastTouchPosition)
    {
        return;
    }
}
