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
        
        public string metod;
        public AddMethodDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            metod = TXT_DIALOG.Text;
        }
        public void Set_Start(string str)
        {
            TXT_DIALOG.Text = str;
        }
    }
}
