using AddressUtility.Commands;
using AddressUtility.Context.Repository;
using AddressUtility.Models.Extra;
using AddressUtility.Views;
using System.Collections.Generic;

namespace AddressUtility.ViewModels
{
    // ViewModel окна добавления новых объектов
    public class AddViewModel : Validating
    {
        private readonly ICollection<AddressData> _addressEntities;
        private readonly IDbRepository _dbRepository;

        private string _addressObjName;
        private AtomAndType _selectedAtomAndType;
        private AddressData _selectedParentItemInMainWindow;
        private AddressData _newAddressObj;

        public AddViewModel(IList<AtomAndType> atomsTypes,
                            AddressData selectedItem,
                            ICollection<AddressData> addressEntities)
        {
            _dbRepository = new DbRepository();

            SaveCmd = new RelayCommand(CanExecuteSaveCmd, ExecuteSaveCmd);
            CancelCmd = new RelayCommand(CanExecuteCancelCmd, ExecuteCancelCmd);

            _addressEntities = addressEntities;
            AtomsAndTypes = atomsTypes;
            SelectedParentItemInMainWindow = selectedItem;
        }

        public IList<AtomAndType> AtomsAndTypes { get; set; }
        public string AddressObjName
        {
            get
                => _addressObjName;
            set
            {
                _addressObjName = value;

                ValidateName(value);
                OnErrorsChanged();
                SaveCmd.OnCanExecuteChanged();
            }
        }
        public AtomAndType SelectedAtomAndType
        {
            get
                => _selectedAtomAndType;
            set
            {
                _selectedAtomAndType = value;

                SaveCmd.OnCanExecuteChanged();
            }
        }
        public AddressData SelectedParentItemInMainWindow
        {
            get => _selectedParentItemInMainWindow;
            set
            {
                if (_selectedParentItemInMainWindow == value)
                    return;

                _selectedParentItemInMainWindow = value;
            }
        }
        public string ParentItemAtomShortName
        {
            get
                => SelectedParentItemInMainWindow.AtomAndType.AtomShortName;
        }
        public string ParentItemName
        {
            get
                => SelectedParentItemInMainWindow.Name;
        }


        public RelayCommand CancelCmd { get; }
        public RelayCommand SaveCmd { get; }

        private void AddNewAddressObjToRepositories()
        {
            _newAddressObj = new()
            {
                Name = AddressObjName,
                ParentId = SelectedParentItemInMainWindow.Id,
                AtomAndType = SelectedAtomAndType
            };

            int idInDb = _dbRepository.AddAddressDataToDb(_newAddressObj);
            _newAddressObj.Id = idInDb;

            AddAddressDataToGrid(_newAddressObj);
        }
        private void AddAddressDataToGrid(AddressData newObj)
            => _addressEntities.Add(newObj);
        private bool CanExecuteCancelCmd(object parameter)
            => true;
        private void ExecuteCancelCmd(object parameter)
        {
            CloseCurrentWindow(parameter);
        }
        private bool CanExecuteSaveCmd(object parameter)
            // Условие !string.IsNullOrEmpty() сюда вписал, потому что валидация срабатывает только после изменения текстового поля.
            // Иначе получается, что кнопка "Сохранить" активно, если только выбрать тип адреса.
            => !HasErrors && SelectedAtomAndType is not null && !string.IsNullOrEmpty(AddressObjName);
        private void ExecuteSaveCmd(object parameter)
        {
            AddNewAddressObjToRepositories();
            CloseCurrentWindow(parameter);
        }
        private void CloseCurrentWindow(object currentWindow)
        {
            var window = (AddView)currentWindow;
            window.Close();
        }
    }
}
