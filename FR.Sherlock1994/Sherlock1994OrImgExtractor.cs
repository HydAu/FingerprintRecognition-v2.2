/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
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
    ///     An implementation of the <see cref="OrientationImage"/> extractor proposed by Sherlock et al. in 1994.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is an implementation of the <see cref="OrientationImage"/> extractor proposed by Sherlock et al. [1] in 1994.
    ///     </para>
    ///     <para>
    ///         We apply a 3x3 median filter to improve the computed <see cref="OrientationImage"/>.
    ///     </para>
    ///     <para>
    ///         References:
    ///     </para>
    ///     <para>
    ///         <list type="number">
    ///             <item>
    ///                B. G. Sherlock, D. M. Monro, and K. Millard, "Fingerprint enhancement by directional Fourier filtering," IEE Proceedings Vision Image and Signal Processing, vol. 141, no. 2, pp. 87-94, 1994.
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public class Sherlock1994OrImgExtractor : FeatureExtractor<OrientationImage>
    {
        /// <summary>
        ///     The block size in pixels.
        /// </summary>
        public byte BlockSize
        {
            get { return bSize; }
            set { bSize = value; }
        }

        /// <summary>
        ///     Extract an orientation image from the specified bitmap image.
        /// </summary>
        /// <param name="image">The source bitmap image to extract features from.</param>
        /// <returns>The features extracted from the specified bitmap image.</returns>
        public override OrientationImage ExtractFeatures(Bitmap image)
        {
            ImageMatrix matrix = new ImageMatrix(image);

            byte width = Convert.ToByte(image.Width / BlockSize);
            byte height = Convert.ToByte(image.Height / BlockSize);
            OrientationImage oi = new OrientationImage(width, height, BlockSize);
            for (int row = 0; row < height; row++)
                for (int col = 0; col < width; col++)
                {
                    int x, y;
                    oi.GetPixelCoordFromBlock(row, col, out x, out y);

                    double maxVariation = 0;
                    double bestAngle = 0;
                    for (int i = 0; i < 16; i++)
                    {
                        double currAngle = i * Math.PI / 16;
                        double currVariation = GetVariation(currAngle, y, x, matrix);
                        if (currVariation > maxVariation)
                        {
                            maxVariation = currVariation;
                            bestAngle = currAngle;
                        }
                    }

                    if (maxVariation != 0)
                    {
                        double angle = Angle.ToDegrees(bestAngle);
                        oi[row, col] = Convert.ToByte(Math.Round(angle));
                    }
                    else
                        oi[row, col] = OrientationImage.Null;
                }

            RemoveBadBlocksVariance(oi, matrix);
            RemoveBadBlocks(oi);
            OrientationImage smoothed = SmoothOrImg(oi);
            return smoothed;
        }

        #region private

        private int GetVariation(double angle, int y, int x, ImageMatrix matrix)
        {
            double orthogonalAngle = angle + Math.PI / 2;

            int maxLength = BlockSize / 2;
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

            int variation = 0;
            for (int i = 0; i < projection.Length - 1; i++)
                variation += Math.Abs(projection[i + 1] - projection[i]);

            return variation;
        }

        private void RemoveBadBlocksVariance(OrientationImage oi, ImageMatrix matrix)
        {
            int maxLength = oi.WindowSize / 2;
            int[,] varianceMatrix = new int[oi.Height, oi.Width];
            double max = 0;
            double min = double.MaxValue;
            for (int row = 0; row < oi.Height; row++)
                for (int col = 0; col < oi.Width; col++)
                {
                    int x, y;
                    oi.GetPixelCoordFromBlock(row, col, out x, out y);

                    // Computing Average
                    int sum = 0;
                    int count = 0;
                    for (int xi = x - maxLength; xi < x + maxLength; xi++)
                        for (int yi = y - maxLength; yi < y + maxLength; yi++)
                            if (xi >= 0 && xi < matrix.Width && yi >= 0 && yi < matrix.Height)
                            {
                                sum += matrix[yi, xi];
                                count++;
                            }
                    double avg = 1.0 * sum / count;

                    // Computing Variance
                    double sqrSum = 0;
                    for (int xi = x - maxLength; xi < x + maxLength; xi++)
                        for (int yi = y - maxLength; yi < y + maxLength; yi++)
                            if (xi >= 0 && xi < matrix.Width && yi >= 0 && yi < matrix.Height)
                            {
                                double diff = matrix[yi, xi] - avg;
                                sqrSum += diff * diff;
                            }
                    varianceMatrix[row, col] = Convert.ToInt32(Math.Round(sqrSum / (count - 1)));

                    // Computing de max variance
                    if (varianceMatrix[row, col] > max)
                        max = varianceMatrix[row, col];
                    if (varianceMatrix[row, col] < min)
                        min = varianceMatrix[row, col];
                }

            for (int row = 0; row < oi.Height; row++)
                for (int col = 0; col < oi.Width; col++)
                    varianceMatrix[row, col] = Convert.ToInt32(Math.Round(254.0 * (varianceMatrix[row, col] - min) / (max - min)));

            const int t = 15;
            for (int row = 0; row < oi.Height; row++)
                for (int col = 0; col < oi.Width; col++)
                    if (!oi.IsNullBlock(row, col) && varianceMatrix[row, col] <= t)
                        oi[row, col] = OrientationImage.Null;
        }

        private void RemoveBadBlocks(OrientationImage oi)
        {
            int[,] neighborsMatrix = new int[oi.Height, oi.Width];
            for (int row0 = 0; row0 < oi.Height; row0++)
                for (int col0 = 0; col0 < oi.Width; col0++)
                    if (oi[row0, col0] != OrientationImage.Null)
                    {
                        int lowRow = Math.Max(0, row0 - 1);
                        int lowCol = Math.Max(0, col0 - 1);
                        int highRow = Math.Min(row0 + 1, oi.Height - 1);
                        int highCol = Math.Min(col0 + 1, oi.Width - 1);
                        for (int row1 = lowRow; row1 <= highRow; row1++)
                            for (int col1 = lowCol; col1 <= highCol; col1++)
                                if (oi[row1, col1] != OrientationImage.Null)
                                    neighborsMatrix[row0, col0]++;
                    }

            for (int row0 = 0; row0 < oi.Height; row0++)
                for (int col0 = 0; col0 < oi.Width; col0++)
                    if (oi[row0, col0] != OrientationImage.Null)
                    {
                        bool shouldRemove = true;
                        int lowRow = Math.Max(0, row0 - 1);
                        int lowCol = Math.Max(0, col0 - 1);
                        int highRow = Math.Min(row0 + 1, oi.Height - 1);
                        int highCol = Math.Min(col0 + 1, oi.Width - 1);
                        for (int row1 = lowRow; row1 <= highRow && shouldRemove; row1++)
                            for (int col1 = lowCol; col1 <= highCol && shouldRemove; col1++)
                                if (neighborsMatrix[row1, col1] == 9)
                                    shouldRemove = false;
                        if (shouldRemove)
                            oi[row0, col0] = OrientationImage.Null;
                    }
        }

        /// <summary>
        ///     Apply 3x3 median filter to the specified <see cref="OrientationImage"/>.
        /// </summary>
        /// <param name="img">The image to be smoothed.</param>
        /// <returns>The smoothed image.</returns>
        private OrientationImage SmoothOrImg(OrientationImage img)
        {
            OrientationImage smoothed = new OrientationImage(img.Width, img.Height, img.WindowSize);
            double xSum, ySum, xAvg, yAvg, angle;
            int count;
            byte wSize = 3;
            for (int row = 0; row < img.Height; row++)
                for (int col = 0; col < img.Width; col++)
                    if (!img.IsNullBlock(row, col))
                    {
                        xSum = ySum = count = 0;
                        for (int y = row - wSize / 2; y <= row + wSize / 2; y++)
                            for (int x = col - wSize / 2; x <= col + wSize / 2; x++)
                                if (y >= 0 && y < img.Height && x >= 0 && x < img.Width && !img.IsNullBlock(y, x))
                                {
                                    angle = img.AngleInRadians(y, x);
                                    xSum += Math.Cos(2 * angle);
                                    ySum += Math.Sin(2 * angle);
                                    count++;
                                }
                        if (count == 0 || (xSum == 0 && ySum == 0))
                            smoothed[row, col] = OrientationImage.Null;
                        else
                        {
                            xAvg = xSum / count;
                            yAvg = ySum / count;
                            angle = Angle.ToDegrees(Angle.ComputeAngle(xAvg, yAvg)) / 2;

                            smoothed[row, col] = Convert.ToByte(Math.Round(angle));
                        }
                    }
                    else
                        smoothed[row, col] = OrientationImage.Null;

            return smoothed;
        }

        private byte bSize = 16;

        #endregion

    }
}
