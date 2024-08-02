using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Predict.Model;
using LiveCharts.Defaults;
using LiveCharts;
using Newtonsoft.Json.Linq;

namespace Predict
{

    public partial class Chart : Page
    {
        public ChartValues<ObservableValue> Values { get; set; }

        public List<string> XLabel { get; set; }

        public static ChartData chartdata = new ChartData();

        List <string> StringCardio { get; set; }

        int a = 0;
        int b = 0;
        int c = 0;
        int d = 0;
        int f = 0;
        int g = 0;

        public Chart()
        {
            InitializeComponent();

            int result;

            result = Login.con.GrapRead();
            //MessageBox.Show(result.ToString(), "result 값"); // 확인용
            StringCardio = new List<string> { };
            for (int i = 0; i < chartdata.SexList.Count; i++)
            {
                //MessageBox.Show(chartdata.SexList[i].ToString(), "Sex");    // 확인용
                //MessageBox.Show(chartdata.RiskList[i].ToString(), "Sex");   // 확인용
                if (chartdata.RiskList[i] <= 30)
                {
                    StringCardio.Add("저위험");
                    //MessageBox.Show(StringCardio[i], "StringCardio");   // 확인용
                }
                else if (chartdata.RiskList[i] > 30 && chartdata.RiskList[i] <= 60)
                {
                    StringCardio.Add("중위험");
                    //MessageBox.Show(StringCardio[i], "StringCardio");   // 확인용
                }
                else
                {
                    StringCardio.Add("고위험");
                    //MessageBox.Show(StringCardio[i], "StringCardio");   // 확인용
                }

                if (chartdata.SexList[i] == 2 && StringCardio[i] == "고위험")  // '남성 / 고위험'
                {
                    a++;
                }
                else if (chartdata.SexList[i] == 2 && StringCardio[i] == "중위험") // '남성 / 중위험'
                {
                    b++;
                }
                else if (chartdata.SexList[i] == 2 && StringCardio[i] == "저위험") // '남성 / 저위험'
                {
                    c++;
                }
                else if (chartdata.SexList[i] == 1 && StringCardio[i] == "고위험")
                {
                    d++;
                }
                else if (chartdata.SexList[i] == 1 && StringCardio[i] == "중위험")
                {
                    f++;
                }
                else
                {
                    g++;
                }
            }

            // 데이터 리스트 생성
            List<int> values = new List<int> { };
            values.Add(a);
            values.Add(b);
            values.Add(c);
            values.Add(d);
            values.Add(f);
            values.Add(g);

            // ChartValues 초기화
            Values = new ChartValues<ObservableValue>();

            // 데이터 리스트 반복하여 ChartValues에 추가
            foreach (int value in values)
            {
                Values.Add(new ObservableValue(value));
            }

            // X 축 레이블 설정 (List<string>으로 변경)
            XLabel = new List<string> { };
            XLabel.Add("남성 / 고위험");
            XLabel.Add("남성 / 중위험");
            XLabel.Add("남성 / 저위험");
            XLabel.Add("여성 / 고위험");
            XLabel.Add("여성 / 중위험");
            XLabel.Add("여성 / 저위험");

            DataContext = this;
        }

        private void Back_btn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Menu.xaml", UriKind.Relative));
        }
    }
}
