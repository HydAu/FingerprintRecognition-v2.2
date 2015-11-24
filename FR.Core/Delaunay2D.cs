// Delaunay 2D triangulation algorithm.
// Salvatore Previti. http://www.salvatorepreviti.it - info@salvatorepreviti.it
// Optimized implementation of Delaunay triangulation algorithm by Paul Bourke (pbourke@swin.edu.au)
// See http://astronomy.swin.edu.au/~pbourke/terrain/triangulate/ for details.
//
// You can use this code or parts of this code providing that above credit remain intact.
//

using System;
using System.Collections.Generic;
using System.Drawing;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Represents a triangle of the Delaunay triangulation.
    /// </summary>
    public struct IntegerTriangle
    {
        /// <summary>
        ///     The index of the first minutia.
        /// </summary>
        public int A;

        /// <summary>
        ///     The index of the second minutia.
        /// </summary>
        public int B;

        /// <summary>
        ///     The index of the third minutia.
        /// </summary>
        public int C;
    }

    /// <summary>
    ///     Performs Delaunay triangulation over a collection of <see cref="Minutia"/>.
    /// </summary>
    /// <remarks>
    ///     This class is based on the implementation of Salvatore Previti (http://www.salvatorepreviti.it - info@salvatorepreviti.it).
    /// </remarks>
    public static class Delaunay2D
    {
        #region Triangulate

        /// <summary>
        ///     Performs Delaunay triangulation over the specified collection of <see cref="Minutia"/>.
        /// </summary>
        /// <param name="minutiae">The collection of <see cref="Minutia"/> to triangulate.</param>
        /// <returns>The Delaunay triangles.</returns>
        public static IntegerTriangle[] Triangulate(ICollection<Minutia> minutiae)
        {
            ICollection<PointF> pointsCollection = new List<PointF>(minutiae.Count);
            foreach (var minutia in minutiae)
            {
                pointsCollection.Add(new PointF(minutia.X, minutia.Y));
            }
            DelaunayTriangulator edges = new DelaunayTriangulator();
            if (edges.Initialize(pointsCollection, 0.00001f))
                edges.Process();

            return edges.ToArray();
        }

        #endregion

        #region Private types

        /// <summary>
        /// A specialized very fast table of edges for triangulation.
        /// </summary>
        private struct DelaunayTriangulator
        {
            #region Structures

            private struct Triangle
            {
                internal int A;
                internal int B;
                internal int C;

                internal float circumCirclecenterX;
                internal float circumCirclecenterY;
                internal float circumCircleRadius;

                internal int Previous;
                internal int Next;

                internal int prevNonCompleted;
                internal int nextNonCompleted;

                #region ToIntegerTriangle

                internal IntegerTriangle ToIntegerTriangle()
                {
                    IntegerTriangle result;
                    result.A = this.A;
                    result.B = this.B;
                    result.C = this.C;
                    return result;
                }

                internal void ToIntegerTriangle(ref IntegerTriangle destination)
                {
                    destination.A = this.A;
                    destination.B = this.B;
                    destination.C = this.C;
                }

                #endregion
            }

            private struct EdgeEntry
            {
                internal int next;
                internal int A;
                internal int B;
                internal int count;
            }

            private struct EdgeBucketEntry
            {
                internal int generation;
                internal int entryIndex;
            }

            #endregion

            #region Points fields

            private int pointsCount;
            private float tolerance;
            private PointF[] points;
            private int[] pointsIndices;

            #endregion

            #region Triangles Fields

            private Triangle[] Triangles;

            private int TrianglesLast;
            private int TrianglesCount;
            private int TrianglesFirst;

            private int firstNonCompletedTriangle;
            private int lastNonCompletedTriangle;

            private int firstFreeTriangle;

            #endregion

            #region Edges Fields

            private EdgeBucketEntry[] edgesBuckets;
            private EdgeEntry[] edgesEntries;

            private int edgesGeneration;
            private int edgesCount;

            #endregion

            #region Initialize

            internal bool Initialize(ICollection<PointF> pointsCollection, float tolerance)
            {
                this.points = null;
                this.pointsIndices = null;
                this.edgesBuckets = null;
                this.edgesEntries = null;
                this.Triangles = null;
                this.Triangles = null;

                // Initialize triangle table

                this.TrianglesFirst = -1;
                this.TrianglesLast = -1;
                this.TrianglesCount = 0;

                this.firstNonCompletedTriangle = -1;
                this.lastNonCompletedTriangle = -1;

                // Initialize edge table

                this.edgesGeneration = 1;
                this.edgesCount = 0;

                this.tolerance = tolerance > 0 ? tolerance : float.Epsilon; // Ensure tolerance is valid

                this.pointsCount = pointsCollection == null ? 0 : pointsCollection.Count;

                if (pointsCollection.Count < 3)
                    return false; // We need a non null collection with at least 3 vertices!

                // Create the array of points.
                // We need 3 more items to add supertriangle vertices

                this.points = new PointF[this.pointsCount + 3];
                pointsCollection.CopyTo(points, 0);

                // Create an array of indices to points sorted by Y (firstly), X (secondly) and insertion order (thirdly)
                this.pointsIndices = DelaunayTriangulator.GetSortedPointIndices(points, this.pointsCount, tolerance);

                // Calculate min and max X and Y coomponents of points

                PointF pointsMin, pointsMax;
                PointF d = new PointF();

                DelaunayTriangulator.GetMinMaxPointCoordinates(points, this.pointsCount, out pointsMin, out pointsMax);

                // Create supertriangle vertices
                d.X = pointsMax.X - pointsMin.X;
                d.Y = pointsMax.Y - pointsMin.Y;

                float dmax = (d.X > d.Y) ? d.X : d.Y;
                PointF mid = new PointF();
                mid.X = (pointsMax.X + pointsMin.X) * 0.5f;
                mid.Y = (pointsMax.Y + pointsMin.Y) * 0.5f;

                points[this.pointsCount] = new PointF(mid.X - 2 * dmax, mid.Y - dmax);
                points[this.pointsCount + 1] = new PointF(mid.X, mid.Y + 2 * dmax);
                points[this.pointsCount + 2] = new PointF(mid.X + 2 * dmax, mid.Y - dmax);

                // Initialize triangle array

                this.Triangles = new Triangle[this.pointsCount * 4 + 1];

                Triangle triangleEntry = new Triangle();
                triangleEntry.prevNonCompleted = -1;
                triangleEntry.nextNonCompleted = -1;

                // Initialized free triangles

                this.Triangles = new Triangle[this.Triangles.Length];

                this.firstFreeTriangle = 0;
                for (int i = 0; i < this.Triangles.Length; ++i)
                {
                    triangleEntry.Previous = i - 1;
                    triangleEntry.Next = i + 1;

                    this.Triangles[i] = triangleEntry;
                }
                this.Triangles[this.Triangles.Length - 1].Next = -1;

                // Initialize edge table

                int size = SPMath.GetPrime(this.Triangles.Length * 3 + 1);
                this.edgesBuckets = new EdgeBucketEntry[size];
                this.edgesEntries = new EdgeEntry[size];

                // Add supertriangle

                this.AddTriangle(this.pointsCount, this.pointsCount + 1, this.pointsCount + 2);

                return true;
            }

            #endregion

            #region Process

            internal void Process()
            {
                // Process all sorted points

                float circumCirclecenterX;
                float circumCirclecenterY;
                float circumCircleRadius;
                float dx, dy;
                float pointX = 0, pointY = 0;
                float pointYplusTolerance;

                PointF point = this.points[this.pointsIndices[this.pointsIndices.Length - 1]];
                for (int sortedIndex = 0; sortedIndex < this.pointsIndices.Length; ++sortedIndex)
                {
                    int pointIndex = this.pointsIndices[sortedIndex];

                    point = this.points[pointIndex];

                    if (sortedIndex != 0 && Math.Abs(point.X - pointX) < tolerance && Math.Abs(point.Y - pointY) < tolerance)
                        continue; // Ignore current point if equals to previous point. We check equality using tolerance.

                    pointX = point.X;
                    pointY = point.Y;
                    pointYplusTolerance = pointY + tolerance;

                    // Check if triangle contains current point in its circumcenter.
                    // If yes, add triangle edges to edges table and remove triangle.
                    for (int nextNonCompleted, triangleIndex = this.firstNonCompletedTriangle; triangleIndex >= 0; triangleIndex = nextNonCompleted)
                    {
                        // Calculate distance between triancle circumcircle center and current point
                        // Compare that distance with radius of triangle circumcircle
                        // If is less, it means that the point is inside of circumcircle, else, it means it is outside.

                        circumCirclecenterX = this.Triangles[triangleIndex].circumCirclecenterX;
                        circumCirclecenterY = this.Triangles[triangleIndex].circumCirclecenterY;
                        circumCircleRadius = this.Triangles[triangleIndex].circumCircleRadius;
                        nextNonCompleted = this.Triangles[triangleIndex].nextNonCompleted;

                        dx = pointX - circumCirclecenterX;
                        dy = pointY - circumCirclecenterY;

                        if ((dx * dx + dy * dy) <= circumCircleRadius)
                        {
                            // Point is inside triangle circumcircle.
                            // Add triangle edges to edge table and remove the triangle

                            this.ReplaceTriangleWithEdges(triangleIndex, ref this.Triangles[triangleIndex]);
                        }
                        else if ((circumCirclecenterY < pointYplusTolerance) && (dy > circumCircleRadius + tolerance))
                        {
                            // Triangle not need to be checked anymore.
                            // Remove it from linked list of non completed triangles.

                            this.MarkAsComplete(ref this.Triangles[triangleIndex]);
                        }
                    }

                    // Form new triangles for the current point
                    // Edges used more than once will be skipped
                    // Triangle vertices are arranged in clockwise order

                    for (int j = 0; j < this.edgesCount; ++j)
                    {
                        DelaunayTriangulator.EdgeEntry edge = this.edgesEntries[j];
                        if (this.edgesEntries[j].count == 1)
                        {
                            // If edge was used only one time, add a new triangle built from current edge.

                            this.AddTriangle(edge.A, edge.B, pointIndex);
                        }
                    }

                    // Clear edges table

                    ++this.edgesGeneration;
                    this.edgesCount = 0;
                }

                this.firstNonCompletedTriangle = -1;

                // Count valid triangles (triangles that don't share vertices with supertriangle) and find the last triangle.

                this.TrianglesLast = this.TrianglesFirst;
                this.TrianglesCount = 0;
                if (this.TrianglesLast != -1)
                {
                    for (; ; )
                    {
                        DelaunayTriangulator.Triangle triangle = this.Triangles[this.TrianglesLast];

                        if (triangle.A < this.pointsCount && triangle.B < this.pointsCount && triangle.C < this.pointsCount)
                        {
                            // Valid triangle found. Increment count.
                            ++this.TrianglesCount;
                        }
                        else
                        {
                            // Current triangle is invalid. Mark it as invalid
                            this.Triangles[this.TrianglesLast].A = -1;
                        }

                        int next = this.Triangles[this.TrianglesLast].Next;
                        if (next == -1)
                            break;

                        this.TrianglesLast = next;
                    }
                }
            }

            #endregion

            #region CopyTo, AddTo, ToArray

            private void CopyTo(IntegerTriangle[] array, int arrayIndex)
            {
                for (int triangleIndex = this.TrianglesLast; triangleIndex >= 0; triangleIndex = this.Triangles[triangleIndex].Previous)
                    if (this.Triangles[triangleIndex].A >= 0)
                        this.Triangles[triangleIndex].ToIntegerTriangle(ref array[arrayIndex++]);
            }

            internal IntegerTriangle[] ToArray()
            {
                IntegerTriangle[] result = new IntegerTriangle[this.TrianglesCount];
                this.CopyTo(result, 0);
                return result;
            }

            #endregion

            #region Edges table

            private void ReplaceTriangleWithEdges(int triangleIndex, ref Triangle triangle)
            {
                // Remove triangle from linked list

                if (triangle.Next >= 0)
                    this.Triangles[triangle.Next].Previous = triangle.Previous;

                if (triangle.Previous >= 0)
                    this.Triangles[triangle.Previous].Next = triangle.Next;
                else
                    this.TrianglesFirst = triangle.Next;

                // Remove triangle from non completed linked list

                this.MarkAsComplete(ref triangle);

                // Add triangle to free triangles linked list

                triangle.Previous = -1;
                triangle.Next = this.firstFreeTriangle;

                this.Triangles[this.firstFreeTriangle].Previous = triangleIndex;

                this.firstFreeTriangle = triangleIndex;

                // Add triangle edges to edges table

                this.AddEdge(triangle.A, triangle.B);
                this.AddEdge(triangle.B, triangle.C);
                this.AddEdge(triangle.C, triangle.A);
            }

            private void AddEdge(int edgeA, int edgeB)
            {
                EdgeEntry entry;

                // Calculate bucked index using an hashcode of edge indices.
                // Hashcode is generated so order of edges is ignored, it means, edge 1, 2 is equals to edge 2, 1
                int targetBucket = unchecked(((edgeA < edgeB ? (edgeA << 8) ^ edgeB : (edgeB << 8) ^ edgeA) & 0x7FFFFFFF) % this.edgesBuckets.Length);

                if (this.edgesBuckets[targetBucket].generation != this.edgesGeneration)
                {
                    // Bucket generation doesn't match current generation.
                    // This means this bucket is empty.
                    // Generations are incremented each time edge table is cleared.

                    // This entry is in the head of this bucket
                    entry.next = -1;

                    // Store the new generation
                    this.edgesBuckets[targetBucket].generation = this.edgesGeneration;
                }
                else
                {
                    int entryIndex = this.edgesBuckets[targetBucket].entryIndex;

                    for (int i = entryIndex; i >= 0; i = entry.next)
                    {
                        entry = this.edgesEntries[i];
                        if ((entry.A == edgeA && entry.B == edgeB) || (entry.A == edgeB && entry.B == edgeA))
                        {
                            ++edgesEntries[i].count;
                            return;
                        }
                    }

                    entry.next = entryIndex;
                }

                entry.A = edgeA;
                entry.B = edgeB;
                entry.count = 1;

                this.edgesEntries[this.edgesCount] = entry;
                this.edgesBuckets[targetBucket].entryIndex = this.edgesCount;
                ++this.edgesCount;
            }

            #endregion

            #region Triangles lists

            private void AddTriangle(int a, int b, int c)
            {
                // Acquire the first free triangle

                int result = this.firstFreeTriangle;
                this.firstFreeTriangle = this.Triangles[result].Next;
                this.Triangles[this.firstFreeTriangle].Previous = -1;

                Triangle triangle;

                // Insert the triangle into triangles linked list

                triangle.Previous = -1;
                triangle.Next = this.TrianglesFirst;

                if (this.TrianglesFirst != -1)
                    this.Triangles[this.TrianglesFirst].Previous = result;

                this.TrianglesFirst = result;

                // Insert the triangle into non completed triangles linked list

                triangle.prevNonCompleted = this.lastNonCompletedTriangle;
                triangle.nextNonCompleted = -1;

                if (this.firstNonCompletedTriangle == -1)
                    this.firstNonCompletedTriangle = result;
                else
                    this.Triangles[this.lastNonCompletedTriangle].nextNonCompleted = result;

                this.lastNonCompletedTriangle = result;

                // Store new entry
                //this.Triangles[result] = triangle;

                // Create the new triangle

                triangle.A = a;
                triangle.B = b;
                triangle.C = c;

                // Compute the circum circle of the new triangle

                PointF pA = this.points[a];
                PointF pB = this.points[b];
                PointF pC = this.points[c];

                float m1, m2;
                float mx1, mx2;
                float my1, my2;
                float cX, cY;

                if (Math.Abs(pB.Y - pA.Y) < this.tolerance)
                {
                    m2 = -(pC.X - pB.X) / (pC.Y - pB.Y);
                    mx2 = (pB.X + pC.X) * 0.5f;
                    my2 = (pB.Y + pC.Y) * 0.5f;

                    cX = (pB.X + pA.X) * 0.5f;
                    cY = m2 * (cX - mx2) + my2;
                }
                else
                {
                    m1 = -(pB.X - pA.X) / (pB.Y - pA.Y);
                    mx1 = (pA.X + pB.X) * 0.5f;
                    my1 = (pA.Y + pB.Y) * 0.5f;

                    if (Math.Abs(pC.Y - pB.Y) < this.tolerance)
                    {
                        cX = (pC.X + pB.X) * 0.5f;
                        cY = m1 * (cX - mx1) + my1;
                    }
                    else
                    {
                        m2 = -(pC.X - pB.X) / (pC.Y - pB.Y);
                        mx2 = (pB.X + pC.X) * 0.5f;
                        my2 = (pB.Y + pC.Y) * 0.5f;

                        cX = (m1 * mx1 - m2 * mx2 + my2 - my1) / (m1 - m2);
                        cY = m1 * (cX - mx1) + my1;
                    }
                }

                triangle.circumCirclecenterX = cX;
                triangle.circumCirclecenterY = cY;

                // Calculate circumcircle radius

                mx1 = pB.X - cX;
                my1 = pB.Y - cY;
                triangle.circumCircleRadius = mx1 * mx1 + my1 * my1;

                // Store the new triangle

                this.Triangles[result] = triangle;
            }

            private void MarkAsComplete(ref Triangle triangle)
            {
                // Remove triangle from non completed linked list

                if (triangle.nextNonCompleted >= 0)
                    this.Triangles[triangle.nextNonCompleted].prevNonCompleted = triangle.prevNonCompleted;
                else
                    this.lastNonCompletedTriangle = triangle.prevNonCompleted;

                if (triangle.prevNonCompleted >= 0)
                    this.Triangles[triangle.prevNonCompleted].nextNonCompleted = triangle.nextNonCompleted;
                else
                    this.firstNonCompletedTriangle = triangle.nextNonCompleted;
            }

            #endregion

            #region Static functions

            /// <summary>
            /// Get minimum and maximum X and Y values from specified array.
            /// </summary>
            private static void GetMinMaxPointCoordinates(PointF[] points, int count, out PointF min, out PointF max)
            {
                if (count <= 0)
                    throw new InvalidOperationException("Array cannot be empty");

                min = points[0];
                max = points[0];

                for (int i = 1; i < count; ++i)
                {
                    PointF v = points[i];
                    if (v.X > max.X)
                        max.X = v.X;
                    else if (v.X < min.X)
                        min.X = v.X;
                    if (v.Y > max.Y)
                        max.Y = v.Y;
                    else if (v.Y < min.Y)
                        min.Y = v.Y;
                }
            }

            /// <summary>
            /// Create an array of indices from a PointF array sorting them by Y (firstly), X (secondly) and insertion order (thirdly)
            /// </summary>
            private static int[] GetSortedPointIndices(PointF[] points, int count, float tolerance)
            {
                int[] result = new int[count];

                // Store index in indices

                for (int i = 0; i < result.Length; ++i)
                    result[i] = i;

                // Sort indices by Y (firstly), X (secondly) and insertion order (thirdly)

                Array.Sort(result, delegate(int a, int b)
                {
                    PointF va = points[a];
                    PointF vb = points[b];

                    float f = va.Y - vb.Y;

                    if (f > tolerance)
                        return +1;
                    if (f < -tolerance)
                        return -1;

                    f = va.X - vb.X;

                    if (f > tolerance)
                        return +1;
                    if (f < -tolerance)
                        return -1;

                    return a - b;
                });

                return result;
            }

            #endregion
        }

        #endregion
    }

    internal static class SPMath
    {
        #region Prime numbers

        internal static readonly int[] primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919, 
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263, 
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

        internal static int GetPrime(int min)
        {
            if (min < 0)
                return min;

            for (int i = 0; i < primes.Length; i++)
            {
                int prime = primes[i];
                if (prime >= min)
                    return prime;
            }

            //outside of our predefined table. 
            //compute the hard way.
            for (int i = (min | 1); i < Int32.MaxValue; i += 2)
            {
                if ((i & 1) != 0)
                {
                    int limit = (int)Math.Sqrt(i);
                    for (int divisor = 3; divisor <= limit; divisor += 2)
                        if ((i % divisor) == 0)
                            continue;

                    return i;
                }

                if (i == 2)
                    return i;
            }

            return min;
        }

        #endregion
    }
}
