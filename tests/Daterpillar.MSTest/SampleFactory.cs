using Acklann.Daterpillar.Prototyping;
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
            const int max = 500;
            if (take > max) throw new ArgumentOutOfRangeException(nameof(take), $"{nameof(take)} cannot be greater than {max}.");

            var path = Path.Combine(Path.GetTempPath(), nameof(Daterpillar));
            var repository = new MockarooRepository<T>(Config.MockarooKey, max, path);
            EditSchema(repository);
            return repository.Take(take);
        }

        private static void EditSchema<T>(MockarooRepository<T> repository)
        {
            switch (repository.Schema)
            {
                case Schema<Contact> contact:
                    contact.Replace(x => x.Name, DataType.UserName);
                    contact.Replace(x => x.Email, DataType.EmailAddress);
                    break;
            }
            repository.Save();
        }
    }
}