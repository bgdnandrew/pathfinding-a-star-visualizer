using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPathRRT : MonoBehaviour
{
    public Maze maze;
    public Material closedMaterial;
    public GameObject start;
    public GameObject end;
    public GameObject pathP;

    private PathMarker startNode;
    private PathMarker goalNode;
    private bool done = false;
    private bool hasStarted = false;
    private List<PathMarker> tree = new List<PathMarker>();

    void RemoveAllMarkers()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
        foreach (GameObject m in markers) Destroy(m);
    }

    void BeginSearch()
    {
        Debug.Log("BeginSearch called");

        done = false;
        RemoveAllMarkers();

        List<MapLocation> locations = new List<MapLocation>();

        for (int z = 1; z < maze.depth - 1; ++z)
        {
            for (int x = 1; x < maze.width - 1; ++x)
            {
                if (maze.map[x, z] != 1)
                {
                    locations.Add(new MapLocation(x, z));
                }
            }
        }
        locations.Shuffle();

        Vector3 startLocation = new Vector3(locations[0].x * maze.scale, 0.0f, locations[0].z * maze.scale);
        startNode = new PathMarker(new MapLocation(locations[0].x, locations[0].z),
            0.0f, 0.0f, 0.0f, Instantiate(start, startLocation, Quaternion.identity), null);

        Vector3 endLocation = new Vector3(locations[1].x * maze.scale, 0.0f, locations[1].z * maze.scale);
        goalNode = new PathMarker(new MapLocation(locations[1].x, locations[1].z),
            0.0f, 0.0f, 0.0f, Instantiate(end, endLocation, Quaternion.identity), null);

        tree.Clear();
        tree.Add(startNode);
    }

    void Search()
    {
        if (done) return;

        MapLocation rand = new MapLocation(Random.Range(1, maze.width - 1), Random.Range(1, maze.depth - 1));
        PathMarker nearest = null;
        float minDist = float.MaxValue;

        foreach (PathMarker p in tree)
        {
            float dist = Vector2.Distance(p.location.ToVector(), rand.ToVector());
            if (dist < minDist)
            {
                minDist = dist;
                nearest = p;
            }
        }

        if (nearest == null) return;

        MapLocation direction = new MapLocation(rand.x - nearest.location.x, rand.z - nearest.location.z);
        MapLocation newLocation = new MapLocation(nearest.location.x + (int)Mathf.Sign(direction.x), nearest.location.z + (int)Mathf.Sign(direction.z));

        if (maze.map[newLocation.x, newLocation.z] == 1) return;

        float g = nearest.G + Vector2.Distance(nearest.location.ToVector(), newLocation.ToVector());
        float h = Vector2.Distance(newLocation.ToVector(), goalNode.location.ToVector());
        float f = g + h;

        GameObject pathBlock = Instantiate(pathP, new Vector3(newLocation.x * maze.scale, 0.0f, newLocation.z * maze.scale), Quaternion.identity);

        TextMesh[] values = pathBlock.GetComponentsInChildren<TextMesh>();

        values[0].text = "G: " + g.ToString("0.00");
        values[1].text = "H: " + h.ToString("0.00");
        values[2].text = "F: " + f.ToString("0.00");

        PathMarker newMarker = new PathMarker(newLocation, g, h, f, pathBlock, nearest);
        tree.Add(newMarker);

        if (newMarker.Equals(goalNode))
        {
            done = true;
            goalNode.parent = newMarker;
            Debug.Log("DONE!");
        }
    }

    void GetPath()
    {
        RemoveAllMarkers();
        PathMarker begin = goalNode;

        while (!startNode.Equals(begin) && begin != null)
        {
            Instantiate(pathP, new Vector3(begin.location.x * maze.scale, 0, begin.location.z * maze.scale), Quaternion.identity);
            begin = begin.parent;
        }

        Instantiate(pathP, new Vector3(startNode.location.x * maze.scale, 0, startNode.location.z * maze.scale), Quaternion.identity);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            BeginSearch();
            hasStarted = true;
        }

        if (hasStarted && !done)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Search();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                GetPath();
            }
        }
    }
}
