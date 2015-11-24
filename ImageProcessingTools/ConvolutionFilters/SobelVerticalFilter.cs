/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

namespace ImageProcessingTools
{
    /// <summary>
    ///     A 3x3 Sobel filter for vertical edge detection.
    /// </summary>
    /// <remarks>This filter has the following matrix: {{1,0,-1},{2,0,-2},{1,0,-1}}.</remarks>
    /// <seealso cref="SobelVerticalFilter"/>
    public class SobelVerticalFilter : ConvolutionFilter
    {
        #region IConvolutionFilter Members

        ///<summary>
        ///     Initialize a <see cref="SobelVerticalFilter"/> with the matrix {{1,0,-1},{2,0,-2},{1,0,-1}}.
        ///</summary>
        public SobelVerticalFilter()
        {
            pixels = new int[3,3] {
                                   {1,0,-1},
                                   {2,0,-2},
                                   {1,0,-1}
                               };
        }

        /// <summary>
        ///     Gets the value 3 which is the height of the filter.
        /// </summary>
        public override int Height { get { return 3; } }

        /// <summary>
        ///     Gets the value 3 which is the width of the filter.
        /// </summary>
        public override int Width { get { return 3; } }

        /// <summary>
        ///     Gets the value 1 which is the factor to divide the value before assigning to the pixel.
        /// </summary>
        public override int Factor { get { return 1; } }

        #endregion
    }
}