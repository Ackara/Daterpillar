using Acklann.Daterpillar.Configuration;
using System;
using System.IO;

namespace Acklann.Daterpillar.Compilation
{
    public abstract class SchemaWriter : IDisposable
    {
        protected SchemaWriter(TextWriter writer)
        {
            Writer = writer;
        }

        protected readonly TextWriter Writer;

        // ==================== CREATE ==================== //

        public virtual void Create(Schema schema)
        {
            foreach (Table table in schema.Tables)
                Create(table);

            foreach (Script script in schema.Scripts)
                Create(script);
        }

        public virtual void Create(Table table)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Create(Column column)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Create(ForeignKey foreignKey)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Create(Index index)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Create(Script script)
        {
            throw new System.NotImplementedException();
        }

        // ==================== DROP ==================== //

        public virtual void Drop(Schema schema)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Drop(Table table)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Drop(Column column)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Drop(ForeignKey foreignKey)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Drop(Index index)
        {
            throw new System.NotImplementedException();
        }

        // ==================== ALTER ==================== //

        public virtual void Alter(Table oldTable, Table newTable)
        {
            throw new System.NotImplementedException();
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Writer?.Dispose();
            }
        }

        #endregion IDisposable

        #region Private Members

        

        #endregion Private Members
    }
}