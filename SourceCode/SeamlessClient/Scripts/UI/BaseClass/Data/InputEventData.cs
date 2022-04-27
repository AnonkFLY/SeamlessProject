using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputEventData
{
    public string inputText;
    public Action<InputView, string> onEnter;
    public TMPro.TMP_InputField.ContentType type;

    public InputEventData(string inputText, Action<InputView, string> onEnter, TMPro.TMP_InputField.ContentType type)
    {
        this.inputText = inputText;
        this.onEnter = onEnter;
        this.type = type;
    }
}