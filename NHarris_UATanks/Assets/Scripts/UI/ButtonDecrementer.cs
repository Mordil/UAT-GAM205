﻿using System;

public class ButtonDecrementer : InputButtonChanger
{
    public override void OnClick()
    {
        int outVal;

        // attempt to parse the string into an int - this is done just as extra type safety
        if (Int32.TryParse(ManagedInputField.text, out outVal))
        {
            outVal--;

            // if the decremented value is higher than the value limit, just return
            if (outVal < ValueLimit)
            {
                return;
            }

            // set the input field's text value to it
            ManagedInputField.text = (outVal).ToString();
        }
    }
}
