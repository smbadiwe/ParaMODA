using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Motif1
{
    public partial class Form1 : Form
    {

        int[] _nodes_degree;
        bool[] _nodes_availabe;
        int _maximum_graph_size;
        graph _network;
        graph _copy_graph;
        List<List<int>> _temp_size_k_graphs;
        List<List<int>> _size_k_graphs;

        List<List<int>> _subgraph_maps_domain;
        List<List<int>> _subgraph_maps_range;
        List<int> _breaking_condition;

        /// <summary>
        /// Set after processing 'Load G' button click on top; meaning G is the network
        /// </summary>
        NewGraph _network2;
        /// <summary>
        /// Set after processing 'Load H' button click on top; meaning H is the query graph - as in, the sugraph we're searcing for
        /// </summary>
        QueryGraph _query2;

        int[] _f;
        graph _graph_h;
        bool[] _flag;

        graph _network_pallete;
        Isomorphism _ism;
        OpenFileDialog _openFileDialog1;
        public Form1()
        {
            _maximum_graph_size = 100000;
            InitializeComponent();
            _nodes_degree = new int[_maximum_graph_size];
            _nodes_availabe = new bool[_maximum_graph_size];
            _network = new graph(false);
            _f = new int[50];
            _graph_h = new graph(false);

            _openFileDialog1 = new OpenFileDialog();

            _openFileDialog1.InitialDirectory = @"C:\SOMA\Drive\MyUW\Research\Kim\MODA\MODAForm";
            _openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            _openFileDialog1.FilterIndex = 1;
        }

        List<List<int>> find_breaking_conditions(List<List<int>> A)
        {
            int n = 0;

            List<int> local;
            List<List<int>> result = new List<List<int>>();
            int max = 0;
            while (A.Count != 1)
            {
                List<List<int>> equvalent = find_equivalents(A);
                max = get_largest_index(equvalent);
                n = equvalent[max][0];
                local = new List<int>();
                local.Add(n);
                for (int i = 1; i < equvalent[max].Count; i++)
                    local.Add(equvalent[max][i]);
                result.Add(local);
                A = refine(A, n);
            }
            return result;
        }
        List<List<int>> refine(List<List<int>> A, int n)
        {
            List<List<int>> result = new List<List<int>>();
            List<int> local;
            for (int i = 0; i < A.Count; i++)
                if (A[i][n] == n)
                {
                    local = new List<int>();
                    local = copy_list(A[i]);
                    result.Add(local);
                }

            return result;
        }
        int get_largest_index(List<List<int>> T)
        {
            int index = 0;
            for (int i = 0; i < T.Count; i++)
                if (T[i].Count > index)
                    index = i;
            return index;
        }

        /// <summary>
        /// From the bottom 'Load G' button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadG_Bottom_Click(object sender, EventArgs e)
        {
            load(_network);
            _copy_graph = new graph(false);
            _copy_graph.Adj_matrix = new List<bool>();
            for (int i = 0; i < _network.Adj_matrix.Count; i++)
            {
                _copy_graph.Adj_matrix.Add(_network.Adj_matrix[i]);
            }
        }

        /// <summary>
        /// Called from the bottom 'Load G' button's event
        /// </summary>
        /// <param name="network"></param>
        void load(graph network)
        {
            Stream myStream = null;
            string filename = string.Empty;

            if (_openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = _openFileDialog1.OpenFile()) != null)
                {
                    // Insert code to read the stream here.
                    filename = _openFileDialog1.FileName;
                    myStream.Close();
                }
            }

            if (myStream == null)
                return;
            progressBar1.Value = 0;
            progressBar1.Visible = true;

            StreamWriter sw = new StreamWriter(@"C:\SOMA\Drive\MyUW\Research\Kim\MODA\MODAForm\nemtmp.txt", false);
            char c = '"';
            //ready
            sw.WriteLine("Graph G{");
            sw.WriteLine("      node [ ");
            sw.WriteLine("      fontname=" + c + "Arial" + c);
            sw.WriteLine("      width=" + c + "0.40000" + c);
            sw.WriteLine("      height=" + c + "0.400000" + c);
            sw.WriteLine("      color=" + c + "black" + c);
            sw.WriteLine("          ]");
            sw.WriteLine("");
            sw.WriteLine("      edge [ ");
            sw.WriteLine("      color=" + c + "black" + c);
            sw.WriteLine("           ]");
            sw.WriteLine("");
            StreamReader sr = new StreamReader(filename);
            string local = string.Empty;

            network.number_node = int.Parse(sr.ReadLine());
            network.prepare_AdjMatrix();

            string[] tmp = new string[3];
            int temp_node = 0;
            int temp_node2 = 0;
            while (!sr.EndOfStream)
            {
                local = sr.ReadLine();
                tmp = local.Split(new string[] { " ", "\t" }, StringSplitOptions.None);
                sw.WriteLine("n_" + tmp[0] + " -- n_" + tmp[1]);
                temp_node = int.Parse(tmp[0]);
                temp_node2 = int.Parse(tmp[1]);
                _nodes_degree[temp_node]++;

                _nodes_availabe[temp_node] = true;
                _nodes_availabe[temp_node2] = true;

                network.add_link(temp_node, temp_node2);
            }
            sw.WriteLine("}");
            sw.Flush();

            progressBar1.Value = 33;
            //string cmd = "C:\\Program Files\\Graphviz2.16\\bin\\dot";
            //string param = " -Tpng -o " + c + "c:\\motifpics\\motif.jpg" + c + " -Kdot " + c + "c:\\motifpics\\nemtmp.txt" + c;
            //Process.Start(cmd, param);
            //Thread.Sleep(500);

            //pictureBox1.Image = Image.FromFile(@"c:\motifpics\motif.jpg");
            progressBar1.Value = 76;
            show_graph_info();

            Thread.Sleep(10);
            progressBar1.Visible = false;

            // vScrollBar1.Size = new Size(vScrollBar1.Width, pictureBox1.Height / pictureBox1.Image.Height);


            sr.Close();
            sw.Close();
            //frm.Show();

            btnFindSubgraphs.Enabled = true;


        }

        /// <summary>
        /// Called from the bottom 'Load H' button event
        /// </summary>
        /// <param name="network"></param>
        void load_with_breaking_conditions(graph network)
        {
            Stream myStream;
            myStream = null;
            string filename = string.Empty;

            if (_openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = _openFileDialog1.OpenFile()) != null)
                {
                    // Insert code to read the stream here.
                    filename = _openFileDialog1.FileName;
                    myStream.Close();
                }
            }

            if (myStream == null)
                return;
            progressBar1.Value = 0;
            progressBar1.Visible = true;

            StreamWriter sw = new StreamWriter(@"C:\SOMA\Drive\MyUW\Research\Kim\MODA\MODAForm\nemtmp.txt", false);
            char c = '"';
            //ready
            sw.WriteLine("Graph G{");
            sw.WriteLine("      node [ ");
            sw.WriteLine("      fontname=" + c + "Arial" + c);
            sw.WriteLine("      width=" + c + "0.40000" + c);
            sw.WriteLine("      height=" + c + "0.400000" + c);
            sw.WriteLine("      color=" + c + "black" + c);
            sw.WriteLine("          ]");
            sw.WriteLine("");
            sw.WriteLine("      edge [ ");
            sw.WriteLine("      color=" + c + "black" + c);
            sw.WriteLine("           ]");
            sw.WriteLine("");
            StreamReader sr = new StreamReader(filename);
            string local = string.Empty;

            network.number_node = int.Parse(sr.ReadLine());
            network.prepare_AdjMatrix();

            _breaking_condition = new List<int>();
            for (int i = 0; i < network.number_node; i++)
                _breaking_condition.Add(-1);

            string[] tmp = new string[3];
            int temp_node = 0;
            int temp_node2 = 0;
            int counter = 0;
            bool condition = true;
            while (condition)
            {
                local = sr.ReadLine();
                if (local[0] != '*')
                {
                    tmp = local.Split(new string[] { " ", "\t" }, StringSplitOptions.None);
                    sw.WriteLine("n_" + tmp[0] + " -- n_" + tmp[1]);
                    temp_node = int.Parse(tmp[0]);
                    temp_node2 = int.Parse(tmp[1]);
                    _nodes_degree[temp_node]++;

                    _nodes_availabe[temp_node] = true;
                    _nodes_availabe[temp_node2] = true;

                    network.add_link(temp_node, temp_node2);
                    counter++;
                    if (counter == network.number_node) condition = false;
                }
                else
                    condition = false;
            }
            sw.WriteLine("}");
            sw.Flush();

            progressBar1.Value = 33;
            //string cmd = "C:\\Program Files\\Graphviz2.16\\bin\\dot";
            string param = " -Tpng -o " + c + "c:\\motifpics\\motif.jpg" + c + " -Kdot " + c + "c:\\motifpics\\nemtmp.txt" + c;
            //Process.Start(cmd, param);
            //Thread.Sleep(500);

            //pictureBox1.Image = Image.FromFile(@"c:\motifpics\motif.jpg");
            progressBar1.Value = 76;
            show_graph_info();

            Thread.Sleep(10);
            progressBar1.Visible = false;

            // vScrollBar1.Size = new Size(vScrollBar1.Width, pictureBox1.Height / pictureBox1.Image.Height);



            //skip next line
            sr.ReadLine();

            while (!sr.EndOfStream)
            {
                local = sr.ReadLine();
                tmp = local.Split(new string[] { " ", "\t" }, StringSplitOptions.None);
                temp_node = int.Parse(tmp[0]);
                temp_node2 = int.Parse(tmp[1]);
                _breaking_condition[temp_node] = temp_node2;
            }
            sr.Close();
            sw.Close();
            //frm.Show();

            btnFindSubgraphs.Enabled = true;


        }

        void show_graph_info()
        {
            long sum_node_degree = 0;
            int number_nodes = 0;
            int i = 1;
            float avg_degree = 0;

            for (i = 0; i < _maximum_graph_size; i++)
            {
                if (_nodes_degree[i] != 0)
                {
                    sum_node_degree += _nodes_degree[i];
                }
                if (_nodes_availabe[i])
                {
                    number_nodes++;
                }
            }

            avg_degree = (float)sum_node_degree / (float)number_nodes;

            progressBar1.Value = 100;
            nodeLBL.Text = number_nodes.ToString();
            edgeLBL.Text = sum_node_degree.ToString();
            DensityLBL.Text = avg_degree.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            progressBar1.Visible = false;
            comboBox1.SelectedIndex = 0;
            button2.Visible = false;
            panel2.Visible = false;
        }

        private void btnFindSubgraphs_Click(object sender, EventArgs e)
        {
            int k = 0;
            try
            {
                //k = int.Parse(textBox1.Text);
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message.ToString());
                return;
            }
            if (k < 1)
            {
                MessageBox.Show("The size of subgraph must be greater than zero");
                return;
            }
            if (k > _network.number_node + 1)
            {
                MessageBox.Show("The size of subgraph must be less than the size of network");
                return;
            }

            //List<List<int>> res = Enumerate_Subgraphs(network, k);
            Enumerate_Subgraphs(_network, k);
            show_result(richTextBox1, _size_k_graphs);

        }
        void show_result(RichTextBox rTxtBox, List<List<int>> result)
        {
            int i = 0;
            int j = 0;
            string s = string.Empty;
            rTxtBox.Clear();
            for (i = 0; i < result.Count; i++)
            {
                s = string.Empty;
                for (j = 0; j < result[i].Count; j++)
                    s += "  " + result[i][j].ToString();
                rTxtBox.Text += s + "\n";
            }


        }

        string convert_list_to_string(List<int> items)
        {
            string s = string.Empty;
            for (int i = 0; i < items.Count; i++)
                s += items[i].ToString();
            return s;
        }

        //finds all mappings
        void clear_function(int[] function, int size)
        {
            for (int i = 0; i < size; i++)
                function[i] = -1;
        }

        void find_subgraph_instances(graph G, graph H)
        {
            int range_tag_size = 1000;
            _temp_size_k_graphs = new List<List<int>>();
            List<int> function_domain = new List<int>();
            _subgraph_maps_domain = new List<List<int>>();
            _subgraph_maps_range = new List<List<int>>();
            List<int> used_range = new List<int>();
            _flag = new bool[10000];
            bool[] domain_tag = new bool[50];
            bool[] range_tag = new bool[range_tag_size];
            int[] local_result = new int[50];
            int i = 0;
            int j = 0;

            for (i = 0; i < G.number_node; i++)
            {
                clear_domain_tag(domain_tag, 50);
                clear_domain_tag(range_tag, range_tag.Length);
                clear_function(_f, 50);
                for (j = 0; j < H.number_node; j++)
                {
                    function_domain = new List<int>();
                    used_range = new List<int>();
                    if (can_support(i, G, j, H))
                    {
                        _f[j] = i;
                        domain_tag[j] = true;
                        range_tag[i] = true;
                        function_domain.Add(j);
                        used_range.Add(i);
                        local_result = isomorphic_extensions(_f, G, H, function_domain, used_range, domain_tag, range_tag);
                    }
                }

                _network.remove_edge(i);
            }

        }

        void clear_domain_tag(bool[] domain_tag, int size)
        {
            for (int i = 0; i < size; i++)
                domain_tag[i] = false;
        }

        int[] isomorphic_extensions(int[] function, graph G, graph H, List<int> domain, List<int> used_range, bool[] domain_tag, bool[] range_tag)
        {
            List<int> neighbor_range = new List<int>();
            List<int> domain_prim = new List<int>();
            List<int> used_range_prim = new List<int>();
            bool[] domain_tag_prim = new bool[50];
            int range_tag_prim_size = 1000;
            bool[] range_tag_prim = new bool[range_tag_prim_size];
            int[] function_prim = new int[50];

            int m = 0;

            if (domain.Count == H.number_node)
            {
                add_mapping_to_list(function, _subgraph_maps_domain, _subgraph_maps_range, domain.Count);
                return function;
            }
            m = choose_one_neighbor_of_domain(domain, H, domain_tag);
            if (m == -1)
                return null;

            neighbor_range = choose_neighbor_of_range(used_range, G, range_tag);


            copy_array(function, function_prim, 50);

            for (int i = 0; i < neighbor_range.Count; i++)
            {
                if (node_is_compatible(G, H, neighbor_range[i], m, domain, function))
                {

                    used_range_prim = copy_list(used_range);
                    function_prim[m] = neighbor_range[i];
                    used_range_prim.Add(neighbor_range[i]);
                    domain_prim = copy_list(domain);
                    domain_prim.Add(m);
                    copy_array(domain_tag, domain_tag_prim, domain_tag_prim.Length);
                    copy_array(range_tag, range_tag_prim, range_tag_prim.Length);
                    domain_tag_prim[m] = true;
                    range_tag_prim[neighbor_range[i]] = true;
                    isomorphic_extensions(function_prim, G, H, domain_prim, used_range_prim, domain_tag_prim, range_tag_prim);
                }

            }
            int[] t = new int[1];
            return t;
        }
        void copy_array(bool[] original, bool[] copy, int size)
        {
            for (int i = 0; i < size; i++)
                copy[i] = original[i];
        }
        void copy_array(int[] original, int[] copy, int size)
        {
            for (int i = 0; i < size; i++)
                copy[i] = original[i];
        }
        bool node_is_compatible(graph G, graph H, int g_node, int h_node, List<int> domain_in_H, int[] function)
        {
            List<int> neighbor = new List<int>();
            List<int> non_neighbor = new List<int>();
            int i = 0;


            for (i = 0; i < 50; i++)
                if (function[i] == g_node)
                    return false;
            //no repeated mapping in range is allowed

            if (radioButton1.Checked)
            {
                int t = find_domain_breaking(h_node);
                if (t != -1)
                {
                    if (function[t] > g_node)
                        return false;
                }
            }
            neighbor = H.find_neighbors(h_node);


            //split into two sets
            int local_counter = 0;
            for (i = 0; i < neighbor.Count + local_counter; i++)
            {
                if (!domain_in_H.Contains(neighbor[i - local_counter]))
                {

                    neighbor.Remove(neighbor[i - local_counter]);
                    local_counter++;
                }
            }
            for (i = 0; i < domain_in_H.Count; i++)
            {
                if (!neighbor.Contains(domain_in_H[i]))
                {
                    non_neighbor.Add(domain_in_H[i]);
                }
            }

            List<int> local = new List<int>();
            local = G.find_neighbors_with_flag(g_node, _flag);
            for (i = 0; i < neighbor.Count; i++)
            {
                if (!local.Contains(function[neighbor[i]]))
                    return false;
            }

            //Inja paper eshtebah dasht
            //for (i = 0; i <non_neighbor.Count; i++)
            //{
            //    if (local.Contains(function[non_neighbor[i]]))
            //        return false;
            //}

            return true;
        }
        int find_domain_breaking(int range)
        {
            for (int i = 0; i < _breaking_condition.Count; i++)
                if (_breaking_condition[i] == range)
                    return i;
            return -1;
        }
        //choose next vertex that is neighbor of domain
        int choose_one_neighbor_of_domain(List<int> used_domain, graph H, bool[] domain_tag)
        {
            List<int> local = new List<int>();
            List<int> result = new List<int>();
            int local_counter = 0;
            for (int i = 0; i < used_domain.Count; i++)
            {
                local = H.find_neighbors(used_domain[i]);
                local_counter = 0;
                for (int j = 0; j < local.Count + local_counter; j++)
                {
                    if (domain_tag[local[j - local_counter]])
                    {
                        local.Remove(local[j - local_counter]);
                        local_counter++;
                    }
                }
                add_list(result, local);
            }
            if (result.Count == 0)
                return -1;
            else
                return result[0];
        }
        List<int> choose_neighbor_of_range(List<int> used_range, graph G, bool[] range_tag)
        {
            List<int> local = new List<int>();
            List<int> result = new List<int>();
            int local_counter = 0;
            for (int i = 0; i < used_range.Count; i++)
            {
                local = G.find_neighbors(used_range[i]);
                if (local.Count == 0)
                    return result;
                local_counter = 0;
                for (int j = 0; j < local.Count + local_counter; j++)
                    if (range_tag[local[j - local_counter]] || _flag[local[j - local_counter]])
                    {
                        local.Remove(local[j - local_counter]);
                        local_counter++;
                    }
                add_list(result, local);
            }
            return result;
        }

        List<int> find_neighbors_of_domain(List<int> myDomain, graph H)
        {
            List<int> result = new List<int>();
            List<int> temp = new List<int>();
            int i = 0;
            int j = 0;
            for (i = 0; i < myDomain.Count; i++)
            {
                temp = new List<int>();
                temp = H.find_neighbors(myDomain[i]);
                for (j = 0; j < temp.Count; j++)
                {
                    result.Add(temp[j]);
                }
            }
            return result;

        }

        void add_mapping_to_list(int[] func, List<List<int>> DOMAIN, List<List<int>> RANGE, int counter)
        {
            int i = 0;
            List<int> local_domain = new List<int>();
            List<int> local_range = new List<int>();
            for (i = 0; i < counter; i++)
            {
                local_domain.Add(i);
                local_range.Add(func[i]);
            }
            DOMAIN.Add(local_domain);
            RANGE.Add(local_range);
        }

        void Enumerate_Subgraphs(graph G, int k)
        {
            List<List<int>> result = new List<List<int>>();
            List<int> v_extend = new List<int>();
            List<int> v_subgraph = new List<int>();
            List<int> local_result = new List<int>();
            _size_k_graphs = new List<List<int>>();
            _temp_size_k_graphs = new List<List<int>>();

            int i = 0;
            for (i = 0; i < G.number_node; i++)
            {
                v_subgraph = new List<int>();
                v_subgraph.Add(i);
                v_extend = G.find_neighbors_grater_than(i, i);
                local_result = Extend_SubGraph(v_subgraph, v_extend, i, k, G);
                _temp_size_k_graphs.Add(local_result);
            }

        }
        List<int> Extend_SubGraph(List<int> V_SUBGRAPH, List<int> V_EXTEND, int node, int k, graph G)
        {
            List<int> temp_list = new List<int>();
            if (V_SUBGRAPH.Count == k)
            {
                temp_list = copy_list(V_SUBGRAPH);
                _size_k_graphs.Add(temp_list);
                return V_SUBGRAPH;
            }
            List<int> extend_prim = new List<int>();
            List<int> V_Subgraph_prim = new List<int>();
            int w = 0;
            int count = 0;
            while (V_EXTEND.Count > 0)
            {
                V_Subgraph_prim = new List<int>();
                V_Subgraph_prim = copy_list(V_SUBGRAPH);
                count = V_EXTEND.Count;
                //remove
                w = V_EXTEND[0];
                V_EXTEND.Remove(w);
                //extend
                extend_prim = copy_list(V_EXTEND);
                add_list(extend_prim, G.find_neighbors_grater_than(w, w));
                V_Subgraph_prim.Add(w);
                Extend_SubGraph(V_Subgraph_prim, extend_prim, node, k, G);
            }
            return null;
        }

        void add_list(List<int> original, List<int> added_list)
        {
            int i = 0;
            for (i = 0; i < added_list.Count; i++)
                if (!original.Contains(added_list[i]))
                    original.Add(added_list[i]);
        }

        List<int> copy_list(List<int> original)
        {
            List<int> copied = new List<int>();
            int i = 0;
            copied = new List<int>();
            for (i = 0; i < original.Count; i++)
                copied.Add(original[i]);
            return copied;
        }


        bool can_support(int node_G, graph G, int node_H, graph H)
        {

            //neighboor condition satisfied must be added in futeure.
            return (G.degree_of_node(node_G) >= H.degree_of_node(node_H));
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void btnLoadH_Bottom_Click(object sender, EventArgs e)
        {
            load_with_breaking_conditions(_graph_h);
            Bitmap bit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics grfx = Graphics.FromImage(bit);
            Draw visual = new Draw(grfx, bit.Width, _graph_h.number_node, _graph_h.Adj_matrix);
            visual.draw(1, Color.Black, Color.OrangeRed);
            bit.Save("c:\\pallete.jpg");
            pictureBox1.Image = Image.FromFile("c:\\pallete.jpg");

        }

        private void btnFindMappings_Click(object sender, EventArgs e)
        {
            //copy network 

            DateTime dt = DateTime.Now;
            int h1 = dt.Hour;
            int m1 = dt.Minute;
            int s1 = dt.Second;
            int ms1 = dt.Millisecond;

            _network_pallete = new graph(false);
            _network_pallete.number_node = _network.number_node;
            _network_pallete.Adj_matrix = new List<bool>();
            for (int k = 0; k < _network.Adj_matrix.Count; k++)
                _network_pallete.Adj_matrix.Add(_network.Adj_matrix[k]);


            find_subgraph_instances(_network, _graph_h);
            dt = DateTime.Now;
            double time = ((dt.Hour - h1) * 3600 +
                (dt.Minute - m1) * 60 +
                (dt.Second - s1) +
                ((double)(dt.Millisecond - ms1) / 1000));
            label11.Text = time + " second(s)";
            panel2.Visible = true;

            label14.Text = _subgraph_maps_domain.Count.ToString();
            label14.Visible = true;
            label20.Visible = true;
            button2.Visible = true;

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
        List<List<int>> find_equivalents(List<List<int>> A)
        {
            int n = A[0].Count;
            List<int> V = new List<int>();
            List<List<int>> result = new List<List<int>>();
            List<int> local;
            int i = 0;
            for (; i < n; i++)
                V.Add(i);
            i = 0;
            while (V.Count != 0 && i < n)
            {
                local = new List<int>();
                for (int k = 0; k < A.Count; k++)
                {
                    if (!local.Contains(A[k][i]))
                    {
                        local.Add(A[k][i]);
                        V.Remove(A[k][i]);
                    }

                }
                result.Add(local);
                i++;
            }
            return result;
        }


        private void button8_Click_1(object sender, EventArgs e)
        {
            //Bitmap bit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            //Graphics grfx = Graphics.FromImage(bit);
            //Draw visual = new Draw(grfx, bit.Width, network_pallete.number_node, network_pallete.Adj_matrix);
            //visual.draw(1, Color.Black, Color.OrangeRed);
            //bit.Save("c:\\network.jpg");

            string address = "c:\\pallete.jpg";
            pallete_frm frm = new pallete_frm(_network_pallete.Adj_matrix, _network_pallete.number_node, _subgraph_maps_domain, _subgraph_maps_range, address, pictureBox1.Width);
            frm.ShowDialog();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string address = "c:\\pallete.jpg";
            pallete_frm frm = new pallete_frm(_network_pallete.Adj_matrix, _network_pallete.number_node, _subgraph_maps_domain, _subgraph_maps_range, address, pictureBox1.Width);
            frm.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            int h1 = dt.Hour;
            int m1 = dt.Minute;
            int s1 = dt.Second;
            int ms1 = dt.Millisecond;

            int count = 0;
            _copy_graph.number_node = _network.number_node;
            panel2.Visible = true;
            for (int i = 0; i < _subgraph_maps_domain.Count; i++)
            {
                //Text = i.ToString();
                if (_copy_graph.is_link_between(_subgraph_maps_range[i][0], _subgraph_maps_range[i][3]))
                    count++;
            }
            label7.Text = count.ToString() + " mappings , have been satisfied";

            dt = DateTime.Now;
            double time = ((dt.Hour - h1) * 3600 +
                (dt.Minute - m1) * 60 +
                (dt.Second - s1) +
                ((double)(dt.Millisecond - ms1)) / 1000);
            label11.Text = time + " second(s)";
        }

        private void btnShowMapping_Click(object sender, EventArgs e)
        {
            string s = string.Empty;
            richTextBox1.Clear();

            for (int i = 0; i < _subgraph_maps_domain.Count; i++)
            {
                for (int j = 0; j < _subgraph_maps_domain[i].Count; j++)
                {
                    s += _subgraph_maps_domain[i][j].ToString() + " --> " +
                        _subgraph_maps_range[i][j].ToString() + " , ";
                }
                s += "\n";
            }
            richTextBox1.Text = s;

        }

        /// <summary>
        /// Called by the 'Load G' button (event) on top in the form
        /// </summary>
        void load_file()
        {


            Stream myStream;
            myStream = null;
            string filename = string.Empty;

            if (_openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = _openFileDialog1.OpenFile()) != null)
                {
                    // Insert code to read the stream here.
                    filename = _openFileDialog1.FileName;
                    myStream.Close();
                }
            }

            if (myStream == null)
                return;

            StreamReader sr = new StreamReader(filename);
            string local = string.Empty;

            int node_number = int.Parse(sr.ReadLine());
            ///////////
            int sum = 0;
            //int i = 1;

            //for (; i < node_number; i++)
            //    sum += i;

            sum = (int)(1.0 * node_number * (node_number - 1));

            ///////////////
            List<bool> adj_matrix = new List<bool>(sum);
            for (int i = 0; i < sum; i++)
                adj_matrix.Add(false);

            string[] tmp = new string[3];
            int temp_node = 0;
            int temp_node2 = 0;
            while (!sr.EndOfStream)
            {
                local = sr.ReadLine();
                tmp = local.Split(new string[] { " ", "\t" }, StringSplitOptions.None);
                temp_node = int.Parse(tmp[0]);
                temp_node2 = int.Parse(tmp[1]);
                _nodes_degree[temp_node]++;

                add_link(temp_node, temp_node2, adj_matrix, node_number);
            }


            sr.Close();

            _network2 = new NewGraph(adj_matrix);

        }

        /// <summary>
        /// Called by the 'Load H' button (event) on top in the form
        /// </summary>
        void load_file2()
        {


            Stream myStream;
            myStream = null;
            string filename = string.Empty;

            if (_openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = _openFileDialog1.OpenFile()) != null)
                {
                    // Insert code to read the stream here.
                    filename = _openFileDialog1.FileName;
                    myStream.Close();
                }
            }

            if (myStream == null)
                return;

            StreamReader sr = new StreamReader(filename);
            string local = string.Empty;

            int node_number = int.Parse(sr.ReadLine());
            ///////////
            int sum = 0;
            //int i = 1;

            //for (; i < node_number; i++)
            //    sum += i;

            sum = (int)(0.5 * node_number * (node_number - 1));

            ///////////////
            List<bool> adj_matrix = new List<bool>(sum);
            for (int i = 0; i < sum; i++)
                adj_matrix.Add(false);

            string[] tmp = new string[3];
            int temp_node = 0;
            int temp_node2 = 0;
            while (!sr.EndOfStream)
            {
                local = sr.ReadLine();
                tmp = local.Split(new string[] { " ", "\t" }, StringSplitOptions.None);
                temp_node = int.Parse(tmp[0]);
                temp_node2 = int.Parse(tmp[1]);
                _nodes_degree[temp_node]++;

                add_link(temp_node, temp_node2, adj_matrix, node_number);
            }


            sr.Close();

            _query2 = new QueryGraph(adj_matrix);

        }

        /// <summary>
        /// Record as edge in the adjacency matrix, given two nodes
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="Adj_matrix"></param>
        /// <param name="number_node"></param>
        public void add_link(int node1, int node2, List<bool> Adj_matrix, int number_node)
        {
            //swap
            if (node2 < node1)
            {
                int temp = node2;
                node2 = node1;
                node1 = temp;
            }

            //the first element in Adj_matrix is stored in index 0
            int index = (node1 * (2 * number_node - 1 - node1) / 2) +
                    (node2 - node1 - 1);
            if (index >= 0 && !Adj_matrix[index])
                Adj_matrix[index] = true;
        }

        //The 'Load G' button on top
        private void btnLoadG_Top_Click(object sender, EventArgs e)
        {
            load_file();
        }

        //The 'Load H' button on top
        private void btnLoadH_Top_Click(object sender, EventArgs e)
        {
            load_file2();
        }

        private void btnIsomorphism_Click(object sender, EventArgs e)
        {
            Isomorphism();
        }

        /// <summary>
        /// On clicking 'Isomorphism' button
        /// </summary>
        void Isomorphism()
        {
            DateTime dt = DateTime.Now;
            int h1 = dt.Hour;
            int m1 = dt.Minute;
            int s1 = dt.Second;
            int ms1 = dt.Millisecond;
            double time = 0;
            _ism = new Isomorphism(_network2, _query2);
            _ism.generateMapping();
            dt = DateTime.Now;
            time = (dt.Hour - h1) * 3600 +
                (dt.Minute - m1) * 60 +
                (dt.Second - s1) +
              ((double)(dt.Millisecond - ms1) / 1000);

            //((double)(dt.Millisecond - ms1) / 1000));
            //((double)(dt.Millisecond-ms1)/1000);

            MessageBox.Show(_ism.getNumberMapping().ToString());
            MessageBox.Show(time.ToString());

            Mapping temp = new Mapping();
            string s = string.Empty;

            //For displaying mappings

            //for (int i = 0; i < ism.getNumberMapping(); i++)
            //{
            //    temp = ism.getSinlgeMapping(i);
            //    for (int k = 0; k < temp.get_mapping().Count; k++)
            //    {
            //        s += temp.get_map(k).get_domain().ToString() + " -> " + temp.get_map(k).get_range().ToString() + " , ";
            //    }
            //    s += "\n";
            //}

            //richTextBox1.Text = s;

        }


        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            int number = 0;
            //DateTime dt = DateTime.Now;
            //int h1 = dt.Hour;
            //int m1 = dt.Minute;
            //int s1 = dt.Second;
            //int ms1 = dt.Millisecond;
            //double time = 0;
            number = _ism.testingIf(0, 3);
            //dt = DateTime.Now;
            //time = (dt.Hour - h1) * 3600 +
            //    (dt.Minute - m1) * 60 +
            //    (dt.Second - s1) +
            //  ((double)(dt.Millisecond - ms1) / 1000);

            MessageBox.Show(number.ToString());
            //MessageBox.Show("Time = " + time.ToString());
        }



    }
}