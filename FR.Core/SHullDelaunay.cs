/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System.Collections.Generic;
using DelaunayTriangulator;

namespace PatternRecognition.FingerprintRecognition.Core
{
    public static class SHullDelaunay
    {
        public static List<Minutia[]> Triangulate(List<Minutia> minutiae, out List<int[]> triplets)
        {
            List<Vertex> points = new List<Vertex>();
            foreach (var minutia in minutiae)
                points.Add(new Vertex(minutia.X, minutia.Y));
            
            List<Triad> triangles = triangulator.Triangulation(points, true);

            List<Minutia[]> mTriplets = new List<Minutia[]>(minutiae.Count);
            triplets = new List<int[]>(minutiae.Count);
            foreach (var triangle in triangles)
            {
                mTriplets.Add(new[] { minutiae[triangle.a], minutiae[triangle.b], minutiae[triangle.c] });
                triplets.Add(new[] { triangle.a, triangle.b, triangle.c });
            }

            return mTriplets;
        }

        private static SHullTriangulator triangulator = new SHullTriangulator();
    }
}
