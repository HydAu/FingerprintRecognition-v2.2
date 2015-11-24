/*
 * Created by: Milton García Borroto (milton.garcia@gmail.com) and 
 *             Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)             
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Represents a non-generic fingerprint resource provider.
    /// </summary>
    /// <remarks>
    ///     A fingerprint resource is any information related to a fingerprint. A resource is identified by a string label, and this label is formed by the fingerprint name and the signature of the resource provider. Consider to make all the resources persistent in order to save computation time.
    /// </remarks>
    /// <seealso cref="IsResourcePersistent"/>
    public interface IResourceProvider
    {
        /// <summary>
        ///     Gets the resource for the specified fingerprint through the specified <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which resource is being retrieved.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <returns>The retrieved resource.</returns>
        object GetResource(string fingerprint, ResourceRepository repository);

        /// <summary>
        ///     Gets the signature of the resource provider.
        /// </summary>
        /// <returns>The signature of the resource provider.</returns>
        string GetSignature();

        /// <summary>
        ///     Determines whether the resource provider is persistent.
        /// </summary>
        /// <returns>True if the resource provider is persistent; otherwise, false.</returns>
        bool IsResourcePersistent();
    }

    /// <summary>
    ///     Represents a fingerprint resource provider.
    /// </summary>
    /// <remarks>
    ///     A fingerprint resource is any information related to a fingerprint. A resource is identified by a string label, and this label is formed by the fingerprint name and the signature of the resource provider. Consider to make all the resources persistent in order to save computation time.
    /// </remarks>
    /// <typeparam name="ResourceType">
    ///     The type of the resource that can be retrieved through this interface.
    /// </typeparam>
    public interface IResourceProvider<ResourceType> : IResourceProvider
    {
        /// <summary>
        ///     Gets the resource for the specified fingerprint through the specified <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which resource is being retrieved.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <returns>The retrieved resource.</returns>
        new ResourceType GetResource(string fingerprint, ResourceRepository repository);
    }

    /// <summary>
    ///     Provides a base class for implementations of the <see cref="IResourceProvider&lt;ResourceType&gt;"/> generic interface.
    /// </summary>
    /// <typeparam name="ResourceType">
    ///     The type of the resource that can be retrieved through this class.
    /// </typeparam>
    public abstract class ResourceProvider<ResourceType> : IResourceProvider<ResourceType> where ResourceType : class
    {
        /// <summary>
        ///     Gets the signature of the resource provider.
        /// </summary>
        /// <returns>The signature of the resource provider.</returns>
        public abstract string GetSignature();

        /// <summary>
        ///     Determines whether the resource provider is persistent.
        /// </summary>
        /// <returns>True if the resource provider is persistent; otherwise, false.</returns>
        public abstract bool IsResourcePersistent();

        /// <summary>
        ///     Gets the resource for the specified fingerprint through the specified <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which resource is being retrieved.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <returns>The retrieved resource.</returns>
        object IResourceProvider.GetResource(string fingerprint, ResourceRepository repository)
        {
            return GetResource(fingerprint, repository);
        }

        /// <summary>
        ///     Gets the resource for the specified fingerprint through the specified <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which resource is being retrieved.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <returns>The retrieved resource.</returns>
        public ResourceType GetResource(string fingerprint, ResourceRepository repository)
        {
            bool isPersistent = IsResourcePersistent();
            string resourceName =
                string.Format("{0}.{1}", fingerprint, GetSignature());
            if (isPersistent && repository.ResourceExists(resourceName))
                return repository.RetrieveObjectResource(resourceName) as ResourceType;

            ResourceType resource = Extract(fingerprint, repository);
            if (resource == null)
                return null;

            if (isPersistent)
                repository.StoreResource(resourceName, resource);
            return resource;
        }

        /// <summary>
        ///     When implemented in a derived class, extracts the requested resource from the specified fingerprint.
        /// </summary>
        /// <param name="fingerprint">The fingerprint which resource is being extracted.</param>
        /// <param name="repository">The object used to store and retrieve resources.</param>
        /// <returns>The extracted resource.</returns>
        protected abstract ResourceType Extract(string fingerprint, ResourceRepository repository);

        /// <summary>
        ///     The provider used to retrieve the fingerprint image.
        /// </summary>
        protected readonly FingerprintImageProvider imageProvider = new FingerprintImageProvider();
    }
}