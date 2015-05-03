
using System;
using System.Windows.Controls;

namespace Shiro.Class
{
    class Appointment
    {
        public int ID_CUSTOMER { get; set; }
        public int ID_APPOINTMENT { get; set; }
        public int ID_SALESMAN { get; set; }
        public string DAY { get; set; }
        public string STARTTIME { get; set; }
        public string ENDTIME { get; set; }
        public Border Border { get; set; }

        public Appointment(int ID, int ID_CUSTOMER, int ID_SALESMAN, string DAY, string STARTTIME, string ENDTIME)
        {
            ID_APPOINTMENT = ID;
            this.ID_CUSTOMER = ID_CUSTOMER;
            this.ID_SALESMAN = ID_SALESMAN;
            this.DAY = DAY;
            this.STARTTIME = STARTTIME;
            this.ENDTIME = ENDTIME;
        }

        public Appointment()
        {
            ID_APPOINTMENT = 0;
            ID_CUSTOMER = 0;
            ID_SALESMAN = 0;
            DAY = String.Empty;
            STARTTIME = String.Empty;
            ENDTIME = String.Empty;
        }
    }
}
