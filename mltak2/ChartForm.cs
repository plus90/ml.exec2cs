using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mltak2
{
    using DataList = List<List<float>>;
    using System.Windows.Forms.DataVisualization.Charting;
    public partial class ChartForm : Form
    {
        public ChartForm(DataList dataList, List<string> labels)
        {
            if (dataList.Count != labels.Count)
                throw new ArgumentException("The data-list# does not watch with labels#");
            InitializeComponent();
            this.__plot_data(dataList, labels);
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler((sender, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape: this.Close(); break;
                }
            });
        }

        private void __plot_data(DataList dataList, List<string> labels)
        {
            this.chart.Series.Clear();
            var max_len = 0;
            var data_len_counter = new List<List<int>>();
            foreach (var data in dataList)
            {
                if (data.Count > max_len)
                    max_len = data.Count;

            }
            foreach (var data in dataList)
            {
                if (data.Count <= 0) continue;
                if (data.Count < max_len)
                {
                    var f = new float[max_len - data.Count];
                    for (int i = 0; i < f.Length; i++) { f[i] = data[data.Count - 1]; }
                    data.AddRange(f);
                }
                data_len_counter.Add(new List<int>());
                for (int i = 0; i < data.Count; i++)
                {
                    data_len_counter[data_len_counter.Count - 1].Add(i);
                }
            }
            this.chart.Titles.Add("Utiliy Trace Show");
            ChartArea chartArea = null;
            foreach (var l in labels)
            {
                 chartArea = this.chart.ChartAreas.Add(l);
            }
            chartArea.AxisX.Title = "Episode";
            chartArea.AxisY.Title = "U-Value";
            for (int i = 0; i < dataList.Count; i++)
            {
                Series series = this.chart.Series.Add(labels[i]);
                series.ChartArea = labels[i];
                series.ChartType = SeriesChartType.Line;
                series.Points.DataBindXY(data_len_counter[i], dataList[i]);
                Application.DoEvents();
                chartArea.Position = new ElementPosition(10, 5, 70, 50);
            }
        }
    }
}