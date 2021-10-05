using AddressUtility.Models.Extra;
using AddressUtility.ViewModels;
using System.Collections.Generic;
using System.Windows;

namespace AddressUtility.Views
{
    //
    // Окно изменения существующих объектов
    public partial class EditView : Window
    {
        public EditView(
            IEnumerable<AtomAndType> atomsTypes,
            AddressData itemToEdit)
        {
            InitializeComponent();

            DataContext = new EditViewModel(atomsTypes, itemToEdit);
        }
    }
}
