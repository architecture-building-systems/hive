using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Hive.IO.Util
{
    /// <summary>
    ///     A helper class for de-serializing JSON resource files from the assembly.
    /// </summary>
    public static class JsonResource
    {
        public static T ReadRecords<T>(string resourceName, ref T result)
        {
            if (result == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var serializer = new JsonSerializer();
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (var streamReader =
                        new StreamReader(
                            stream ?? throw new InvalidOperationException(
                                $"Could not find manifest resource '{resourceName}'")))
                    {
                        using (var jsonTextReader = new JsonTextReader(streamReader))
                        {
                            result = serializer.Deserialize<T>(jsonTextReader);
                        }
                    }
                }
            }

            return result;
        }
    }
}