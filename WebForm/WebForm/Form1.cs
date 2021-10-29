using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using WebForm.Entities;
using WebForm.MNBServiceReference;

namespace WebForm
{
    
    public partial class Form1 : Form
    {
        BindingList<RateData> Rates = new BindingList<RateData>();
        BindingList<string> currencies = new BindingList<string>();
        public Form1()
        {
            InitializeComponent();
            comboBox1.DataSource = currencies;
            MNBArfolyamServiceSoapClient mnbService = new MNBArfolyamServiceSoapClient();
            GetCurrenciesRequestBody request = new GetCurrenciesRequestBody();
            
            var response = mnbService.GetCurrencies(request);
            string result = response.GetCurrenciesResult;
            XmlDocument vxml = new XmlDocument();
            vxml.LoadXml(result);
            foreach (XmlElement item in vxml.DocumentElement.FirstChild.ChildNodes)
            {
                currencies.Add(item.InnerText);

            }


            RefreshData();
        }

        private void RefreshData()
        {
            if (comboBox1.SelectedItem == null) return;
            Rates.Clear();
            string xmlstring = Consume();
            LoadXml(xmlstring);
            dataGridView1.DataSource = Rates;
            Charting();
        }

        private void Charting()
        {
            chartRateData.DataSource = Rates;

            var series = chartRateData.Series[0];
            series.ChartType = SeriesChartType.Line;
            series.XValueMember = "Date";
            series.YValueMembers = "Value";
            series.BorderWidth = 2;

            var legend = chartRateData.Legends[0];
            legend.Enabled = false;

            var chartArea = chartRateData.ChartAreas[0];
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisY.IsStartedFromZero = false;

        }

        string Consume()
        {
            
            MNBArfolyamServiceSoapClient mnbService = new MNBArfolyamServiceSoapClient();
            GetExchangeRatesRequestBody request = new GetExchangeRatesRequestBody();
            request.currencyNames = comboBox1.SelectedItem.ToString();
            request.startDate = dateTimePickerStart.ToString();
            request.endDate = dateTimePickerEnd.ToString();
            var response = mnbService.GetExchangeRates(request);
            string result = response.GetExchangeRatesResult;
            File.WriteAllText("export.xml", result);
            return result;
        }
        
        private void LoadXml(string input)
        {
            XmlDocument xml = new XmlDocument();
           
            xml.LoadXml(input);

            foreach (XmlElement element in xml.DocumentElement)
            {
                 var rate = new RateData();
                Rates.Add(rate);
                rate.Date = DateTime.Parse(element.GetAttribute("date"));

                var childElement = (XmlElement)element.ChildNodes[0];
                rate.Currency = childElement.GetAttribute("curr");

                var unit = decimal.Parse(childElement.GetAttribute("unit"));
                var value = decimal.Parse(childElement.InnerText);
                if (unit != 0)
                    rate.Value = value / unit;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void dateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void dateTimePickerEnd_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
