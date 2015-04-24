using System.Windows.Controls;

namespace Shiro.Class
{
    class CUSTOMER
    {
        public int ID_CUSTOMER { get; set; }
        public string TELEPHONE { get; set; }
        public string NAME { get; set; }
        public string FIRSTNAME { get; set; }
        public string MAIL { get; set; }
        public string COMPANY { get; set; }
        public Border Border { get; set; }

        public CUSTOMER(int ID, string TELEPHONE, string NAME, string FIRSTNAME, string MAIL, string COMPANY)
        {
            ID_CUSTOMER = ID;
            this.TELEPHONE = TELEPHONE;
            this.NAME = NAME;
            this.FIRSTNAME = FIRSTNAME;
            this.MAIL = MAIL;
            this.COMPANY = COMPANY;
        }
    }
}
