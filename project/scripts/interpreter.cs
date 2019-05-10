using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class interpreter : MonoBehaviour
{
    public string interpretation;

    public Material wallMat;
    public Material roofMat;
    public Material windowMat;

    public Vector3[] outline;

    public InputField seedEntry;

    private builder maker;
    private int seed;

    private bool redraw = false;
    private List<Vector3> newOutline;
    LineRenderer line;

    QStateMachine designer;

    // Start is called before the first frame update
    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.enabled = false;

        designer = new QStateMachine(Random.Range(1, 50));
        
        designer.startState = "start";

        designer.addState("extrude");
        designer.addConnection("start", "extrude");

        designer.addConnection("start", "curved");
        designer.addConnection("curved", "extrude");

        designer.addConnection("extrude", "modify");

        designer.addConnection("modify", "shrink");
        designer.addConnection("modify", "grow");
        designer.addConnection("shrink", "extrude");
        designer.addConnection("grow", "extrude");
        designer.addConnection("extrude", "finished");

        seed = 0;

        maker = null;
        Make();
    }

    [ContextMenu("Make")]
    public void Make()
    {
        if (maker != null)
        {
            maker.deleteBuilding();
        }

        maker = new builder( outline.ToList<Vector3>());
        maker.randomizeValues(seed);

        designer.setSeed(seed);
        string traversal = designer.fullTraversal();
        Debug.Log(traversal);
        maker.interpret(traversal, seed);

        maker.setMaterials(wallMat, roofMat, windowMat);
        maker.makeBuilding();

        maker.updateBuilding();
    }

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

    public void startRedraw()
    {
        redraw = true;
        maker.deleteBuilding();
        newOutline = new List<Vector3>();
        line.enabled = true;
        line.positionCount = 0;
    }

    public void clickGround(Vector3 v)
    {
        if (redraw)
        {
            if (newOutline.Count > 3 && Vector3.Distance(newOutline[0], v) < 20)
            {
                outline = newOutline.ToArray();
                redraw = false;
                line.enabled = false;
                Make();
            }
            else
            {
                v.y = 0;
                newOutline.Add(v);

                line.positionCount = newOutline.Count();
                line.SetPositions(newOutline.ToArray());

            }
            
        }
    }
}
