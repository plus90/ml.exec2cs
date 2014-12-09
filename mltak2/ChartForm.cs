using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace mltak2
{
    using System.Windows.Forms.DataVisualization.Charting;
    using DataList = List<List<float>>;
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
                    case Keys.Escape: 
#if __DEBUG_PLOT__
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
#else
                        this.Close();
#endif
                        break;
                }
            });
        }
        /// <summary>
        /// Plots data and labels them
        /// </summary>
        /// <param name="dataList">The data list to plot</param>
        /// <param name="labels">The label of each data in data-list</param>
        private void __plot_data(DataList dataList, List<string> labels)
        {
            this.chart.Series.Clear();
            for (int i = 0; i < dataList.Count; i++) { if (dataList[i] == null) dataList[i] = new List<float>() { 0 }; }
            var max_len = 0;
            var data_len_counter = new List<List<int>>();
            foreach (var data in dataList)
            {
                if (data.Count > max_len)
                    max_len = data.Count;

            }
            foreach (var data in dataList)
            {
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
            chart.ChartAreas.Clear();
            chart.Series.Clear();
            ChartArea chartarea = new ChartArea();
            chart.ChartAreas.Add(chartarea);
            var c = 0;
            foreach (var data  in dataList)
            {
                var ser = chart.Series.Add(labels[c]);
                ser.ChartType = SeriesChartType.Line;
                ser.ChartArea = chartarea.Name;
                ser.Points.DataBindXY(data_len_counter[c++], data);
            }
        }
    }
}