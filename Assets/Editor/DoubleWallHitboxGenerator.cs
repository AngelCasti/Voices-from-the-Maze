using UnityEngine;
using UnityEditor;

public class DoubleWallHitboxGenerator : EditorWindow
{
    private float extraThicknessPerSide = 0.4f;
    private float extraLengthPerEnd = 0.4f;
    private float hitboxHeight = 5f;
    private bool includeInactiveChildren = true;

    [MenuItem("Tools/Maze/Add Double Hitbox To Wall Children")]
    public static void ShowWindow()
    {
        GetWindow<DoubleWallHitboxGenerator>("Wall Hitbox Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Double-Sided Wall Hitbox", EditorStyles.boldLabel);

        extraThicknessPerSide = EditorGUILayout.FloatField("Extra thickness per side", extraThicknessPerSide);
        extraLengthPerEnd = EditorGUILayout.FloatField("Extra length per end", extraLengthPerEnd);
        hitboxHeight = EditorGUILayout.FloatField("Hitbox height", hitboxHeight);
        includeInactiveChildren = EditorGUILayout.Toggle("Include inactive children", includeInactiveChildren);

        GUILayout.Space(10);

        if (GUILayout.Button("Add Hitboxes To Children Of Selected Object"))
        {
            AddHitboxesToChildren();
        }
    }

    private void AddHitboxesToChildren()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Selecciona el objeto padre que contiene las paredes, por ejemplo: Walls o Walls_Prueba.");
            return;
        }

        GameObject parent = Selection.activeGameObject;

        Transform[] children = parent.GetComponentsInChildren<Transform>(includeInactiveChildren);

        int created = 0;
        int skipped = 0;

        foreach (Transform child in children)
        {
            if (child == parent.transform)
                continue;

            GameObject wall = child.gameObject;

            if (wall.transform.Find("Hitbox_" + wall.name) != null)
            {
                skipped++;
                continue;
            }

            MeshFilter meshFilter = wall.GetComponent<MeshFilter>();

            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                skipped++;
                continue;
            }

            Bounds meshBounds = meshFilter.sharedMesh.bounds;

            GameObject hitbox = new GameObject("Hitbox_" + wall.name);
            hitbox.transform.SetParent(wall.transform);

            hitbox.transform.localPosition = meshBounds.center;
            hitbox.transform.localRotation = Quaternion.identity;
            hitbox.transform.localScale = Vector3.one;

            BoxCollider boxCollider = hitbox.AddComponent<BoxCollider>();

            Vector3 size = meshBounds.size;

            float worldSizeX = size.x * wall.transform.lossyScale.x;
            float worldSizeZ = size.z * wall.transform.lossyScale.z;

            float totalExtraThickness = extraThicknessPerSide * 2f;
            float totalExtraLength = extraLengthPerEnd * 2f;

            // Si X es menor que Z, X es el grosor y Z es el largo.
            if (worldSizeX < worldSizeZ)
            {
                size.x += totalExtraThickness / wall.transform.lossyScale.x;
                size.z += totalExtraLength / wall.transform.lossyScale.z;
            }
            // Si Z es menor que X, Z es el grosor y X es el largo.
            else
            {
                size.z += totalExtraThickness / wall.transform.lossyScale.z;
                size.x += totalExtraLength / wall.transform.lossyScale.x;
            }

            size.y = hitboxHeight / wall.transform.lossyScale.y;

            boxCollider.size = size;
            boxCollider.center = Vector3.zero;
            boxCollider.isTrigger = false;

            created++;
        }

        Debug.Log("Hitboxes creadas: " + created + " | Objetos omitidos: " + skipped);
    }
}