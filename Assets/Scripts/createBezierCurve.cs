using UnityEngine;
using UnityEngine.ProBuilder; // Include ProBuilder namespace

public class BezierCurveGenerator : MonoBehaviour
{
    // Public variables to set the three 3D points
    public Vector3 startPoint = new Vector3(0, 0, 0);
    public Vector3 controlPoint = new Vector3(1, 2, 0);
    public Vector3 endPoint = new Vector3(3, 0, 0);

    public int curveResolution = 20; // The number of segments for a smooth curve

    void Start()
    {
        GenerateBezierCurve();
    }

    void GenerateBezierCurve()
    {
        // Create a new ProBuilder mesh
        ProBuilderMesh mesh = ProBuilderMesh.Create();

        // Generate the Bezier curve vertices
        Vector3[] bezierVertices = new Vector3[curveResolution + 1];
        for (int i = 0; i <= curveResolution; i++)
        {
            float t = i / (float)curveResolution;
            bezierVertices[i] = CalculateBezierPoint(t, startPoint, controlPoint, endPoint);
        }

        // Build the mesh from the generated points
        Vector3[] vertices = new Vector3[bezierVertices.Length * 2];
        for (int i = 0; i < bezierVertices.Length; i++)
        {
            vertices[i * 2] = bezierVertices[i];
            vertices[i * 2 + 1] = bezierVertices[i] + Vector3.up * 0.1f; // Adding thickness
        }

        // Create edges between points
        int[] indices = new int[(bezierVertices.Length - 1) * 6];
        for (int i = 0, j = 0; i < bezierVertices.Length - 1; i++, j += 6)
        {
            indices[j] = i * 2;
            indices[j + 1] = i * 2 + 1;
            indices[j + 2] = (i + 1) * 2;

            indices[j + 3] = i * 2 + 1;
            indices[j + 4] = (i + 1) * 2 + 1;
            indices[j + 5] = (i + 1) * 2;
        }

        // Set vertices and triangles in the ProBuilder mesh
        mesh.positions = vertices;
        mesh.faces = new[] { new Face(indices) };

        // Rebuild the mesh to update
        mesh.ToMesh();
        mesh.Refresh();
    }

    // Function to calculate the point on a Bezier curve given t
    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return point;
    }
}
