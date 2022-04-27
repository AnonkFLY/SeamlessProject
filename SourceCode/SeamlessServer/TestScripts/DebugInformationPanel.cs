using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class DebugInformationPanel : MonoBehaviour
{
    private WaitForEndOfFrame _nextFrame;
    [SerializeField] private int fontSize = 30;
    [SerializeField] private Text informationText;
    [SerializeField] private InputField inputField;
    public CanvasGroup _canvasGroup;
    private ConsoleCommand _commandContr;
    private InputList _inputList;

    public UnityAction<DebugInformationPanel, string> onInputEnter;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _commandContr = new ConsoleCommand();
        _inputList = new InputList();
        _nextFrame = new WaitForEndOfFrame();

        InitCommand();
        //_commandContr.RegisterCommand()

        inputField.ActivateInputField();
        inputField.onEndEdit.AddListener(OnEnterInput);
        inputField.onValueChanged.AddListener(onInputChange);
        inputField.text = "";
    }

    private void onInputChange(string value)
    {
        _inputList.ResetIndex();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            inputField.text = _inputList.GetUp();
            MoveToEnd();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            inputField.text = _inputList.GetDown();
            MoveToEnd();
        }
    }

    private void InitCommand()
    {
        var help = new HelpCommand();
        var display = new DisplayCommand();


        help.onExecute += (data, command) =>
        {
            help.infoPanel = this;
            help.ShowCommandList(_commandContr.GetCommandList());
        };
        display.onExecute += (data, command) =>
        {
            display.infoPanel = this;
            display.DisplayData(data.indexExecute);
        };


        _commandContr.RegisterCommand(new SavelogCommand());
        _commandContr.RegisterCommand(help);
        _commandContr.RegisterCommand(display);
    }

    public void AddInputText(string input)
    {
        if (string.IsNullOrEmpty(input))
            return;
        informationText.text += $"\n>{input}";
        if (input == "Quit")
            Application.Quit();
        // var debug = _commandContr.Execute(input);
        // if (!string.IsNullOrEmpty(debug))
        // {
        //     AddInformation(debug);
        // }
    }
    public void AddInformation(string info, string color = null)
    {
        return;
        ServerHub.Instance.OnMainThreadExecute(() =>
        {
            if (!informationText)
                return;
            if (color != null)
            {
                informationText.text += $"\n<color={color}>{info}</color>";
                return;
            }
            informationText.text += $"\n{info}";
        });
    }
    public string GetServerLog()
    {
        if (!informationText)
            return "";
        return informationText.text;
    }
    private void OnEnterInput(string value)
    {
        AddInputText(value);
        _inputList.AddInput(value);
        onInputEnter?.Invoke(this, value);
        inputField.text = "";
        inputField.ActivateInputField();
    }
    private void MoveToEnd()
    {
        StartCoroutine(ResetCursor());
    }
    private IEnumerator ResetCursor()
    {
        yield return _nextFrame;
        inputField.MoveTextEnd(false);
    }
    private void OnValidate()
    {
        informationText.fontSize = fontSize;
        inputField.textComponent.fontSize = fontSize;
    }
}
