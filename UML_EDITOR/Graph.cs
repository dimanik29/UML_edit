using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Lab5
{
    class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void Set<T>(ref T field, T value, params string[] propNames) where T : IEquatable<T>
        {
            if (field != null && !field.Equals(value) || value != null && !value.Equals(field))
            {
                field = value;
                if (PropertyChanged != null)
                {
                    foreach (var name in propNames)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(name));
                    }
                }
            }
        }

        protected void Fire(params string[] names)
        {
            if (PropertyChanged != null)
                foreach (var name in names)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
        }
    }


    class Graph
    {
        //public string HeaderName { get { return "UML 1"; } }
        //string _Text;
        //public string Text  // полезные данные, характеризующие узел
        //{
        //    get { return _Text; }
        //    set { Set(ref _Text, value, "Text"); }
        //}
        public string Hedee;
        public string HeaderName
        {
            get { return Hedee; }
        }
        public ObservableCollection<Node> nodes = new ObservableCollection<Node>();
        public ObservableCollection<Edge> edges = new ObservableCollection<Edge>();
        public ObservableCollection<Node> Nodes { get { return nodes; } }
        public ObservableCollection<Edge> Edges { get { return edges; } }

        public void Save()
        {
            var sb = new StringBuilder();
            sb.AppendLine("" + Nodes.Count + "\t" + Edges.Count);
            foreach (var node in Nodes)
            {
                int corn = node.Corner;
                sb.AppendLine(node.ToString() + "\t" + "Corner = " + corn);
            }
            foreach (var edge in Edges)
            {
                sb.AppendLine("" + Nodes.IndexOf(edge.A) + "\t" + Nodes.IndexOf(edge.B));
            }
            System.IO.File.WriteAllText(@"data.txt", sb.ToString());
        }

        public void Load()
        {
            Clear();

            var lines = System.IO.File.ReadAllLines(@"Data.txt");

            var countNode = int.Parse(lines[0].Split('\t')[0]);
            //var countCorner = int.Parse(lines[0].Split('\t')[2]);
            for (int i = 1; i <= countNode; i++)
            {

                Nodes.Add(Node.Parse(lines[i]));
            }
            for (int i = countNode + 1; i < lines.Length; i++)
            {
                var f = lines[i].Split('\t').Select(x => int.Parse(x)).ToArray();
                Edges.Add(new Edge { A = Nodes[f[0]], B = Nodes[f[1]] });
            }
        }

        internal void Clear()
        {
            Nodes.Clear();
            Edges.Clear();
        }
        public int Radius;
        public string edge_Dash;

        public override string ToString()
        {
            return this.HeaderName;
        }
    }

    class Node : ViewModelBase
    {
        string _Text;
        public string Text  // полезные данные, характеризующие узел
        {
            get { return _Text; }
            set { Set(ref _Text, value, "Text"); }
        }
        public int Corner { get; set; }
        public Point Pos { get; set; }   //  узла на Canvas, нужна только для View
        bool selected;
        public bool Selected { get { return selected; } }
        public int ShadowOpacity { get { return selected ? 1 : 0; } }
        internal void InvSelect()
        {
            selected = !selected;
            Fire("ShadowOpacity");
        }
        internal void Select()
        {
            selected = true;
            Fire("ShadowOpacity");
        }
        internal void UnSelect()
        {
            selected = false;
            Fire("ShadowOpacity");
        }

        internal void MoveRef(Vector vector)
        {
            Pos += vector;
            Fire("Pos");
        }
        bool editMode = false;
        public bool EditMode
        {
            get { return editMode; }
            set
            {
                Set(ref editMode, value, "EditModeVisibility", "ViewModeVisibility");
            }
        }
        public Visibility EditModeVisibility { get { return EditMode ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility ViewModeVisibility { get { return !EditMode ? Visibility.Visible : Visibility.Collapsed; } }

        public override string ToString()
        {
            return Text + "\t" + Pos.X + "\t" + Pos.Y;
        }

        public static Node Parse(string nodeAsString)
        {
            var f = nodeAsString.Split('\t');
            return new Node { Text = f[0], Pos = new Point(double.Parse(f[1]), double.Parse(f[2])) };
        }
    }

    class Edge : ViewModelBase
    {
        public string Dash { get; set; }
        // public Point Pos { get; set; }
        public Node A { get; set; }
        public Node B { get; set; }
        public Point start { get; set; }
        public Point finish { get; set; }
        //public int ShadowOpacity_edge { get; set; }
        bool selected_edge;
        public bool Selected { get { return selected_edge; } }
        public int ShadowOpacity { get { return selected_edge ? 1 : 0; } }
        internal void InvSelect()
        {
            selected_edge = !selected_edge;
            Fire("ShadowOpacity");
        }
        internal void Select()
        {
            selected_edge = true;
            Fire("ShadowOpacity");
        }
        internal void UnSelect()
        {
            selected_edge = false;
            Fire("ShadowOpacity");
        }
    }
}
