using Acklann.Daterpillar.Configuration;

namespace Acklann.Daterpillar
{
    static partial class TestData
    {
        public static Schema CreateInstance()
        {
            var schema = new Schema();
            schema.Add(new Table("song",
                new Column("Id", new DataType(SchemaType.INT), autoIncrement: true),
                new Column("Name", new DataType(SchemaType.VARCHAR, 64)),
                new Column("Length", new DataType(SchemaType.INT)),
                new Column("Genre", new DataType(SchemaType.VARCHAR, 64)),
                new Column("Artist", new DataType(SchemaType.VARCHAR, 64)),
                new Column("Album", new DataType(SchemaType.VARCHAR, 64))
                ));

            return schema;
        }
    }
}