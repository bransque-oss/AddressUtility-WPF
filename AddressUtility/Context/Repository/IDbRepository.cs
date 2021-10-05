using AddressUtility.Models.Base;
using AddressUtility.Models.Extra;
using System.Collections.Generic;

namespace AddressUtility.Context.Repository
{
    public interface IDbRepository
    {
        // Добавляет в базу данных и присваивает переданному объекту Id из базы.
        int AddAddressDataToDb(AddressData newObj);
        void RemoveAddressDataFromDb(AddressData addrObj);

        IList<AtomAndType> GetAtomsTypes();
        IList<AddressData> GetAddrObjectsByRegion(Region region);
        IList<Region> GetRegions();
    }
}
