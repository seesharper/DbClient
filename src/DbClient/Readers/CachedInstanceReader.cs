namespace DbClient.Readers
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using Construction;

    /// <summary>
    /// An <see cref="IInstanceReader{T}"/> decorator that caches instances
    /// of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be cached.</typeparam>
    public class CachedInstanceReader<T> : IInstanceReader<T>
    {
        private readonly IInstanceReader<T> instanceReader;

        private readonly IKeyReader keyReader;

        private readonly IOneToManyMethodBuilder<T> oneToManyMethodBuilder;
        private readonly IInstanceReaderFactory instanceReaderFactory;
        private readonly ConcurrentDictionary<IStructuralEquatable, T> queryCache = new ConcurrentDictionary<IStructuralEquatable, T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedInstanceReader{T}"/> class.
        /// </summary>
        /// <param name="instanceReader">The <see cref="IInstanceReader{T}"/> that is responsible
        /// for reading an instance of <typeparamref name="T"/> from an <see cref="IDataRecord"/>.</param>
        /// <param name="keyReader">An instance of <typeparamref name="T"/>.</param>
        /// <param name="oneToManyMethodBuilder"></param>
        /// <param name="instanceReaderFactory">The <see cref="IInstanceReaderFactory"/> that is responsible for resolving <see cref="IInstanceReader{T}"/> instances.</param>
        public CachedInstanceReader(IInstanceReader<T> instanceReader, IKeyReader keyReader, IOneToManyMethodBuilder<T> oneToManyMethodBuilder, IInstanceReaderFactory instanceReaderFactory)
        {
            this.instanceReader = instanceReader;
            this.keyReader = keyReader;
            this.oneToManyMethodBuilder = oneToManyMethodBuilder;
            this.instanceReaderFactory = instanceReaderFactory;
        }

        /// <summary>
        /// Reads an instance of <typeparamref name="T"/> from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> from which to read an instance of <typeparamref name="T"/>.</param>
        /// <param name="currentPrefix">The current prefix.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        public T Read(IDataRecord dataRecord, string currentPrefix)
        {
            var instance = ReadInstance(dataRecord, currentPrefix);
            if (instance == null)
            {
                return default;
            }
            var method = oneToManyMethodBuilder.CreateMethod(dataRecord, currentPrefix);
            method?.Invoke(instance, dataRecord, instanceReaderFactory);
            return instance;
        }

        private T ReadInstance(IDataRecord dataRecord, string currentPrefix)
        {
            var key = keyReader.Read(typeof(T), dataRecord, currentPrefix);
            if (key == null)
            {
                return default;
            }

            IStructuralEquatable cacheKey = key;
            if (!string.IsNullOrWhiteSpace(currentPrefix))
            {
                cacheKey = (key, currentPrefix);
            }

            // Note the cache need to include the prefix
            if (!queryCache.TryGetValue(cacheKey, out T instance))
            {
                instance = instanceReader.Read(dataRecord, currentPrefix);
                queryCache.TryAdd(cacheKey, instance);
            }
            return instance;

        }
    }
}