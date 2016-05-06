using MODA.Impl;
using MODA.Impl.Isomorphism;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MODA.UI
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Set after processing 'Load G' button click on top; meaning G is the network
        /// </summary>
        UndirectedGraph<int, Edge<int>> _networkGraph;
        /// <summary>
        /// Set after processing 'Load H' button click on top; meaning H is the query graph - as in, the sugraph we're searcing for
        /// </summary>
        UndirectedGraph<int, Edge<int>> _queryGraph;

        OpenFileDialog _openFileDialog1;
        public Form1()
        {
            InitializeComponent();
            _networkGraph = new UndirectedGraph<int, Edge<int>>();
            _queryGraph = new UndirectedGraph<int, Edge<int>>();

            _openFileDialog1 = new OpenFileDialog();

            _openFileDialog1.InitialDirectory = @"C:\SOMA\Drive\MyUW\Research\Kim\MODA\MODAForm"; // @"C:\SOMA\Drive\MyUW\Research\Kim\remodaalgorithmimplementation"; // Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            _openFileDialog1.FilterIndex = 1;

            _networkGraph.Cleared += OnNetworkGraphCleared;
            _queryGraph.Cleared += OnQueryGraphCleared;
        }

        private void OnQueryGraphCleared(object sender, EventArgs e)
        {
        }

        private void OnNetworkGraphCleared(object sender, EventArgs e)
        {
            DirectedLBL.Text = "";
            edgeLBL.Text = "";
            DensityLBL.Text = "";
            nodeLBL.Text = "";
        }

        /// <summary>
        /// From the bottom 'Load G' button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearG_Click(object sender, EventArgs e)
        {
            _networkGraph.Clear();
            MessageBox.Show($"Network (G) reset completed", MessageBoxTitle);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            progressBar1.Visible = false;
            comboBox1.SelectedIndex = 0;
            button2.Visible = false;
            panel2.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {

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

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void btnClearH_Click(object sender, EventArgs e)
        {
            _queryGraph.Clear();
            MessageBox.Show($"Query Graph (H) reset completed", MessageBoxTitle);
        }

        private void btnFindMappings_Click(object sender, EventArgs e)
        {
            Isomorphism<int, UndirectedGraph<int, Edge<int>>> iso = null;
            if (rbUseSymmetricBreaking.Checked)
            {
                iso = SymmetryIsomorphism<int, UndirectedGraph<int, Edge<int>>>.getInstance();
            }
            else
            {
                iso = MappingIsomorphism<int, UndirectedGraph<int, Edge<int>>>.getInstance();
            }
            var mappings = iso.findIsomorphism(_networkGraph, _queryGraph);
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

        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Called by the 'Load G' button (event) on top in the form
        /// </summary>
        private void LoadNetworkGraph()
        {
            var filename = ReadFile();
            if (string.IsNullOrWhiteSpace(filename))
            {
                MessageBox.Show($"File not loaded", MessageBoxTitle);
                return;
            }
            _networkGraph.Clear();
            string gist = GraphProcessor.LoadGraph(filename, _networkGraph);
            nodeLBL.Text = _networkGraph.VertexCount.ToString();
            edgeLBL.Text = _networkGraph.EdgeCount.ToString();
            DensityLBL.Text = _networkGraph.GetAverageDegree().ToString();
            DirectedLBL.Text = _networkGraph.IsDirected ? "Directed" : "Undirected";
            MessageBox.Show($"Network Graph (G) loaded. {gist}", "MODA", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string ReadFile()
        {
            if (_openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Insert code to read the stream here.
                return _openFileDialog1.FileName;
            }
            return null;
        }

        private const string MessageBoxTitle = "MODA - Motif Finder";

        /// <summary>
        /// Called by the 'Load H' button (event) on top in the form
        /// </summary>
        private void LoadQueryGraph()
        {
            var filename = ReadFile();
            if (string.IsNullOrWhiteSpace(filename))
            {
                MessageBox.Show($"File not loaded", MessageBoxTitle);
                return;
            }
            _queryGraph.Clear();
            string gist = GraphProcessor.LoadGraph(filename, _queryGraph);
            MessageBox.Show($"Query Graph (H) loaded. {gist}", MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        //The 'Load G' button on top
        private void btnLoadG_Top_Click(object sender, EventArgs e)
        {
            LoadNetworkGraph();
        }

        //The 'Load H' button on top
        private void btnLoadH_Top_Click(object sender, EventArgs e)
        {
            LoadQueryGraph();
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
        }


        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {

        }



    }
}
