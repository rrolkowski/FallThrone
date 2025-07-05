using System.Collections.Generic;
using UnityEngine;

public class CheatCode : MonoBehaviour
{
    [SerializeField] private GameObject _cheatButtons;
    [SerializeField] private GameObject _fpsCounter;
    [SerializeField] private Canvas _levelSelectCanvas;

    private string _input = "";

    private Dictionary<string, System.Action> _cheatActions;
    private void Awake()
    {
        _cheatActions = new Dictionary<string, System.Action>()
        {
            { "DAMIAN", ActivateCheatButtons },
            { "FPS", ToggleFPSCounter },
        };
    }

    private void Update()
    {
        if (!_levelSelectCanvas.gameObject.activeInHierarchy)
        {
            _cheatButtons.SetActive(false);
            return;
        }

        foreach (char c in Input.inputString)
        {
            if (char.IsLetter(c))
            {
                _input += char.ToUpper(c);

                int maxLen = 0;
                foreach (var code in _cheatActions.Keys)
                    maxLen = Mathf.Max(maxLen, code.Length);

                if (_input.Length > maxLen)
                    _input = _input.Substring(_input.Length - maxLen);

                foreach (var cheat in _cheatActions)
                {
                    if (_input.EndsWith(cheat.Key))
                    {
                        cheat.Value.Invoke();
                        _input = "";
                        break;
                    }
                }
            }
        }
    }

    private void ActivateCheatButtons()
    {
        _cheatButtons.SetActive(true);
    }

    private void ToggleFPSCounter()
    {
        if (_fpsCounter != null)
        {
            _fpsCounter.SetActive(true);
           DontDestroyOnLoad(_fpsCounter.gameObject);
        }
    }
}
