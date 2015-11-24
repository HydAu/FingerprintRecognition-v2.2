/*
 * Created by: Octavio Loyola González (octavioloyola@gmail.com) and Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Drawing;
using ImageProcessingTools;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.FeatureExtractors
{
    /// <summary>
    ///     An implementation of the <see cref="SkeletonImage"/> extractor proposed by Ratha et al. in 1995.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is an implementation of the <see cref="SkeletonImage"/> extractor proposed by Ratha et al. [1] in 1995.
    ///     </para>
    ///     <para>
    ///         Take into account that this algorithm is created to work with fingerprint images at 500 dpi. Proper modifications have to be made for different image resolutions.
    ///     </para>
    ///     <para>
    ///         References:
    ///     </para>
    ///     <para>
    ///         <list type="number">
    ///             <item>
    ///                Ratha N.K., Chen S.Y. and Jain A.K., "Adaptive flow orientation-based feature extraction in fingerprint images," Pattern Recognition, vol. 28, no. 11, pp. 1657–1672, 1995.
    ///             </item>             
    ///         </list>
    ///     </para>
    /// </remarks>
    public class Ratha1995SkeImgExtractor : FeatureExtractor<SkeletonImage>
    {
        #region public

        /// <summary>
        ///     Extract a skeleton image from the specified bitmap image.
        /// </summary>
        /// <param name="image">The source bitmap image to extract features from.</param>
        /// <returns>The features extracted from the specified bitmap image.</returns>
        public override SkeletonImage ExtractFeatures(Bitmap image)
        {
            Ratha1995OrImgExtractor ratha1995OrImgExtractor = new Ratha1995OrImgExtractor();
            OrientationImage orientationImage = ratha1995OrImgExtractor.ExtractFeatures(image);
            ImageMatrix skeletonImage = ExtractSkeletonImage(image, orientationImage);

            byte[] img = new byte[skeletonImage.Width * skeletonImage.Height];
            for (int i = 0; i < skeletonImage.Height; i++)
                for (int j = 0; j < skeletonImage.Width; j++)
                    img[skeletonImage.Width * i + j] = (byte) skeletonImage[i, j];

            return new SkeletonImage(img, skeletonImage.Width, skeletonImage.Height);
        }

        /// <summary>
        ///     Extract a skeleton image from the specified bitmap image.
        /// </summary>
        /// <param name="image">The source bitmap image to extract features from.</param>
        /// <returns>The extracted skeleton image represented by a matrix.</returns>
        public ImageMatrix ExtractSkeletonImage(Bitmap image, OrientationImage orientationImage)
        {
            ImageMatrix matrix = new ImageMatrix(image);

            GaussianBlur gb = new GaussianBlur();
            matrix = gb.Apply(matrix);

            matrix = GetBinaryImage(matrix, orientationImage);
            ApplyThinning(matrix, orientationImage);
            RemoveSpikes(matrix, orientationImage);
            FixBifurcations(matrix, orientationImage);
            RemoveSmallSegments(matrix, orientationImage);

            return matrix;
        }

        #endregion
        
        #region private
        
        private ImageMatrix GetBinaryImage(ImageMatrix matrix, OrientationImage orientationImage)
        {
            int[] filter = new int[] { 1, 2, 5, 7, 5, 2, 1 };
            ImageMatrix newImg = new ImageMatrix(matrix.Width, matrix.Height);
            for (int i = 0; i < matrix.Width; i++)
                for (int j = 0; j < matrix.Height; j++)
                {
                    newImg[j, i] = 255;
                }
            for (int row = 0; row < orientationImage.Height; row++)
                for (int col = 0; col < orientationImage.Width; col++)
                    if (!orientationImage.IsNullBlock(row, col))
                    {
                        int x, y;
                        orientationImage.GetPixelCoordFromBlock(row, col, out x, out y);

                        int maxLength = orientationImage.WindowSize / 2;
                        for (int xi = x - maxLength; xi < x + maxLength; xi++)
                            for (int yi = y - maxLength; yi < y + maxLength; yi++)
                            {
                                int[] projection = GetProjection(orientationImage, row, col, xi, yi, matrix);
                               
                                int[] smoothed = new int[orientationImage.WindowSize + 1];
                                const int n = 7;
                                for (int j = 0; j < projection.Length; j++)
                                {
                                    int idx = 0;
                                    int sum = 0, count = 0;
                                    for (int k = j - n / 2; k <= j + n / 2; k++, idx++)
                                        if (k >= 0 && k < projection.Length)
                                        {
                                            sum += projection[k] * filter[idx];
                                            count++;
                                        }
                                    smoothed[j] = sum / count;
                                }

                                int center = smoothed.Length / 2;
                                int left;
                                for (left = center - 1; smoothed[left] == smoothed[center] && left > 0; left--) ;

                                int rigth;
                                for (rigth = center + 1; smoothed[rigth] == smoothed[center] && rigth < smoothed.Length - 1; rigth++) ;

                                if (xi >= 0 && xi < matrix.Width && yi >= 0 && yi < matrix.Height)
                                    newImg[yi, xi] = 255;

                                if (xi > 0 && xi < matrix.Width - 1 && yi > 0 && yi < matrix.Height - 1 && !(left == 255 && rigth == smoothed.Length - 1))
                                {
                                    if (smoothed[center] < smoothed[left] && smoothed[center] < smoothed[rigth])
                                        newImg[yi, xi] = 0;
                                    else if (rigth - left == 2 &&
                                             ((smoothed[left] < smoothed[left - 1] &&
                                               smoothed[left] < smoothed[center]) ||
                                        (smoothed[rigth] < smoothed[rigth + 1] &&
                                               smoothed[rigth] < smoothed[center]) ||
                                        (smoothed[center] < smoothed[left - 1] &&
                                               smoothed[center] < smoothed[rigth + 1]) ||
                                              (smoothed[center] < smoothed[left - 1] &&
                                               smoothed[center] < smoothed[rigth]) ||
                                              (smoothed[center] < smoothed[left] &&
                                               smoothed[center] < smoothed[rigth + 1])))
                                        newImg[yi, xi] = 0;
                                }
                            }
                    }
            return newImg;
        }

        private int[] GetProjection(OrientationImage oi, int row, int col, int x, int y, ImageMatrix matrix)
        {
            double angle = oi.AngleInRadians(row, col);
            double orthogonalAngle = oi.AngleInRadians(row, col) + Math.PI / 2;
            
            int maxLength = oi.WindowSize / 2;
            int[] projection = new int[2 * maxLength + 1];
            int[] outlayerCount = new int[2 * maxLength + 1];
            bool outlayerFound = false;
            int totalSum = 0;
            int validPointsCount = 0;
            for (int li = -maxLength, i = 0; li <= maxLength; li++, i++)
            {
                int xi = Convert.ToInt32(x - li * Math.Cos(orthogonalAngle));
                int yi = Convert.ToInt32(y - li * Math.Sin(orthogonalAngle));

                int ySum = 0;
                for (int lj = -maxLength; lj <= maxLength; lj++)
                {
                    int xj = Convert.ToInt32(xi - lj * Math.Cos(angle));
                    int yj = Convert.ToInt32(yi - lj * Math.Sin(angle));
                    if (xj >= 0 && yj >= 0 && xj < matrix.Width && yj < matrix.Height)
                    {
                        ySum += matrix[yj, xj];
                        validPointsCount++;
                    }
                    else
                    {
                        outlayerCount[i]++;
                        outlayerFound = true;
                    }
                }
                projection[i] = ySum;
                totalSum += ySum;
            }

            if (outlayerFound)
            {
                int avg = totalSum / validPointsCount;
                for (int i = 0; i < projection.Length; i++)
                    projection[i] += avg * outlayerCount[i];
            }

            return projection;
        }

        private void ApplyThinning(ImageMatrix matrix, OrientationImage orientationImage)
        {
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int row = 0; row < orientationImage.Height; row++)
                    for (int col = 0; col < orientationImage.Width; col++)
                        if (!orientationImage.IsNullBlock(row, col))
                        {
                            int x, y;
                            orientationImage.GetPixelCoordFromBlock(row, col, out x, out y);

                            int maxLength = orientationImage.WindowSize / 2;
                            for (int xi = x - maxLength; xi < x + maxLength; xi++)
                                for (int yi = y - maxLength; yi < y + maxLength; yi++)
                                    if (xi > 0 && xi < matrix.Width - 1 && yi > 0 && yi < matrix.Height - 1)
                                    {
                                        int tl = matrix[yi - 1, xi - 1]; 
                                        int tc = matrix[yi - 1, xi]; 
                                        int tr = matrix[yi - 1, xi + 1]; 

                                        int le = matrix[yi, xi - 1]; 
                                        int ce = matrix[yi, xi]; 
                                        int ri = matrix[yi, xi + 1]; 

                                        int bl = matrix[yi + 1, xi - 1]; 
                                        int bc = matrix[yi + 1, xi];
                                        int br = matrix[yi + 1, xi + 1]; 

                                        if (IsVL(tl, tc, tr, le, ce, ri, bl, bc, br) ||
                                            IsVR(tl, tc, tr, le, ce, ri, bl, bc, br) ||
                                            IsHT(tl, tc, tr, le, ce, ri, bl, bc, br) ||
                                            IsHB(tl, tc, tr, le, ce, ri, bl, bc, br))
                                        {
                                            matrix[yi, xi] = 255;
                                            changed = true;
                                        }
                                    }
                                    else
                                        matrix[yi, xi] = 255;
                        }
            }
        }

        private void RemoveSpikes(ImageMatrix matrix, OrientationImage orientationImage)
        {
            for (int row = 0; row < orientationImage.Height; row++)
                for (int col = 0; col < orientationImage.Width; col++)
                    if (!orientationImage.IsNullBlock(row, col))
                    {
                        double[] cos = new double[3];
                        double[] sin = new double[3];

                        double orthogonalAngle = orientationImage.AngleInRadians(row, col) + Math.PI / 2;
                        cos[0] = Math.Cos(orthogonalAngle);
                        sin[0] = Math.Sin(orthogonalAngle);

                        double orthogonalAngle1 = orthogonalAngle + Math.PI / 12;
                        cos[1] = Math.Cos(orthogonalAngle1);
                        sin[1] = Math.Sin(orthogonalAngle1);

                        double orthogonalAngle2 = orthogonalAngle - Math.PI / 12;
                        cos[2] = Math.Cos(orthogonalAngle2);
                        sin[2] = Math.Sin(orthogonalAngle2);

                        int x, y;
                        orientationImage.GetPixelCoordFromBlock(row, col, out x, out y);

                        int maxLength = orientationImage.WindowSize / 2;
                        for (int xi = x - maxLength; xi < x + maxLength; xi++)
                            for (int yi = y - maxLength; yi < y + maxLength; yi++)
                            {
                                int xj = xi;
                                int yj = yi;
                                bool spikeFound = true;
                                while (spikeFound)
                                {
                                    spikeFound = false;
                                    if (xj > 0 && xj < matrix.Width - 1 && yj > 0 && yj < matrix.Height - 1)
                                    {
                                        int tl = matrix[yj - 1, xj - 1];
                                        int tc = matrix[yj - 1, xj]; 
                                        int tr = matrix[yj - 1, xj + 1]; 

                                        int le = matrix[yj, xj - 1]; 
                                        int ce = matrix[yj, xj]; 
                                        int ri = matrix[yj, xj + 1]; 

                                        int bl = matrix[yj + 1, xj - 1]; 
                                        int bc = matrix[yj + 1, xj]; 
                                        int br = matrix[yj + 1, xj + 1]; 

                                        if (CouldBeSpike(tl, tc, tr, le, ce, ri, bl, bc, br))
                                            for (int i = 0; i < sin.Length && !spikeFound; i++)
                                            {
                                                int xk = Convert.ToInt32(Math.Round(xj - cos[i]));
                                                int yk = Convert.ToInt32(Math.Round(yj - sin[i]));
                                                if (matrix[yk, xk] == 0)
                                                {
                                                    matrix[yj, xj] = 255;
                                                    xj = xk;
                                                    yj = yk;
                                                    spikeFound = true;
                                                }
                                                else
                                                {
                                                    xk = Convert.ToInt32(Math.Round(xj + cos[i]));
                                                    yk = Convert.ToInt32(Math.Round(yj + sin[i]));
                                                    if (matrix[yk, xk] == 0)
                                                    {
                                                        matrix[yj, xj] = 255;
                                                        xj = xk;
                                                        yj = yk;
                                                        spikeFound = true;
                                                    }
                                                }
                                            }
                                    }
                                }
                            }
                    }
        }

        private void FixBifurcations(ImageMatrix matrix, OrientationImage orientationImage)
        {
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int row = 0; row < orientationImage.Height; row++)
                    for (int col = 0; col < orientationImage.Width; col++)
                        if (!orientationImage.IsNullBlock(row, col))
                        {
                            int x, y;
                            orientationImage.GetPixelCoordFromBlock(row, col, out x, out y);

                            int maxLength = orientationImage.WindowSize / 2;
                            for (int xi = x - maxLength; xi < x + maxLength; xi++)
                                for (int yi = y - maxLength; yi < y + maxLength; yi++)
                                    if (xi > 0 && xi < matrix.Width - 1 && yi > 0 && yi < matrix.Height - 1)
                                    {
                                        int tl = matrix[yi - 1, xi - 1];
                                        int tc = matrix[yi - 1, xi];
                                        int tr = matrix[yi - 1, xi + 1];

                                        int le = matrix[yi, xi - 1];
                                        int ce = matrix[yi, xi];
                                        int ri = matrix[yi, xi + 1];

                                        int bl = matrix[yi + 1, xi - 1];
                                        int bc = matrix[yi + 1, xi];
                                        int br = matrix[yi + 1, xi + 1]; 

                                        if (IsCorner(tl, tc, tr, le, ce, ri, bl, bc, br))
                                        {
                                            matrix[yi, xi] = 255;
                                            changed = true;
                                        }
                                    }
                                    else
                                        matrix[yi, xi] = 255;
                        }
            }
        }

        private void RemoveSmallSegments(ImageMatrix matrix, OrientationImage orientationImage)
        {
            for (int row = 0; row < orientationImage.Height; row++)
                for (int col = 0; col < orientationImage.Width; col++)
                    if (!orientationImage.IsNullBlock(row, col))
                    {
                        int x, y;
                        orientationImage.GetPixelCoordFromBlock(row, col, out x, out y);

                        int maxLength = orientationImage.WindowSize / 2;
                        for (int xi = x - maxLength; xi < x + maxLength; xi++)
                            for (int yi = y - maxLength; yi < y + maxLength; yi++)
                            {
                                const int pThreshold = 10;
                                int[] xArr = new int[pThreshold + 1];
                                int[] yArr = new int[pThreshold + 1];

                                int x0, y0;
                                if (IsEnd(matrix, xi, yi, out x0, out y0))
                                {
                                    xArr[0] = xi;
                                    yArr[0] = yi;

                                    xArr[1] = x0;
                                    yArr[1] = y0;

                                    bool endFound = false;
                                    bool bifurcationFound = false;
                                    int pCount = 1;
                                    for (int i = 1; i < pThreshold && !endFound && !bifurcationFound; i++)
                                    {
                                        if (IsEnd(matrix, xArr[i], yArr[i], out xArr[i + 1], out yArr[i + 1]))
                                            endFound = true;
                                        else if (!IsContinous(matrix, xArr[i - 1], yArr[i - 1], xArr[i], yArr[i],
                                                             out xArr[i + 1], out yArr[i + 1]))
                                            bifurcationFound = true;

                                        pCount++;
                                    }
                                    if (endFound || (bifurcationFound && pCount <= pThreshold))
                                        for (int i = 0; i < pCount; i++)
                                            matrix[yArr[i], xArr[i]] = 255;
                                }
                            }
                    }
        }

        private bool IsVR(int tl, int tc, int tr, int le, int ce, int ri, int bl, int bc, int br)
        {
            if (tl == 0 && tc == 255 && tr == 255 &&
                le == 0 && ce == 0 && ri == 255 &&
                bl == 0 && bc == 255 && br == 255
                )
                return true;
            if (tl == 0 && tc == 255 && tr == 255 &&
                le == 0 && ce == 0 && ri == 255 &&
                bl == 0 && bc == 0 && br == 0
                )
                return true;
            if (tl == 255 && tc == 255 && tr == 255 &&
                le == 0 && ce == 0 && ri == 255 &&
                bl == 0 && bc == 0 && br == 255
                )
                return true;
            if (tl == 0 && tc == 255 && tr == 255 &&
                le == 0 && ce == 0 && ri == 255 &&
                bl == 0 && bc == 0 && br == 255
                )
                return true;
            if (tl == 0 && tc == 0 && tr == 255 &&
                le == 0 && ce == 0 && ri == 255 &&
                bl == 0 && bc == 255 && br == 255
                )
                return true;
            if (tl == 0 && tc == 0 && tr == 255 &&
                le == 0 && ce == 0 && ri == 255 &&
                bl == 0 && bc == 0 && br == 255
                )
                return true;

            return false;
        }

        private bool IsVL(int tl, int tc, int tr, int le, int ce, int ri, int bl, int bc, int br)
        {
            if (tl == 255 && tc == 255 && tr == 0 &&
                            le == 255 && ce == 0 && ri == 0 &&
                            bl == 255 && bc == 255 && br == 0
                            )
                return true;
            if (tl == 0 && tc == 0 && tr == 0 &&
                le == 255 && ce == 0 && ri == 0 &&
                bl == 255 && bc == 255 && br == 0
                )
                return true;
            if (tl == 255 && tc == 0 && tr == 0 &&
                le == 255 && ce == 0 && ri == 0 &&
                bl == 255 && bc == 255 && br == 255
                )
                return true;
            if (tl == 255 && tc == 0 && tr == 0 &&
                le == 255 && ce == 0 && ri == 0 &&
                bl == 255 && bc == 255 && br == 0
                )
                return true;
            if (tl == 255 && tc == 255 && tr == 0 &&
                le == 255 && ce == 0 && ri == 0 &&
                bl == 255 && bc == 0 && br == 0
                )
                return true;
            if (tl == 255 && tc == 0 && tr == 0 &&
                le == 255 && ce == 0 && ri == 0 &&
                bl == 255 && bc == 0 && br == 0
                )
                return true;

            return false;
        }

        private bool IsHB(int tl, int tc, int tr, int le, int ce, int ri, int bl, int bc, int br)
        {
            if (tl == 0 && tc == 0 && tr == 0 &&
                            le == 255 && ce == 0 && ri == 255 &&
                            bl == 255 && bc == 255 && br == 255
                            )
                return true;
            if (tl == 0 && tc == 0 && tr == 0 &&
                le == 0 && ce == 0 && ri == 255 &&
                bl == 0 && bc == 255 && br == 255
                )
                return true;
            if (tl == 0 && tc == 0 && tr == 255 &&
                le == 0 && ce == 0 && ri == 255 &&
                bl == 255 && bc == 255 && br == 255
                )
                return true;
            if (tl == 0 && tc == 0 && tr == 0 &&
                le == 0 && ce == 0 && ri == 255 &&
                bl == 255 && bc == 255 && br == 255
                )
                return true;
            if (tl == 0 && tc == 0 && tr == 0 &&
                le == 255 && ce == 0 && ri == 0 &&
                bl == 255 && bc == 255 && br == 255
                )
                return true;
            if (tl == 0 && tc == 0 && tr == 0 &&
                le == 0 && ce == 0 && ri == 0 &&
                bl == 255 && bc == 255 && br == 255
                )
                return true;
            return false;
        }

        private bool IsHT(int tl, int tc, int tr, int le, int ce, int ri, int bl, int bc, int br)
        {
            if (tl == 255 && tc == 255 && tr == 255 &&
                            le == 255 && ce == 0 && ri == 255 &&
                            bl == 0 && bc == 0 && br == 0
                            )
                return true;
            if (tl == 255 && tc == 255 && tr == 0 &&
                le == 255 && ce == 0 && ri == 0 &&
                bl == 0 && bc == 0 && br == 0
                )
                return true;
            if (tl == 255 && tc == 255 && tr == 255 &&
                le == 255 && ce == 0 && ri == 0 &&
                bl == 255 && bc == 0 && br == 0
                )
                return true;
            if (tl == 255 && tc == 255 && tr == 255 &&
                le == 255 && ce == 0 && ri == 0 &&
                bl == 0 && bc == 0 && br == 0
                )
                return true;
            if (tl == 255 && tc == 255 && tr == 255 &&
                le == 0 && ce == 0 && ri == 255 &&
                bl == 0 && bc == 0 && br == 0
                )
                return true;
            if (tl == 255 && tc == 255 && tr == 255 &&
                le == 0 && ce == 0 && ri == 0 &&
                bl == 0 && bc == 0 && br == 0
                )
                return true;
            return false;
        }

        private bool CouldBeSpike(int tl, int tc, int tr, int le, int ce, int ri, int bl, int bc, int br)
        {
            if (tl == 255 && tc == 255 && tr == 255 &&
                le == 255 && ce == 0 && ri == 255 &&
                             bc == 0
                )
                return true;
            if (tl == 255 && tc == 255 && tr == 255 &&
                le == 255 && ce == 0 &&
                bl == 255 && br == 0
                )
                return true;
            if (tl == 255 && tc == 255 &&
                le == 255 && ce == 0 && ri == 0 &&
                bl == 255 && bc == 255
                )
                return true;
            if (tl == 255 && tr == 0 &&
                le == 255 && ce == 0 &&
                bl == 255 && bc == 255 && br == 255
                )
                return true;
            if (tc == 0 &&
                le == 255 && ce == 0 && ri == 255 &&
                bl == 255 && bc == 255 && br == 255
                )
                return true;
            if (tl == 0 && tr == 255 &&
                             ce == 0 && ri == 255 &&
                bl == 255 && bc == 255 && br == 255
                )
                return true;
            if (tc == 255 && tr == 255 &&
                le == 0 && ce == 0 && ri == 255 &&
                            bc == 255 && br == 255
                )
                return true;
            if (tl == 255 && tc == 255 && tr == 255 &&
                             ce == 0 && ri == 255 &&
                bl == 0 && br == 255
                )
                return true;

            return false;
        }

        private bool IsCorner(int tl, int tc, int tr, int le, int ce, int ri, int bl, int bc, int br)
        {
            if (tl == 255 && tc == 255 && //tr == 255 &&
                le == 255 && ce == 0 && ri == 0 &&
                /*bl == 255 &&*/ bc == 0 /*&& br == 255*/
                )
                return true;

            if (/*tl == 255 &&*/ tc == 0 && //tr == 255 &&
                le == 255 && ce == 0 && ri == 0 &&
                bl == 255 && bc == 255 //&& br == 255
                )
                return true;

            if (/*tl == 255 &&*/ tc == 0 && //tr == 255 &&
                le == 0 && ce == 0 && ri == 255 &&
                /*bl == 255 &&*/ bc == 255 && br == 255
                )
                return true;

            if (/*tl == 255 &&*/ tc == 255 && tr == 255 &&
                le == 0 && ce == 0 && ri == 255 &&
                /*bl == 255 &&*/ bc == 0 //&& br == 255
                )
                return true;

            return false;
        }

        private bool IsEnd(ImageMatrix matrix, int x, int y, out int x1, out int y1)
        {
            x1 = -1;
            y1 = -1;

            int tl = (x > 0 && y > 0) ? matrix[y - 1, x - 1] : 255;
            int tc = (y > 0) ? matrix[y - 1, x] : 255;
            int tr = (x < matrix.Width - 1 && y > 0) ? matrix[y - 1, x + 1] : 255;
            int cl = (x > 0) ? matrix[y, x - 1] : 255;
            int ce = matrix[y, x];
            int cr = (x < matrix.Width - 1) ? matrix[y, x + 1] : 255;
            int bl = (x > 0 && y < matrix.Height - 1) ? matrix[y + 1, x - 1] : 255;
            int bc = (y < matrix.Height - 1) ? matrix[y + 1, x] : 255;
            int br = (x < matrix.Width - 1 && y < matrix.Height - 1) ? matrix[y + 1, x + 1] : 255;

            if (tl == 255 && tc == 255 && tr == 255 &&
                cl == 255 && ce == 0 && cr == 255 &&
                bl == 255 && bc == 255 && br == 255
                )
            {
                x1 = x;
                y1 = y;
                return true;
            }
            if (tl == 0 && tc == 255 && tr == 255 &&
                cl == 255 && ce == 0 && cr == 255 &&
                bl == 255 && bc == 255 && br == 255
                )
            {
                x1 = x - 1;
                y1 = y - 1;
                return true;
            }
            if (tl == 255 && tc == 0 && tr == 255 &&
                cl == 255 && ce == 0 && cr == 255 &&
                bl == 255 && bc == 255 && br == 255
                )
            {
                x1 = x;
                y1 = y - 1;
                return true;
            }
            if (tl == 255 && tc == 255 && tr == 0 &&
                cl == 255 && ce == 0 && cr == 255 &&
                bl == 255 && bc == 255 && br == 255
                )
            {
                x1 = x + 1;
                y1 = y - 1;
                return true;
            }
            if (tl == 255 && tc == 255 && tr == 255 &&
                cl == 255 && ce == 0 && cr == 0 &&
                bl == 255 && bc == 255 && br == 255
                )
            {
                x1 = x + 1;
                y1 = y;
                return true;
            }
            if (tl == 255 && tc == 255 && tr == 255 &&
                cl == 255 && ce == 0 && cr == 255 &&
                bl == 255 && bc == 255 && br == 0
                )
            {
                x1 = x + 1;
                y1 = y + 1;
                return true;
            }
            if (tl == 255 && tc == 255 && tr == 255 &&
                cl == 255 && ce == 0 && cr == 255 &&
                bl == 255 && bc == 0 && br == 255
                )
            {
                x1 = x;
                y1 = y + 1;
                return true;
            }
            if (tl == 255 && tc == 255 && tr == 255 &&
                cl == 255 && ce == 0 && cr == 255 &&
                bl == 0 && bc == 255 && br == 255
                )
            {
                x1 = x - 1;
                y1 = y + 1;
                return true;
            }
            if (tl == 255 && tc == 255 && tr == 255 &&
                cl == 0 && ce == 0 && cr == 255 &&
                bl == 255 && bc == 255 && br == 255
                )
            {
                x1 = x - 1;
                y1 = y;
                return true;
            }
            return false;
        }

        private bool IsContinous(ImageMatrix matrix, int x0, int y0, int x, int y, out int x1, out int y1)
        {
            x1 = -1;
            y1 = -1;
            bool isBlack = false;
            if (matrix[y0, x0] == 0)
            {
                matrix[y0, x0] = 255;
                isBlack = true;
            }

            int tl = (x > 0 && y > 0) ? matrix[y - 1, x - 1] : 255;
            int tc = (y > 0) ? matrix[y - 1, x] : 255;
            int tr = (x < matrix.Width - 1 && y > 0) ? matrix[y - 1, x + 1] : 255;
            int cl = (x > 0) ? matrix[y, x - 1] : 255;
            int ce = matrix[y, x];
            int cr = (x < matrix.Width - 1) ? matrix[y, x + 1] : 255;
            int bl = (x > 0 && y < matrix.Height - 1) ? matrix[y + 1, x - 1] : 255;
            int bc = (y < matrix.Height - 1) ? matrix[y + 1, x] : 255;
            int br = (x < matrix.Width - 1 && y < matrix.Height - 1) ? matrix[y + 1, x + 1] : 255;

            if (tl == 0 && tc == 255 && tr == 255 &&
                cl == 255 && ce == 0 && cr == 255 &&
                bl == 255 && bc == 255 && br == 255
                )
            {
                x1 = x - 1;
                y1 = y - 1;
                if (isBlack)
                    matrix[y0, x0] = 0;
                return true;
            }
            if (tl == 255 && tc == 0 && tr == 255 &&
                cl == 255 && ce == 0 && cr == 255 &&
                bl == 255 && bc == 255 && br == 255
                )
            {
                x1 = x;
                y1 = y - 1;
                if (isBlack)
                    matrix[y0, x0] = 0;
                return true;
            }
            if (tl == 255 && tc == 255 && tr == 0 &&
                cl == 255 && ce == 0 && cr == 255 &&
                bl == 255 && bc == 255 && br == 255
                )
            {
                x1 = x + 1;
                y1 = y - 1;
                if (isBlack)
                    matrix[y0, x0] = 0;
                return true;
            }
            if (tl == 255 && tc == 255 && tr == 255 &&
                cl == 255 && ce == 0 && cr == 0 &&
                bl == 255 && bc == 255 && br == 255
                )
            {
                x1 = x + 1;
                y1 = y;
                if (isBlack)
                    matrix[y0, x0] = 0;
                return true;
            }
            if (tl == 255 && tc == 255 && tr == 255 &&
                cl == 255 && ce == 0 && cr == 255 &&
                bl == 255 && bc == 255 && br == 0
                )
            {
                x1 = x + 1;
                y1 = y + 1;
                if (isBlack)
                    matrix[y0, x0] = 0;
                return true;
            }
            if (tl == 255 && tc == 255 && tr == 255 &&
                cl == 255 && ce == 0 && cr == 255 &&
                bl == 255 && bc == 0 && br == 255
                )
            {
                x1 = x;
                y1 = y + 1;
                if (isBlack)
                    matrix[y0, x0] = 0;
                return true;
            }
            if (tl == 255 && tc == 255 && tr == 255 &&
                cl == 255 && ce == 0 && cr == 255 &&
                bl == 0 && bc == 255 && br == 255
                )
            {
                x1 = x - 1;
                y1 = y + 1;
                if (isBlack)
                    matrix[y0, x0] = 0;
                return true;
            }
            if (tl == 255 && tc == 255 && tr == 255 &&
                cl == 0 && ce == 0 && cr == 255 &&
                bl == 255 && bc == 255 && br == 255
                )
            {
                x1 = x - 1;
                y1 = y;
                if (isBlack)
                    matrix[y0, x0] = 0;
                return true;
            }
            return false;
        }

        #endregion
    }
}
