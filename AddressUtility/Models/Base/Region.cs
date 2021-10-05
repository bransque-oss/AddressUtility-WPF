using System.Collections.Generic;

namespace AddressUtility.Models.Base
{
    // Модель региона в базе данных. В адресных объектах используется для указания принадлежности к региону.
    // Например, если город должен быть регионом (Сочи), то объекта будет указан это регион.
    public class Region
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        public ICollection<AddressObject> AddressObjects { get; set; }
    }
}
