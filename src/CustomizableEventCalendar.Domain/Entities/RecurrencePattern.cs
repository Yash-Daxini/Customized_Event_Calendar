﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities
{
    internal class RecurrencePattern : BaseData
    {
        public RecurrencePattern() { }
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
            FREQ = sqlDataReader["FREQ"] == DBNull.Value ? null : sqlDataReader["FREQ"].ToString();
            COUNT = sqlDataReader["COUNT"] == DBNull.Value ? null : sqlDataReader["COUNT"].ToString();
            INTERVAL = sqlDataReader["INTERVAL"] == DBNull.Value ? null : sqlDataReader["INTERVAL"].ToString();
            BYDAY = sqlDataReader["BYDAY"] == DBNull.Value ? null : sqlDataReader["BYDAY"].ToString();
            BYWEEK = sqlDataReader["BYWEEK"] == DBNull.Value ? null : sqlDataReader["BYWEEK"].ToString();
            BYMONTH = sqlDataReader["BYMONTH"] == DBNull.Value ? null : sqlDataReader["BYMONTH"].ToString();
            BYYEAR = sqlDataReader["BYYEAR"] == DBNull.Value ? null : sqlDataReader["BYYEAR"].ToString();
            BYMONTHDAY = sqlDataReader["BYMONTHDAY"] == DBNull.Value ? null : sqlDataReader["BYMONTHDAY"].ToString();
        }
        [NotMapped]
        public int Id { get; set; }
        public DateTime DTSTART { get; set; }
        public DateTime UNTILL { get; set; }
        public string? FREQ { get; set; }
        public string? COUNT { get; set; }
        public string? INTERVAL { get; set; }
        public string? BYDAY { get; set; }
        public string? BYWEEK { get; set; }
        public string? BYMONTH { get; set; }
        public string? BYYEAR { get; set; }
        public string? BYMONTHDAY { get; set; }
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}", DTSTART, FREQ, UNTILL, COUNT, INTERVAL, BYDAY, BYWEEK, BYMONTH, BYYEAR, BYMONTHDAY);
        }
    }
}