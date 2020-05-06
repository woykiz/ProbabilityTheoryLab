using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace LaboratornayaRabota
{
    public partial class Form1 : Form
    {
        Random random { get; set; }
        List<double> expirements { get; set; } = new List<double>();
        double function(double x, double d)
        {
            return Math.Sqrt(-2 * d * d * Math.Log(1 - x));
        }

        double p(double y, double d)
        {
            return y / (d * d) * Math.Exp(-y * y / (2 * d * d));
        }

        double D => Convert.ToDouble(sTextBox.Text.Replace('.', ','));

        public Form1()
        {
            InitializeComponent();

            dataGridView1.ColumnCount = 1;
            dataGridView1.Columns[0].Name = "X";

            button1.Enabled = false;
            chart1.Series.Clear();

        }

        void DrawChart()
        {
            chart1.Series.Clear();
            var ser = new Series();
            ser.ChartType = SeriesChartType.Line;

            ser.Name = "Плотность";

            for(double x = 0;  x < 10; x += 0.01)
            {
                ser.Points.AddXY(x, p(x, D));
            }

            var bar = new Series();
            bar.ChartType = SeriesChartType.Line;

            bar.Name = "Эксперименты";


            chart1.Series.Add(ser);
            chart1.Series.Add(bar);
        }

        async private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;

            await Task.Run(new Action(() =>
            {
                var count = Convert.ToInt32(rTextBox.Text);

                for (int i = 0; i < count; ++i)
                {
                    var d = D;
                    var number = random.NextDouble();
                    var X = function(number, d);

                    expirements.Add(X);
                }
                expirements = expirements.OrderBy(x => x).ToList();

            }));

            
            dataGridView1.Rows.Clear();
            foreach (var n in expirements)
                dataGridView1.Rows.Add(n + "");

            BuildData();

            this.button1.Enabled = true;
            this.button2.Enabled = true;
            this.button3.Enabled = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            expirements.Clear();
            dataGridView1.Rows.Clear();

            random = null;
            button1.Enabled = false;
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                random = new Random(Convert.ToInt32(seedTextBox.Text));
            }
            else
            {
                random = new Random();
            }

            DrawChart();
            button1.Enabled = true;
            button3.Enabled = false;
        }

        void BuildData()
        {
            chart1.Series[1].Points.Clear();
            int k = 0;
            List<double> points = new List<double>();
            for (double x = 0; x < 10; x += 0.01)
            {
                foreach(var n in expirements)
                {
                    if (Math.Abs(n - x) <= 0.8)
                    {
                        ++k;
                    }
                }

                points.Add(k);
                k = 0;
            }

            var max = points.Max();
            int i = 0;
            for (double x = 0; x < 10; x += 0.01, ++i)
            {
                chart1.Series[1].Points.AddXY(x, points[i] / max);
            }
        }
    }
}
