using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class RecurrencePattern : BaseData
    {
        public RecurrencePattern(int Id, DateTime DTSTART, DateTime UNTILL, string FREQ, string COUNT, string INTERVAL, string BYDAY, string BYWEEK, string BYMONTH, string BYYEAR, string BYMONTHDAY)
        {
            this.Id = Id;
            this.DTSTART = DTSTART;
            this.UNTILL = UNTILL;
            this.FREQ = FREQ;
            this.COUNT = COUNT;
            this.INTERVAL = INTERVAL;
            this.BYDAY = BYDAY;
            this.BYWEEK = BYWEEK;
            this.BYMONTH = BYMONTH;
            this.BYYEAR = BYYEAR;
            this.BYMONTHDAY = BYMONTHDAY;
        }
        public RecurrencePattern(DateTime DTSTART, DateTime UNTILL, string FREQ, string COUNT, string INTERVAL, string BYDAY, string BYWEEK, string BYMONTH, string BYYEAR, string BYMONTHDAY)
        {
            this.DTSTART = DTSTART;
            this.UNTILL = UNTILL;
            this.FREQ = FREQ;
            this.COUNT = COUNT;
            this.INTERVAL = INTERVAL;
            this.BYDAY = BYDAY;
            this.BYWEEK = BYWEEK;
            this.BYMONTH = BYMONTH;
            this.BYYEAR = BYYEAR;
            this.BYMONTHDAY = BYMONTHDAY;
        }
        public RecurrencePattern(SqlDataReader sqlDataReader)
        {
            DTSTART = Convert.ToDateTime(sqlDataReader["DTSTART"]);
            UNTILL = Convert.ToDateTime(sqlDataReader["UNTILL"]);
            FREQ = sqlDataReader["FREQ"].ToString();
            COUNT = sqlDataReader["COUNT"].ToString();
            INTERVAL = sqlDataReader["INTERVAL"].ToString();
            BYDAY = sqlDataReader["BYDAY"].ToString();
            BYWEEK = sqlDataReader["BYWEEK"].ToString();
            BYMONTH = sqlDataReader["BYMONTH"].ToString();
            BYYEAR = sqlDataReader["BYYEAR"].ToString();
            BYMONTHDAY = sqlDataReader["BYMONTHDAY"].ToString();
        }

        public int Id { get; set; }
        public DateTime DTSTART { get; set; }
        public DateTime UNTILL { get; set; }
        public string FREQ { get; set; }
        public string COUNT { get; set; }
        public string INTERVAL { get; set; }
        public string BYDAY { get; set; }
        public string BYWEEK { get; set; }
        public string BYMONTH { get; set; }
        public string BYYEAR { get; set; }
        public string BYMONTHDAY { get; set; }

        public Dictionary<string, object> generateDictionary()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Add("@DTSTART", DTSTART);
            keyValuePairs.Add("@FREQ", FREQ);
            keyValuePairs.Add("@UNTILL", UNTILL);
            keyValuePairs.Add("@COUNT", COUNT);
            keyValuePairs.Add("@INTERVAL", INTERVAL);
            keyValuePairs.Add("@BYDAY", BYDAY);
            keyValuePairs.Add("@BYWEEK", BYWEEK);
            keyValuePairs.Add("@BYMONTH", BYMONTH);
            keyValuePairs.Add("@BYYEAR", BYYEAR);
            keyValuePairs.Add("@BYMONTHDAY", BYMONTHDAY);

            return keyValuePairs;
        }
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}", DTSTART, FREQ, UNTILL, COUNT, INTERVAL, BYDAY, BYWEEK, BYMONTH, BYYEAR, BYMONTHDAY);
        }
    }
}