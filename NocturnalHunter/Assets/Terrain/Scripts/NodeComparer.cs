using System.Collections.Generic;

public class NodeComparer : IComparer<RouteNodeID>
{
    /// <summary>
    /// Compare two RouteNodeID objects basing on their order number.
    /// </summary>
    /// <param name="x">First RouteNodeID object</param>
    /// <param name="y">Second RouteNodeID object</param>
    /// <returns>The order of x relative to y.</returns>
    public int Compare(RouteNodeID x, RouteNodeID y) {
        int xOrder = x.routeOrder, yOrder = y.routeOrder;

        if (xOrder < yOrder) return -1;
        else if (xOrder > yOrder) return 1;
        else return 0;
    }
}