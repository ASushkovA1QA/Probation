using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WebdriverFramework.Framework.Util
{
    /// <summary>
    /// Utility for work with Embedded Resources
    /// </summary>
    public static class EmbeddedResourceUtility
    {
        /// <summary>
        /// get file contents using path to the folder
        /// </summary>
        /// <param name="assembly">assembly</param>
        /// <param name="folderPath">path to the folder that contains embedded resource</param>
        /// <param name="fileName">name of file</param>
        /// <returns>file contents as string</returns>
        public static string GetFileContents(Assembly assembly, string folderPath, string fileName)
        {
            var resourcePath = BuildResourcePath(assembly.GetName().Name, folderPath, fileName);

            var stream = assembly.GetManifestResourceStream(resourcePath);

            if (stream == null)
            {
                throw new InvalidOperationException(String.Format("Assembly {0} doesn't contain embedded resource {1}. Check that {1} is marked as EmbeddedResource.",
                    assembly.FullName, resourcePath));
            }

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// get file content using namespace
        /// </summary>
        /// <param name="assembly">assembly</param>
        /// <param name="namespace">namespace</param>
        /// <param name="fileName">name of file</param>
        /// <returns>file contents as string</returns>
        public static string GetFileContentsFromNamespace(Assembly assembly, string @namespace, string fileName)
        {
            var resourcePath = BuildResourcePath(@namespace, fileName);
            var stream = assembly.GetManifestResourceStream(resourcePath);

            if (stream == null)
            {
                throw new InvalidOperationException(String.Format("Assembly {0} doesn't contain embedded resource {1}. Check that {1} is marked as EmbeddedResource.",
                    assembly.FullName, resourcePath));
            }

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// get file contents using path to the folder and assembly name
        /// </summary>
        /// <param name="assemblyName">name of assembly</param>
        /// <param name="folderPath">path to the folder that contains embedded resource</param>
        /// <param name="fileName">name of file</param>
        /// <returns>file contents as string</returns>
        public static string GetFileContents(string assemblyName, string folderPath, string fileName)
        {
            var assembly = GetAssemblyByName(assemblyName);
            return GetFileContents(assembly, folderPath, fileName);
        }

        /// <summary>
        /// check for embedded resource
        /// </summary>
        /// <param name="assembly">assembly</param>
        /// <param name="folderPath">path to the folder that contains embedded resource</param>
        /// <param name="fileName">name of file</param>
        /// <returns></returns>
        public static bool HasEmbeddedResource(Assembly assembly, string folderPath, string fileName)
        {
            var resourcePath = BuildResourcePath(assembly.GetName().Name, folderPath, fileName);
            var stream = assembly.GetManifestResourceStream(resourcePath);

            if (stream == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// check for embedded resource
        /// </summary>
        /// <param name="assemblyName">name of assembly</param>
        /// <param name="folderPath">path to the folder that contains embedded resource</param>
        /// <param name="fileName">name of file</param>
        /// <returns></returns>
        public static bool HasEmbeddedResource(string assemblyName, string folderPath, string fileName)
        {
            var assembly = GetAssemblyByName(assemblyName);
            return HasEmbeddedResource(assembly, folderPath, fileName);
        }

        private static string BuildResourcePath(string assemblyName, string folderPath, string fileName)
        {
            var resourcePath = String.Format("{0}.{1}.{2}", assemblyName, folderPath, fileName);
            return resourcePath;
        }

        private static string BuildResourcePath(string @namespace, string fileName)
        {
            var resourcePath = String.Format("{0}.{1}", @namespace, fileName);
            return resourcePath;
        }

        private static Assembly GetAssemblyByName(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == assemblyName) ?? AppDomain.CurrentDomain.Load(assemblyName);
        }
    }
}
