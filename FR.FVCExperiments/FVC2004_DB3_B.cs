/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created:
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.Experiments
{
    /// <summary>
    ///     Allows testing a fingerprint matching algorithm in database DB3_B from FVC2004.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The performance indicators computed in this experiments are: EER(%), FMR100(%), FMR1000(%), ZeroFMR(%), Time(ms) and ROC curves. This indicators are saved in a file which name is formed by the name of the matching algorithm concatenated with ".Summary.csv". This file is saved in a folder by the name of "Results" in the same folder where the fingerprints are stored. Two more files are saved, one file containing the false matching fingerprints and the other containing the false not matching fingerprints.
    ///     </para>
    ///     <para>
    ///         In order to execute the experiment, you must set the properties <see cref="Matcher{FeatureType}"/>, <see cref="ResourceProvider"/> and <see cref="ResourcePath"/>. Optionally, you can set the property <see cref="StatusEvent"/> in order to get updates concerning the status of the experiment.
    ///     </para>
    /// </remarks>
    public class FVC2004_DB3_B : FVC_DB_B 
    {

    }
}
