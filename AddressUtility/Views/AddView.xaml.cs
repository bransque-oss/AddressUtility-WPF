using AddressUtility.Models.Extra;
using AddressUtility.ViewModels;
using System.Collections.Generic;
using System.Windows;

namespace AddressUtility.Views
{
    //
    // Окно добавления новых объектов
    public partial class AddView : Window
    {
        public AddView(IList<AtomAndType> atomsTypes,
                       AddressData selectedItem,
                       ICollection<AddressData> addressEntities)
        {
            InitializeComponent();
            DataContext = new AddViewModel(atomsTypes, selectedItem, addressEntities);
        }
    }
}
