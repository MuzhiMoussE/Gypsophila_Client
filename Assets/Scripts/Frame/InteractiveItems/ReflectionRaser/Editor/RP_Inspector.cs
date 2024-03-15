using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ReflectiveProjection))]
[CanEditMultipleObjects]

public class RP_Inspector : Editor
{
    SerializedProperty Reflections,
    MaxDistance,
    projectionWidth,
    reflectiveLayers,
    stopReflectionLayers,
    ricochetObject,
    ricochetSpeed,
    projectionMaterial;

    void OnEnable()
    {
        Reflections = serializedObject.FindProperty("reflections");
        MaxDistance = serializedObject.FindProperty("maxDistance");
        projectionWidth = serializedObject.FindProperty("projectionWidth");
        reflectiveLayers = serializedObject.FindProperty("reflectiveLayers");
        stopReflectionLayers = serializedObject.FindProperty("stopReflectionLayers");
        ricochetObject = serializedObject.FindProperty("ricochetObject");
        ricochetSpeed = serializedObject.FindProperty("ricochetSpeed");
        projectionMaterial = serializedObject.FindProperty("projectionMaterial");
    }

    public override void OnInspectorGUI()
    {
        var button = GUILayout.Button("Click for more tools");
        if (button) Application.OpenURL("https://assetstore.unity.com/publishers/39163");
        EditorGUILayout.Space(5);

        ReflectiveProjection script = (ReflectiveProjection)target;

        EditorGUILayout.LabelField("REFLECTION OPTIONS", EditorStyles.boldLabel);
        
        // Reflections
        EditorGUILayout.PropertyField(Reflections, new GUIContent("Reflections", "Number of reflections allowed"));

        // Max Distance
        EditorGUILayout.PropertyField(MaxDistance, new GUIContent("Max Distance", "Max Distance of the traveling projections before stopping for not hitting any object"));

        // Reflection Width
        EditorGUILayout.PropertyField(projectionWidth, new GUIContent("Projection Width", "The width of the projection (line renderer)"));

        // Reflective Layer
        EditorGUILayout.PropertyField(reflectiveLayers, new GUIContent("Reflective Layer", "The layers you want to be reflective"));

        EditorGUILayout.PropertyField(stopReflectionLayers);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("RICOCHET OPTIONS", EditorStyles.boldLabel);

        // Ricochet Object
        EditorGUILayout.PropertyField(ricochetObject, new GUIContent("Ricochet Object", "Object that will ricochet (follow the projection trail)"));

        // Ricochet Speed
        EditorGUILayout.PropertyField(ricochetSpeed, new GUIContent("Ricochet Speed", "Speed of the ricochet object"));
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("MATERIAL OPTIONS", EditorStyles.boldLabel);
        
        // Projection Material
        EditorGUILayout.PropertyField(projectionMaterial, new GUIContent("Projection Material", "Custom material for the projection. Leaving it empty will draw back to the default material that comes with the package"));

        serializedObject.ApplyModifiedProperties();
    }
}
