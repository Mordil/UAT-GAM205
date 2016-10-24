using UnityEngine;
using UnityEditor;

/*
    Via of http://answers.unity3d.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
*/

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string value;

        switch (property.propertyType)
        {
            case SerializedPropertyType.Float:
                value = property.floatValue.ToString("0.0000");
                break;
            case SerializedPropertyType.Integer:
                value = property.intValue.ToString();
                break;
            case SerializedPropertyType.Boolean:
                value = property.boolValue.ToString();
                break;
            case SerializedPropertyType.String:
                value = property.stringValue;
                break;
            case SerializedPropertyType.Enum:
                value = property.enumNames[property.enumValueIndex];
                break;

            default:
                value = "(not supported)";
                break;
        }

        EditorGUI.LabelField(position, label.text, value);
    }
}
