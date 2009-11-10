using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WorldNavigator;

using System.Drawing;

namespace Testing1
{
    class Geometry
    {
        static void Main(string[] args)
        {

            Vector v1 = new Vector(0F, 5F, 0F);
            Vector v2 = new Vector(5F, 5F, 0F);
            Vector v3 = new Vector(0F, 5F, 5F);
            Vector v4 = new Vector(5F, 5F, 5F);
            Quad quad = new Quad(v1, v2, v3, v4, new MyColor(0F, 0F, 0F));
            List<Quad> quads = new List<Quad>();
            quads.Add(quad);
            
            Vector observer = new Vector(2F, 6.5F, 3F);

            float distanceToGround = MovingTools.DistanceToGround(observer, quads);

            /*
            float Distance = MovingTools.PointToPlaneDistance(observer, v1, v2, v3);
            Console.WriteLine("distance: " + Distance.ToString());

            bool intersectWithWall = MovingTools.PointIsWithinRangeOfTriangle(10F, observer, v1, v2, v3);
            Console.WriteLine("intersection?: " + intersectWithWall);

            v1 = new Vertex(10F, 0F, 0F);
            v2 = new Vertex(10F, 3F, 0F);
            v3 = new Vertex(0F, 3F, 0F);

            observer = new Vertex(8F, 0.1F, 0.1F);

            intersectWithWall = MovingTools.PointIsWithinRangeOfTriangle(1F, observer, v1, v2, v3);
            Console.WriteLine("intersection?: " + intersectWithWall);

            */

            Console.ReadKey();
        }
    }
}
