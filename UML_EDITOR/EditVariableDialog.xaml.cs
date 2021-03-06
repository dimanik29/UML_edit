﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Lab5
{
    /// <summary>
    /// Логика взаимодействия для EditVariableDialog.xaml
    /// </summary>
    public partial class EditVariableDialog : Window
    {
        public List<Variable> Variables = new List<Variable>();

        public EditVariableDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Variables = lst_V.Items.Cast<Variable>().ToList();
        }
        public void SetVariables(Node n)
        {
            foreach (var item in n.variables)
            {
                lst_V.Items.Add(item);
            }
            Variables = n.variables;
        }
    }
}
