using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
namespace Entities.AI
{
    /// <summary>
    /// Representation of an area of space in a map.  
    /// These compose a nav mesh or a path through a nav mesh.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class PolygonNode
    {
        public readonly Rectangle rectangle;
        private readonly List<PolygonNode> adjacentNodes;
        public readonly int weight;
        public PolygonNode parent { get; set; }
        public Color debugColor{ get; set; }
        private static Random rand = new Random(DateTime.Now.Millisecond);
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rect"></param>
        public PolygonNode(Rectangle rect)
        {
            rectangle = rect;
            weight = (int)Math.Sqrt((rect.Width * rect.Width + rect.Height * rect.Height));
            adjacentNodes = new List<PolygonNode>();
            debugColor = new Color(rand.Next()%255,rand.Next()%255,rand.Next()%255);
        }

        /// <summary>
        /// Link this node with all of the given nodes.
        /// </summary>
        /// <param name="listOfPolygonLists">List of adjacent nodes</param>
        public void initNode(List<List<PolygonNode>> listOfPolygonLists)
        {
             foreach (List<PolygonNode> listOfPolygons in listOfPolygonLists)
             {
                 foreach(PolygonNode node in listOfPolygons)
                 {
                     if (areTheseAdjacent(this, node))
                         linkPolygons(this,node);
                 }
             }
        }

        /// <summary>
        /// Removes an adjacent node from this node.
        /// </summary>
        /// <param name="pn">Node to remove</param>
        /// <returns>True if it was in the list, false otherwise.</returns>
        public bool removeNode(PolygonNode pn)
        {
            return adjacentNodes.Remove(pn);
        }

        /// <summary>
        /// Adds an adjacent node from this node.
        /// </summary>
        /// <param name="pn">Node to add</param>
        /// <returns>True</returns>
        public bool addNode(PolygonNode pn)
        {
            adjacentNodes.Add(pn);
            return true;
        }

        /// <summary>
        /// Get adjacent nodes as array.
        /// </summary>
        /// <returns>PolygonNode[]</returns>
        public PolygonNode[] getAdjacentNodes()
        {
            return adjacentNodes.ToArray();
        }

        /// <summary>
        /// Check if Point is in the polygon.
        /// </summary>
        /// <param name="p">Point to check.</param>
        /// <returns>True if inside polygon.</returns>
        public bool inPolygon(Point p)
        {
            return rectangle.Contains(p);
        }

        /// <summary>
        /// Check if Vector is in the polygon.
        /// </summary>
        /// <param name="v">Vector to check</param>
        /// <returns>True if inside polygon.</returns>
        public bool inPolygon(Vector2 v)
        {
            Point p = new Point((int)v.X, (int)v.Y);
            return rectangle.Contains(p);
        }


        /// <summary>
        /// Checks if the rectangle overlaps this polygon
        /// </summary>
        /// <param name="r">Rectangle to check</param>
        /// <returns>True if they intersect.</returns>
        public bool inPolygon(Rectangle r)
        {
            return rectangle.Intersects(r);
        }

        /// <summary>
        /// Hashcode function.
        /// </summary>
        /// <returns>Hashcode for this PolygonNode</returns>
        public override int GetHashCode()
        {
            int hashCode = 0;
            hashCode += rectangle.Width * rectangle.Height;
            hashCode += rectangle.Y * rectangle.Width;
            hashCode += rectangle.X * rectangle.Height;
            return hashCode;
        }

        /// <summary>
        /// Adds each node to the other's adjacency list.
        /// </summary>
        /// <param name="p1">First node.</param>
        /// <param name="p2">Second node.</param>
        public static void linkPolygons(PolygonNode p1, PolygonNode p2)
        {
            if (p1 == p2)
                throw new Exception("A polygon can't be adjacent to itself.");
            if(!p1.adjacentNodes.Contains(p2))
                p1.addNode(p2);
            if (!p2.adjacentNodes.Contains(p1))
                p2.addNode(p1);
        }

        /// <summary>
        /// Check if two PolygonNodes are adjacent.
        /// </summary>
        /// <param name="p1">First Node</param>
        /// <param name="p2">Second Node</param>
        /// <returns>True if they are next to each other.</returns>
        public static bool areTheseAdjacent(PolygonNode p1, PolygonNode p2)
        {
            Rectangle r1 = p1.rectangle;
            Rectangle r2 = p2.rectangle;

            r1.Inflate(1, 0);
            if (r1.Intersects(r2))
                return true;
            r1.Inflate(-1, 1);
            if (r1.Intersects(r2))
                return true;
            r1.Inflate(0,-1);
            return false;
        }

        /// <summary>
        /// This is not really working it should be the point
        /// that one should pass through between to polygons.
        /// </summary>
        /// <param name="pn1">First Polygon</param>
        /// <param name="pn2">Second Polygon</param>
        /// <returns>the sweet spot</returns>
        public static Point getConnectionPoint(PolygonNode pn1, PolygonNode pn2)
        {
            if(pn1 == pn2)
                throw new Exception("A node cannot be connected to itself.");
            if(!pn1.adjacentNodes.Contains(pn2))
                throw new Exception("These nodes are not adjacent and therefore have no connection point.");
            Rectangle r1 = pn1.rectangle;
            Rectangle r2 = pn2.rectangle;
            int x = -10;
            int y = -10;
            if (r1.Top == r2.Bottom)
                y = r1.Top;
            if (r1.Bottom == r2.Top)
                y = r1.Bottom;
            if (r1.Left == r2.Right)
                x = r1.Left;
            if (r1.Right == r2.Left)
                x = r1.Right;

            if (x == -10)
            {
                int n = (Math.Max(r1.Width, r2.Width) - Math.Abs(r1.X - r2.X));
                x = n / 2 + Math.Max(r1.X, r2.X);
            }

            if (y == -10)
            {
                int n = (Math.Max(r1.Height, r2.Height) - Math.Abs(r1.Y - r2.Y));
                y = n / 2 + Math.Max(r1.Y, r2.Y);
            }

            return new Point(x,y);
        }
    }
}
