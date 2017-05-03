using Ackara.Daterpillar;

namespace MSTest.Daterpillar.Fake
{
    [Table]
    public class ComplexTable
    {
        [Column]
        [Index(IndexType.PrimaryKey)]
        public int CompositeKey1 { get; set; }

        [Column]
        [Index(IndexType.PrimaryKey)]
        public int CompositeKey2 { get; set; }

        [Column]
        [Index(IndexType.Index)]
        public int StandaloneIdx { get; set; }

        [Column]
        [Index(IndexType.Index, GroupName = "A")]
        public int Idx1 { get; set; }

        [Column]
        [Index(IndexType.Index, GroupName = "A")]
        public int Idx2 { get; set; }

        [Column]
        [ForeignKey(nameof(AnEnum), "Id", OnUpdate = ReferentialAction.Cascade)]
        public AnEnum Color { get; set; }

        [Column]
        [ForeignKey(nameof(DependencyA), nameof(DependencyA.Id))]
        public int ConstraintA { get; set; }

        [Column]
        [ForeignKey(nameof(DependencyB), nameof(DependencyB.Id))]
        public int ConstraintB { get; set; }

        public string Hidden { get; set; }
    }

    [Table("dependencyAA")]
    public class DependencyA
    {
        [Column("xId")]
        public int Id { get; set; }

        [Column("xName")]
        [Index(Unique = true)]
        public string Name { get; set; }
    }

    [Table]
    public class DependencyB
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public string Name { get; set; }
    }
}