/*
 * Created by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 * Created: Thursday, October 25, 2007
 * Comments by: Miguel Angel Medina Pérez (migue.cu@gmail.com)
 */

namespace PatternRecognition.Core
{
    /// <summary>
    ///     Represents a boolean similarity comparison function.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A boolean similarity is a function that compares objects and returns true is both objects are equal, otherwise it returns false.
    ///     </para>
    /// </remarks>
    /// <typeparam name="T">
    ///     The type of the objects that can be compared.
    /// </typeparam>
    public interface IBooleanSimilarity<T>
    {
        /// <summary>
        ///     Compute the similarity between two objects.
        /// </summary>
        /// <param name="source">
        ///     An object to compare.
        /// </param>
        /// <param name="compareTo">
        ///     The object to compare with.
        /// </param>
        /// <returns>
        ///     The similarity boolean value.
        /// </returns>
        bool Compare(T source, T compareTo);
    }
}