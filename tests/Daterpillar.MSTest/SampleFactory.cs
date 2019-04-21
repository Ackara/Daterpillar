using Acklann.Mockaroo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar
{
    partial class Sample
    {
        public static IEnumerable<T> CreateInstances<T>(int take)
        {
            const int max = 100;
            if (take > max) throw new ArgumentOutOfRangeException(nameof(take), $"{nameof(take)} cannot be greater than {max}.");

            var path = Path.Combine(Path.GetTempPath(), nameof(Mockaroo), typeof(T).Name);
            var repository = new MockarooRepository<T>(Config.MockarooKey, max, path);
            return repository.Take(take);
        }
    }
}