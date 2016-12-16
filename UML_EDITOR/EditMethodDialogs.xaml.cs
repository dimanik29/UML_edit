﻿using System;
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