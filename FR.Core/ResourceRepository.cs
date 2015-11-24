/*
 * Created by: Milton García Borroto (milton.garcia@gmail.com)
 *             Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 * Created: 
 * Comments by: Miguel Angel Medina Pérez (miguel.medina.perez@gmail.com)
 */

using System.IO;
using PatternRecognition.FingerprintRecognition.Core;

namespace PatternRecognition.FingerprintRecognition.Core
{
    /// <summary>
    ///     Used to store and retrieve resources.
    /// </summary>
    public class ResourceRepository
    {
        /// <summary>
        ///     Gets or sets the path where resources are stored.
        /// </summary>
        public string ResourcePath
        {
            set
            {
                if (value.EndsWith(@"\"))
                    resourceBasePath = value;
                else
                    resourceBasePath = value + @"\";
            }
            get { return resourceBasePath; }
        }

        /// <summary>
        ///     Initialize a <see cref="ResourceRepository"/> with the specified path.
        /// </summary>
        /// <param name="resourcePath">The path where resources are stored.</param>
        public ResourceRepository(string resourcePath)
        {
            ResourcePath = resourcePath;
        }

        /// <summary>
        ///     Gets the full path where resource with the specified name is found.
        /// </summary>
        /// <param name="resourceName">The name of the resource which full path is requested.</param>
        /// <returns>The full path where resource with the specified name is found.</returns>
        public string GetFullPath(string resourceName)
        {
            return ResourcePath + resourceName.Replace('/', '\\');
        }

        /// <summary>
        ///     Retrieves a byte array containing the resource with the specified name.
        /// </summary>
        /// <param name="resourceName">The name of the resource which is being requested.</param>
        /// <returns>A byte array containing the requested resource.</returns>
        public byte[] RetrieveResource(string resourceName)
        {
            string FullPath = GetFullPath(resourceName);
            if (File.Exists(FullPath))
                return File.ReadAllBytes(FullPath);
            return null;
        }

        /// <summary>
        ///     Retrieves an object containing the resource with the specified name.
        /// </summary>
        /// <param name="resourceName">The name of the resource which is being requested.</param>
        /// <returns>An object containing the requested resource.</returns>
        public object RetrieveObjectResource(string resourceName)
        {
            string FullPath = GetFullPath(resourceName);
            if (File.Exists(FullPath))
                return BinarySerializer.Deserialize(FullPath);
            return null;
        }

        /// <summary>
        ///     Store the specified byte array with the specified resource name.
        /// </summary>
        /// <param name="resourceName">The name of the resource to be stored.</param>
        /// <param name="resource">The byte array containing the resource to be stored.</param>
        public void StoreResource(string resourceName, byte[] resource)
        {
            string FullPath = GetFullPath(resourceName);
            Directory.CreateDirectory(Path.GetDirectoryName(FullPath));
            File.WriteAllBytes(FullPath, resource);
        }

        /// <summary>
        ///     Store the specified object with the specified resource name.
        /// </summary>
        /// <param name="resourceName">The name of the resource to be stored.</param>
        /// <param name="resource">The object containing the resource to be stored.</param>
        public void StoreResource(string resourceName, object resource)
        {
            string FullPath = GetFullPath(resourceName);
            Directory.CreateDirectory(Path.GetDirectoryName(FullPath));
            BinarySerializer.Serialize(resource, FullPath);
        }

        /// <summary>
        ///     Determines whether the resource with the specified name exits.
        /// </summary>
        /// <param name="resourceName">The name of the resource.</param>
        /// <returns>True if the resource with the specified name exits; otherwise, false.</returns>
        public bool ResourceExists(string resourceName)
        {
            string FullPath = GetFullPath(resourceName);
            return File.Exists(FullPath);
        }

        private string resourceBasePath;
    }
}