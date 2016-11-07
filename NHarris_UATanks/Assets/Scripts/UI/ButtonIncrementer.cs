using L4.Unity.Common;
using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class InputButtonChanger : BaseScript
{
    [SerializeField]
    protected int ValueLimit;

    [SerializeField]
    protected InputField ManagedInputField;

    protected override void CheckDependencies()
    {
        base.CheckDependencies();

        this.CheckIfDependencyIsNull(ManagedInputField);
    }

    public abstract void OnClick();
}

public class ButtonIncrementer : InputButtonChanger
{
    public override void OnClick()
    {
        int outVal;
        
        // attempt to parse the string into an int - this is done just as extra type safety
        if (Int32.TryParse(ManagedInputField.text, out outVal))
        {
            outVal++;

            // if the incremented value is higher than the value limit, just return
            if (outVal > ValueLimit)
            {
                return;
            }

            // set the input field's text value to it
            ManagedInputField.text = (outVal).ToString();
        }
    }
}
