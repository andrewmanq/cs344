using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class vecOps
{
    //Does a quadratic curve operation on the corners of the outline
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

                if (Vector3.Distance(thisVec, nextVec) < distance)
                {
                    answer.RemoveAt(i);
                    answer[i] = (thisVec + nextVec) / 2;
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
}
