using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* StateMachines is a script containing abstract classes for implementing a custom state machine and graph search for a CSP to utilize.
 * author: Andrew Quist
 * 
 * note: all classes start with Q to prevent overlap with built-in unity functions
 */

public class StateMachines : MonoBehaviour
{
    public int seed = 1;

    private QStateMachine test;

    private void Start()
    {
        test =  new QStateMachine(seed);
        test.startState = "A";

        test.addConnection("A", "B");
        test.addConnection("A", "C");
        test.addConnection("C", "D");
        test.addConnection("B", "D");
        test.addConnection("D", "C");
        test.addConnection("D", "F");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            test.setSeed(seed);
            Debug.Log(test.fullTraversal());
        }
    }
} 

/* QStateMachine provides a dictionary of states that can be randomly traversed.
*  This is a directed graph approach.
*/
public class QStateMachine
{
    //This dict holds all QState objects used for traversal.
    public Dictionary<string, QState> states;
    //StartState points to the first state in the directed graph
    public string startState;

    //seed stores the original seed, while workingSeed changes each traversal.
    private int seed;
    private int workingSeed;

    public string currentState = "";

    public QStateMachine(int mySeed)
    {
        states = new Dictionary<string, QState>();
        seed = mySeed;
        workingSeed = seed;
    }

    public void setSeed(int newSeed)
    {
        seed = newSeed;
        workingSeed = seed;
    }

    public void addConnection(string from, string to)
    {
        if (!states.ContainsKey(from))
        {
            addState(from);
        }
        if (!states.ContainsKey(to))
        {
            addState(to);
        }

        states[from].addConnection(to);
    }

    public void addState(string id)
    {
        addState(id, new List<string>());
    }

    public void addState(string id, List<string> connections)
    {
        addState(id, connections, false);
    }

    public void addState(string id, List<string> connections, bool isStartState)
    {
        states.Add(id, new QState(connections));

        if (isStartState)
        {
            startState = id;
        }
    }

    public string fullTraversal()
    {
        string answer = "";
        setCurrentState("");

        string arg = "";
        while(arg != "end")
        {
            answer += " " + arg;
            arg = traverse();
        }

        return (" " + answer);
    }

    public string traverse()
    {
        if(currentState == "")
        {
            currentState = startState;
            return startState;
        }
        else
        {
            QState connections;

            if(states.TryGetValue(currentState, out connections))
            {
                if (connections.isDeadEnd())
                {
                    return "end";
                }

                string answ = connections.getRandomState(getNextSeed());
                currentState = answ;

                return answ;
            }
            else
            {
                return "end";
            }
        }
    }

    public bool setCurrentState(string statePos)
    {
        if (states.ContainsKey(statePos) || statePos == "")
        {
            currentState = statePos;
            return true;
        }
        else
        {
            return false;
        }
    }

    private int getNextSeed()
    {
        workingSeed += 1;
        return workingSeed;
    }
}

/* Qstate is a glorified struct that handles state info, and provides easy randomization for selecting the next state.
 * Contains a list of connections (other state IDs)
 */
public class QState
{
    //A list of names of states it's connected to
    private List<string> connections;

    public QState()
    {
        connections = new List<string>();
    }

    public QState(List<string> newConnections)
    {
        connections = newConnections;
    }

    //Connections are simply the IDs of the other states
    public void setConnections(List<string> myConnections)
    {
        connections = myConnections;
    }

    public void addConnection(string id)
    {
        connections.Add(id);
    }

    public List<string> getStates()
    {
        return connections;
    }

    public bool isDeadEnd()
    {
        return (connections.Count < 1);
    }

    //The seed is for getting reproducible results. It is reccomended to use a different seed for each search to maximize randomness
    public string getRandomState(int seed)
    {
        int range = connections.Count;

        if (range >= 0)
        {
            //sets the random seed
            Random.InitState(seed);
            int selection = Mathf.RoundToInt(Random.Range(0, range));
            return connections[selection];
        }
        else
        {
            return "";
        }

    }


}
