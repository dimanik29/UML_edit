using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Graph> graphs = new ObservableCollection<Graph>();
        Graph graph { get { return tabControl1 != null && tabControl1.SelectedItem != null ? tabControl1.SelectedItem as Graph : graphs[0]; } }

        public MainWindow()
        {
            graphs.Add(new Graph());

            InitializeComponent();

            tabControl1.ItemsSource = graphs;

            var node1 = new Node { Pos = new Point(200, 100), Text = "Node 1", Corner = 10 };
            graph.Nodes.Add(node1);
            var node2 = new Node { Pos = new Point(200, 200), Text = "Node 2", Corner = 100 };
            graph.Nodes.Add(node2);

            var edge1 = new Edge { start = new Point(300, 300), finish = new Point(400, 400)};
            graph.Edges.Add(edge1);

            //DataContext = graph;
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
                        _curNode.EditMode = false;
                    }

                    _curNode = value;
                }
            }
        }
        //Point dig_edge = new Point(10,10);
        double mousePath;
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var border = sender as Border;
                var ic = sender as ItemsControl;
                var selectRegion = border.FindName("selectRegion") as Rectangle;

                if (e.ClickCount == 2)
                {
                    (border.DataContext as Node).EditMode = true;
                    var tbx = border.FindName("tbx") as TextBox;
                    tbx.SelectAll();
                    tbx.Focus();
                }
                else
                {
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
                                graph.Edges.Remove(edge);
                            }
                            else
                            {
                                edge = new Edge { A = nodes[0], B = nodes[1], Dash = graph.edge_Dash }; 
                                graph.Edges.Add(edge);
                            }
                            nodes[0].InvSelect();
                            nodes[1].InvSelect();
                        }
                        curNode = border.DataContext as Node;
                        curNode.InvSelect();
                    }
                    else
                    {
                        //selectRegion.Visibility = System.Windows.Visibility.Collapsed;
                        curNode = border.DataContext as Node;
                    }
                }
                e.Handled = true;
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (curNode != null && !curNode.EditMode && e.LeftButton == MouseButtonState.Pressed && Keyboard.Modifiers != ModifierKeys.Shift)
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
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    var p = e.GetPosition(sender as IInputElement);
                    mousePress = p;
                    if (curNode != null && curNode.Selected)
                        curNode.InvSelect();
                    curNode = new Node { Pos = p, Text = "Node " + (graph.Nodes.Count() + 1), Corner = graph.Radius };
                    graph.Nodes.Add(curNode);
                    curNode.InvSelect();
                }
                else
                {
                    curNode = null;
                    var g = e.GetPosition(sender as IInputElement);
                    selectRegionMousePress = g;
                    e.Handled = false;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2 && curNode != null)
            {
                curNode.EditMode = true;
            }
            if (e.Key == Key.Delete)//&& curNode != null)
            {

                var del_edge = graph.Edges.Where(x => x.Selected).FirstOrDefault();
                graph.Edges.Remove(del_edge);

                var rem = graph.Nodes.Where(x => x.Selected).ToArray();
                graph.Nodes.Remove(curNode);
                for (int i = 0; i < rem.Length; i++)
                {
                    graph.Nodes.Remove(rem[i]);
                }
                var edge_count = graph.Edges.ToArray();
                for (int i = 0; i < rem.Length; i++)
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
            graph.Save();
        }
        //private void Button_newWindow(object sender, RoutedEventArgs e)
        //{
        //    graph.Hedee = "Test";
        //    var mw = new TableName_redact();
        //    var res = mw.ShowDialog();
        //    var rr = res;
        //}
        private void Button_Load(object sender, RoutedEventArgs e)
        {
            graph.Load();
        }

        private void Button_New(object sender, RoutedEventArgs e)
        {
            graph.Clear();
        }
        private void Button_newTab(object sender, RoutedEventArgs e)
        {
            Grid_create.Visibility = Visibility.Visible;
            EditButton.Visibility = Visibility.Collapsed;
            CreateButton.Visibility = Visibility.Visible;
           // graphs.Add(new Graph{ Hedee = "322"});
        }

        private void ItemsControl_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
               // var g = e.GetPosition(sender as IInputElement);
                //var pr = g - selectRegionMousePress;
                var ic =  sender as ItemsControl;
                var grid = (ic.Parent as Grid);
                var selectRegion = grid.FindName("selectRegion") as Rectangle;
                var tt = selectRegion.RenderTransform as TranslateTransform;
                var ghost_line = grid.FindName("ghost_line") as Line;
                var g = e.GetPosition(grid);
                var pr = g - selectRegionMousePress;



                if (Keyboard.Modifiers != ModifierKeys.Shift && Keyboard.Modifiers != ModifierKeys.Alt)
                {
                    selectRegion.Width = Math.Abs(pr.X);
                    tt.X = Math.Min(g.X, selectRegionMousePress.X);
                    selectRegion.Height = Math.Abs(pr.Y);
                    tt.Y = Math.Min(g.Y, selectRegionMousePress.Y);


                    var sel = graph.Nodes.Where(x => x.Pos != null).ToArray();

                    for (i = 0; i < sel.Length; i++)
                    {
                        if (sel[i].Pos.X < selectRegionMousePress.X
                            && sel[i].Pos.Y < selectRegionMousePress.Y
                            && sel[i].Pos.X > g.X
                            && sel[i].Pos.Y > g.Y
                            || sel[i].Pos.X > selectRegionMousePress.X
                            && sel[i].Pos.Y > selectRegionMousePress.Y
                            && sel[i].Pos.X < g.X
                            && sel[i].Pos.Y < g.Y
                            || sel[i].Pos.X > selectRegionMousePress.X
                            && sel[i].Pos.Y < selectRegionMousePress.Y
                            && sel[i].Pos.X < g.X
                            && sel[i].Pos.Y > g.Y
                            || sel[i].Pos.X > selectRegionMousePress.X
                            && sel[i].Pos.Y < selectRegionMousePress.Y
                            && sel[i].Pos.X < g.X
                            && sel[i].Pos.Y > g.Y
                            || sel[i].Pos.X < selectRegionMousePress.X
                            && sel[i].Pos.Y > selectRegionMousePress.Y
                            && sel[i].Pos.X > g.X
                            && sel[i].Pos.Y < g.Y
                            || sel[i].Pos.X < selectRegionMousePress.X
                            && sel[i].Pos.Y > selectRegionMousePress.Y
                            && sel[i].Pos.X > g.X
                            && sel[i].Pos.Y < g.Y)
                        {
                            sel[i].Select();
                        }
                        else
                        {
                            sel[i].UnSelect();
                        }
                    }
                    var sel_edges = graph.Edges.Where(x => x.finish != null).ToArray();

                    for (int i = 0; i < sel_edges.Length; i++)
                    {
                        if ((sel_edges[i].finish.X < selectRegionMousePress.X
                            && sel_edges[i].finish.Y < selectRegionMousePress.Y
                            && sel_edges[i].finish.X > g.X
                            && sel_edges[i].finish.Y > g.Y
                            || sel_edges[i].finish.X > selectRegionMousePress.X
                            && sel_edges[i].finish.Y > selectRegionMousePress.Y
                            && sel_edges[i].finish.X < g.X
                            && sel_edges[i].finish.Y < g.Y
                            || sel_edges[i].finish.X > selectRegionMousePress.X
                            && sel_edges[i].finish.Y < selectRegionMousePress.Y
                            && sel_edges[i].finish.X < g.X
                            && sel_edges[i].finish.Y > g.Y
                            || sel_edges[i].finish.X > selectRegionMousePress.X
                            && sel_edges[i].finish.Y < selectRegionMousePress.Y
                            && sel_edges[i].finish.X < g.X
                            && sel_edges[i].finish.Y > g.Y
                            || sel_edges[i].finish.X < selectRegionMousePress.X
                            && sel_edges[i].finish.Y > selectRegionMousePress.Y
                            && sel_edges[i].finish.X > g.X
                            && sel_edges[i].finish.Y < g.Y
                            || sel_edges[i].finish.X < selectRegionMousePress.X
                            && sel_edges[i].finish.Y > selectRegionMousePress.Y
                            && sel_edges[i].finish.X > g.X
                            && sel_edges[i].finish.Y < g.Y) 
                            || (sel_edges[i].start.X < selectRegionMousePress.X
                            && sel_edges[i].start.Y < selectRegionMousePress.Y
                            && sel_edges[i].start.X > g.X
                            && sel_edges[i].start.Y > g.Y
                            || sel_edges[i].start.X > selectRegionMousePress.X
                            && sel_edges[i].start.Y > selectRegionMousePress.Y
                            && sel_edges[i].start.X < g.X
                            && sel_edges[i].start.Y < g.Y
                            || sel_edges[i].start.X > selectRegionMousePress.X
                            && sel_edges[i].start.Y < selectRegionMousePress.Y
                            && sel_edges[i].start.X < g.X
                            && sel_edges[i].start.Y > g.Y
                            || sel_edges[i].start.X > selectRegionMousePress.X
                            && sel_edges[i].start.Y < selectRegionMousePress.Y
                            && sel_edges[i].start.X < g.X
                            && sel_edges[i].start.Y > g.Y
                            || sel_edges[i].start.X < selectRegionMousePress.X
                            && sel_edges[i].start.Y > selectRegionMousePress.Y
                            && sel_edges[i].start.X > g.X
                            && sel_edges[i].start.Y < g.Y
                            || sel_edges[i].start.X < selectRegionMousePress.X
                            && sel_edges[i].start.Y > selectRegionMousePress.Y
                            && sel_edges[i].start.X > g.X
                            && sel_edges[i].start.Y < g.Y))
                        {
                            sel_edges[i].Select();
                        }
                        else
                        {
                            sel_edges[i].UnSelect();
                        }
                    }


                    x_e.Text = Convert.ToString(selectRegionMousePress.X);
                    y_e.Text = Convert.ToString(selectRegionMousePress.Y);
                    x_txt.Text = Convert.ToString(g.X);
                    y_txt.Text = Convert.ToString(g.Y);



                    selectRegion.Visibility = System.Windows.Visibility.Visible;
                }
                else if (Keyboard.Modifiers == ModifierKeys.Alt)
                {
                    ghost_line.Visibility = System.Windows.Visibility.Visible;
                    ghost_line.X1 = selectRegionMousePress.X;
                    ghost_line.Y1 = selectRegionMousePress.Y;
                    ghost_line.X2 = g.X;
                    ghost_line.Y2 = g.Y;
                }
            }
        }

        private void ItemsControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var ic = sender as ItemsControl;
            var grid = (ic.Parent as Grid);
            var c = grid.Children;
            var selectRegion = grid.FindName("selectRegion") as Rectangle;
            var ghost_line = grid.FindName("ghost_line") as Line;

            var tt = selectRegion.FindName("tt") as TranslateTransform;

            var g = e.GetPosition(ic);
            selectRegion.Visibility = System.Windows.Visibility.Collapsed;

            ghost_line.Visibility = System.Windows.Visibility.Collapsed;
            if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                var edge = new Edge { start = selectRegionMousePress, finish = g };
                graph.Edges.Add(edge);
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var win = sender as Window;
            //var grid = (win.Parent as Grid);
            //var selectRegion = win.FindName("selectRegion") as Rectangle;

            //selectRegion.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Square_Checked(object sender, RoutedEventArgs e)
        {
            graph.Radius = 10;
        }

        private void Oval_Checked(object sender, RoutedEventArgs e)
        {
            graph.Radius = 100;
        }

        private void Line_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var line = sender as Line;
                (line.DataContext as Edge).InvSelect();
            }
        }
        private void Line_Anchor_Checked(object sender, RoutedEventArgs e)
        {
            graph.edge_Dash = "2 0.3";
        }

        private void Line_Equal_Checked(object sender, RoutedEventArgs e)
        {
            graph.edge_Dash = "1 0";
        }

        private void selectRegion_MouseMove(object sender, MouseEventArgs e)
        {
            var selectRegion = sender as Rectangle;
            var grid = selectRegion.Parent as Grid;
            var g = e.GetPosition(grid);
            var pr = g - selectRegionMousePress;
            var tt = selectRegion.RenderTransform as TranslateTransform;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                selectRegion.Width = Math.Abs(pr.X);
                tt.X = Math.Min(g.X, selectRegionMousePress.X);
                selectRegion.Height = Math.Abs(pr.Y);
                tt.Y = Math.Min(g.Y, selectRegionMousePress.Y);
            }
        }

        private void selectRegion_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //selectRegion.Visibility = System.Windows.Visibility.Collapsed;
            var rec = sender as Rectangle;
            rec.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            graphs.Add(new Graph { Hedee = textBox_nameDiag.Text });
            Grid_create.Visibility = Visibility.Collapsed;
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            graph.Hedee = textBox_nameDiag.Text;
            //Не изменяется имя графа
            Grid_create.Visibility = Visibility.Collapsed;
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                if(e.ClickCount == 2)
                {
                    Grid_create.Visibility = Visibility.Visible;
                    EditButton.Visibility = Visibility.Visible;
                    CreateButton.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}