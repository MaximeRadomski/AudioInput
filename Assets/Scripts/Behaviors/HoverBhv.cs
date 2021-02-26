using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverBhv : InputBhv
{
    public Sprite Sprite;
    [TextArea]
    public string Content;
    private GameObject _hoverWindow;
    private GameObject _clickMeHoverWindow;
    public bool ClickMe = false;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        _hoverWindow = GameObject.Find("HoverWindow");
        _clickMeHoverWindow = GameObject.Find("ClickMeHoverWindow");
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
        if (Constants.HoverHelpStatus == OnOffStatus.Off)
            return;
        var x = 11.3f;
        var y = -4;
        if (touchPosition.x > 0)
            x = -x + 0.3f;
        if (touchPosition.y < 0)
            y = -y;
        if (!ClickMe && _hoverWindow != null)
        {
            _hoverWindow.transform.position = new Vector3(touchPosition.x + x, touchPosition.y + y, 0.0f);
            if (Sprite != null)
            {
                _hoverWindow.GetComponent<SpriteRenderer>().sprite = Sprite;
                _hoverWindow.transform.Find("Content").GetComponent<TMPro.TextMeshPro>().text = string.Empty;
            }
            else
            {
                _hoverWindow.GetComponent<SpriteRenderer>().sprite = null;
                _hoverWindow.transform.Find("Content").GetComponent<TMPro.TextMeshPro>().text = Content.ToLower();
            }
        }
        else if (ClickMe && _clickMeHoverWindow != null)
        {
            _clickMeHoverWindow.transform.position = new Vector3(touchPosition.x + 2.6f, touchPosition.y - 1.0f, 0.0f);
        }
    }

    public override void EndAction(Vector2 lastTouchPosition)
    {
        return;
    }
}
