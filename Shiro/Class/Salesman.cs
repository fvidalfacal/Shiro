
using System.Windows.Controls;

namespace Shiro.Class
{
    class SalesMan
    {
        public int ID_SALESMAN { get; set; }
        public string FIRSTNAME { get; set; }
        public string NAME { get; set; }
        public string MAIL { get; set; }
        public string TELEPHONE { get; set; }
        public Border Border { get; set; }

        public SalesMan(int ID, string TELEPHONE, string NAME, string FIRSTNAME, string MAIL)
        {
            ID_SALESMAN = ID;
            this.TELEPHONE = TELEPHONE;
            this.NAME = NAME;
            this.FIRSTNAME = FIRSTNAME;
            this.MAIL = MAIL;
        }

        
    }
}
