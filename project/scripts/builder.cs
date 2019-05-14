﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static vecOps;

public class builder
{
    //holds the wall mesh
    theoreticalMesh walls;
    //holds the roof mesh
    theoreticalMesh roofs;
    //holds a secondary wall mesh for varience
    //(originally with more windows hence the name)
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
    //The amount of growth/shrinkage a building goes through
    public float sizeChange = 3;
    //how high a building can extrude
    public float heightChange = 15;

    //Limit of extrusions
    public int complexity = 5;
    private int complexCount = 0;

    //If any of these are enabled, their corresponding operations
    //are banned by the CSP. This allows for architectural consistency among
    //buildings
    public bool noGrow = false;
    public bool noShrink = false;
    public bool noInset = false;
    public bool noOutset = false;
    public bool noCurves = false;
    public bool noBevel = false;

    public bool curved = false;
    public float curveAmount = 0;

    public bool bevel = false;
    public float bevelAmount = 0;

    private QStateMachine designer;

    //All builders start off with an original parcel outline
    public builder(List<Vector3> parcel)
    {
        myParcel = parcel;
        workingOutline = myParcel;

        walls = new theoreticalMesh();
        roofs = new theoreticalMesh();
        windows = new theoreticalMesh();

        initializeStates();
    }

    //This function creates the state machine. A diagram of these states
    //can be found in the project folder github page
    public void initializeStates()
    {
        designer = new QStateMachine(0);

        //start is ignored by interpreter. Used only for branching to modify or extrude
        designer.startState = "start";

        designer.addConnection("start", "extrude");
        designer.addConnection("start", "modify");

        designer.addConnection("extrude", "modify");

        designer.addConnection("modify", "curved");
        designer.addConnection("modify", "shrink");
        designer.addConnection("modify", "grow");
        designer.addConnection("modify", "outset");
        designer.addConnection("modify", "inset");
        designer.addConnection("modify", "bevel");

        designer.addConnection("curved", "extrude");
        designer.addConnection("shrink", "extrude");
        designer.addConnection("grow", "extrude");
        designer.addConnection("inset", "extrude");
        designer.addConnection("outset", "extrude");
        designer.addConnection("bevel", "extrude");

        designer.addConnection("curved", "modify");
        designer.addConnection("shrink", "modify");
        designer.addConnection("grow", "modify");
        designer.addConnection("outset", "modify");
        designer.addConnection("inset", "modify");
        designer.addConnection("bevel", "modify");

        designer.addConnection("modify", "extrude");
        designer.addConnection("extrude", "extrude");
        designer.addConnection("extrude", "finished");
    }

    //Uses the state machine to feed instructions to the interpret() function
    public void designBuilding(int seed)
    {
        designer.setSeed(seed);

        string rejection = "";

        while (rejection != "done")
        {
            string traversal = designer.fullTraversal(rejection);
            rejection = interpret(traversal, seed);
        }
    }

    //randomizes some sandard globals to make buildings more unique
    public void randomizeValues(int seed)
    {
        Random.InitState(seed);

        sizeChange = Random.Range(2, 20);
        heightChange = Random.Range(2, 20);
        complexity = Random.Range(10, 30);
    }

    //takes a string with instructions to make the building
    public string interpret(string input, int heightSeed)
    {

        var words = input.Split(' ');
        string rejection = "done";

        Random.InitState(heightSeed);

        foreach (string s in words)
        {
            switch (s)
            {

                case "bevel":
                    if (noBevel || bevel || curved)
                    {
                        return rejection;
                    }
                    else
                    {
                        addBevel();
                    }
                    break;

                case "curved":
                    if (noCurves || curved)
                    {
                        return rejection;
                    }
                    bevel = false;
                    curve();
                    break;

                case "inset":
                    if(!noInset && inset(.2f))
                    {
                        break;
                    }
                    else
                    {
                        return rejection;
                    }

                case "outset":
                    if (!noOutset && complexCount > 0 && outset(.2f))
                    {
                        break;
                    }
                    else
                    {
                        return rejection;
                    }

                case "extrude":
                    extrudeUp(Random.Range(2, heightChange) * storyHeight);
                    if(complexCount > complexity)
                    {
                        return "done";
                    }
                    break;

                case "shrink":

                    if (!noShrink && shrink(sizeChange))
                    {
                        break;
                    }
                    else
                    {
                        return rejection;
                    }

                case "grow":
                    if (!noGrow)
                    {
                        if (complexCount > 0 && grow(sizeChange))
                        {
                            break;
                        }
                        else
                        {
                            return rejection;
                        }
                    }
                    else
                    {
                        return rejection;
                    }
            }

            rejection = s;
        }

        return "done";

    }

    //takes the working outline and makes a wall out of it
    public void extrudeUp(float height)
    {
        //assigns new working outline if not made
        if (workingOutline == null)
        {
            workingOutline = myParcel;
        }

        List<Vector3> newOutline = new List<Vector3>();
        List<Vector3> oldOutline = workingOutline;
        oldOutline.Reverse();

        //moves up working outline for the next modification cycle
        foreach (Vector3 v in oldOutline)
        {
            Vector3 upperPoint = v + Vector3.up * height;
            newOutline.Add(upperPoint);
        }
        newOutline.Reverse();
        workingOutline = newOutline;

        //Curves outline if curved is enabled
        if (curved)
        {
            oldOutline = pointMerge(oldOutline, 4);
            oldOutline = smoothBevel(oldOutline, .1f, curveAmount);
            oldOutline = pointMerge(oldOutline, .05f);
        }else if (bevel) //bevels outline if bevel is enabled
        {
            oldOutline = basicBevel(oldOutline, bevelAmount);
            Debug.Log("beveled");
        }

        //wallPoints will be used for mesh data
        List<Vector3> wallPoints = new List<Vector3>();
        newOutline = new List<Vector3>();

        foreach(Vector3 v in oldOutline)
        {
            Vector3 upperPoint = v + Vector3.up * height;
            newOutline.Add(upperPoint);

            wallPoints.Add(upperPoint);
            wallPoints.Add(v);
        }
        
        Vector3 lastV = oldOutline[0];
        Vector3 lastUpperPoint = lastV + Vector3.up * height;

        wallPoints.Add(lastUpperPoint);
        wallPoints.Add(lastV);
        
        //a simple way to introduce variance: if section is smaller than arbitrary height,
        //make it a different type of face
        if (height > 40)
        {
            //creates wall geometry
            walls.addRibbon(wallPoints, 20f, false, height);
        }
        else
        {
            //creates secondary wall geometry
            windows.addRibbon(wallPoints, 20f, false, height);
        }

        newOutline.Reverse();

        //creates roof geometry
        roofs.addSmartNgon(newOutline);
        roofs.addSmartNgon(oldOutline);

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
        //creates empty game objects, destroys old ones
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

        //Applies mesh renderers and materials
        if (wallMat != null && roofMat != null && windowMat != null)
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

        //Uses my custom mesh creation tools to make the meshes
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

    //adds square inset to working outline. Returns true if there were no self-intersections
    public bool inset(float amount)
    {
        var newOutline = insetSide(workingOutline, amount);

        if (noIntersections(newOutline))
        {
            workingOutline = newOutline;
            return true;
        }
        else
        {
            return false;
        }
    }

    //adds square outset to the side of the outline. Returns true if no self-intersections
    public bool outset(float amount)
    {
        var newOutline = outsetSide(workingOutline, amount);

        if (noIntersections(newOutline))
        {
            workingOutline = newOutline;
            return true;
        }
        else
        {
            return false;
        }
    }

    //shrinks the working outline. Returns true if shrink was successful (no overlapping lines)
    public bool shrink(float amount)
    {
        var newOutline = proportionalShrink(workingOutline, amount);
        newOutline = pointMerge(newOutline, 3);

        if (noIntersections(newOutline))
        {
            workingOutline = newOutline;
            return true;
        }
        else
        {
            return false;
        }
    }

    //Returns true if grown outline does not overlap itself
    public bool grow(float amount)
    {
        var newOutline = proportionalGrow(workingOutline, amount);
        newOutline = pointMerge(newOutline, 1);

        if (noIntersections(newOutline))
        {
            workingOutline = newOutline;
            return true;
        }
        else
        {
            return false;
        }
    }

    //Curves the building. A building cannot be uncurved.
    public void curve()
    {
        if (!curved)
        {
            curved = true;
            curveAmount = Random.Range(.1f, .5f);
        }
    }

    //Bevels the building. A building can only be unbeveled if curve is enabled
    public void addBevel()
    {
        if (!bevel)
        {
            bevel = true;
            bevelAmount = Random.Range(4f, 40f);
        }
    }
}
