using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ConvarBinding : MonoBehaviour
{
    public string convarName;
    public bool useValuesNotNumbers;

    public bool GetStringValueFromControl(out string value)
    {
        dfSlider component = base.GetComponent<dfSlider>();
        if (component != null)
        {
            value = component.Value.ToString();
            return true;
        }
        dfDropdown dropdown = base.GetComponent<dfDropdown>();
        if (dropdown != null)
        {
            int selectedIndex = dropdown.SelectedIndex;
            if (selectedIndex == -1)
            {
                value = string.Empty;
                return false;
            }
            if (this.useValuesNotNumbers)
            {
                value = dropdown.SelectedValue;
            }
            else
            {
                value = selectedIndex.ToString();
            }
            return true;
        }
        dfCheckbox checkbox = base.GetComponent<dfCheckbox>();
        if (checkbox != null)
        {
            value = !checkbox.IsChecked ? bool.FalseString : bool.TrueString;
            return true;
        }
        value = string.Empty;
        return false;
    }

    private void Start()
    {
        this.UpdateFromConVar();
    }

    public void UpdateConVars()
    {
        string str;
        if (this.GetStringValueFromControl(out str))
        {
            ConsoleSystem.Run(this.convarName + " \"" + str + "\"", false);
        }
    }

    public void UpdateFromConVar()
    {
        dfSlider component = base.GetComponent<dfSlider>();
        if (component != null)
        {
            component.Value = ConVar.GetFloat(this.convarName, component.Value);
        }
        dfDropdown dropdown = base.GetComponent<dfDropdown>();
        if (dropdown != null)
        {
            if (this.useValuesNotNumbers)
            {
                string str = ConVar.GetString(this.convarName, string.Empty);
                if (!string.IsNullOrEmpty(str))
                {
                    int selectedIndex = dropdown.SelectedIndex;
                    dropdown.SelectedValue = str;
                    if (dropdown.SelectedIndex == -1)
                    {
                        dropdown.SelectedIndex = selectedIndex;
                    }
                }
            }
            else
            {
                int @int = ConVar.GetInt(this.convarName, -1f);
                if (@int != -1)
                {
                    dropdown.SelectedIndex = @int;
                }
            }
        }
        dfCheckbox checkbox = base.GetComponent<dfCheckbox>();
        if (checkbox != null)
        {
            checkbox.IsChecked = ConVar.GetBool(this.convarName, checkbox.IsChecked);
        }
    }
}

