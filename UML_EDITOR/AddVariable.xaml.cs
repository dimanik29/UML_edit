﻿using System.Windows;

namespace Lab5
{
    /// <summary>
    /// Логика взаимодействия для AddVariable.xaml
    /// </summary>
    public partial class AddVariable : Window
    {
        public string type;
        public string variable;

        public Variable result = new Variable();
        public AddVariable()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            result.access = type;
            result.name = TXT_DIALOG.Text;
            result.tip = TXT_SCOBE.Text;
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
