using AddressUtility.Commands;
using AddressUtility.Context.Repository;
using AddressUtility.Models.Base;
using AddressUtility.Models.Extra;
using AddressUtility.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace AddressUtility.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IDbRepository _dbRepository;
        private ObservableCollection<AddressData> _addressEntitiesInRegion;
        private ICollectionView _filteredAddressEntities;
        private string _searchName;
        private AddressData _selectedAddressItem;
        private Region _selectedRegion;

        public MainViewModel()
        {
            Application.Current.DispatcherUnhandledException += UnhandledExceptionHandle;

            _dbRepository = new DbRepository();
            Regions = _dbRepository.GetRegions();
            AtomsAndTypes = _dbRepository.GetAtomsTypes();

            AddCmd = new RelayCommand(CanExecuteAddCmd, ExecuteAddCmd);
            DeleteCmd = new RelayCommand(CanExecuteDeleteCmd, ExecuteDeleteCmd);
            EditCmd = new RelayCommand(CanExecuteEditCmd, ExecuteEditCmd);

            RegionChanged += FillAddressEntitiesByRegion;
            // Если в фильтре остается текст, то после смены региона, фильтр применяется, но не выполняет FillInheritancePath(),
            // потому что текст в фильтре не обновлялся. Сделал, что при смене региона текст в фильтре стирался.
            RegionChanged += () => SearchName = string.Empty;
        }

        private event Action RegionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        // Основной список объектов в зависимости от выбранного региона. Заполняется в свойстве SelectedRegion.
        public ObservableCollection<AddressData> AddressEntitiesInRegion
        {
            get => _addressEntitiesInRegion;
            set
            {
                _addressEntitiesInRegion = value;
                CreateFilteredAddressEntities();

                OnPropertyChanged();
            }
        }
        // Для фильтрации по названиям адресных объектов. Заполняется в свойстве AddressEntitiesInRegion.
        // Обновляется в свойстве SearchName
        public ICollectionView FilteredAddressEntities
        {
            get
                => _filteredAddressEntities;
            set
            {
                _filteredAddressEntities = value;

                OnPropertyChanged();
            }
        }
        public RelayCommand AddCmd { get; }
        public IList<AtomAndType> AtomsAndTypes { get; }
        public RelayCommand DeleteCmd { get; }
        public RelayCommand EditCmd { get; }
        public IList<Region> Regions { get; }
        public AddressData SelectedAddressItem
        {
            get => _selectedAddressItem;
            set
            {
                if (_selectedAddressItem == value)
                    return;

                _selectedAddressItem = value;

                AddCmd.OnCanExecuteChanged();
                DeleteCmd.OnCanExecuteChanged();
                EditCmd.OnCanExecuteChanged();
            }
        }
        public Region SelectedRegion
        {
            get => _selectedRegion;
            set
            {
                if (_selectedRegion == value)
                    return;

                _selectedRegion = value;
                OnRegionChanged();
            }
        }
        public string SearchName
        {
            get
                => _searchName;
            set
            {
                _searchName = value;

                if (FilteredAddressEntities is not null)
                    FillParentNamesInFilteredAddressEntities();

                OnPropertyChanged();
            }
        }

        private void AlignByMainWindowCenter(Window window)
        {
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private bool CanExecuteAddCmd(object parameter)
        {
            // Id типов, которые содержатся в базе, и у которых в базе есть признаки IsStreet, IsTransportation.
            // 4 - улица;
            // 5 - метро;
            // 6 - ж/д станция.
            // В улицу и транспортные узлы точно нельзя добавлять другие объекты.
            byte[] typeIds = { 4, 5, 6 };

            return SelectedAddressItem is not null
                     && !typeIds.Contains(SelectedAddressItem.AtomAndType.TypeId);
        }
        private bool CanExecuteDeleteCmd(object parameter)
        {
            // Чтобы необдуманно не удалили страну, административную единицу (регион) или федеральный город.

            // Id типов, которые содержатся в базе, и у которых в базе есть признаки IsRegion
            // 0 - AtomAndType пустышка, для объектов у которых в базе нет атома
            // 1 - административная единица;
            // 7 - город, видимо федерального значения, потому что в базе этот тип у СПб, Москвы, Сочи, Севастополя;
            // 8 - страна.
            byte[] typeIds = { 0, 1, 7, 8 };

            return SelectedAddressItem is not null
                 && !typeIds.Contains(SelectedAddressItem.AtomAndType.TypeId);
        }
        private bool CanExecuteEditCmd(object parameter)
            => SelectedAddressItem is not null
        && SelectedAddressItem.AtomAndType.TypeId != 0;
        private void ExecuteAddCmd(object parameter)
        {
            AddView addWindow = new(AtomsAndTypes, SelectedAddressItem, AddressEntitiesInRegion);

            AlignByMainWindowCenter(addWindow);
            addWindow.ShowDialog();
            FillParentNamesInFilteredAddressEntities();
        }
        private void ExecuteDeleteCmd(object parameter)
        {
            RemoveAddressObjFromRepositories();
        }
        private void ExecuteEditCmd(object parameter)
        {
            var filteredAtomsTypes = AtomsAndTypes.Where(x => x.TypeId == SelectedAddressItem.AtomAndType.TypeId);

            EditView editWindow = new(filteredAtomsTypes, SelectedAddressItem);
            AlignByMainWindowCenter(editWindow);
            editWindow.ShowDialog();
            FillParentNamesInFilteredAddressEntities();
        }

        private void CreateFilteredAddressEntities()
        {
            FilteredAddressEntities = CollectionViewSource.GetDefaultView(AddressEntitiesInRegion);
            FilteredAddressEntities.Filter = FilterAddressEntitiesByName;
        }
        private void UnhandledExceptionHandle(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            StringBuilder exceptionText = new();
            exceptionText.Append($"Тип ошибки:\n{e.Exception.InnerException?.GetType()}\n\n");
            exceptionText.Append($"Источник ошибки:\n{e.Exception.InnerException?.Source}\n\n");
            exceptionText.Append($"Где именно возникла ошибка:\n{e.Exception.InnerException?.TargetSite.Name}\n\n");
            exceptionText.Append($"Текст ошибки:\n{e.Exception.InnerException?.Message}\n\n");

            MessageBox.Show(exceptionText.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void FillAddressEntitiesByRegion()
        {
            AddressEntitiesInRegion = new(_dbRepository.GetAddrObjectsByRegion(SelectedRegion));
        }
        private void FillParentNamesInFilteredAddressEntities()
        {
            // Для отфильтрованных с помощью Поиска в MainView объектов нужно выводить цепочку имен родительских объектов.
            // Так как сначала нужно отфильтровать, потом вывести иерархию, объединил в один метод.
            FilteredAddressEntities?.Refresh();

            List<string> entitiesName = new();
            foreach (AddressData entity in FilteredAddressEntities)
            {
                FindParentRecursively(entity);
                entitiesName.Reverse();
                StringBuilder fullPath = new();
                fullPath.AppendJoin(", ", entitiesName);

                entity.InheritancePath = fullPath.ToString();

                entitiesName.Clear();
            }

            void FindParentRecursively(AddressData entity)
            {
                entitiesName.Add(entity.AtomAndType.AtomShortName + " " + entity.Name);
                var parent = AddressEntitiesInRegion.SingleOrDefault(x => x.Id == entity.ParentId);

                if (parent is not null)
                {
                    FindParentRecursively(parent);
                }
            }
        }
        private bool FilterAddressEntitiesByName(object item)
        {
            var addressEntity = (AddressData)item;

            // Без этого условия показываются все объекты в регионе. Чтобы не показывались, пока не будет что-то в фильтре.
            if (string.IsNullOrWhiteSpace(SearchName))
                return false;

            return addressEntity.Name.StartsWith(SearchName, StringComparison.OrdinalIgnoreCase);
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
        private void OnRegionChanged()
            => RegionChanged?.Invoke();
        private void RemoveAddressObjFromRepositories()
        {
            _dbRepository.RemoveAddressDataFromDb(SelectedAddressItem);
            _addressEntitiesInRegion.Remove(SelectedAddressItem);
        }
    }
}
