using UnityEngine;
using System.Collections.Generic;

public class BezierSurfaceGenerator : MonoBehaviour
{
    // Define the profile and path points
    public List<Vector3> profilePoints;
    public List<Vector3> pathPoints;
    public int profileResolution = 10;
    public int pathResolution = 10;

    void Start()
    {
        GenerateSurface();
    }

    void GenerateSurface()
    {
        if (profilePoints.Count < 2 || pathPoints.Count < 2)
        {
            Debug.LogError("Insufficient points to generate surface.");
            return;
        }

        // Generate profile curve points
        List<Vector3> profileCurve = GenerateBezierCurve(profilePoints, profileResolution);

        // Generate path curve points
        List<Vector3> pathCurve = GenerateBezierCurve(pathPoints, pathResolution);

        // Create mesh data
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Generate vertices
        foreach (var pathPoint in pathCurve)
        {
            foreach (var profilePoint in profileCurve)
            {
                vertices.Add(pathPoint + profilePoint);
            }
        }

        // Generate triangles
        int profileCount = profileCurve.Count;
        for (int i = 0; i < pathCurve.Count - 1; i++)
        {
            for (int j = 0; j < profileCount - 1; j++)
            {
                int start = i * profileCount + j;
                triangles.Add(start);
                triangles.Add(start + profileCount);
                triangles.Add(start + 1);

                triangles.Add(start + 1);
                triangles.Add(start + profileCount);
                triangles.Add(start + profileCount + 1);
            }
        }

        // Create mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        // Assign mesh to MeshFilter and MeshRenderer
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }

    List<Vector3> GenerateBezierCurve(List<Vector3> controlPoints, int resolution)
    {
        List<Vector3> curve = new List<Vector3>();
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            curve.Add(CalculateBezierPoint(t, controlPoints));
        }
        return curve;
    }

    Vector3 CalculateBezierPoint(float t, List<Vector3> points)
    {
        while (points.Count > 1)
        {
            List<Vector3> newPoints = new List<Vector3>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                newPoints.Add(Vector3.Lerp(points[i], points[i + 1], t));
            }
            points = newPoints;
        }
        return points[0];
    }
}
