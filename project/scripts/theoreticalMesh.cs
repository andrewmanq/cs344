using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class theoreticalMesh
{

    private List<Vector3> verts;
    private List<int> tris;
    private List<Vector2> uvs;

    public theoreticalMesh()
    {
        verts = new List<Vector3>();
        tris = new List<int>();
        uvs = new List<Vector2>();
    }

    public theoreticalMesh(Mesh aMesh)
    {
        adopt(aMesh);
    }

    public void addRaisedNgon(List<Vector3> points, float height, Vector3 direction, bool fillBottom)
    {
        List<Vector3> raisedPoints = new List<Vector3>();

        for (int i = 0; i < points.Count; i++)
        {
            raisedPoints.Add(points[i] + direction * height);
        }

        addSmartNgon(raisedPoints);

        List<Vector3> ribbonPoints = new List<Vector3>();

        for (int i = points.Count - 1; i >= 0; i--)
        {
            ribbonPoints.Add(raisedPoints[i]);
            ribbonPoints.Add(points[i]);
        }

        ribbonPoints.Add(raisedPoints[raisedPoints.Count - 1]);
        ribbonPoints.Add(points[points.Count - 1]);

        addRibbon(ribbonPoints, 10, false, height);

        if (fillBottom)
        {
            List<Vector3> reversePoints = new List<Vector3>();

            for (int i = points.Count - 1; i >= 0; i--)
            {
                reversePoints.Add(points[i]);
            }

            addSmartNgon(reversePoints);
        }
    }

    /*
     * a ribbon is just a series of quads that are sowed together side by side. The list must put each segment side by side:
     *                            1---3---5---7
     * example list and diagram:  |   |   |   |  etc....
     *                            2---4---6---8
     */
    public void addRibbon(List<Vector3> points)
    {
        addRibbon(points, float.PositiveInfinity, false, 1);
    }
    public void addRibbon(List<Vector3> points, bool shearing)
    {
        addRibbon(points, float.PositiveInfinity, shearing, 1);
    }
    public void addRibbon(List<Vector3> points, float smoothingAngle)
    {
        addRibbon(points, smoothingAngle, false, 1);
    }

    /*
    * this Version of a ribbon only smooths angles that are lower than the smoothing angle.
    */
    public void addRibbon(List<Vector3> points, float smoothingAngle, bool shearing, float textureHeight)
    {
        int theSize = points.Count;
        int vertOffset = verts.Count - 1;

        if (theSize % 2 == 1)
        {
            theSize -= 1;

            if (theSize < 4)
            {
                Debug.Log("not enough ribbon points");
                return;
            }
        }

        verts.Add(points[0]);
        verts.Add(points[1]);

        float uvDistance1;
        float uvDistance2;
        float distanceScale;

        //This if() block minimizes shearing
        if (shearing)
        {
            Quaternion ribbonDirection = Quaternion.LookRotation(points[2] - points[0]);
            Quaternion otherSide = Quaternion.LookRotation(points[1] - points[0]);
            float between = Quaternion.Angle(ribbonDirection, otherSide);
            Quaternion rightAngle = Quaternion.SlerpUnclamped(ribbonDirection, otherSide, 90 / between);
            float hypotenuse = Vector3.Distance(points[0], points[1]);
            float cos = Mathf.Cos(Quaternion.Angle(otherSide, rightAngle) * Mathf.Deg2Rad);
            float sin = Mathf.Sin(Quaternion.Angle(otherSide, rightAngle) * Mathf.Deg2Rad);
            float adjacent = hypotenuse * cos;

            Vector3 uvPoint = points[0];
            uvPoint += rightAngle * (Vector3.forward * adjacent);

            distanceScale = Vector3.Distance(points[0], uvPoint) / textureHeight;
            float secondOffset = Vector3.Distance(points[1], uvPoint) / distanceScale;
            if (Quaternion.Angle(ribbonDirection, otherSide) > 90)
            {
                secondOffset *= -1;
            }

            //Debug.DrawLine(points[0], uvPoint);
            //Debug.DrawLine(points[1], uvPoint);
            //Debug.DrawLine(points[0], points[1]);

            uvs.Add(new Vector2(0, 1 * textureHeight));
            uvs.Add(new Vector2(secondOffset * textureHeight, 0));

            vertOffset += 2;

            uvDistance1 = 0;
            uvDistance2 = secondOffset;
        }
        else
        {
            distanceScale = Vector3.Distance(points[0], points[1]) / textureHeight;
            uvs.Add(new Vector2(0, 1 * textureHeight));
            uvs.Add(new Vector2(0, 0));

            vertOffset += 2;

            uvDistance1 = 0;
            uvDistance2 = 0;
        }

        for (int i = 2; i < theSize - 1; i += 2)
        {
            float angleValue = 0;
            float angleValue2 = 0;

            try
            {
                angleValue = Mathf.Abs(Vector3.Angle(points[i - 4] - points[i - 2], points[i - 2] - points[i]));
                //Debug.Log("angle " + i + " = " + angleValue);
                angleValue2 = Mathf.Abs(Vector3.Angle(points[i - 3] - points[i - 1], points[i - 1] - points[i + 1]));
                //Debug.Log("angle " + (i + 1) + " = " + angleValue2);
            }
            catch (Exception e)
            {
                //
            }

            if (((angleValue + angleValue2) / 2) < smoothingAngle)
            {

                verts.Add(points[i]);
                verts.Add(points[i + 1]);

                uvDistance1 += Vector3.Distance(points[i], points[i - 2]) / distanceScale;
                uvDistance2 += Vector3.Distance(points[i + 1], points[i - 1]) / distanceScale;

                uvs.Add(new Vector2(uvDistance1, 1 * textureHeight));
                uvs.Add(new Vector2(uvDistance2, 0));

                vertOffset += 2;

                tris.Add(vertOffset - 3);
                tris.Add(vertOffset - 1);
                tris.Add(vertOffset);

                tris.Add(vertOffset);
                tris.Add(vertOffset - 2);
                tris.Add(vertOffset - 3);
            }
            else
            {
                verts.Add(points[i - 2]);
                verts.Add(points[i - 1]);
                verts.Add(points[i]);
                verts.Add(points[i + 1]);

                uvs.Add(new Vector2(uvDistance1, 1 * textureHeight));
                uvs.Add(new Vector2(uvDistance1, 0));

                uvDistance1 += Vector3.Distance(points[i], points[i - 2]) / distanceScale;
                uvDistance2 += Vector3.Distance(points[i + 1], points[i - 1]) / distanceScale;

                uvs.Add(new Vector2(uvDistance1, 1 * textureHeight));
                uvs.Add(new Vector2(uvDistance2, 0));

                vertOffset += 4;

                tris.Add(vertOffset - 3);
                tris.Add(vertOffset - 1);
                tris.Add(vertOffset);

                tris.Add(vertOffset);
                tris.Add(vertOffset - 2);
                tris.Add(vertOffset - 3);
            }
        }
    }



    /*
     * takes a list of points + middlepoint and makes a triangle fan with the midpoint in the center
     */
    public void addNgon(Vector3 center, List<Vector3> points)
    {
        int theSize = points.Count;
        int oldSize = verts.Count;

        float averageDist = 0;
        foreach (Vector3 p in points)
        {
            averageDist += Vector3.Distance(p, center);
        }
        averageDist /= points.Count;

        //to find the angle from A to C with pivot B:     Vector3.Angle(b - a, b - c);
        float accumAngle = 0;

        verts.Add(center);
        Vector2 centerUV = new Vector2(.5f, .5f);
        uvs.Add(centerUV);

        verts.Add(points[0]);
        uvs.Add(findNgonUVCoordinates(averageDist, Vector3.Distance(points[0], center), 0));

        for (int i = 1; i < theSize; i++)
        {

            verts.Add(points[i]);

            accumAngle += Vector3.Angle(center - points[i - 1], center - points[i]);
            float relativeAngle = Vector3.Angle(center - points[0], center - points[i]);
            if (accumAngle > 180)
            {
                relativeAngle *= -1;
            }

            uvs.Add(findNgonUVCoordinates(averageDist, Vector3.Distance(points[i], center), relativeAngle));

            tris.Add(oldSize);
            tris.Add(oldSize + i);
            tris.Add(oldSize + i + 1);

        }

        tris.Add(oldSize);
        tris.Add(oldSize + theSize);
        tris.Add(oldSize + 1);

    }

    private Vector2 findNgonUVCoordinates(float avgDist, float distFromCenter, float angleFromPoint1)
    {
        Vector2 centerUV = new Vector2(.5f, .5f);
        float UVdist = distFromCenter / avgDist;
        Vector2 startVec = (Vector2.left * .5f) * UVdist;

        Vector2 answer = centerUV + startVec.Rotate(angleFromPoint1);
        return answer;
    }

    public void addSmartNgon(List<Vector3> points)
    {
        points = points.Distinct().ToList();
        int oldSize = verts.Count;

        if(points.Count < 3)
        {
            return;
        }
        var outline2d = findNgon2dCoordinates(points);

        Triangulator tr = new Triangulator(outline2d);
        int[] indices = tr.Triangulate();

        foreach (Vector3 v in points)
        {
            verts.Add(v);
        }
        foreach (int i in indices)
        {
            tris.Add(i + oldSize);
        }
        foreach (Vector2 v in outline2d)
        {
            uvs.Add(v);
        }

    }

    private Vector2[] findNgon2dCoordinates(List<Vector3> points)
    {
        Vector2[] answer = new Vector2[points.Count];

        Vector3 avgNormal = Vector3.zero;

        int avgCount = 0;
        for (int i = 1; i < points.Count; i++)
        {
            var a = points[i - 1];
            var b = points[i];
            var c = points[(i + 1) % points.Count];

            var side1 = b - a;
            var side2 = c - a;

            var norm = new Plane(a, b, c).normal;

            avgCount += 1;
            avgNormal += norm;
        }

        avgNormal /= points.Count;

        var normal = avgNormal.normalized;

        var diff = Quaternion.FromToRotation(normal, Vector3.up);

        var projectedPoints = new List<Vector3>();

        foreach (Vector3 v in points)
        {
            projectedPoints.Add(diff * v);
        }

        for (int i = 0; i < answer.Count(); i++)
        {
            answer[i] = new Vector2(projectedPoints[i].x, projectedPoints[i].z);
        }

        float area = polygonArea(answer);
        if (area > 0)
        {
            //for (int i = 0; i < points.Count; i++)
            //{
            //    Vector3 current = points[i] + Vector3.up;
            //    Vector3 next = points[(i + 1) % points.Count] + Vector3.up;
            //    Debug.DrawLine(current, next, Color.yellow);
            //}

            normal = -normal;

            diff = Quaternion.FromToRotation(normal, Vector3.up);
            projectedPoints = new List<Vector3>();

            foreach (Vector3 v in points)
            {
                projectedPoints.Add(diff * v);
            }

            for (int i = 0; i < answer.Count(); i++)
            {
                answer[i] = new Vector2(projectedPoints[i].x, projectedPoints[i].z);
            }
        }

        return answer;
    }

    float polygonArea(Vector2[] vertices)
    {
        float area = 0;
        for (int i = 0; i < vertices.Count(); i++)
        {
            int j = (i + 1) % vertices.Count();
            area += vertices[i].x * vertices[j].y;
            area -= vertices[j].x * vertices[i].y;
        }
        return area / 2;
    }


    //This version takes the average of the points and makes it the center
    public void addNgon(List<Vector3> points)
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;
        foreach (Vector3 pos in points)
        {
            x += pos.x;
            y += pos.y;
            z += pos.z;
        }
        Vector3 center = new Vector3(x / points.Count, y / points.Count, z / points.Count);

        addNgon(center, points);
    }


    /*
     * makes a cube. First four points are clockwise on top, 2nd four points are clockwise on bottom.
     */
    public void addCube(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4,  //top four
        Vector3 p5, Vector3 p6, Vector3 p7, Vector3 p8)  //bottom four
    {
        addQuad(p1, p2, p3, p4);
        addQuad(p8, p7, p6, p5);
        addQuad(p2, p1, p5, p6);
        addQuad(p3, p2, p6, p7);
        addQuad(p4, p3, p7, p8);
        addQuad(p1, p4, p8, p5);

    }


    /*
     * takes 3 points and converts to a single triangle (clockwise faces up)
     */
    public void addTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        int oldSize = verts.Count;

        verts.Add(p1);
        verts.Add(p2);
        verts.Add(p3);

        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));

        tris.Add(oldSize);
        tris.Add(oldSize + 1);
        tris.Add(oldSize + 2);
    }
    public void addTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector2 u1, Vector2 u2, Vector2 u3)
    {
        int oldSize = verts.Count;

        verts.Add(p1);
        verts.Add(p2);
        verts.Add(p3);

        uvs.Add(u1);
        uvs.Add(u2);
        uvs.Add(u3);

        tris.Add(oldSize);
        tris.Add(oldSize + 1);
        tris.Add(oldSize + 2);
    }


    /*
     * takes 4 points and makes a 2-triangle quad (clockwise also faces up)
     */
    public void addQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        int oldSize = verts.Count;

        verts.Add(p1);
        verts.Add(p2);
        verts.Add(p3);
        verts.Add(p4);

        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));

        float xScale = Vector3.Distance(p1, p2);
        float yScale1 = Vector3.Distance(p2, p3) - xScale;
        float yScale2 = Vector3.Distance(p1, p4) - xScale;

        uvs.Add(new Vector2(1, 0 - yScale1));
        uvs.Add(new Vector2(0, 0 - yScale2));

        tris.Add(oldSize);
        tris.Add(oldSize + 1);
        tris.Add(oldSize + 2);

        tris.Add(oldSize);
        tris.Add(oldSize + 2);
        tris.Add(oldSize + 3);
    }


    /*
     * constructMesh() takes all of the data you've dumped into it and turns it into a standard unity Mesh
     */
    public Mesh constructMesh()
    {
        Mesh newMesh = new Mesh();

        newMesh.vertices = verts.ToArray();
        newMesh.triangles = tris.ToArray();
        newMesh.uv = uvs.ToArray();
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();
        newMesh.RecalculateTangents();

        return newMesh;
    }


    /*
     * adopt() takes data from the inputted mesh and copies it.
     */
    public void adopt(Mesh aMesh)
    {
        int offset = verts.Count;
        //verts = new List<Vector3>();
        //tris = new List<int>();

        for (int i = 0; i < aMesh.vertexCount; i++)
        {
            verts.Add(aMesh.vertices[i]);

            try
            {
                uvs.Add(aMesh.uv[i]);
            }
            catch (Exception e)
            {
                uvs.Add(new Vector2(0, 0));
            }
        }

        foreach (int i in aMesh.triangles)
        {
            tris.Add(i + offset);
        }

    }


    /*
    * adopt(theoreticalMesh) takes data from the inputted mesh interface and adds it to the existing model.
    */
    public void adopt(theoreticalMesh other)
    {
        int offset = verts.Count;

        for (int i = 0; i < other.verts.Count; i++)
        {
            verts.Add(other.verts[i]);
            uvs.Add(other.uvs[i]);
        }

        foreach(int i in other.tris)
        {
            if (i >= 0 && i < verts.Count)
            {
                tris.Add(i + offset);
            }
        }


    }

}

public static class Vector2Extension
{

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}

//public concave triangulation code courtesy of the Unify community, special thanks to user runevision
public class Triangulator
{
    private List<Vector2> m_points = new List<Vector2>();

    public Triangulator(Vector2[] points)
    {
        m_points = new List<Vector2>(points);
    }

    public int[] Triangulate()
    {
        List<int> indices = new List<int>();

        int n = m_points.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area() > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private float Area()
    {
        int n = m_points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = m_points[p];
            Vector2 qval = m_points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private bool Snip(int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector2 A = m_points[V[u]];
        Vector2 B = m_points[V[v]];
        Vector2 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector2 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}