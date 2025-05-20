using System.IO;
using UnityEditor;
using UnityEngine;

public class SimpleHierarchyExporter : EditorWindow
{
    [MenuItem("Tools/Simple Hierarchy Export")]
    public static void ShowWindow()
    {
        GetWindow<SimpleHierarchyExporter>("Simple Exporter");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Export Selected to TXT"))
        {
            string path = EditorUtility.SaveFilePanel("Save Hierarchy", "", "hierarchy.txt", "txt");
            if (!string.IsNullOrEmpty(path))
            {
                ExportSelectedObjectsToTxt(path);
            }
        }
    }

    private void ExportSelectedObjectsToTxt(string filePath)
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (GameObject obj in selectedObjects)
            {
                WriteObjectInfo(writer, obj, 0);
            }
        }
        EditorUtility.DisplayDialog("Export Complete", "Objects exported to " + filePath, "OK");
    }

    private void WriteObjectInfo(StreamWriter writer, GameObject obj, int depth)
    {
        string indent = new string(' ', depth * 2);
        writer.WriteLine($"{indent}GameObject: {obj.name}");
        writer.WriteLine($"{indent}Position: {obj.transform.position}");
        writer.WriteLine($"{indent}Rotation: {obj.transform.eulerAngles}");
        writer.WriteLine($"{indent}Scale: {obj.transform.localScale}");

        Component[] components = obj.GetComponents<Component>();
        writer.WriteLine($"{indent}Components:");
        foreach (Component component in components)
        {
            writer.WriteLine($"{indent}  - {component.GetType().Name}");
        }

        writer.WriteLine();

        foreach (Transform child in obj.transform)
        {
            WriteObjectInfo(writer, child.gameObject, depth + 1);
        }
    }
}