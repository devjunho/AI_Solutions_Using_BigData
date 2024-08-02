using Predict.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public partial class InputData : Page
    {
        private List<int> InforAddition = new List<int>();
        public InputData()
        {
            InitializeComponent();
            Sex_cb.Items.Add("남성");
            Sex_cb.Items.Add("여성");
            Smoke_cb.Items.Add("비흡연자");
            Smoke_cb.Items.Add("흡연자");
            Alchol_cb.Items.Add("알코올을 섭취하지 않음");
            Alchol_cb.Items.Add("알코올을 소비함");
            Activity_cb.Items.Add("물리적으로 활동하지 않음");
            Activity_cb.Items.Add("물리적으로 활동적");
        }

        private void Back_btn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Menu.xaml", UriKind.Relative));
        }

        private void Predict_btn_Click(object sender, RoutedEventArgs e)
        {
            int result;
            InforAddition.Clear();

            // Age
            InforAddition.Add(int.Parse(Age_tb.Text));

            // Gender
            if (Sex_cb.SelectedItem.ToString() == "남성")
            {
                InforAddition.Add(2);
            }
            else
            {
                InforAddition.Add(1);
            }

            // Ap_hi
            InforAddition.Add(int.Parse(Max_tb.Text));

            // Ap_lo
            InforAddition.Add(int.Parse(Min_tb.Text));

            // Smoke
            if (Smoke_cb.SelectedItem.ToString() == "비흡연자")
            {
                InforAddition.Add(0);
            }
            else
            {
                InforAddition.Add(1);
            }

            // Alco
            if (Alchol_cb.SelectedItem.ToString() == "알코올을 섭취하지 않음")
            {
                InforAddition.Add(0);
            }
            else
            {
                InforAddition.Add(1);
            }

            // Active
            if (Alchol_cb.SelectedItem.ToString() == "물리적으로 활동하지 않음")
            {
                InforAddition.Add(0);
            }
            else
            {
                InforAddition.Add(1);
            }

            // Bmi
            int Cal_Bmi = (int)(float.Parse(Weight_tb.Text) / (float.Parse(Height_tb.Text) * float.Parse(Height_tb.Text)));
            InforAddition.Add(Cal_Bmi);

            // Bp
            // InforAddition[2] = 수축기
            // InforAddition[3] = 이완기
            if (InforAddition[2] < 120 && InforAddition[3] < 80)
            {
                InforAddition.Add(0);
            }
            else if (InforAddition[2] >= 180 || InforAddition[3] >= 120)
            {
                InforAddition.Add(3);
            }
            else if ((InforAddition[2] < 140  && InforAddition[2] >= 120) || (InforAddition[3] < 90 && InforAddition[3] >= 80))
            {
                InforAddition.Add(1);
            }
            else
            {
                InforAddition.Add(2);
            }

            // 확인용
            //MessageBox.Show(InforAddition.Count.ToString());
            //for (int i = 0; i < InforAddition.Count; i++)
            //{
            //    MessageBox.Show(InforAddition[i].ToString(), "뭐냐");
            //}

            result = Login.con.SendInputData(InforAddition);
            //MessageBox.Show(result.ToString(), "뭘까");
            //MessageBox.Show(Login.data.Cardio.ToString(), "뭘까");
            NavigationService.Navigate(new Uri("/Result.xaml", UriKind.Relative));
        }
    }
}

