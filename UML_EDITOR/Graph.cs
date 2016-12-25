using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Lab5
{
    [Serializable]
    public class ViewModelBase : INotifyPropertyChanged
    {
        [field:NonSerialized]
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


    [Serializable]
    class Graph : ViewModelBase
    {
        public Graph()
        {

        }
        public void createEdge(Node a, Node b)
        {
            var t = new Edge() { Dash = edge_Dash, arrowVis = edge_TriVis };
            t.SetNode(a, b);
            Edges.Add(t);
            a.linqs.Add(t);
            b.linqs.Add(t);
        }
        public void delEdge(Edge e)
        {
            e.A.linqs.Remove(e);
            e.B.linqs.Remove(e);
            Edges.Remove(e);
        }
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


        public void Save(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Close();
        }

        public static Graph Load(Stream stre)
        {            BinaryFormatter formatter = new BinaryFormatter();
            var t = formatter.Deserialize(stre) as Graph;
            stre.Close();
            return t;
        }
        public string edge_Dash;
        public Visibility edge_TriVis;

        public override string ToString()
        {
            return this.HeaderName;
        }
    }
    
    [Serializable]
    public class Method
    {
        public List<string> lst_s { get { return dict_Method.Values.ToList(); } }
        public string access { get; set; }
        public string beautiful_access { get { return dict_Method[access]; } set { SetAccess(value); } }
        public string name { get; set; }
        public string variables { get; set; }
        public Dictionary<string, string> dict_Method = new Dictionary<string, string> { { "+ ", "public" }, { "- ", "private" }, { "# ", "protected" }, { "/ ", "derived" }, { "~ ", "package" } };
        public void SetAccess(string str)
        {
            if (!dict_Method.ContainsValue(str))
                return;
            access = dict_Method.FirstOrDefault(x => x.Value == str).Key;
        }
        public override string ToString()
        {
            return (access + name + "(" + variables + ")");
        }
    }

    [Serializable]
    public class Variable
    {
        public List<string> lst_s_var { get { return dict_Variable.Values.ToList(); } }
        public string access { get; set; }
        public string beautiful_access_var { get { return dict_Variable[access]; } set { SetAccess(value); } }
        public string name { get; set; }
        public string tip { get; set; }
        public Dictionary<string, string> dict_Variable = new Dictionary<string, string> { { "+ ", "public" }, { "- ", "private" }, { "# ", "protected" }, { "/ ", "perived" }, { "~ ", "package" } };
        public void SetAccess(string str)
        {
            if (!dict_Variable.ContainsValue(str))
                return;
            access = dict_Variable.FirstOrDefault(x => x.Value == str).Key;
        }
        public override string ToString()
        {
            return (access + name + " : " + tip);
        }
    }

    [Serializable]
    public class Node : ViewModelBase, INotifyPropertyChanged
    {
        public int stereotype_index;
        const int stereotype_length = 4;
        public Node()
        {
            SizeMode = Visibility.Hidden;
            metods = new List<Method>();
            variables = new List<Variable>();
            _vis_stereotype = new Visibility[stereotype_length];
            for (int i = 0; i < stereotype_length; i++)
            {
                _vis_stereotype[i] = Visibility.Collapsed;
            }
            linqs = new List<Edge>();
        }
        public void SetMethods(List<Method> lm)
        {
            metods = lm;
            Fire(nameof(Metods));
        }
        public void SetVariables(List<Variable> lv)
        {
            variables = lv;
            Fire(nameof(Variables));
        }
        /// <summary>
        /// Начало кластера изменения переменных
        /// </summary>
        /// <param name="f"> входная строка</param> 
        public void AddVariable(Variable f)
        {
            variables.Add(f);
            Fire(nameof(Variables));
        }
        public List<Variable> variables;
        private string variables_str()
        {
            string res = "";
            foreach (var item in variables)
            {
                res += item + Environment.NewLine;
            }
            return res;
        }
        public string Variables
        {
            get { return variables_str(); }
        }
        /// <summary>
        /// Начало кластера изменения методов
        /// </summary>
        /// <param name="f">да это строка входная</param>
        public void AddMethod(Method f)
        {
            metods.Add(f);
            Fire(nameof(Metods));
        }
        public List<Method> metods;
        private string metods_str()
        {
            string res = "";
            foreach (var item in metods)
            {
                res += item + Environment.NewLine;
            }
            return res;
        }
        public string Metods
        {
            get { return metods_str(); }
        }
        string _Text;
        public string Text  // полезные данные, характеризующие узел
        {
            get { return _Text; }
            set { Set(ref _Text, value); }
        }


        public Point Pos { get; set; }   //  узла на Canvas, нужна только для View
        public Point Bot { get { return new Point(Pos.X + (int)Width / 2, Height + Pos.Y); } }
        public Point Center { get { return new Point(Pos.X + (int)Width / 2, Pos.Y + (int)Height / 2); } }
        public Point Right { get { return new Point(Pos.X + Width, (int)Height / 2 + Pos.Y); } }
        public Point Right_Bot { get { return new Point((Pos.X + Width) - 1, (Height + Pos.Y) - 1); } }
        public Point Top_Centr { get { return new Point((Pos.X), Pos.Y - (int)(Height / 2) - 18); } }
        public Point Left { get { return new Point(Pos.X, (int)Height / 2 + Pos.Y); } }
        public Point Top { get { return new Point(Pos.X + (int)Width/2, Pos.Y); } }

        public Visibility SizeMode { get; set; }

        public Visibility VisMode_stereotype_interface { get { return _vis_stereotype[0]; } }
        public Visibility VisMode_stereotype_control { get { return _vis_stereotype[1]; } }
        public Visibility VisMode_stereotype_boundary { get { return _vis_stereotype[2]; } }
        public Visibility VisMode_stereotype_entity { get { return _vis_stereotype[3]; } }

        private Visibility[] _vis_stereotype;

        public void StereotypeVis(int f)
        {
            for (int i = 0; i < stereotype_length; i++)
            {
                _vis_stereotype[i] = Visibility.Collapsed;
            }
            if (f<stereotype_length && f>=0)
            {
                _vis_stereotype[f] = Visibility.Visible;
            }
            Fire(nameof(VisMode_stereotype_boundary));
            Fire(nameof(VisMode_stereotype_control));
            Fire(nameof(VisMode_stereotype_entity));
            Fire(nameof(VisMode_stereotype_interface));
        }
        public void StereotypeVis()
        {
            StereotypeVis(stereotype_index-1);
        }

        public double Width { get; set; }
        public double Height { get; set; }

        public void ResizeModOn()
        {
            SizeMode = Visibility.Visible;
            Fire("SizeMode");
        }
        public void ResizeModOff()
        {
            SizeMode = Visibility.Hidden;
            Fire("SizeMode");   
        }
        public void Resize(double w, double h)
        {
            if (w > 50 && w < 203)
                Width = w;
            if (h > 50)
                Height = h;
            Fire("Width");
            Fire("Height");
            FireAnchors();
            foreach (var item in linqs)
            {
                item.calcReplace();
                item.calcArrow();
            }
        }

        public void FireAnchors()
        {
            Fire("Right");
            Fire("Bot");
            Fire(nameof(Right_Bot));
            Fire(nameof(Center));
            Fire(nameof(Top_Centr));
            foreach (var item in linqs)
            {
                item.calcReplace();
                item.calcArrow();
            }
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
            foreach (var item in linqs)
            {
                item.calcReplace();
                item.calcArrow();
            }
        }
        public List<Edge> linqs;
        public override string ToString()
        {
            return Text + "\t" + Pos.X + "\t" + Pos.Y;
        }

        private string parent;
        public string Parent { get { return String.IsNullOrEmpty(parent)? parent: " : "+parent; }set { parent = value; } }
        
        public string Parse()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("public class {0}{1}", Text, Parent));
            sb.AppendLine("{");
            sb.AppendLine("\n// автоматически сгенерированный список переменных");
            foreach (var item in variables)
            {
                string t = !(item.access == "+ " || item.access == "# ") ? item.dict_Variable["- "] : item.beautiful_access_var;
                sb.AppendLine(String.Format("\t{0} {1} {2} ;", t, item.tip, item.name));
            }
            sb.AppendLine("\n// автоматически сгенерированный список методов");
            foreach (var item in metods)
            {
                string t = !(item.access == "+ " || item.access == "# ") ? item.dict_Method["- "] : item.beautiful_access;
                sb.AppendLine(String.Format("\t{0} void {1} ({2}) ", t, item.name, item.variables));
                sb.AppendLine("\t{ ");
                sb.AppendLine("\t\t"+@"new NullReferenceException(""null realizatoin"");");
                sb.AppendLine("\t} ");
            }
            return sb.ToString();
        }
    }
    [Serializable]
    public class Edge : ViewModelBase
    {
        public string Dash { get; set; }
        public Visibility arrowVis { get; set; }
        public double X3 {get;set;}
        public double Y3 {get;set;}
        public double X5 {get;set;}
        public double Y5 {get;set;}
        public double X6 {get;set;}
        public double Y6 { get; set; }
        public Node A { get; set; }
        public Node B { get; set; }
        public Point start { get; set; }
        public Point finish { get; set; }
        private bool Direction;
        
        public void InvDirection()
        {
            Direction = !Direction;
            calcReplace();
            calcArrow();
        }
        public void calcArrow()
        {
            //centr
            X3 = (start.X + finish.X) / 2;
            Y3 = (start.Y + finish.Y) / 2;

            // Length of edge
            double d = Math.Sqrt(Math.Pow((finish.X - start.X), 2) + Math.Pow((finish.Y - start.Y), 2));

            //vector coords
            double X = finish.X - start.X;
            double Y = finish.Y - start.Y;

            //coords of point center+10px
            double X4 = X3 - (X / d) * 16;
            double Y4 = Y3 - (Y / d) * 16;

            //Line ur
            double Xp = finish.Y - start.Y;
            double Yp = start.X - finish.X;

            // координаты перпендикуляров, удалённой от точки X4;Y4 на 5px в разные стороны
            X5 = X4 + (Xp / d) * 7;
            Y5 = Y4 + (Yp / d) * 7;
            X6 = X4 - (Xp / d) * 7;
            Y6 = Y4 - (Yp / d) * 7;
            Fire(nameof(X3));
            Fire(nameof(X5));
            Fire(nameof(X6));
            Fire(nameof(Y3));
            Fire(nameof(Y5));
            Fire(nameof(Y6));
        }
        public void calcReplace()
        {
            bool dx = A.Pos.X - B.Pos.X > 0;
            bool dy = A.Pos.Y - B.Pos.Y > 0;

            if (dx)
            {
                start = A.Left;
            }
            else
            {
                start = A.Right;  
            }
            if (dy)
            {
                finish = B.Bot;
            }
            else
            {
                finish = B.Top;
            }
            if (!Direction)
            {
                var e = finish;
                finish = start;
                start = e;
            }
            Fire(nameof(start));
            Fire(nameof(finish));
        }
        public void SetNode(Node from, Node to)
        {
            A = from;
            B = to;
            Direction = true;
            calcReplace();
        }
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
