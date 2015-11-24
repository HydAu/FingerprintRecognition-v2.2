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
    ///     An implementation of the <see cref="OrientationImage"/> extractor proposed by Ratha et al. in 1995.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is an implementation of the <see cref="OrientationImage"/> extractor proposed by Ratha et al. [1] in 1995.
    ///     </para>
    ///     <para>
    ///         We apply a 3x3 median filter to improve the computed <see cref="OrientationImage"/>.
    ///     </para>
    ///     <para>
    ///         In order to better deal with diverse fingerprint databases, we modified the algorithm proposed by Ratha et al. [1] to detect foreground. 
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
    public class Ratha1995OrImgExtractor : FeatureExtractor<OrientationImage>
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

            ImageMatrix Gx = yFilter.Apply(matrix);
            ImageMatrix Gy = xFilter.Apply(matrix);

            byte width = Convert.ToByte(image.Width / BlockSize);
            byte height = Convert.ToByte(image.Height / BlockSize);
            OrientationImage oi = new OrientationImage(width, height, BlockSize);
            for (int row = 0; row < height; row++)
                for (int col = 0; col < width; col++)
                {
                    int x, y;
                    oi.GetPixelCoordFromBlock(row, col, out x, out y);
                    int x0 = Math.Max(x - BlockSize / 2, 0);
                    int x1 = Math.Min(image.Width - 1, x + BlockSize / 2);
                    int y0 = Math.Max(y - BlockSize / 2, 0);
                    int y1 = Math.Min(image.Height - 1, y + BlockSize / 2);

                    int Gxy = 0, Gxx = 0, Gyy = 0;
                    for (int yi = y0; yi <= y1; yi++)
                        for (int xi = x0; xi <= x1; xi++)
                        {
                            Gxy += Gx[yi, xi] * Gy[yi, xi];
                            Gxx += Gx[yi, xi] * Gx[yi, xi];
                            Gyy += Gy[yi, xi] * Gy[yi, xi];
                        }

                    if (Gxx - Gyy == 0 && Gxy == 0)
                        oi[row, col] = OrientationImage.Null;
                    else
                    {
                        double angle = Angle.ToDegrees(Angle.ComputeAngle(Gxx - Gyy, 2 * Gxy));
                        angle = angle / 2 + 90;
                        if (angle > 180)
                            angle = angle - 180;
                        oi[row, col] = Convert.ToByte(Math.Round(angle));
                    }
                }

            RemoveBadBlocksVariance(oi, matrix);
            RemoveBadBlocks(oi);
            OrientationImage smoothed = SmoothOrImg(oi);
            return smoothed;
        }

        #region private

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

        private SobelHorizontalFilter xFilter = new SobelHorizontalFilter();

        private SobelVerticalFilter yFilter = new SobelVerticalFilter();

        private byte bSize = 16;

        #endregion

    }
}
