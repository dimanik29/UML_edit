using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Lab5
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void Set<T>(ref T field, T value, params string[] propNames)
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

        protected void Set<T>(ref T field, T value, [CallerMemberName] string propName = null)
        {
            if (field != null && !field.Equals(value) || value != null && !value.Equals(field))
            {
                field = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propName));
                }
            }
        }

        protected void Fire([CallerMemberName] string propName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
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


    class Graph : ViewModelBase
    {
        public Graph()
        {

        }
        //public string HeaderName { get { return "UML 1"; } }
        //string _Text;
        //public string Text  // полезные данные, характеризующие узел
        //{
        //    get { return _Text; }
        //    set { Set(ref _Text, value, "Text"); }
        //}
        string hedee;
        public string HeaderName
        {
            get { return hedee; }
            set { Set(ref hedee, value); }
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
                sb.AppendLine(node.ToString() + "\t" + "Corner = ");
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
        public string edge_Dash;

        public override string ToString()
        {
            return this.HeaderName;
        }
    }

    class Node : ViewModelBase,INotifyPropertyChanged
    {
        public Node()
        {
            SizeMode = Visibility.Hidden;
            metods = new StringBuilder();
        }
        public void AddMethod(string f)
        {
            metods.Append(f);
            metods.Append(Environment.NewLine);
            Fire(nameof(Metods));
        }
        private StringBuilder metods;
        public string Metods
        {
            get { return metods.ToString(); }
        }
        //public event PropertyChangedEventHandler PropertyChanged;
        string _Text;
        public string Text  // полезные данные, характеризующие узел
        {
            get { return _Text; }
            set { Set(ref _Text, value); }
        }
        

        public Point Pos { get; set; }   //  узла на Canvas, нужна только для View
        public Point Bot { get { return new Point(Pos.X + (int)Width / 2, Height + Pos.Y); } }
        public Point Center { get { return new Point(Pos.X + (int)Width / 2, Pos.Y + (int)Height/2  ); } }
        public Point Right { get { return new Point(Pos.X + Width, (int)Height / 2 + Pos.Y); } }
        public Point Right_Bot { get { return new Point((Pos.X + Width)-1, (Height + Pos.Y)-1); } }

        public Visibility SizeMode { get; set; }




        public double Width { get; set; }
        public double Height { get; set; }

        public void ResizeModOn()
        {
            SizeMode = Visibility.Visible;
            Fire("SizeMode");
            //Pch?.Invoke(this, new PropertyChangedEventArgs(nameof(SizeMode)));
        }
        public void ResizeModOff()
        {
            SizeMode = Visibility.Hidden;
            Fire("SizeMode");
            //Pch?.Invoke(this, new PropertyChangedEventArgs(nameof(SizeMode)));
        }
        public void Resize(double w, double h)
        {
            if (w > 50 && w < 203)
            Width = w;
            if(h>50)
            Height = h;
            Fire("Width");
            Fire("Height");
            FireAnchors();
        }

        public void FireAnchors()
        {
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Left)));
            Fire("Right");
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Right)));
            Fire("Bot");
            Fire(nameof(Right_Bot));//test
            Fire(nameof(Center));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Bot)));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Top)));
        }

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
            FireAnchors();
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
