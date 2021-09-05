using UnityEngine;
using UnityEditor;

public class GizmosExample : MonoBehaviour
{
    public bool showBaseShapes;
    public bool showCircle;
    public bool showBezier;

    [Range(0, 1)]
    public float cubeSize;

    public Transform[] points = new Transform[4];

    private void OnDrawGizmos()
    {
        if (showBaseShapes)
        {
            Gizmos.DrawCube(
                -1f * Vector3.right,
                Vector3.one * cubeSize
            );

            Gizmos.color = Color.red;

            Gizmos.DrawSphere(
                Vector3.right,
                0.5f
            );

            Gizmos.color = Color.white;

            Gizmos.DrawSphere(
                3f * Vector3.right,
                0.5f
            );
        }

        if (showCircle)
        {
            Handles.color = Color.green;
            Handles.Disc(Quaternion.identity, Vector2.zero, Vector2.up, 1f, false, 1f);
        }

        if (showBezier)
        {
            for (int i = 0; i < points.Length; i++)
                Gizmos.DrawCube(points[i].position, Vector3.one * 0.2f);
            Handles.DrawBezier(
                points[0].position,
                points[3].position,
                points[1].position,
                points[2].position,
                Color.red,
                EditorGUIUtility.whiteTexture,
                3f
            );
        }
    }
}
