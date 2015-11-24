/*
 * Created by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created:
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using PatternRecognition.FingerprintRecognition.Core;
using PatternRecognition.ROC;

namespace PatternRecognition.FingerprintRecognition.Experiments
{
    /// <summary>
    ///     Allows testing a fingerprint matching algorithm in database DB2_A from FVC2004.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This experiment uses the evaluation protocol of the Fingerprint Verification Competitions [1]. The performance indicators computed in this experiments are: EER(%), FMR100(%), FMR1000(%), ZeroFMR(%), Time(ms) and ROC curves. This indicators are saved in a file which name is formed by the name of the matching algorithm concatenated with ".Summary.csv". This file is saved in a folder by the name of "Results" in the same folder where the fingerprints are stored. Two more files are saved, one file containing the false fingerprint matches and the other containing the false not matches.
    ///     </para>
    ///     <para>
    ///         In order to execute the experiment, you must set the properties <see cref="Matcher"/>, <see cref="ResourceProvider"/> and <see cref="ResourcePath"/>. Optionally, you can set the property <see cref="StatusEvent"/> in order to get updates concerning the status of the experiment.
    ///     </para>
    ///     <para>
    ///         References:
    ///     </para>
    ///     <para>
    ///         <list type="number">
    ///             <item>
    ///                R. Cappelli, D. Maio, D. Maltoni, J. L. Wayman, and A. K. Jain, "Performance evaluation of fingerprint verification systems," IEEE Transactions on Pattern Analysis and Machine Intelligence, vol. 28, pp. 3-18, 2006.
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public class FVC2004_DB2_A : FVC2004_DB_A
    {
       
    }
}
