using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
using Predict.Model;

namespace Predict
{
    public partial class Join : Page
    {
        public Join()
        {
            InitializeComponent();
        }

        private void Back_btn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
        }

        private void Join_btn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Name_tb.Text) || string.IsNullOrEmpty(Id_tb.Text) || string.IsNullOrEmpty(Pw_tb.Password))
            {
                MessageBox.Show("모든 정보에 입력해주세요.", "Error");
            }
            else
            {
                int result;
                string EnterName = Name_tb.Text;
                string EnterID = Id_tb.Text;
                string EnterPW = Pw_tb.Password;
                result = Login.con.JoinUser(EnterName, EnterID, EnterPW);
                MessageBox.Show(result.ToString());
                if (result == 10)
                {
                    NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                }
            }
        }
    }
}
