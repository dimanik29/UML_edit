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
    /// Логика взаимодействия для AddMethodDialog.xaml
    /// </summary>
    public partial class AddMethodDialog : Window
    {
        public string parametr;
        public string type;
        public string metod;
        public Method result = new Method();
        public AddMethodDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            result.access = type;
            result.name = TXT_DIALOG.Text;
            result.variables = TXT_SCOBE.Text;
        }

        private void RadioButton_public(object sender, RoutedEventArgs e)
        {
            type = "+ ";
        }

        private void RadioButton_private(object sender, RoutedEventArgs e)
        {
            type = "- ";
        }

        private void RadioButton_protected(object sender, RoutedEventArgs e)
        {
            type = "# ";
        }

        private void RadioButton_derived(object sender, RoutedEventArgs e)
        {
            type = "/ ";
        }

        private void RadioButton_package(object sender, RoutedEventArgs e)
        {
            type = "~ ";
        }

        public void Set_Start(string str)
        {
            TXT_DIALOG.Text = str;
        }
    }
}
