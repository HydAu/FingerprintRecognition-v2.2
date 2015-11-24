/*
 * Created by: Milton Garcia Borroto (milton.garcia@gmail.com)
 *             Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */


using System.ComponentModel;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     The common interface for any experiment.
    /// </summary>
    public interface IExperiment
    {
        /// <summary>
        ///     Executes the experiment.
        /// </summary>
        void Execute();

        /// <summary>
        ///     Event that provides information about the experiment status.
        /// </summary>
        event ProgressChangedEventHandler StatusEvent;
    }

    /// <summary>
    ///     Used to evaluate matching algorithms based on the protocol of the Fingerprint Verification Competitions.
    /// </summary>
    public interface IMatchingExperiment : IExperiment
    {
        /// <summary>
        ///     The path where the resources are stored.
        /// </summary>
        string ResourcePath { get; set; }

        /// <summary>
        ///     The matcher to be evaluated.
        /// </summary>
        IMatcher Matcher { set; get; }

        /// <summary>
        ///     Used to provide the features used by the specified matcher.
        /// </summary>
        IResourceProvider ResourceProvider { set; get; }
    }
}