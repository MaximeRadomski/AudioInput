using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupEnumBhv : PopupBhv
{
    private System.Func<int, object> _resultAction;
    private Vector3 _listStartPosition;
    private int _currentId;

    public void Init<EnumType>(string title, int currentId, System.Func<int, object> resultAction) where EnumType : System.Enum
    {
        _currentId = currentId;
        _resultAction = resultAction;

        transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = title;

        var buttonNegative = transform.Find("ButtonNegative");
        buttonNegative.GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;

        _listStartPosition = transform.Find("ListStartPosition").position;
        var spaceBetween = 12 * Constants.Pixel;
        var values = (EnumType[])System.Enum.GetValues(typeof(EnumType));
        for (int i = 0; i < values.Length; ++i)
        {
            var tmpButtonObject = Resources.Load<GameObject>("Prefabs/EnumButton");
            var tmpButtonInstance = Instantiate(tmpButtonObject, _listStartPosition + new Vector3(0.0f, -spaceBetween * i, 0.0f), tmpButtonObject.transform.rotation);
            tmpButtonInstance.name = $"EnumChoice{i}";
            string tmpName = values[i].ToString();
            tmpButtonInstance.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = tmpName.ToLower();
            tmpButtonInstance.GetComponent<ButtonBhv>().EndActionDelegate = SelectEnum;
            tmpButtonInstance.transform.SetParent(transform);
        }
    }

    private void SelectEnum()
    {
        var subString = Constants.LastEndActionClickedName.Substring(10);
        int id = int.Parse(subString);
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(id);
        Destroy(gameObject);
    }

    private void NegativeDelegate()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(_currentId);
        Destroy(gameObject);
    }

    public override void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke(_currentId);
        Destroy(gameObject);
    }
}
