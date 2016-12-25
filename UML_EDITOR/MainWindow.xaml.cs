using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Lab5
{
    public partial class MainWindow : Window
    {
        ObservableCollection<Graph> graphs = new ObservableCollection<Graph>();
        Graph graph { get { return tabControl1 != null && tabControl1.SelectedItem != null ? tabControl1.SelectedItem as Graph : graphs[0]; } }

        public MainWindow()
        {
            graphs.Add(new Graph() {HeaderName = "First Graph" });
            InitializeComponent();
            tabControl1.ItemsSource = graphs;
            var node1 = new Node { Pos = new Point(this.Width / 2, this.Height/2), Text = "Hello World!", Width = 110, Height = 40, };
            graph.Nodes.Add(node1);
        }

        private void Line_Loaded(Object sender, RoutedEventArgs e)
        {
            var line = sender as Line;
            var edge = line.DataContext as Edge;
        }

        private void Border_Loaded(Object sender, RoutedEventArgs e)
        {
            var border = sender as Border;
            var node = border.DataContext as Node;
        }

        int i;
        Border SizeNode;
        Point mousePress;
        Node _curNode;
        Node curNode
        {
            get { return _curNode; }
            set
            {
                if (_curNode != value)
                {
                    if (_curNode != null)
                    {
                        if (_curNode.Selected)
                        {
                            _curNode.InvSelect();
                        }
                    }

                    _curNode = value;
                }
            }
        }

        double mousePath;
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                curNode = (sender as Border).DataContext as Node;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var border = sender as Border;
                var grid = (border.Parent as Grid);
                var grid_par = grid.Parent as ItemsControl;
                
                mousePress = e.GetPosition(this);
                mousePath = 0;

                border.CaptureMouse();  // захватим мышку, чтобы при движении не "отлипал" узел
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    (border.DataContext as Node).InvSelect();

                    var nodes = graph.Nodes.Where(x => x.Selected).ToArray();
                    if (nodes.Length == 2)
                    {
                        var edge = graph.Edges
                            .Where(x => x.A == nodes[0] && x.B == nodes[1]
                                    || x.A == nodes[1] && x.B == nodes[0])
                            .FirstOrDefault();

                        if (edge != null)
                        {
                            graph.delEdge(edge);
                        }
                        else
                        {
                            graph.createEdge(nodes[0], nodes[1]);
                        }
                        nodes[0].InvSelect();
                        nodes[1].InvSelect();
                    }
                    curNode = border.DataContext as Node;
                    curNode.FireAnchors();
                    curNode.InvSelect();
                }
                else
                {
                    selectRegion.Visibility = System.Windows.Visibility.Collapsed;
                    curNode = border.DataContext as Node;
                }
            e.Handled = true;
            }
        }
        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (curNode != null && e.LeftButton == MouseButtonState.Pressed && Keyboard.Modifiers != ModifierKeys.Shift)//!curNode.EditMode
            {
                var p = e.GetPosition(this);
                var dr = p - mousePress;
                curNode.MoveRef(dr);
                mousePath += dr.Length;
                mousePress = p;
                e.Handled = true;
            }

        }
        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mousePath < 5 && curNode != null && Keyboard.Modifiers != ModifierKeys.Shift)
                curNode.InvSelect();

            var border = sender as Border;
            border.ReleaseMouseCapture();
        }
        Point selectRegionMousePress;
        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(sender as IInputElement);
            mousePress = p;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    if (curNode != null && curNode.Selected)
                        curNode.InvSelect();
                    curNode = new Node { Pos = p, Text = "Class " + (graph.Nodes.Count() + 1), Width = 100, Height = 100 };
                    graph.Nodes.Add(curNode);
                    curNode.InvSelect();
                }
                else
                {
                    //var g = e.GetPosition(sender as IInputElement);
                    curNode = null;
                    selectRegionMousePress = p;
                    e.Handled = false;
                }
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var rem = graph.Nodes.Where(x => x.Selected).ToArray();
                graph.Nodes.Remove(curNode);
                for (int i = 0; i < rem.Length; i++) //удаление нескольких узлов
                {
                    graph.Nodes.Remove(rem[i]);
                }
                var edge_count = graph.Edges.ToArray();
                for (int i = 0; i < rem.Length; i++) //удаление нескольких рёбер
                {
                    var tmp = graph.Edges.Where(x => x.A == rem[i] || x.B == rem[i]).ToArray();
                    foreach (var item in tmp)
                    {
                        graph.Edges.Remove(item);
                    }
                }
            }
        }
        private void Button_Save(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Graph"; // Default file name
            dlg.DefaultExt = ".dat"; // Default file extension
            dlg.Filter = "Binary type format|*.dat"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                string filename = dlg.FileName;
                // Save document
                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
                    {
                        graphs[graphs.IndexOf(graph)].Save(fs);
                    }
            }
        }
        private void Button_Load(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = " "; // Default file name
            dlg.DefaultExt = ".dat"; // Default file extension
            dlg.Filter = "Binary type format|*.dat"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                string filename = dlg.FileName; // full path of file to string
                // Save document
                graphs.Add(new Graph());
                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    graphs[graphs.Count - 1] = Graph.Load(fs);
                }
            }
            tabControl1.SelectedItem = graphs[graphs.Count - 1];
        }
        private void Button_newTab(object sender, RoutedEventArgs e)
        {
            Grid_create.Visibility = Visibility.Visible;
            EditButton.Visibility = Visibility.Collapsed;
            CreateButton.Visibility = Visibility.Visible;
            textBox_nameDiag.SelectAll();
            textBox_nameDiag.Focus();
        }

        private void ItemsControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectRegion.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectRegion.Visibility = Visibility.Collapsed;
        }
        private void Line_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var line = sender as Line;
                var t = (line.DataContext as Edge);
                t.InvSelect();
                if(e.ClickCount == 2)
                {
                    t.InvDirection();
                }
                
            }
            selectRegion.Visibility = Visibility.Collapsed;
        }
        private void Dependency_Checked(object sender, RoutedEventArgs e)
        {
            graph.edge_TriVis = Visibility.Collapsed;
            graph.edge_Dash = "3 3";
        }
        private void Association_Checked(object sender, RoutedEventArgs e)
        {
            graph.edge_TriVis = Visibility.Collapsed;
            graph.edge_Dash = "3 0";
        }
        private void implementation_Checked(object sender, RoutedEventArgs e)
        {
            graph.edge_TriVis = Visibility.Visible;
            graph.edge_Dash = "3 3";
        }
        private void Inheritance_Checked(object sender, RoutedEventArgs e)
        {
            graph.edge_TriVis = Visibility.Visible;
            graph.edge_Dash = "3 0";
        }
        private void selectRegion_MouseMove(object sender, MouseEventArgs e)
        {
            selectRegion.Visibility = Visibility.Collapsed;
            e.Handled = false;
        }
        private void selectRegion_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectRegion.Visibility = System.Windows.Visibility.Collapsed;
            var rec = sender as Rectangle;
            rec.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //TbcView.Items.RemoveAt(TbcView.SelectedIndex);
            graphs.Remove(graph);
        }
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            graphs.Add(new Graph { HeaderName = textBox_nameDiag.Text });
            Grid_create.Visibility = Visibility.Collapsed;
            tabControl1.SelectedItem = graphs[graphs.Count - 1];
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            graph.HeaderName = textBox_nameDiag.Text;
            textBox_nameDiag.SelectAll();
            textBox_nameDiag.Focus();
            Grid_create.Visibility = Visibility.Collapsed;
        }
        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 2)
                {
                    Grid_create.Visibility = Visibility.Visible;
                    EditButton.Visibility = Visibility.Visible;
                    CreateButton.Visibility = Visibility.Collapsed;
                    textBox_nameDiag.SelectAll();
                    textBox_nameDiag.Focus();
                }
            }
        }
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            selectRegion.Visibility = Visibility.Collapsed;
        }
        private void border_node_MouseEnter(object sender, MouseEventArgs e)
        {
            if (selectRegion.Visibility == Visibility.Collapsed)
            {
                var el = (sender as Border).DataContext as Node;
                el.ResizeModOn();
                SizeNode = (sender as Border);
            }
        }
        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            if (selectRegion.Visibility == Visibility.Collapsed)
            {
                var el = SizeNode.DataContext as Node;
                if (el != null)
                {
                    el.ResizeModOff();
                }
            }
        }
        private void Rectangle_MouseMove_bot(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // только для нижнего прямоугольника для размеров
                // остальные по аналогии
                var t = e.GetPosition(SizeNode);
                var t2 = (SizeNode.DataContext as Node);
                (sender as Rectangle).CaptureMouse();
                t2.Resize(t2.Width, t.Y);
                e.Handled = true;
            }
        }
        private void Rectangle_MouseMove_right(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // только для правого прямоугольника для размеров
                // остальные по аналогии
                var t = e.GetPosition(SizeNode);
                var t2 = (SizeNode.DataContext as Node);
                (sender as Rectangle).CaptureMouse();
                t2.Resize(t.X, t2.Height);
                e.Handled = true;
            }
        }
        private void Rectangle_MouseMove_right_bot(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // только для правого прямоугольника для размеров
                // остальные по аналогии
                var t = e.GetPosition(SizeNode);
                var t2 = (SizeNode.DataContext as Node);
                (sender as Rectangle).CaptureMouse();
                t2.Resize(t.X, t.Y);
                e.Handled = true;
            }
        }
        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as Rectangle).ReleaseMouseCapture();
        }
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            Vector f = new Vector(4, 24);
            Vector pd = new Vector(2, 0);
            var grid = sender as Grid;
            var g = e.GetPosition(grid);
            var pr = (g - selectRegionMousePress);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Keyboard.Modifiers != ModifierKeys.Shift && Keyboard.Modifiers != ModifierKeys.Alt)
                {
                    selectRegion.Width = Math.Abs(pr.X - pd.X) ;
                    double f_typeX = f.X;
                    tt.X = Math.Min(g.X, selectRegionMousePress.X)+f_typeX;
                    selectRegion.Height = Math.Abs(pr.Y);
                    double f_type = f.Y;
                    tt.Y = Math.Min(g.Y, selectRegionMousePress.Y) + f_type;


                    var sel = graph.Nodes.Where(x => x.Pos != null).ToArray();

                    for (i = 0; i < sel.Length; i++)
                    {
                        if (sel[i].Center.X < selectRegionMousePress.X
                            && sel[i].Center.Y < selectRegionMousePress.Y
                            && sel[i].Center.X > g.X
                            && sel[i].Center.Y > g.Y
                            || sel[i].Center.X > selectRegionMousePress.X
                            && sel[i].Center.Y > selectRegionMousePress.Y
                            && sel[i].Center.X < g.X
                            && sel[i].Center.Y < g.Y
                            || sel[i].Center.X > selectRegionMousePress.X
                            && sel[i].Center.Y < selectRegionMousePress.Y
                            && sel[i].Center.X < g.X
                            && sel[i].Center.Y > g.Y
                            || sel[i].Center.X > selectRegionMousePress.X
                            && sel[i].Center.Y < selectRegionMousePress.Y
                            && sel[i].Center.X < g.X
                            && sel[i].Center.Y > g.Y
                            || sel[i].Center.X < selectRegionMousePress.X
                            && sel[i].Center.Y > selectRegionMousePress.Y
                            && sel[i].Center.X > g.X
                            && sel[i].Center.Y < g.Y
                            || sel[i].Center.X < selectRegionMousePress.X
                            && sel[i].Center.Y > selectRegionMousePress.Y
                            && sel[i].Center.X > g.X
                            && sel[i].Center.Y < g.Y)
                        {
                            sel[i].Select();
                        }
                        else
                        {
                            sel[i].UnSelect();
                        }
                    }
                    selectRegion.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
        private void AddMethod_Item_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddMethodDialog();
            if (dlg.ShowDialog() == true)
            {
                (SizeNode.DataContext as Node).AddMethod(dlg.result);
            }
        }
        private void EditClassName_Item_Click(object sender, RoutedEventArgs e)
        {
            var ed_class_dlg = new Edit_class_name();
            ed_class_dlg.SetNode(curNode);
            if (ed_class_dlg.ShowDialog() == true)
            {
                curNode.Text = ed_class_dlg.name;
                curNode.stereotype_index = ed_class_dlg.ster;
                curNode.StereotypeVis();
            }
        }
        private void AddVariable_click(object sender, RoutedEventArgs e)
        {
            var var_add_dlg = new AddVariable();
            if (var_add_dlg.ShowDialog() == true)
            {
                (SizeNode.DataContext as Node).AddVariable(var_add_dlg.result);
            }
        }
        private void EditMethods_Click(object sender, RoutedEventArgs e)
        {
            var edit_dlg = new EditMethodDialogs();
            edit_dlg.SetMethods(curNode);
            if (edit_dlg.ShowDialog() == true)
            {
                curNode.SetMethods(edit_dlg.Metods);
            }
        }
        private void EditVariables_click(object sender, RoutedEventArgs e)
        {
            var edit_variables_dlg = new EditVariableDialog();
            edit_variables_dlg.SetVariables(curNode);
            if (edit_variables_dlg.ShowDialog() == true)
            {
                curNode.SetVariables(edit_variables_dlg.Variables);
            }
        }
        private void AddClass_Click(object sender, RoutedEventArgs e)
        {
            if (curNode != null && curNode.Selected)
                curNode.InvSelect();
            curNode = new Node { Pos = mousePress, Text = "Class " + (graph.Nodes.Count() + 1), Width = 100, Height = 100 };
            graph.Nodes.Add(curNode);
            curNode.InvSelect();
        }
        private void Button_export_click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            DirectoryInfo di = Directory.CreateDirectory(@"classes\\");
            foreach (var item in graph.Nodes)
            {
                Directory.CreateDirectory(@"classes\\" + graph.HeaderName + "\\");
                var fs = new StreamWriter(File.Create(@"classes\\" + graph.HeaderName + "\\" + item.Text + i + ".cs"));
                fs.WriteLine(item.Parse());
                i++;
                fs.Close();
            }
            MessageBox.Show("Files succesfully created!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}