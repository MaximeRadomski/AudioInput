﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupYesNoBhv : PopupBhv
{
    public Sprite BigButton;
    
    private System.Func<bool, object> _resultAction;

    public void Init(string title, string content, string negative, string positive,
        System.Func<bool, object> resultAction, Sprite sprite = null)
    {
        //transform.position = Camera.main.transform.position;
        transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = title;
        transform.Find("Content").GetComponent<TMPro.TextMeshPro>().text = content;
        _resultAction = resultAction;

        var buttonPositive = transform.Find("ButtonPositive");
        buttonPositive.GetComponent<ButtonBhv>().EndActionDelegate = PositiveDelegate;
        buttonPositive.transform.Find("ButtonPositiveText").GetComponent<TMPro.TextMeshPro>().text = positive;
        if (string.IsNullOrEmpty(negative))
            buttonPositive.transform.position = new Vector3(buttonPositive.transform.parent.position.x, buttonPositive.transform.position.y, 0.0f);

        var buttonNegative = transform.Find("ButtonNegative");
        buttonNegative.GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;
        buttonNegative.transform.Find("ButtonNegativeText").GetComponent<TMPro.TextMeshPro>().text = negative;
        if (string.IsNullOrEmpty(negative))
            buttonNegative.gameObject.SetActive(false);

        if (string.IsNullOrEmpty(content))
        {
            buttonPositive.transform.position = new Vector3(buttonPositive.transform.position.x, buttonPositive.transform.parent.position.y - 0.5f, 0.0f);
            buttonPositive.GetComponent<SpriteRenderer>().sprite = BigButton;
            buttonNegative.transform.position = new Vector3(buttonNegative.transform.position.x, buttonPositive.transform.parent.position.y - 0.5f, 0.0f);
            buttonNegative.GetComponent<SpriteRenderer>().sprite = BigButton;
        }
    }

    private void PositiveDelegate()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(true);
        Destroy(gameObject);
    }

    private void NegativeDelegate()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(false);
        Destroy(gameObject);
    }

    public override void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        //_resultAction?.Invoke(false);
        Destroy(gameObject);
    }

    public override void ValidatePopup()
    {
        PositiveDelegate();
    }
}
