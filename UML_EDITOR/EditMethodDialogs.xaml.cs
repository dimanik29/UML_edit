using System.Collections.Generic;
using System.Linq;
using System.Windows;

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
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Metods = lst_V.Items.Cast<Method>().ToList();
        }
        public void SetMethods(Node n)
        {
            foreach (var item in n.metods)
            {
                lst_V.Items.Add(item);
            }
            Metods = n.metods;
        }
    }
}
