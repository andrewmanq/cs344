using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static vecOps;

public class builder
{
    //holds the wall mesh
    theoreticalMesh walls;
    //holds the roof mesh
    theoreticalMesh roofs;

    theoreticalMesh windows;

    //The parcel (AKA outline) of the building. This building must not exceed the parcel.
    List<Vector3> myParcel;

    //The outline of the current extrusion of the building.
    List<Vector3> workingOutline;

    //materials for the walls and roofs
    Material wallMat;
    Material roofMat;
    Material windowMat;

    //GameObjects for storing building mesh data
    private GameObject wallObj;
    private GameObject windowObj;
    private GameObject roofObj;

    //height of a story of the building
    public float storyHeight = 10;

    public float sizeChange = 3;

    public float heightChange = 15;

    public int complexity = 5;
    private int complexCount = 0;

    public bool noGrow = false;

    //All builders start off with an original parcel outline
    public builder(List<Vector3> parcel)
    {
        myParcel = parcel;
        workingOutline = myParcel;

        walls = new theoreticalMesh();
        roofs = new theoreticalMesh();
        windows = new theoreticalMesh();
    }

    public void randomizeValues(int seed)
    {
        Random.InitState(seed);

        sizeChange = Random.Range(2, 9);
        heightChange = Random.Range(2, 40);
        complexity = Random.Range(1, 5);

        noGrow = Random.Range(0, 10) > 5;
    }

    //takes a string with instructions to make the building
    public void interpret(string input, int heightSeed)
    {
        clearData();

        var words = input.Split(' ');

        Random.InitState(heightSeed);

        foreach (string s in words)
        {
            switch (s)
            {
                case "curved":
                    curve();
                    break;

                case "extrude":
                    extrudeUp(Random.Range(2, heightChange) * storyHeight);

                    if(complexCount > complexity)
                    {
                        return;
                    }
                    break;

                case "shrink":
                    shrink(sizeChange);
                    break;

                case "grow":
                    if (!noGrow)
                    {
                        grow(sizeChange);
                    }
                    else
                    {
                        shrink(sizeChange);
                    }
                    break;
            }
        }

    }

    //takes the working outline and makes a wall out of it
    public void extrudeUp(float height)
    {
        if(workingOutline == null)
        {
            workingOutline = myParcel;
        }

        List<Vector3> newOutline = new List<Vector3>();
        List<Vector3> oldOutline = workingOutline;

        List<Vector3> wallPoints = new List<Vector3>();

        foreach(Vector3 v in workingOutline)
        {
            Vector3 upperPoint = v + Vector3.up * height;
            newOutline.Add(upperPoint);

            wallPoints.Add(v);
            wallPoints.Add(upperPoint);
        }
        //closes the loop
        Vector3 lastV = workingOutline[0];
        Vector3 lastUpperPoint = lastV + Vector3.up * height;
        wallPoints.Add(lastV);
        wallPoints.Add(lastUpperPoint);

        workingOutline = newOutline;

        if (height > 40)
        {
            walls.addRibbon(wallPoints, 20f, false, height);
        }
        else
        {
            windows.addRibbon(wallPoints, 20f, false, height);
        }
        

        oldOutline.Reverse();
        roofs.addSmartNgon(oldOutline);

        roofs.addSmartNgon(newOutline);

        complexCount += 1;
    }

    //sets wall and roof materials
    public void setMaterials(Material forWalls, Material forRoofs, Material forWindows)
    {
        wallMat = forWalls;
        roofMat = forRoofs;
        windowMat = forWindows;
    }

    //Makes a game object with determined meshes and materials (make sure to use setMaterials()!)
    public GameObject[] makeBuilding()
    {
        if (wallObj != null)
        {
            GameObject.Destroy(wallObj);
            wallObj = null;
        }
        if (roofObj != null)
        {
            GameObject.Destroy(roofObj);
            roofObj = null;
        }

        if (windowObj != null)
        {
            GameObject.Destroy(windowObj);
            windowObj = null;
        }

        wallObj = GameObject.Instantiate(new GameObject());
        roofObj = GameObject.Instantiate(new GameObject());
        windowObj = GameObject.Instantiate(new GameObject());

        if (wallMat != null && roofMat != null)
        {
            wallObj.AddComponent<MeshRenderer>().material = wallMat;
            roofObj.AddComponent<MeshRenderer>().material = roofMat;
            windowObj.AddComponent<MeshRenderer>().material = windowMat;
        }
        else
        {
            wallObj.AddComponent<MeshRenderer>();
            roofObj.AddComponent<MeshRenderer>();
            windowObj.AddComponent<MeshRenderer>();
        }

        wallObj.AddComponent<MeshFilter>().mesh = walls.constructMesh();
        roofObj.AddComponent<MeshFilter>().mesh = roofs.constructMesh();
        windowObj.AddComponent<MeshFilter>().mesh = windows.constructMesh();

        GameObject[] answer = new GameObject[2];
        answer[0] = wallObj;
        answer[1] = roofObj;

        return answer;
    }

    public void updateBuilding()
    {
        deleteBuilding();
        makeBuilding();
    }

    //deletes the game representation of the building
    public void deleteBuilding()
    {
        if (wallObj != null && roofObj != null && windowObj != null)
        {
            GameObject.Destroy(wallObj, .01f);
            GameObject.Destroy(roofObj, .01f);
            GameObject.Destroy(windowObj, .01f);
        }
    }
    //Clears all structure data to start anew
    public void clearData()
    {
        workingOutline = myParcel;
        walls = new theoreticalMesh();
        roofs = new theoreticalMesh();
        complexCount = 0;
    }

    //shrinks the working outline
    public void shrink(float amount)
    {
        workingOutline = proportionalShrink(workingOutline, amount);
        workingOutline = pointMerge(workingOutline, 3);
    }

    public void grow(float amount)
    {
        workingOutline = proportionalGrow(workingOutline, amount);
    }

    public void curve()
    {
        workingOutline = pointMerge(workingOutline, 5);
        workingOutline = smoothBevel(workingOutline, 1, Random.Range(.1f, .5f));
        workingOutline = pointMerge(workingOutline, .05f);
    }

    //shrinks a given vector3
    private List<Vector3> pointGrow(List<Vector3> points, float amount)
    {
        var avg = Vector3.zero;

        foreach (Vector3 v in points)
        {
            avg += v;
        }
        avg /= points.Count;

        List<Vector3> answer = new List<Vector3>();
        foreach (Vector3 v in points)
        {
            var dist = avg - v;
            dist *= amount;
            answer.Add(v - dist);
        }
        return answer;
    }
}
