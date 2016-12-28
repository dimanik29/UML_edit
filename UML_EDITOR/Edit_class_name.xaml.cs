using System.Windows;
using System.Windows.Controls;

namespace Lab5
{
    /// <summary>
    /// Логика взаимодействия для Edit_class_name.xaml
    /// </summary>
    public partial class Edit_class_name : Window
    {
        public Node node_;
        public int ster;
        public int stereotype;
        public string name;
        public Edit_class_name()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            name = TXT_DIALOG.Text;

            ster = stereotype_comboBox.SelectedIndex;
        
        }

        private void stereotype_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        public void SetNode(Node n)
        {
            try
            {
            Title = n.Text;
            TXT_DIALOG.Text = n.Text;
            stereotype_comboBox.SelectedIndex = n.stereotype_index;
            }
            catch (System.NullReferenceException)
            {
                Title = "AddClass";
                TXT_DIALOG.Text = "classname";
                stereotype_comboBox.SelectedIndex = 0;
            }
        }
    }
}
