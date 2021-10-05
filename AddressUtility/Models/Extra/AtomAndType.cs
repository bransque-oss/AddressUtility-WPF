namespace AddressUtility.Models.Extra
{
    // Класс созданный после сопоставления из базы классов Atom, AddrType и AtomTypeMappings
    // Используется везде где нужны типы (виды) адресных объектов
    public class AtomAndType
    {
        private string _atomName;

        public AtomAndType()
        {
        }
        public AtomAndType(byte atomId, string atomName, string atomShortName, byte typeId, string typeName)
        {
            AtomId = atomId;
            AtomName = atomName;
            AtomShortName = atomShortName;
            TypeId = typeId;
            TypeName = typeName;
        }

        public byte? AtomId { get; }
        public string AtomName
        {
            get
            {
                // Об этом разделении можно узнать, проверив в базе таблицу с типами, и опытным путем.
                if (TypeId == 3 && AtomId == 13)
                    return "город маленький без районов внутри";
                if (TypeId == 9 && AtomId == 13)
                    return "город большой с районами внутри";
                return _atomName;
            }
            private set => _atomName = value;
        }
        public string AtomShortName { get; }
        public byte TypeId { get; }
        public string TypeName { get; }
    }
}
