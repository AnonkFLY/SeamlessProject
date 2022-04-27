using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ErrorPrompt
{
    public UnityAction closeButtonAction;
    public UnityAction confirmButtonAction;
    public string infoText;
    public string confirmText;

    public ErrorPrompt(string confirmText, string infoText)
    {
        this.confirmText = confirmText;
        this.infoText = infoText;
    }
}
