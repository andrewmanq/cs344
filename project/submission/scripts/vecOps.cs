using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* vecOps.cs
 * 
 * This script is a maths-heavy utility script. Each function is used by builder.cs to manipulate
 * the shape of building facades.
 * 
 * author: Andrew Quist
 * date: 5/13/2019
 * 
 */

public static class vecOps
{
    //Does a quadratic curve operation on the corners of the outline, like those curvy line graph things
    //that kids like to draw on their notes
    public static List<Vector3> smoothBevel(List<Vector3> points, float resolution, float extent)
    {
        //points = pointMerge(points, 1);
        extent = Mathf.Clamp(extent, 0, .5f);

        List<Vector3> curvedPoints = new List<Vector3>();

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 previous;
            if (i == 0)
            {
                previous = points[points.Count - 1];
            }
            else
            {
                previous = points[(i - 1) % points.Count];
            }
            Vector3 current = points[i];
            Vector3 next = points[(i + 1) % points.Count];

            Vector3 start = Vector3.Lerp(current, previous, extent);
            Vector3 end = Vector3.Lerp(current, next, extent);

            float straightDist = Vector3.Distance(start, end);
            int res = Mathf.RoundToInt(straightDist * resolution) + 2;

            //curvedPoints.Add(start);
            //curvedPoints.Add(end);

            for (int j = 0; j < res; j++)
            {
                float interPol = (1f / res) * j;

                Vector3 l1 = Vector3.Lerp(start, current, interPol);
                Vector3 l2 = Vector3.Lerp(current, end, interPol);
                Vector3 curvePoint = Vector3.Lerp(l1, l2, interPol);
                curvedPoints.Add(curvePoint);
            }


        }

        return curvedPoints;
    }

    //merges points that get too close
    public static List<Vector3> pointMerge(List<Vector3> pointList, float distance)
    {
        List<Vector3> answer = pointList;

        int removeCount = 1;

        while (removeCount > 0)
        {
            removeCount = 0;

            for (int i = 0; i < answer.Count; i++)
            {

                Vector3 nextVec = answer[(i + 1) % answer.Count];
                Vector3 thisVec = answer[i];

                float currentDist = Vector3.Distance(thisVec, nextVec);

                if (currentDist < distance)
                {
                    answer.RemoveAt((i + 1) % answer.Count);

                    if (currentDist > 0.001f)
                    {
                        answer[i % answer.Count] = (thisVec + nextVec) / 2;
                    }
                    removeCount += 1;
                    //Debug.Log("point removed");
                }
            }
        }
        return answer;
    }

    //shrinks a given vector3 proportionally to outline
    public static List<Vector3> proportionalShrink(List<Vector3> points, float amount)
    {

        List<Vector3> answer = new List<Vector3>();
        int len = points.Count - 1;

        for (int i = 0; i < len + 1; i++)
        {
            Vector3 previous;
            Vector3 current = points[i];
            Vector3 next;

            if (i == 0)
            {
                previous = points[len];
            }
            else
            {
                previous = points[i - 1];
            }

            if (i == len)
            {
                next = points[0];
            }
            else
            {
                next = points[i + 1];
            }

            Quaternion direction1 = Quaternion.LookRotation(previous - current, Vector3.up);
            Quaternion direction2 = Quaternion.LookRotation(current - next, Vector3.up);

            direction1 = rotateQuat(direction1, new Vector3(0, -90, 0));
            direction2 = rotateQuat(direction2, new Vector3(0, -90, 0));

            Quaternion moveDir = Quaternion.Lerp(direction1, direction2, .5f);

            Vector3 deltaPos = moveDir * (Vector3.forward * amount);

            answer.Add(current + deltaPos);

        }

        return answer;
    }

    //rotates a quaternion by its euler angles using the delta rotation r
    private static Quaternion rotateQuat(Quaternion q, Vector3 r)
    {
        Quaternion answer = q;

        Vector3 rot = q.eulerAngles;
        rot = new Vector3(rot.x + r.x, rot.y + r.y, rot.z + r.z);
        answer = Quaternion.Euler(rot);

        return answer;
    }

    //grows a given vector3 proportionally to outline
    public static List<Vector3> proportionalGrow(List<Vector3> points, float amount)
    {

        List<Vector3> answer = new List<Vector3>();
        int len = points.Count - 1;

        for (int i = 0; i < len + 1; i++)
        {
            Vector3 previous;
            Vector3 current = points[i];
            Vector3 next;

            if (i == 0)
            {
                previous = points[len];
            }
            else
            {
                previous = points[i - 1];
            }

            if (i == len)
            {
                next = points[0];
            }
            else
            {
                next = points[i + 1];
            }

            Quaternion direction1 = Quaternion.LookRotation(previous - current, Vector3.up);
            Quaternion direction2 = Quaternion.LookRotation(current - next, Vector3.up);

            direction1 = rotateQuat(direction1, new Vector3(0, -90, 0));
            direction2 = rotateQuat(direction2, new Vector3(0, -90, 0));

            Quaternion moveDir = Quaternion.Lerp(direction1, direction2, .5f);

            Vector3 deltaPos = moveDir * (Vector3.forward * amount);

            answer.Add(current - deltaPos);

        }

        return answer;
    }

    //Adds a diagonal shear to corners
    public static List<Vector3> basicBevel(List<Vector3> points, float dist)
    {
        dist /= 2;

        List<Vector3> bevelPoints = new List<Vector3>();

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 previous;
            if (i == 0)
            {
                previous = points[points.Count - 1];
            }
            else
            {
                previous = points[(i - 1) % points.Count];
            }
            Vector3 current = points[i];
            Vector3 next = points[(i + 1) % points.Count];

            float extent1 = dist / (Vector3.Distance(current, previous) + .00001f);
            extent1 = Mathf.Clamp(extent1, 0, .5f);
            Vector3 start = Vector3.Lerp(current, previous, extent1);

            float extent2 = dist / (Vector3.Distance(current, next) + .00001f);
            extent2 = Mathf.Clamp(extent2, 0, .5f);
            Vector3 end = Vector3.Lerp(current, next, extent2);

            bevelPoints.Add(start);
            bevelPoints.Add(end);
        }

        return bevelPoints;
    }

    //                                                       ___v___
    //insets the largest side of a polygon, like this:  ____|       |_____
    //Parameter extent is a ratio: .5f insets the polygon by .5x its length
    public static List<Vector3> insetSide(List<Vector3> points, float extent)
    {
        extent = Mathf.Clamp(extent, .02f, 1f);

        var newPoints = points;

        var largestSide = findLargestSide(newPoints);

        Vector3 old1 = points[largestSide[0]];
        Vector3 old2 = points[largestSide[1]];

        Quaternion perpendicular = Quaternion.LookRotation(old1 - old2, Vector3.up);
        Quaternion parallell = rotateQuat(perpendicular, new Vector3(0, -90, 0));

        float dist = Vector3.Distance(old1, old2);
        Vector3 pushDist = parallell * (Vector3.forward * (dist * extent));

        var base1 = Vector3.Lerp(old1, old2, .25f);
        var base2 = Vector3.Lerp(old1, old2, .75f);

        var inset1 = base1 + pushDist;
        var inset2 = base2 + pushDist;

        newPoints.Insert(largestSide[1], base1);
        newPoints.Insert(largestSide[1] + 1, inset1);
        newPoints.Insert(largestSide[1] + 2, inset2);
        newPoints.Insert(largestSide[1] + 3, base2);

        return newPoints;
    }

    //                                                        _______
    //outsets the largest side of a polygon, like this:  ____|   ^   |_____
    //Parameter extent is a ratio: .5f insets the polygon by .5x its length
    public static List<Vector3> outsetSide(List<Vector3> points, float extent)
    {
        extent = Mathf.Clamp(extent, .02f, 1f);

        var newPoints = points;

        var largestSide = findLargestSide(newPoints);

        Vector3 old1 = points[largestSide[0]];
        Vector3 old2 = points[largestSide[1]];

        Quaternion perpendicular = Quaternion.LookRotation(old1 - old2, Vector3.up);
        Quaternion parallell = rotateQuat(perpendicular, new Vector3(0, 90, 0));

        float dist = Vector3.Distance(old1, old2);
        Vector3 pushDist = parallell * (Vector3.forward * (dist * extent));

        var base1 = Vector3.Lerp(old1, old2, .25f);
        var base2 = Vector3.Lerp(old1, old2, .75f);

        var inset1 = base1 + pushDist;
        var inset2 = base2 + pushDist;

        newPoints.Insert(largestSide[1], base1);
        newPoints.Insert(largestSide[1] + 1, inset1);
        newPoints.Insert(largestSide[1] + 2, inset2);
        newPoints.Insert(largestSide[1] + 3, base2);

        return newPoints;
    }

    //finds the largest side of a polygon
    public static int[] findLargestSide(List<Vector3> input)
    {
        float largest = 0;
        int[] answer = { 0, 0 };

        for (int i = 0; i < input.Count; i++)
        {
            int next = (i + 1) % input.Count;

            float dist = Vector3.Distance(input[i], input[next]);
            if (dist > largest)
            {
                answer[0] = i;
                answer[1] = next;

                largest = dist;
            }
        }

        return answer;
    }

    //checks that an outline does not intersect with itself. Useful for avoiding geometry issues
    public static bool noIntersections(List<Vector3> points)
    {
        int count = points.Count;

        for (int a = 0; a < count; a++)
        {
            int nextA = (a + 1) % count;

            for (int b = 0; b < count; b++)
            {
                int nextB = (b + 1) % count;

                if (a != b && a != nextB && nextA != nextB && nextA != b )
                {

                    var p1 = points[a];
                    var p2 = points[nextA];

                    var p3 = points[b];
                    var p4 = points[nextB];

                    if (LineSegmentsIntersection(p1, p2, p3, p4))
                    {
                        return false;
                    }
                }

            }
        }

        return true;
    }

    //returns true if a line(p1, p2) intersects with line(p3, p4). Writes the intersection point out to last parameter.
    private static bool LineSegmentsIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {

        var d = (p2.x - p1.x) * (p4.z - p3.z) - (p2.z - p1.z) * (p4.x - p3.x);

        if (d == 0.0f) { return false; }

        var u = ((p3.x - p1.x) * (p4.z - p3.z) - (p3.z - p1.z) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.z - p1.z) - (p3.z - p1.z) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        return true;
    }
}
