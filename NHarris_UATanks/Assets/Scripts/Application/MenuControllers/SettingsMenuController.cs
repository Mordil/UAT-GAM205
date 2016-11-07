using System;
using L4.Unity.Common;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : BaseScript
{
    [Serializable]
    public class SettingsComponents
    {
        #region Audio
        [Serializable]
        public class SliderComponents
        {
            public Slider MasterVolumeSlider;
            public Slider SFXVolumeSlider;
            public Slider MusicVolumeSlider;

            public void CheckDependencies()
            {
                if (MasterVolumeSlider == null)
                {
                    throw new UnityException("MasterVolumeSlider has not been assigned!");
                }

                if (SFXVolumeSlider == null)
                {
                    throw new UnityException("SFXVolumeSlider has not been assigned!");
                }

                if (MusicVolumeSlider == null)
                {
                    throw new UnityException("MusicVolumeSlider has not been assigned!");
                }
            }
        }
        [Serializable]
        public class InputFieldComponents
        {
            [Header("Audio Input")]
            public InputField MasterVolumeInput;
            public InputField SFXVolumeInput;
            public InputField MusicVolumeInput;

            [Header("Gameplay Input")]
            public InputField NumberOfPlayers;
            public InputField NumberOfLives;

            public void CheckDependencies()
            {
                if (MasterVolumeInput == null)
                {
                    throw new UnityException("MasterVolumeInput has not been assigned!");
                }

                if (SFXVolumeInput == null)
                {
                    throw new UnityException("SFXVolumeInput has not been assigned!");
                }

                if (MusicVolumeInput == null)
                {
                    throw new UnityException("MusicVolumeInput has not been assigned!");
                }

                if (NumberOfPlayers == null)
                {
                    throw new UnityException("NumberOfPlayers has not been assigned!");
                }

                if (NumberOfLives == null)
                {
                    throw new UnityException("NumberOfLives has not been assigned!");
                }
            }
        }
        [Serializable]
        public class ToggleFieldComponents
        {
            public Toggle MatchOfTheDayToggle;

            public void CheckDependencies()
            {
                if (MatchOfTheDayToggle == null)
                {
                    throw new UnityException("MatchOfTheDayToggle has not been assigned!");
                }
            }
        }
        #endregion

        public SliderComponents Sliders;
        public InputFieldComponents InputFields;
        public ToggleFieldComponents Toggles;

        public void CheckDependencies()
        {
            // inner InspectorContainers should have their CheckDependencies called 
            Sliders.CheckDependencies();
            InputFields.CheckDependencies();
            Toggles.CheckDependencies();
        }
    }
    
    [SerializeField]
    private SettingsComponents SettingsEditingComponents;

    protected override void Start()
    {
        base.Start();
        // pulls initialized values from the GameManager.Settings
        InitDefaults();
        // adds listeners to the settings UI objects
        RegisterUIEvents();
    }

    protected override void CheckDependencies()
    {
        SettingsEditingComponents.CheckDependencies();
    }

    public void OnApplySettingsButtonClicked()
    {
        // commit all values to the GameManger
        // this probably could be a lot more selective to only commit changed values
        GameManager.Instance.Settings.MasterVolume = SettingsEditingComponents.Sliders.MasterVolumeSlider.value;
        GameManager.Instance.Settings.SFXVolume = SettingsEditingComponents.Sliders.SFXVolumeSlider.value;
        GameManager.Instance.Settings.MusicVolume = SettingsEditingComponents.Sliders.MusicVolumeSlider.value;
        GameManager.Instance.Settings.MatchOfTheDay = SettingsEditingComponents.Toggles.MatchOfTheDayToggle.isOn;
        GameManager.Instance.Settings.NumberOfPlayers = Int32.Parse(SettingsEditingComponents.InputFields.NumberOfPlayers.text);
        GameManager.Instance.Settings.NumberOfLives = Int32.Parse(SettingsEditingComponents.InputFields.NumberOfLives.text);
    }

    public void OnRevertChangesButtonClicked()
    {
        // resets to the last "apply" state.
        InitDefaults();
    }

    public void CloseMenu()
    {
        GameManager.Instance.CurrentScene.As<MainMenu>().GoToMainMenu();
    }

    private void InitDefaults()
    {
        SettingsEditingComponents.Sliders.MasterVolumeSlider.value = GameManager.Instance.Settings.MasterVolume;
        SettingsEditingComponents.Sliders.SFXVolumeSlider.value = GameManager.Instance.Settings.SFXVolume;
        SettingsEditingComponents.Sliders.MusicVolumeSlider.value = GameManager.Instance.Settings.MusicVolume;
        // turns the float value between 0 and 1 from the sliders into a "readable" format of 0-100 for percentages
        SettingsEditingComponents.InputFields.MasterVolumeInput.text = (GameManager.Instance.Settings.MasterVolume * 100).ToString();
        SettingsEditingComponents.InputFields.SFXVolumeInput.text = (GameManager.Instance.Settings.SFXVolume * 100).ToString();
        SettingsEditingComponents.InputFields.MusicVolumeInput.text = (GameManager.Instance.Settings.MusicVolume * 100).ToString();

        SettingsEditingComponents.Toggles.MatchOfTheDayToggle.isOn = GameManager.Instance.Settings.MatchOfTheDay;

        SettingsEditingComponents.InputFields.NumberOfLives.text = GameManager.Instance.Settings.NumberOfLives.ToString();
        SettingsEditingComponents.InputFields.NumberOfPlayers.text = GameManager.Instance.Settings.NumberOfPlayers.ToString();
    }

    private void RegisterUIEvents()
    {
        // takes the new string, divides it by 100 (as values are displayed in text as percentages)
        // and converts to a double before casting to a float to proprogate the value to the slider's value
        #region InputField.onEndEdit handlers
        SettingsEditingComponents.InputFields.MasterVolumeInput.onEndEdit.AddListener((newString) =>
        {
            // if the new string will be empty, revert changes
            if (newString == string.Empty)
            {
                SettingsEditingComponents.InputFields.MasterVolumeInput.text = (SettingsEditingComponents.Sliders.MasterVolumeSlider.value * 100).ToString();
            }
            else
            {
                // convert the new string into a number
                var newValue = (float)Convert.ToDouble(newString);

                // if the number is greater than 100 (the max)
                if (newValue > 100)
                {
                    // set the newValue to 100
                    newValue = 100;
                    // set the text property to the new value (to update the text input)
                    SettingsEditingComponents.InputFields.MasterVolumeInput.text = newValue.ToString();
                    // return, as that change will cause this function to be re-called and we don't want to set the slider
                    // twice.
                    return;
                }

                // set the slider (whose values are betweeen 0-1) to the value divided by 100 (79 will be .79).
                SettingsEditingComponents.Sliders.MasterVolumeSlider.value = newValue / 100f;
            }
        });
        SettingsEditingComponents.InputFields.MusicVolumeInput.onEndEdit.AddListener((newString) =>
        {
            // if the new string will be empty, revert changes
            if (newString == string.Empty)
            {
                SettingsEditingComponents.InputFields.MusicVolumeInput.text = (SettingsEditingComponents.Sliders.MusicVolumeSlider.value * 100).ToString();
            }
            else
            {
                // convert the new string into a number
                var newValue = (float)Convert.ToDouble(newString);

                // if the number is greater than 100 (the max)
                if (newValue > 100)
                {
                    // set the newValue to 100
                    newValue = 100;
                    // set the text property to the new value (to update the text input)
                    SettingsEditingComponents.InputFields.MusicVolumeInput.text = newValue.ToString();
                    // return, as that change will cause this function to be re-called and we don't want to set the slider
                    // twice.
                    return;
                }

                // set the slider (whose values are betweeen 0-1) to the value divided by 100 (79 will be .79).
                SettingsEditingComponents.Sliders.MusicVolumeSlider.value = (float)Convert.ToDouble(newString) / 100f;
            }
        });
        SettingsEditingComponents.InputFields.SFXVolumeInput.onEndEdit.AddListener((newString) =>
        {
            // if the new string will be empty, revert changes
            if (newString == string.Empty)
            {
                SettingsEditingComponents.InputFields.SFXVolumeInput.text = (SettingsEditingComponents.Sliders.SFXVolumeSlider.value * 100).ToString();
            }
            else
            {
                // convert the new string into a number
                var newValue = (float)Convert.ToDouble(newString);

                // if the number is greater than 100 (the max)
                if (newValue > 100)
                {
                    // set the newValue to 100
                    newValue = 100;
                    // set the text property to the new value (to update the text input)
                    SettingsEditingComponents.InputFields.SFXVolumeInput.text = newValue.ToString();
                    // return, as that change will cause this function to be re-called and we don't want to set the slider
                    // twice.
                    return;
                }

                // set the slider (whose values are betweeen 0-1) to the value divided by 100 (79 will be .79).
                SettingsEditingComponents.Sliders.SFXVolumeSlider.value = (float)Convert.ToDouble(newString) / 100f;
            }
        });
        #endregion

        // takes the new value, multiplies it by 100 (as values are displayed in text as percentages)
        // and takes the string to assign to the inputfield's text variable.
        #region Slider.onValueChanged handlers
        SettingsEditingComponents.Sliders.MasterVolumeSlider.onValueChanged.AddListener((newValue) =>
        {
            SettingsEditingComponents.InputFields.MasterVolumeInput.text = (newValue * 100).ToString();
        });
        SettingsEditingComponents.Sliders.MusicVolumeSlider.onValueChanged.AddListener((newValue) =>
        {
            SettingsEditingComponents.InputFields.MusicVolumeInput.text = (newValue * 100).ToString();
        });
        SettingsEditingComponents.Sliders.SFXVolumeSlider.onValueChanged.AddListener((newValue) =>
        {
            SettingsEditingComponents.InputFields.SFXVolumeInput.text = (newValue * 100).ToString();
        });
        #endregion
    }
}
