using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Entities.World;
namespace Entities.AI
{
    /// <summary>
    /// Navigation Mesh Management. 
    /// Add obstacles.
    /// Get Paths.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class NavMeshManager
    {
        private readonly List<PolygonNode> nodeList;
        private static Texture2D tex;
        private readonly Game game;
        private readonly GameWorld gw;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        /// <param name="g">Game Reference</param>
        /// <param name="_gw">GameWorld Reference</param>
        public NavMeshManager(int width, int height, Game g)
        {
            nodeList = new List<PolygonNode>();
            PolygonNode root = new PolygonNode(new Rectangle(0, 0, width, height));
            nodeList.Add(root);
            game = g;
        }

        /// <summary>
        /// Draw each polygon with its assigned color.
        /// </summary>
        /// <param name="sb">SpriteBatch</param>
        public void DebugDraw(SpriteBatch sb)
        {
            foreach(PolygonNode pn in nodeList)
            {
                Rectangle r = new Rectangle(pn.rectangle.X, pn.rectangle.Y, pn.rectangle.Width, pn.rectangle.Height);
                r.X -= Singletons.EntityManager.getEntityManager(game).getCurrentGameWorld().viewport.X;//gw.viewport.X;
                r.Y -= Singletons.EntityManager.getEntityManager(game).getCurrentGameWorld().viewport.Y;//gw.viewport.X;
               sb.Draw(tex,r,pn.debugColor);   
            }
        }

        /// <summary>
        /// Add a new unwalkable rectangle
        /// </summary>
        /// <param name="rect">Rectangle to block.</param>
        public void addRectangle(Rectangle rect)
        {
            tex = game.Content.Load<Texture2D>("White");
            PolygonNode[] overlaps = getOverlappingNodes(rect);
            List<PolygonNode> newNodes = new List<PolygonNode>();
            List<Point> vertices = new List<Point>();
            addVertex(vertices, new Point(rect.Left, rect.Top));
            addVertex(vertices, new Point(rect.Left, rect.Bottom));
            addVertex(vertices, new Point(rect.Right, rect.Top));
            addVertex(vertices, new Point(rect.Right, rect.Bottom));
            foreach (PolygonNode pn in overlaps)
            {
                Rectangle oldRect = pn.rectangle;
                addVertex(vertices, new Point(oldRect.Left, oldRect.Top));
                addVertex(vertices, new Point(oldRect.Left, oldRect.Bottom));
                addVertex(vertices, new Point(oldRect.Right, oldRect.Top));
                addVertex(vertices, new Point(oldRect.Right, oldRect.Bottom));
                addOverlapVerts(vertices,oldRect,rect);
            }

            Point badPoint = new Point(-1,-1);
            foreach (Point origin in vertices)
            {
                Point left = getClosestLeft(origin, vertices);
                Point bottom = getClosestBottom(origin, vertices);
                if (left.Equals(badPoint) || bottom.Equals(badPoint))
                    continue;
                int width = left.X - origin.X;
                int height = bottom.Y - origin.Y;
                Rectangle newRect = new Rectangle(origin.X, origin.Y,width,height);
                if (rect.Contains(newRect.Center))
                    continue;
                if(!newRect.Equals(rect))
                {
                    PolygonNode newPoly = new PolygonNode(newRect);
                   // newPoly.rectangle.Inflate(1,1);
                    nodeList.Add(newPoly);
                    newNodes.Add(newPoly);
                }
            }
            redoAdj(overlaps, newNodes.ToArray());

            foreach (PolygonNode pn in overlaps.ToArray())
            {
                nodeList.Remove(pn);
            }

        }

        /// <summary>
        /// Redoes the adjacency's of all of the effected polygons.
        /// </summary>
        /// <param name="oldNodes">Nodes to be deleted</param>
        /// <param name="newNodes">New nodes generated</param>
        private void redoAdj(IEnumerable<PolygonNode> oldNodes, IEnumerable<PolygonNode> newNodes)
        {
            foreach(PolygonNode pnOld in oldNodes)
            {
                foreach (PolygonNode pnAdj in pnOld.getAdjacentNodes())
                {
                    pnAdj.removeNode(pnOld);
                    foreach (PolygonNode pnNew in newNodes)
                    {
                        if (pnNew == pnAdj)
                            continue;
                        if (PolygonNode.areTheseAdjacent(pnNew, pnAdj))
                            PolygonNode.linkPolygons(pnNew, pnAdj);
                    }
                }
            }

            foreach (PolygonNode pn1 in newNodes)
            {
                foreach (PolygonNode pn2 in newNodes)
                {
                    if (pn1 == pn2)
                        continue;
                    if (PolygonNode.areTheseAdjacent(pn1, pn2))
                        PolygonNode.linkPolygons(pn1, pn2);
                }
            }
        }

        /// <summary>
        /// Gets the closest point to the left of the specified point.
        /// </summary>
        /// <param name="origin">Origin to be checked from.</param>
        /// <param name="vertices">List to be checked</param>
        /// <returns>Return point if found, if not returns -1,-1</returns>
        private static Point getClosestLeft(Point origin, IEnumerable<Point> vertices)
        {
            Point closest = new Point(int.MaxValue,int.MaxValue);
            foreach (Point p in vertices)
            {
                if (p.Y == origin.Y && p.X > origin.X && p.X < closest.X)
                {
                    closest = p;
                }
            }
            if (closest.X == int.MaxValue)
                return new Point(-1,-1);
            return closest;
        }

        /// <summary>
        /// Gets the closest point under the specified point.
        /// </summary>
        /// <param name="origin">Origin to be checked from.</param>
        /// <param name="vertices">List to be checked</param>
        /// <returns>Return point if found, if not returns -1,-1</returns>
        private static Point getClosestBottom(Point origin, IEnumerable<Point> vertices)
        {
            Point closest = new Point(int.MaxValue, int.MaxValue);
            foreach (Point p in vertices)
            {
                if (p.X == origin.X && p.Y > origin.Y && p.Y < closest.Y)
                {
                    closest = p;
                }
            }
            if (closest.Y == int.MaxValue)
                return new Point(-1, -1);
            return closest;
        }

        /// <summary>
        /// Adds the vertices of where the two given rectangles intersect.
        /// </summary>
        /// <param name="vertices">List to be amended</param>
        /// <param name="oldRect">Rectangle 1</param>
        /// <param name="rect">Rectangle 2</param>
        private static void addOverlapVerts(List<Point> vertices, Rectangle oldRect, Rectangle rect)
        {

            addVertex(vertices, new Point(oldRect.Left, rect.Top));
            addVertex(vertices, new Point(oldRect.Right, rect.Top));
            addVertex(vertices, new Point(oldRect.Left, rect.Bottom));
            addVertex(vertices, new Point(oldRect.Right, rect.Bottom));

            addVertex(vertices, new Point(rect.Right, oldRect.Bottom));
            addVertex(vertices, new Point(rect.Left, oldRect.Bottom));
            addVertex(vertices, new Point(rect.Right, oldRect.Top));
            addVertex(vertices, new Point(rect.Left, oldRect.Top));
        }

        /// <summary>
        /// Method for adding a vertex 
        /// if it does not already exist.
        /// </summary>
        /// <param name="vertices">List to be checked and amended</param>
        /// <param name="p">Point to add</param>
        private static void addVertex(List<Point> vertices, Point p)
        {
            foreach (Point point in vertices)
            {
                if (point.Equals(p))
                    return;
            }
            vertices.Add(p);
        }

        /// <summary>
        /// Returns an array of polygons that form a path
        /// between the two provided points from start to finish.
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <param name="finish">Finish</param>
        /// <returns>Path</returns>
        public PolygonNode[] getPath(Point start, Point finish)
        {

            List<PolygonNode> closedSet = new List<PolygonNode>();
            List<PolygonNode> openSet = new List<PolygonNode>();
            List<PolygonNode> came_from = new List<PolygonNode>();
            int g_score = 0;
            int h_score = getHScore(start, finish);
            int f_score = 0;
            PolygonNode endzone = getNodeFromPosition(finish);
            openSet.Add(getNodeFromPosition(start));
            PolygonNode current = openSet.ElementAt<PolygonNode>(0);
            openSet.Remove(current);
            closedSet.Add(current);
            while (true)
            {
                if (current == endzone)
                    break;
                foreach (PolygonNode pn in current.getAdjacentNodes())
                {
                    if (closedSet.Contains(pn))
                        continue;
                    if(!openSet.Contains(pn))
                    {
                        openSet.Add(pn);
                        pn.parent = current;
                    }
                }


                current = getNodeWithLowestFScore(openSet,g_score,finish);
                if (current == null)
                    throw new Exception("There is no spoon.");
                openSet.Remove(current);
                closedSet.Add(current);
            }

            List<PolygonNode> output = new List<PolygonNode>();
            while (current != null)
            {
                output.Add(current);
                current = current.parent;
            }
            output.Reverse();
            nullParents();
            return output.ToArray();
        }

        /// <summary>
        /// Method for getting the node with the f_score from the given set.
        /// </summary>
        /// <param name="set">Nodes to check</param>
        /// <param name="g_score">Current g_score</param>
        /// <param name="end">Finish</param>
        /// <returns></returns>
        private PolygonNode getNodeWithLowestFScore(IEnumerable<PolygonNode> set, int g_score, Point end)
        {
            int lowest = int.MaxValue;
            PolygonNode pnLowest = null;
            foreach(PolygonNode pn in set)
            {
                int _f = getHScore(pn.rectangle.Center, end) + g_score;
                if (_f < lowest)
                {
                    lowest = _f;
                    pnLowest = pn;
                }

            }
            return pnLowest;
        }

        /// <summary>
        /// Gets the nodes which the rectangle overlaps
        /// </summary>
        /// <param name="rect">Rectangle to check against</param>
        /// <returns></returns>
        private PolygonNode[] getOverlappingNodes(Rectangle rect)
        {
            List<PolygonNode> nodes = new List<PolygonNode>();
            foreach (PolygonNode pn in nodeList)
            {
                if (pn.inPolygon(rect))
                    nodes.Add(pn);
            }

            return nodes.ToArray();
        }

        /// <summary>
        /// Gets the polygon from specified position
        /// </summary>
        /// <param name="position">Position</param>
        /// <returns>Polygon point is located in.</returns>
        public PolygonNode getNodeFromPosition(Point position)
        {
            foreach (PolygonNode pn in nodeList)
            {
                if (pn.inPolygon(position))
                    return pn;
            }

          //  throw new PointNotOnNodeException ("Point not contained in any listed polygon.  Point: "
           //     + position.ToString() + " may not be contained in the map.");

            return null;
        }

        /// <summary>
        /// Sets the parent of all polygons to null.
        /// </summary>
        private void nullParents()
        {
            foreach (PolygonNode pn in nodeList)
            {
                pn.parent = null;
            }
        }

        /// <summary>
        /// Gets the h score between the two given points.
        /// </summary>
        /// <param name="current">Start</param>
        /// <param name="end">Finish</param>
        /// <returns>H Value</returns>
        private int getHScore(Point current, Point end)
        {
            int w = Math.Abs((end.X - current.X));
            int h = Math.Abs((end.Y - current.Y));
            double d = w * w + h * h;
            return (int)Math.Sqrt(d);
        }

    }


    /// <summary>
    /// Point is not on a node and shouldn't be used for NavMesh stuff.
    /// </summary>
    public class PointNotOnNodeException:Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="s"></param>
        public PointNotOnNodeException(String s):base(s)
        {

        }
    }
}
