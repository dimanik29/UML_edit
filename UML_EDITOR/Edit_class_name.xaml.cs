using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lab5
{
    /// <summary>
    /// Логика взаимодействия для Edit_class_name.xaml
    /// </summary>
    public partial class Edit_class_name : Window
    {
        public string ster;
        public string stereotype;
        public string name;
        public Edit_class_name()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            name = TXT_DIALOG.Text;
            stereotype = ster;
        }

        private void stereotype_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cBox = (ComboBox)sender;
            TextBlock selectedItem = cBox.SelectedItem as TextBlock;
            ster = selectedItem.Text;
        }
    }
}
