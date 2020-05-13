using System.Collections.Generic;
using System.Linq;

namespace Common.Extension
{
    public static class ListExtension
    {
        public static List<List<T>> Split<T>(this List<T> collection, int size)
        {
            var chunks = new List<List<T>>();
            var chunkCount = collection.Count() / size;

            if (collection.Count % size > 0)
                chunkCount++;

            for (var i = 0; i < chunkCount; i++)
                chunks.Add(collection.Skip(i * size).Take(size).ToList());

            return chunks;
        }
    }
}
