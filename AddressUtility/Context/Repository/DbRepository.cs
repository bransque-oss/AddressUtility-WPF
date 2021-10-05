using AddressUtility.Models.Base;
using AddressUtility.Models.Extra;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AddressUtility.Context.Repository
{
    public class DbRepository : IDbRepository
    {
        public int AddAddressDataToDb(AddressData newObj)
        {
            using AddressContext context = new();
            // В базе данных в dbo.AddressObj в колонке ObjectId нет INDENTITY.
            // Поэтому ищу максимальный id, а потом увеличиваю на 1 и вставляю в базу.
            int maxIdInDB = context.AddressObjects.Max(ao => ao.Id);

            AddressObject objForDb = new()
            {
                Id = ++maxIdInDB,
                ParentObjectId = newObj.ParentId,
                Name = newObj.Name,
                TypeId = newObj.AtomAndType.TypeId,
                AtomId = newObj.AtomAndType.AtomId
            };
            context.Add(objForDb);

            // TimeStamp есть, но не используется, но и NULL не позволяет вставлять. Сказали, что добавлять руками со значением 10.
            // FakeTimeStamp - теневое свойство. Задано в AddressUtility.Context.Configurations.AddressObjectConfiguration.cs
            context.Entry(objForDb).Property("FakeTimeStamp").CurrentValue = Convert.ToInt64(10);

            context.SaveChanges();
            return objForDb.Id;
        }
        public void RemoveAddressDataFromDb(AddressData addrObj)
        {
            using AddressContext context = new();
            var objInDb = context.AddressObjects.Find(addrObj.Id);
            context.Remove(objInDb);
            context.SaveChanges();
        }

        public IList<AddressData> GetAddrObjectsByRegion(Region region)
        {
            using AddressContext context = new();

            // Рекурсивный запрос, чтобы получить объекты в регионе
            string query = @"
                DECLARE @regionId INT = @p0;
    
                WITH addrRec
                AS
                (
                    SELECT parents.*
                    FROM dbo.AddressObj AS parents
                    WHERE parents.ObjectId IN (SELECT ObjectId
                                                FROM dbo.AddressObj
                                                WHERE RegionGroupId = @regionId)

                    UNION ALL

                    SELECT childs.*
                    FROM dbo.AddressObj AS childs
                        JOIN addrRec 
                            ON childs.ParentObj = addrRec.ObjectId
                )

                SELECT *
                FROM addrRec
                OPTION (MAXRECURSION 100)";
            var addressObjectsFromDb = context.AddressObjects.FromSqlRaw(query, region.Id).ToList();

            // Сопоставляет адресные объекты из базы с их типом (уже подходящем для использования виде).
            var result = (from addrObj in addressObjectsFromDb
                          join atomType in GetAtomsTypes()
                              on new { addrObj.TypeId, addrObj.AtomId } equals new { atomType.TypeId, atomType.AtomId }
                              into atomTypeGroup
                          // У объектов, которым суждено быть регионами в базе, нет атома, и тип AtomAndType не вывести в том виде, как для всех остальных.
                          // Это только у таких объектов.
                          // Поэтому, если у объекта нет атома в базе, создаю AtomAndType пустышку.
                          from subAtomType in atomTypeGroup.DefaultIfEmpty(new(0, "регион", "рег", 0, "регион"))
                          select new AddressData()
                          {
                              Id = addrObj.Id,
                              Name = addrObj.Name,
                              ParentId = addrObj.ParentObjectId,
                              AtomAndType = subAtomType
                          }).ToList();
            return result;
        }
        public IList<AtomAndType> GetAtomsTypes()
        {
            using AddressContext context = new();

            // Нужно соединить Atoms и Types с локальной коллекцией.
            // Если использовать только join, пишет, что EF не может распознать запрос.
            // Если AsEnumerable(), то "There is already an open DataReader associated with this Connection"
            // Работает только с ToList()
            var result = (from atom in context.Atoms.ToList()
                          join atomType in AtomTypeMappings.GetMapping()
                              on atom.Id equals atomType.AtomId
                          join addrType in context.AddrTypes.ToList()
                              on atomType.TypeId equals addrType.Id
                          select new AtomAndType(atom.Id, atom.Description, atom.Name, addrType.Id, addrType.Name))
                          .OrderBy(x => x.AtomName)
                          .ToList();
            return result;
        }
        public IList<Region> GetRegions()
        {
            using AddressContext context = new();
            return context.Regions.OrderBy(x => x.Name).ToList();
        }
    }
}
