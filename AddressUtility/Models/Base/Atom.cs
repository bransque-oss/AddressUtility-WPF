using System.Collections.Generic;

namespace AddressUtility.Models.Base
{
    // Модель атома в базе данных. Являются более мелким делением чем "типы". Атомы это уже конкретные виды объектов.
    // Например, поселок, село, улица, переулок.
    public class Atom
    {
        public byte Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public byte Priority { get; set; }

        public ICollection<AddressObject> AddressObjects { get; set; }
    }
}
