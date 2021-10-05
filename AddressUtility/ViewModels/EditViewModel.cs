using AddressUtility.Commands;
using AddressUtility.Context;
using AddressUtility.Models.Extra;
using AddressUtility.Views;
using System.Collections.Generic;
using System.Linq;

namespace AddressUtility.ViewModels
{
    // ViewModel окна изменения объектов.
    public class EditViewModel : Validating
    {
        private AtomAndType _currentAtomAndType;
        private string _currentItemName;

        public EditViewModel(
            IEnumerable<AtomAndType> atomsTypes,
            AddressData itemToEdit)
        {
            CancelCmd = new RelayCommand(CanExecuteCancelCmd, ExecuteCancelCmd);
            SaveCmd = new RelayCommand(CanExecuteSaveCmd, ExecuteSaveCmd);

            AtomsAndTypes = atomsTypes;
            SelectedItemToEdit = itemToEdit;
            CurrentItemName = itemToEdit.Name;
            // Для выбора в выпадающем списке типов текущего типа выбранного адресного объекта.
            CurrentAtomAndType = atomsTypes.First(x => x.AtomId == SelectedItemToEdit.AtomAndType.AtomId
                                                    && x.TypeId == SelectedItemToEdit.AtomAndType.TypeId);
        }

        // Для выпадающего списка с выбором типа объекта.
        public IEnumerable<AtomAndType> AtomsAndTypes { get; }
        public RelayCommand CancelCmd { get; }
        public RelayCommand SaveCmd { get; }
        // Поле с названием объекта в форме, которое редактируется.
        public string CurrentItemName
        {
            get
                => _currentItemName;
            set
            {
                if (_currentItemName == value)
                    return;

                _currentItemName = value;

                ValidateName(value);
                OnErrorsChanged();
                SaveCmd.OnCanExecuteChanged();
            }
        }
        public AddressData SelectedItemToEdit { get; set; }
        public AtomAndType CurrentAtomAndType
        {
            get
                => _currentAtomAndType;
            set
            {
                _currentAtomAndType = value;
                SaveCmd.OnCanExecuteChanged();
            }
        }

        private bool CanExecuteCancelCmd(object parameter)
            => true;
        private bool CanExecuteSaveCmd(object parameter)
        {
            if (HasErrors)
                return false;

            // Если название и тип объекта не изменились
            if (CurrentItemName == SelectedItemToEdit.Name
                && CurrentAtomAndType.AtomId == SelectedItemToEdit.AtomAndType.AtomId)
            {
                return false;
            }

            return true;
        }
        private void ChangeSourceItemInDb()
        {
            using AddressContext context = new();

            var objectInDb = context.AddressObjects.FirstOrDefault(x => x.Id == SelectedItemToEdit.Id);
            objectInDb.TypeId = CurrentAtomAndType.TypeId;
            objectInDb.AtomId = CurrentAtomAndType.AtomId;
            objectInDb.Name = CurrentItemName;
            context.SaveChanges();
        }
        private void ChangeSourceItemInGrid()
        {
            // Изменение свойств исходного объекта
            SelectedItemToEdit.AtomAndType = CurrentAtomAndType;
            SelectedItemToEdit.Name = CurrentItemName;
        }
        private void CloseCurrentWindow(object currentWindow)
        {
            var window = (EditView)currentWindow;
            window.Close();
        }
        private void ExecuteCancelCmd(object parameter)
        {
            CloseCurrentWindow(parameter);
        }
        private void ExecuteSaveCmd(object parameter)
        {
            ChangeSourceItemInDb();
            ChangeSourceItemInGrid();
            CloseCurrentWindow(parameter);
        }
    }
}
