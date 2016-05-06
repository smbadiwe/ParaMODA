using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Motif1
{
    public partial class pallete_frm : Form
    {
        List<List<int>> mappings_domain;
        List<List<int>> mappings_range;
        int node_number;
        List<bool> adj_matrix;
        Label[] lbl;
        Label active_label;
        public pallete_frm(List<bool> Adj_matrix,int Node_number, List<List<int>> Mappings_domain,List<List<int>> Mappings_range,string address,int picture_size)
        {
            InitializeComponent();

            label15.Text = "(" + Mappings_domain.Count.ToString() + ")";
            
            mappings_domain=new List<List<int>>();
            mappings_range=new List<List<int>>();
            List<int> temp_domain=new List<int>();
            List<int> temp_range=new List<int>();
            node_number = Node_number;
            adj_matrix = new List<bool>();

            int i = 0;
            for (i = 0; i < Adj_matrix.Count; i++)
                adj_matrix.Add(Adj_matrix[i]);

            for (i = 0; i < Mappings_domain.Count; i++)
            {
                temp_domain = new List<int>();
                temp_range = new List<int>();
                for (int k = 0; k < Mappings_domain[i].Count; k++)
                {
                    temp_domain.Add(Mappings_domain[i][k]);
                    temp_range.Add(Mappings_range[i][k]);
                }
                mappings_domain.Add(temp_domain);
                mappings_range.Add(temp_range);
                
            }
            pictureBox3.Size = new Size(picture_size, picture_size);

            pictureBox3.Image = Image.FromFile(address);
//            load();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Bitmap bit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics grfx = Graphics.FromImage(bit);
            draw_both visual = new draw_both(grfx, bit.Width, node_number, adj_matrix, node_number,mappings_domain[0],mappings_range[0]);
            visual.draw(1, Color.Black, Color.Black, Color.OrangeRed, Color.OrangeRed);
            bit.Save("c:\\network_pallete.jpg");
            pictureBox1.Image = Image.FromFile("c:\\network_pallete.jpg");

            
        }
        void load()
        {
            //fill_panel();
            //return;
            int i = 0;
            int j = 0;
            string s = string.Empty;
            richTextBox1.Clear();
            for (i = 0; i <mappings_domain.Count; i++)
            {
                for (j = 0; j < mappings_domain[i].Count; j++)
                {
                    s += mappings_domain[i][j].ToString() + " --> " +
                        mappings_range[i][j].ToString() + " , ";
                }
                s += "\n";
            }
            richTextBox1.Text = s;
                
        }
        void fill_panel()
        {
            int i = 0;
            int j = 0;
            string s = string.Empty;
            lbl = new Label[mappings_domain.Count];
            int y=5;
            for (i = 0; i < mappings_domain.Count; i++)
            {
                lbl[i] = new Label();
                lbl[i].Location = new Point(10, y);
                lbl[i].Parent = panel2;
                lbl[i].AutoSize = true;
                lbl[i].MouseClick += new MouseEventHandler(pallete_frm_MouseClick);
                lbl[i].Tag = i;
                s = string.Empty;
                for (j = 0; j < mappings_domain[i].Count; j++)
                {
                    
                    s += mappings_domain[i][j].ToString() + " --> " +
                        mappings_range[i][j].ToString() + " , ";
                   
                }
                lbl[i].Text = s;
                y += 16;
            }
            

        }
        
        void pallete_frm_MouseClick(object sender, MouseEventArgs e)
        {
            if (active_label != null)
                active_label.ForeColor = Color.Black;
            Label label = (Label)sender;
            label.ForeColor = Color.OrangeRed;
            active_label = new Label();
            active_label = (Label)sender;
            

            try
            {
                pictureBox1.Image.Dispose();
            }
            catch (Exception e1)
            {
            }
            Bitmap bit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics grfx = Graphics.FromImage(bit);
            int index = int.Parse(textBox1.Text);
            draw_both visual = new draw_both(grfx, bit.Width, node_number, adj_matrix, node_number, mappings_domain[(int)label.Tag], mappings_range[(int)label.Tag]);
            visual.draw(2, Color.Black, Color.Black, Color.OrangeRed, Color.OrangeRed);
            bit.Save("c:\\network_pallete.jpg");
            pictureBox1.Image = Image.FromFile("c:\\network_pallete.jpg");
            
        }
        

        private void pallete_frm_Load(object sender, EventArgs e)
        {
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.Image.Dispose();
            }
            catch (Exception e1)
            {
            }
            Bitmap bit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics grfx = Graphics.FromImage(bit);
            int index=int.Parse(textBox1.Text);
            draw_both visual = new draw_both(grfx, bit.Width, node_number, adj_matrix, node_number, mappings_domain[index], mappings_range[index]);
            visual.draw(2, Color.Black, Color.Black, Color.OrangeRed, Color.OrangeRed);
            bit.Save("c:\\network_pallete.jpg");
            pictureBox1.Image = Image.FromFile("c:\\network_pallete.jpg");

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                timer1.Enabled = true;
                timer1.Interval = int.Parse(textBox2.Text);
            }
            else
                timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.Image.Dispose();
            }
            catch (Exception e1)
            {
            }
            Bitmap bit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics grfx = Graphics.FromImage(bit);
            int index = int.Parse(textBox1.Text);
            draw_both visual = new draw_both(grfx, bit.Width, node_number, adj_matrix, node_number, mappings_domain[index], mappings_range[index]);
            visual.draw(1, Color.Black, Color.Black, Color.OrangeRed, Color.OrangeRed);
            bit.Save("c:\\network_pallete.jpg");
            pictureBox1.Image = Image.FromFile("c:\\network_pallete.jpg");
            if (index < mappings_domain.Count - 1)
            {
                index++;
                textBox1.Text = index.ToString();
                label8.Text = index.ToString();
            }
            else
            {
                index = 0;
                textBox1.Text = index.ToString();
                label8.Text = index.ToString();
            }
            
        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {

        }
    }
}