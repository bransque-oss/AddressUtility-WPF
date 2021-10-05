using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AddressUtility.Models.Extra
{
    // Класс созданный из моделей из базы данных. Используется как основная модель для адресных объектов.
    public class AddressData : INotifyPropertyChanged
    {
        private readonly Dictionary<string, List<string>> _errors = new();
        private AtomAndType _atomType;
        private string _inheritancePath;
        private string _name;

        public event PropertyChangedEventHandler PropertyChanged;

        public AtomAndType AtomAndType
        {
            get
                => _atomType;
            set
            {
                if (value == _atomType)
                    return;
                _atomType = value;
                OnPropertyChanged();
            }
        }
        public int Id { get; set; }
        // Цепочка родительских объектов, которая заполняется у объектов, оставшихся в результатах поиска в MainViewModel
        public string InheritancePath
        {
            get
                => _inheritancePath;
            set
            {
                if (value == _inheritancePath)
                    return;

                _inheritancePath = value;
                OnPropertyChanged();
            }
        }
        public bool HasErrors
            => _errors.Count > 0;
        public string Name
        {
            get
                => _name;
            set
            {
                if (value == _name)
                    return;

                _name = value;
                OnPropertyChanged();
            }
        }
        public int? ParentId { get; set; }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
