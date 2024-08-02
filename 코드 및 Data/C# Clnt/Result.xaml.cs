using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Predict
{
    /// <summary>
    /// Result.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Result : Page
    {
        public Result()
        {
            InitializeComponent();
            string Result;
            Result_tb.Text = "";
            if(Login.data.Cardio <= 30)
            {
                Result = "저위험";
            }
            else if (Login.data.Cardio > 30 && Login.data.Cardio <=70)
            {
                Result = "중위험";
            }
            else
            {
                Result = "고위험";
            }
            Result_tb.Text = "회원님의 예측 결과: " + Login.data.Cardio.ToString() + "% / " + Result;
        }

        private void Back_btn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Menu.xaml", UriKind.Relative));
        }
    }
}
