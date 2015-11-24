/*
 * Created by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 * Created: 1/5/2007
 * Comments by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 */

namespace PatternRecognition.Core
{
    /// <summary>
    ///     Represents a similarity comparison function.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A similarity is a function that compares objects and returns a similarity degree of these objects. The higher returned value, the greater similarity.
    ///     </para>
    /// </remarks>
    /// <typeparam name="T">
    ///     The type of the objects that can be compared.
    /// </typeparam>
    public interface ISimilarity <T>
    {
        /// <summary>
        ///     Compute the similarity between two objects.
        /// </summary>
        /// <param name="source">
        ///     One object to compare.
        /// </param>
        /// <param name="compareTo">
        ///     The object to compare with.
        /// </param>
        /// <returns>
        ///     The similarity value.
        /// </returns>
        double Compare(T source, T compareTo);
    }
}