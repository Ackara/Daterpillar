using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace Acklann.Daterpillar.Linq
{
    public abstract class EntityBase : IEntity
    {
        private EntityConstructor _ctor = null;

        public static EntityConstructor CreateConstructor(Type entityType)
        {
            ConstructorInfo ctor = entityType.GetConstructor(new Type[0]);

            var method = new DynamicMethod((entityType.Name + "Ctor"), entityType, new Type[0], typeof(Activator));
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ret);

            return (EntityConstructor)method.CreateDelegate(typeof(EntityConstructor));
        }

        public abstract string GetTableName();

        public abstract string[] GetColumnList();

        public abstract object[] GetValueList();

        public EntityConstructor GetConstructor()
        {
            if (_ctor == null) _ctor = CreateConstructor(GetType());
            return _ctor;
        }

        public abstract void Load(IDataRecord record);
    }
}