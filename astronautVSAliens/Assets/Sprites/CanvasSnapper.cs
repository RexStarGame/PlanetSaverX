using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class SmartGridSnapper : Editor
{
    [MenuItem("Tools/Sprite Snapper/Snap Selected as Grid")]
    public static void SnapAsGrid()
    {
        GameObject[] selectedGOs = Selection.gameObjects;
        var renderers = selectedGOs
            .Select(go => go.GetComponent<SpriteRenderer>())
            .Where(sr => sr != null)
            .ToList();

        if (renderers.Count < 2)
        {
            Debug.LogWarning("Vælg mindst to sprites for at snappe dem som et grid.");
            return;
        }

        Undo.RecordObjects(selectedGOs.Select(go => go.transform).ToArray(), "Smart Grid Snap");

        // 1. Find gennemsnitsstørrelsen på dine sprites (vi antager de er ca. samme størrelse)
        Vector2 size = renderers[0].bounds.size;

        // 2. Find det øverste venstre punkt blandt alle valgte (Ankeret)
        float minX = renderers.Min(sr => sr.transform.position.x);
        float maxY = renderers.Max(sr => sr.transform.position.y);
        Vector3 anchorPos = new Vector3(minX, maxY, renderers[0].transform.position.z);

        // 3. For hver sprite, find ud af hvor mange "pladser" den er fra ankeret
        foreach (var sr in renderers)
        {
            // Beregn hvor mange 'enheder' væk den er i x og y
            // Vi runder af (Round), så den hopper til det nærmeste "grid slot"
            float gridX = Mathf.Round((sr.transform.position.x - anchorPos.x) / size.x);
            float gridY = Mathf.Round((sr.transform.position.y - anchorPos.y) / size.y);

            // Beregn den nye præcise position
            Vector3 newPos = new Vector3(
                anchorPos.x + (gridX * size.x),
                anchorPos.y + (gridY * size.y),
                sr.transform.position.z // Bevar original dybde (Z)
            );

            sr.transform.position = newPos;
        }

        Debug.Log($"Smart-snappede {renderers.Count} sprites på plads i et grid!");
    }
}