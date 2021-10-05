using System.Collections.Generic;

namespace AddressUtility.Models.Base
{
    // Модель типа, которая хранится в базе данных. Например, административная единица, район, населенный пункт.
    // Типы это более общие виды адресных объектов.
    public class AddrType
    {
        public byte Id { get; set; }
        public bool IsRegion { get; set; }
        public bool IsDistrict { get; set; }
        public bool IsCity { get; set; }
        public bool IsStreet { get; set; }
        public bool IsTransportation { get; set; }
        public string Name { get; set; }

        public ICollection<AddressObject> AddressObjects { get; set; }
    }
}
