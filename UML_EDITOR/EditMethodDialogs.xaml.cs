using System;
using System.Collections;
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
    /// Логика взаимодействия для EditMethodDialogs.xaml
    /// </summary>
    public partial class EditMethodDialogs : Window
    {
        public List<Method> Metods = new List<Method>();
        public EditMethodDialogs()
        {
            InitializeComponent();
            //lst_V.ItemsSource = Metods;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Metods = ((ArrayList)lst_V.Resources["metods"]).Cast<Method>().ToList();
            //foreach (Method item in t)
            //{
            //    Metods.Add(new Method { access = item[beautiful_access] });
            //}
        }
        public void SetMethods(Node n)
        {
            foreach (var item in n.metods)
            {
                ((ArrayList)lst_V.Resources["metods"]).Add(item);
            }
        }

        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            var t = sender as ComboBox;

        }
    }
}
