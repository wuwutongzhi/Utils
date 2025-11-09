using System.Collections.Generic;
using AStar;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathFindingSample))]
public class PathFindingSampleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathFindingSample pathFindingSample = (PathFindingSample)target;

        if (GUILayout.Button("Generate Map and Obstacles"))
        {
            pathFindingSample.GenerateMapAndObstacles();
        }

        if (GUILayout.Button("Visualize Map"))
        {
            pathFindingSample.VisualizeMap();
        }

        if (GUILayout.Button("Find Path and Animate"))
        {
            Vector2Int startPosition = pathFindingSample.StartPos;
            Vector2Int goalPosition = pathFindingSample.EndPos;

            if (pathFindingSample.TryFindPath(startPosition, goalPosition, out List<Vector2Int> path))
            {
                pathFindingSample.StartCoroutine(pathFindingSample.AnimatePath(path));
            }
            else
            {
                Debug.Log("No path found!");
            }
        }
    }
}