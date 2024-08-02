using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predict.Model
{
    public class Data
    {
        public int Type { get; set; }
        public int Cardio { get; set; }
        public int NO { get; set; }
        public string ID { get; set; }
        public string PW { get; set; }
        public string NAME { get; set; }
    }

    public class InformationData
    {
        public int Type { get; set; }
        public int Age { get; set; }
        public int Gender { get; set; }
        public int Ap_hi { get; set; }
        public int Ap_lo { get; set; }
        public int Smoke { get; set; }
        public int Alco {  get; set; }
        public int Active { get; set; }
        public int Bmi { get; set; }
        public int Bp { get; set; }
        public string ID { get; set; }
    }

    public class ChartData
    {
        public int Type { get; set; }
        public List<int> SexList { get; set; }
        public List<int> RiskList { get; set; }
    }

    enum TYPE
    {
        // 0번
        CONNECT_FAIL = 0,

        // 10번
        SUCCEED = 10,

        // 20번
        FAIL = 20,

        // 21번
        DUPLICATE = 21,

        // 30번
        EMPTY = 30,

        // 40번
        LOGIN = 40,

        // 50번
        JOIN = 50,

        // 60번
        SHOWGRAPH = 60,

        // 70번
        SENDINPUTDATA = 70
    }
}
