using System.Collections.Generic;
using UnityEngine;

public class RouteManager : MonoBehaviour
{
    private IDictionary<char, List<RouteNodeID>> routeMap;
    private GameObject[] childObjects;
    private NodeComparer comparer;

    private void Start() {
        this.routeMap = new Dictionary<char, List<RouteNodeID>>();
        this.comparer = new NodeComparer();

        //init child array
        this.childObjects = new GameObject[transform.childCount];
        for (int i = 0; i < childObjects.Length; i++)
            childObjects[i] = transform.GetChild(i).gameObject;

        //init and sort route map
        InitMap(childObjects);
        foreach (char path in routeMap.Keys)
            routeMap[path].Sort(comparer);
    }

    /// <summary>
    /// Initiate the map of paths.
    /// </summary>
    /// <param name="childArr">Array of this object's children</param>
    private void InitMap(GameObject[] childArr) {
        foreach (GameObject child in childArr) {
            //try to get the child's RouteNodeID component
            RouteNodeID nodeID;
            try { nodeID = child.GetComponent<RouteNodeID>(); }
            catch (MissingComponentException) { continue; }

            char path = nodeID.routePath;

            //add a new list for the current path if it hasn't already been added
            if (!routeMap.ContainsKey(path))
                routeMap.Add(path, new List<RouteNodeID>());

            //add the RouteNodeID component to the correct list
            routeMap[path].Add(nodeID);
        }
    }

    /// <summary>
    /// Get the closest node on a path of the route to a given position.
    /// </summary>
    /// <param name="position">The object's position</param>
    /// <param name="path">The specific path that's checked</param>
    /// <param name="nodeDistance">The distance from the nearest route node</param>
    /// <returns>The closest node of the route in the specified path.</returns>
    private RouteNodeID NearestRoute(Vector3 position, char path, out float nodeDistance) {
        float bestDistance = Mathf.Infinity;
        RouteNodeID bestCase = null;

        for (int i = 0; i < routeMap[path].Count; i++) {
            Vector3 routePoint = routeMap[path][i].Point;
            float distance = Vector3.Distance(position, routePoint);

            //found good point
            if (distance < bestDistance) {
                bestDistance = distance;
                bestCase = routeMap[path][i];
            }
        }

        nodeDistance = bestDistance;
        return bestCase;
    }

    /// <summary>
    /// Get the closest node of the route to a given position.
    /// </summary>
    /// <param name="position">The object's position</param>
    /// <param name="nodeDistance">The distance from the nearest route node</param>
    /// <returns>The closest node of the route.</returns>
    public RouteNodeID NearestRoute(Vector3 position) {
        float bestDistance = Mathf.Infinity;
        RouteNodeID nearest = null;

        foreach (char path in routeMap.Keys) {
            RouteNodeID potential = NearestRoute(position, path, out float potentialDist);

            if (potentialDist < bestDistance) {
                bestDistance = potentialDist;
                nearest = potential;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Get the next node on the route.
    /// If the current node is a junction, there's a chance to randomly change the path.
    /// </summary>
    /// <param name="position">The object's current position</param>
    /// <returns>The next node on the route.</returns>
    public RouteNodeID NextNode(Vector3 position) {
        RouteNodeID currentNode = NearestRoute(position);

        List<char> junctionPaths = new List<char> { currentNode.routePath }; //add current
        foreach (char alternativePath in currentNode.junctionOf) //add all others
            junctionPaths.Add(alternativePath);

        int pathIndex = Random.Range(0, junctionPaths.Count);
        char path = junctionPaths[pathIndex];

        //change path
        if (pathIndex != 0) return NearestRoute(position, path, out _);
        //stay on same path - get next
        else {
            for (int i = 0; i < routeMap[path].Count; i++) {
                RouteNodeID node = routeMap[path][i];

                if (node == currentNode) {
                    if (i < routeMap[path].Count - 1) return routeMap[path][i + 1];
                    else return routeMap[path][0];
                }
            }

            return currentNode; //formal return statement
        }
    }
}