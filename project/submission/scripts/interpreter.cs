using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/* interpreter.cs
 * 
 * This script is a middle-man between the UI and the builder class.
 * Most functions are called by UI elements within the unity scene editor.
 * 
 * author: Andrew Quist
 * date: 5/13/2019
 * 
 */
public class interpreter : MonoBehaviour
{

    //materials of the walls, windows, and roofs
    public Material wallMat;
    public Material roofMat;
    public Material windowMat;

    //The outline given to the building maker
    public Vector3[] outline;

    //Where seed is taken from the user
    public InputField seedEntry;

    //The class that actually makes the building
    private builder maker;
    private int seed;

    //These will determine the stylistic constraints of the CSP later on
    private bool grow = true;
    private bool shrink = true;
    private bool inset = true;
    private bool outset = true;
    private bool curves = true;
    private bool bevel = true;

    //when redraw is true, the player will be able to redraw the building outline
    private bool redraw = false;
    private List<Vector3> newOutline;
    LineRenderer line;
    
    //called at the start of the program
    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.enabled = false;

        seed = 0;

        maker = null;
        Make();
    }

    //makes the building. Creates a new builder class after deleting the old one
    [ContextMenu("Make")]
    public void Make()
    {
        if (maker != null)
        {
            maker.deleteBuilding();
        }

        maker = new builder( outline.ToList<Vector3>());
        maker.randomizeValues(seed);

        maker.noGrow = !grow;
        maker.noShrink = !shrink;
        maker.noInset = !inset;
        maker.noOutset = !outset;
        maker.noCurves = !curves;
        maker.noBevel = !bevel;

        maker.designBuilding(seed);

        randomizeColor();

        maker.setMaterials(wallMat, roofMat, windowMat);
        maker.makeBuilding();

        maker.updateBuilding();
    }

    //sets the seed of the state machine and building generation
    public void SetSeed()
    {
        if (seedEntry != null)
        {
            string input = seedEntry.text;
            int newSeed;
            if (int.TryParse(input, out newSeed))
            {
                seed = newSeed;
            }
        }

        Debug.Log("seed changed to " + seed.ToString());
    }

    //randomizes the seed based on program counter value
    public void RandomizeSeed()
    {
        int newSeed = Mathf.RoundToInt(Time.time * 1000) % 1000;
        seed = newSeed;
        if (seedEntry != null)
        {
            seedEntry.text = newSeed.ToString();
        }

        Make();
    }

    //begins the process of redrawing outline, called by button
    public void startRedraw()
    {
        redraw = true;
        maker.deleteBuilding();
        newOutline = new List<Vector3>();
        line.enabled = false;
        line.positionCount = 0;
    }

    //called by the camera when the player clicks the ground
    public void clickGround(Vector3 v)
    {
        if (redraw)
        {
            if (newOutline.Count > 2 && Vector3.Distance(newOutline[0], v) < 10)
            {
                outline = newOutline.ToArray();
                redraw = false;
                //line.enabled = false;
                Make();
            }
            else
            {
                v.y = .3f;
                newOutline.Add(v);
                line.enabled = true;
                line.positionCount = newOutline.Count();
                line.SetPositions(newOutline.ToArray());

            }
            
        }
    }

    //randomizes the color of windows and walls, uses complimentary color scheming
    public void randomizeColor()
    {
        Color wallCol = Random.ColorHSV();
        Color windowCol = Random.ColorHSV();

        float h;
        float s;
        float v;
        Color.RGBToHSV(windowCol, out h, out s, out v);
        v = Mathf.Clamp(v, .6f, 1);
        s = Mathf.Clamp(s, .1f, .4f);
        windowCol = Color.HSVToRGB(h, s, v);

        float newh;
        Color.RGBToHSV(wallCol, out newh, out s, out v);
        s = Mathf.Clamp(s,0f, .2f);
        h += .5f;
        h %= 1f;
        wallCol = Color.HSVToRGB(h, s, v);

        wallMat.SetColor("Color_AE56ABDC", wallCol);
        windowMat.SetColor("Color_AE56ABDC", wallCol);

        wallMat.SetColor("Color_F2D77008", windowCol);
        windowMat.SetColor("Color_F2D77008", wallCol);
    }


    //All remaining functions are called by UI buttons to enable/disable certain operations
    public void setGrow(bool value)
    {
        grow = value;
    }

    public void setShrink(bool value)
    {
        shrink = value;
    }

    public void setInset(bool value)
    {
        inset = value;
    }

    public void setOutset(bool value)
    {
        outset = value;
    }

    public void setCurves(bool value)
    {
        curves = value;
    }

    public void setBevel(bool value)
    {
        bevel = value;
    }
}
