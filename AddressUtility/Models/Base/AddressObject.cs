namespace AddressUtility.Models.Base
{
    // Модель адресного объекта, которая хранится в базе данных
    public class AddressObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentObjectId { get; set; }

        public byte TypeId { get; set; }
        public AddrType AddrType { get; set; }

        public byte? AtomId { get; set; }
        public Atom Atom { get; set; }

        public int? RegionId { get; set; }
        public Region Region { get; set; }
    }
}
