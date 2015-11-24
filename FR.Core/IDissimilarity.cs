/*
 * Created by: Milton Garcia Borroto
 * Created: 1/5/2007
 * Comments by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 */

namespace PatternRecognition.Core
{
    /// <summary>
    ///     Represents a dissimilarity comparison function.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A dissimilarity is a function that compares objects and returns a similarity degree of these objects. The lower returned value, the greater similarity.
    ///     </para>
    /// </remarks>
    /// <typeparam name="T">
    ///     The type of the objects that can be compared.
    /// </typeparam>
    public interface IDissimilarity <T> 
    {
        /// <summary>
        ///     Compute the dissimilarity between two objects.
        /// </summary>
        /// <param name="source">
        ///     One object to compare.
        /// </param>
        /// <param name="compareTo">
        ///     One object to compare with.
        /// </param>
        /// <returns>
        ///     The dissimilarity value.
        /// </returns>
        double Compare(T source, T compareTo);
    }
}