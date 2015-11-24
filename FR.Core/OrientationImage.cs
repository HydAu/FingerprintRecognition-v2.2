/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Represents an orientation image.
    /// </summary>
    [Serializable]
    public class OrientationImage
    {
        /// <summary>
        ///     Initialize an <see cref="OrientationImage"/> with the specified width, height, orientation values, and window size.
        /// </summary>
        /// <param name="width">The width of the orientation image.</param>
        /// <param name="height">The height of the orientation image.</param>
        /// <param name="orientations">The orientation values.</param>
        /// <param name="wSize">The window size of the orientation image.</param>
        public OrientationImage(byte width, byte height, byte[,] orientations, byte wSize)
        {
            Width = width;
            Height = height;
            WindowSize = wSize;
            this.orientations = orientations;
        }

        /// <summary>
        ///     Initialize an <see cref="OrientationImage"/> with the specified width, height, orientation values, and window size.
        /// </summary>
        /// <param name="width">The width of the orientation image.</param>
        /// <param name="height">The height of the orientation image.</param>
        /// <param name="wSize">The window size of the orientation image.</param>
        public OrientationImage(byte width, byte height, byte wSize)
        {
            Width = width;
            Height = height;
            WindowSize = wSize;
            orientations = new byte[height, width];
        }

        /// <summary>
        ///     Gets or sets the orientation in degrees of the block in the specified coordinates.
        /// </summary>
        /// <param name="row">The row of the specified block.</param>
        /// <param name="col">The column of the specified block.</param>
        /// <returns>
        ///     The angle in degrees of the specified block.
        /// </returns>
        public byte this[int row, int col]
        {
            get { return orientations[row, col]; }
            set { orientations[row, col] = value; }
        }

        /// <summary>
        ///     Gets the orientation in radians of the block in the specified coordinates.
        /// </summary>
        /// <param name="row">The row of the specified block.</param>
        /// <param name="col">The column of the specified block.</param>
        /// <returns>
        ///     The angle in radians of the specified block.
        /// </returns>
        public double AngleInRadians(int row, int col)
        {
            return IsNullBlock(row, col) ? double.NaN : orientations[row, col] * Math.PI / 180;
        }

        /// <summary>
        ///     Gets or sets the width of the orientation image.
        /// </summary>
        public byte Width { set; get; }

        /// <summary>
        ///     Gets or sets the height of the orientation image.
        /// </summary>
        public byte Height { set; get; }

        /// <summary>
        ///     Gets or sets the window size of the orientation image.
        /// </summary>
        public byte WindowSize { set; get; }

        /// <summary>
        ///     Constains the numerical value of undefined blocks.
        /// </summary>
        public const byte Null = 255;

        /// <summary>
        ///     Determines whether the block with the specified coordinates is undefined.
        /// </summary>
        /// <param name="row">The row coordinate of the block.</param>
        /// <param name="col">The column coordinate of the block.</param>
        /// <returns>
        ///     True if the block with the specified coordinates is undefined; otherwise, false.
        /// </returns>
        public bool IsNullBlock(int row, int col)
        {
            return orientations[row, col] == Null;
        }

        /// <summary>
        ///     Gets, in out parameters, the block coordinates where the pixel, with specified coordinates, resides.
        /// </summary>
        /// <param name="x">The x component of the pixel.</param>
        /// <param name="y">The y component of the pixel.</param>
        /// <param name="row">The row of the returned block.</param>
        /// <param name="col">The column of the returned block.</param>
        public void GetBlockCoordFromPixel(double x, double y, out int row, out int col)
        {
            row = Convert.ToInt16(Math.Round((y - 1.0 * WindowSize / 2) / (1.0 * WindowSize)));
            col = Convert.ToInt16(Math.Round((x - 1.0 * WindowSize / 2) / (1.0 * WindowSize)));
        }

        /// <summary>
        ///     Gets, in out parameters, the pixel coordinates of the center of the block with the specified coordinates.
        /// </summary>
        /// <param name="row">The row of the block.</param>
        /// <param name="col">The column of the block.</param>
        /// <param name="x">The x component of the returned pixel.</param>
        /// <param name="y">The y component of the returned pixel.</param>
        public void GetPixelCoordFromBlock(int row, int col, out double x, out double y)
        {
            x = col * WindowSize + 1.0 * WindowSize / 2;
            y = row * WindowSize + 1.0 * WindowSize / 2;
        }

        /// <summary>
        ///     Gets, in out parameters, the pixel coordinates of the center of the block with the specified coordinates.
        /// </summary>
        /// <param name="row">The row of the block.</param>
        /// <param name="col">The column of the block.</param>
        /// <param name="x">The x component of the returned pixel.</param>
        /// <param name="y">The y component of the returned pixel.</param>
        public void GetPixelCoordFromBlock(int row, int col, out int x, out int y)
        {
            x = col * WindowSize + WindowSize / 2;
            y = row * WindowSize + WindowSize / 2;
        }

        /// <summary>
        ///     Gets, in out parameters, the coordinates of the block with the specified index.
        /// </summary>
        /// <param name="blockIdx">The block index.</param>
        /// <param name="row">The row of the returned block.</param>
        /// <param name="col">The column of the returned block.</param>
        public void GetBlockCoordFromIdx(int blockIdx, out int row, out int col)
        {
            row = Math.DivRem(blockIdx, Width, out col);
        }

        /// <summary>
        ///    Gets the index of the block with the specified coordinates.
        /// </summary>
        /// <param name="row">The row of the block.</param>
        /// <param name="col">The column of the block.</param>
        /// <returns>
        ///     The index of the block with the specified coordinates.
        /// </returns>
        public int GetBlockIdxFromCoord(int row, int col)
        {
            return row * Width + col;
        }

        #region private fields

        private readonly byte[,] orientations;

        #endregion
    }
}