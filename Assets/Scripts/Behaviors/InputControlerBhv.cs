using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControlerBhv : MonoBehaviour
{
    private Vector3 _touchPosWorld;
    private InputBhv _currentInput;
    private InputBhv _lastDownInput;
    private bool _beginPhase, _doPhase, _endPhase;
    private Camera _mainCamera;
    private bool _hasInit;
    private GameObject _hoverWindow;

    private void Start()
    {
        if (!_hasInit)
            Init();
    }

    private void Init()
    {
        _mainCamera = Helper.GetMainCamera();
        _hoverWindow = GameObject.Find("HoverWindow");
        _hasInit = true;
    }

    void Update()
    {
        // IF BACK BUTTON //
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Constants.InputLayer > 0)
            {
                var gameObjectToDestroy = GameObject.Find(Constants.InputTopLayerNames[Constants.InputTopLayerNames.Count - 1]);
                gameObjectToDestroy.GetComponent<PopupBhv>().ExitPopup();
            }
            return;
        }
        // IF ENTER BUTTON //
        else if (Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return))
        {
            if (Constants.InputLayer > 0)
            {
                var gameObjectToDestroy = GameObject.Find(Constants.InputTopLayerNames[Constants.InputTopLayerNames.Count - 1]);
                gameObjectToDestroy.GetComponent<PopupBhv>().ValidatePopup();
            }
            return;
        }
        var currentFrameInputLayer = Constants.InputLayer;
        _beginPhase = _doPhase = _endPhase = false;
        // IF MOUSE //
        if ((_beginPhase = Input.GetMouseButtonDown(0))
            || (_endPhase = Input.GetMouseButtonUp(0))
            || (_doPhase = Input.GetMouseButton(0)))
        {
            _touchPosWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
            RaycastHit2D[] hitsInformation = Physics2D.RaycastAll(touchPosWorld2D, _mainCamera.transform.forward);
            foreach (var hitInformation in hitsInformation)
            {
                if (hitInformation.collider != null)
                {
                    CancelCurrentObjectIfNewBeforeEnd(hitInformation.transform.gameObject);
                    _currentInput = hitInformation.transform.gameObject.GetComponent<InputBhv>();
                    if (_currentInput == null
                        || _currentInput.Layer < currentFrameInputLayer
                        || IsTheLowestInput(_currentInput, hitsInformation)
                        || IsUnderSprite(_currentInput, hitsInformation))
                        continue;
                    ResetHoverWindow();
                    if (_beginPhase)
                    {
                        _lastDownInput = _currentInput;
                        _currentInput.BeginAction(touchPosWorld2D);
                    }
                    else if (_endPhase && _lastDownInput?.name == _currentInput.name)
                    {
                        Constants.SetLastEndActionClickedName(_currentInput.name);
                        _currentInput.EndAction(touchPosWorld2D);
                        _currentInput = null;
                        _lastDownInput = null;
                    }
                    else if (_doPhase)
                        _currentInput.DoAction(touchPosWorld2D);
                }
                else
                    CancelCurrentObjectIfNewBeforeEnd();
            }
        }
        //IF HOVER OR ELSE //
        else 
        {
            _touchPosWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
            RaycastHit2D[] hitsInformation = Physics2D.RaycastAll(touchPosWorld2D, _mainCamera.transform.forward);
            var atLeastOneHover = 0;
            foreach (var hitInformation in hitsInformation)
            {
                if (hitInformation.collider != null)
                {
                    CancelCurrentObjectIfNewBeforeEnd(hitInformation.transform.gameObject);
                    _currentInput = hitInformation.transform.gameObject.GetComponent<HoverBhv>();
                    if (_currentInput == null
                        || _currentInput.Layer < currentFrameInputLayer
                        || IsTheLowestInput(_currentInput, hitsInformation)
                        || IsUnderSprite(_currentInput, hitsInformation))
                        continue;
                    ++atLeastOneHover;
                    _currentInput.DoAction(touchPosWorld2D);
                }
                else
                    CancelCurrentObjectIfNewBeforeEnd();
            }
            if (atLeastOneHover <= 0)
            {
                ResetHoverWindow();
                _touchPosWorld = new Vector3(-99, -99, -99);
            }
        }
    }

    private void ResetHoverWindow()
    {
        _hoverWindow.transform.position = new Vector3(-35.0f, 20.0f, 0.0f);
    }

    private void CancelCurrentObjectIfNewBeforeEnd(GameObject touchedGameObject = null)
    {
        if (_currentInput == null || _currentInput.gameObject == touchedGameObject)
            return;
        _currentInput.CancelAction();
        _currentInput = null;
        //_lastDownInput = null; Not Sure
    }

    private bool IsTheLowestInput(InputBhv currentInput, RaycastHit2D[] hitsInformation)
    {
        int highest = -1;
        foreach (var hitInformation in hitsInformation)
        {
            var tmpInput = hitInformation.transform.gameObject.GetComponent<InputBhv>();
            if (tmpInput != null && tmpInput.Layer > highest)
                highest = tmpInput.Layer;
        }
        return currentInput.Layer < highest;
    }

    private bool IsUnderSprite(InputBhv currentInput, RaycastHit2D[] hitsInformation)
    {
        var currentSprite = currentInput.GetComponent<SpriteRenderer>();
        if (currentSprite == null)
            return false;
        foreach (var hitInformation in hitsInformation)
        {
            var tmpSprite = hitInformation.transform.gameObject.GetComponent<SpriteRenderer>();
            if (tmpSprite != null &&
                SortingLayer.GetLayerValueFromName(tmpSprite.sortingLayerName) > SortingLayer.GetLayerValueFromName(currentSprite.sortingLayerName))
                return true;
        }
        return false;
    }
}
