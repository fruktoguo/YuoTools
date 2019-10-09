using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public class DebugCodeLocation
{
#if UNITY_EDITOR
    [UnityEditor.Callbacks.OnOpenAsset(0)]
    static bool OnOpenAsset(int instance, int line)
    {
        string stack_trace = GetStackTrace();
        if (!string.IsNullOrEmpty(stack_trace))
        {
            string strLower = stack_trace.ToLower();
            if (strLower.Contains(YuoLog.ExtensionTagContains))
            {
                Match matches = Regex.Match(stack_trace, @"\(at(.+)\)", RegexOptions.IgnoreCase);
                string pathline = "";
                if (matches.Success)
                {
                    pathline = matches.Groups[1].Value;
                    matches = matches.NextMatch();
                    if (matches.Success)
                    {
                        pathline = matches.Groups[1].Value;
                        pathline = pathline.Replace(" ", "");
                        int split_index = pathline.LastIndexOf(":");
                        string path = pathline.Substring(0, split_index);
                        line = Convert.ToInt32(pathline.Substring(split_index + 1));
                        string fullpath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                        fullpath = fullpath + path;
                        string strPath = fullpath.Replace('/', '\\');
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(strPath, line);
                    }
                    else
                    {
                        Debug.LogError("DebugCodeLocation OnOpenAsset, Error StackTrace");
                    }
                    //matches = matches.NextMatch();
                }
                return true;
            }
        }
        return false;
    }

    static string GetStackTrace()
    {
        var assembly_unity_editor = Assembly.GetAssembly(typeof(UnityEditor.EditorWindow));
        if (assembly_unity_editor == null) return null;
        var type_console_window = assembly_unity_editor.GetType("UnityEditor.ConsoleWindow");
        if (type_console_window == null) return null;
        var field_console_window = type_console_window.GetField("ms_ConsoleWindow", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        if (field_console_window == null) return null;
        var instance_console_window = field_console_window.GetValue(null);
        if (instance_console_window == null) return null;
        if ((object)UnityEditor.EditorWindow.focusedWindow == instance_console_window)
        {
            var type_list_view_state = assembly_unity_editor.GetType("UnityEditor.ListViewState");
            if (type_list_view_state == null) return null;
            var field_list_view = type_console_window.GetField("m_ListView", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field_list_view == null) return null;
            var value_list_view = field_list_view.GetValue(instance_console_window);
            if (value_list_view == null) return null;
            var field_active_text = type_console_window.GetField("m_ActiveText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field_active_text == null) return null;
            string value_active_text = field_active_text.GetValue(instance_console_window).ToString();
            return value_active_text;
        }

        return null;
    }
}
#endif
