using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HeroManager))]
public class HeroDebugger : Editor
{
    private HeroManager _hero;
    private FieldInfo[] _fields;

    private int _selectedFloatFieldIndex = 0;
    private string[] _floatFieldNames;

    private void OnEnable()
    {
        if (HeroManager.fieldDebugColors == null)
            HeroManager.fieldDebugColors = new Dictionary<string, Color>();

        // get reference to edited object (the hero class)
        _hero = (HeroManager)target;

        // list all available parameters
        System.Type HeroType = _hero.GetType();
        _fields = HeroType.GetFields();
        // filter to only float fields, and extract names
        _floatFieldNames = _fields
            .Where(f => f.FieldType == typeof(float))
            .Select(f => f.Name)
            .ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 1 - show properties
        // ------------------------
        foreach (FieldInfo field in _fields)
        {
            //check for "hide in inspector" attribute:
            // if there is one, cancel the display for this field
            if (System.Attribute.IsDefined(field, typeof(HideInInspector), false))
                continue;

            // ignore static fields
            if (field.IsStatic)
                continue;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name), true);
            GUILayout.Space(16);
            EditorGUILayout.BeginVertical(GUILayout.Width(20f));
            Color fieldColor;
            if (!HeroManager.fieldDebugColors.TryGetValue(field.Name, out fieldColor))
                fieldColor = Color.white;
            HeroManager.fieldDebugColors[field.Name] = EditorGUILayout.ColorField(fieldColor);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }


        // 2 - add debugger options
        // ------------------------
        // get reference to edited object (the hero class)
        HeroManager hero = (HeroManager)target;

        EditorGUILayout.LabelField("Debugger settings", EditorStyles.boldLabel);

        // show popup for float fields
        if (_floatFieldNames.Length > 0)
            _selectedFloatFieldIndex = EditorGUILayout.Popup(
                "Edited float:",
                _selectedFloatFieldIndex,
                _floatFieldNames,
                EditorStyles.popup
            );

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        string fieldName = _floatFieldNames[_selectedFloatFieldIndex];
        Color fieldColor;
        if (!HeroManager.fieldDebugColors.TryGetValue(fieldName, out fieldColor))
            fieldColor = Color.white;

        // get debugged field value dynamically
        FieldInfo field = _hero.GetType().GetField(fieldName);
        float val = (float)field.GetValue(_hero);

        // set proper colors
        Handles.color = fieldColor;
        GUI.color = fieldColor;

        // draw some debugging handles
        Vector3 pos = _hero.transform.position;
        Handles.DrawWireDisc(pos, _hero.transform.up, val);
        Handles.Label(
            pos + (val + 0.5f) * Vector3.right,
            $"{_CapitalizeWords(_floatFieldNames[_selectedFloatFieldIndex])}: " + val.ToString("0.0")
        );

        // connect the handle to the actual value and allow for
        // update via the gizmo!
        field.SetValue(_hero, Handles.ScaleValueHandle(
            val,
            _hero.transform.position + _hero.transform.forward * val,
            _hero.transform.rotation,
            5,
            Handles.ConeHandleCap,
            1
        ));
    }


    private static Regex _camelCaseRegex = new Regex(
        @"(?:[a-z]+|[A-Z]+|^)([a-z]|\d)*", RegexOptions.Compiled
    );
    private static string _CapitalizeWords(string str)
    {
        List<string> words = new List<string>();
        MatchCollection matches = _camelCaseRegex.Matches(str);
        string word;
        foreach (Match match in matches)
        {
            word = match.Groups[0].Value;
            word = word[0].ToString().ToUpper() + word.Substring(1);
            words.Add(word);
        }
        return string.Join(" ", words);
    }

}
