using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.EnterpriseTask
{
    public class Enterprise
    {
        private readonly Guid guid;
        public Guid Guid => guid;

        public Enterprise(Guid guid, DateTime establishDate)
        {
            this.guid = guid;
            EstablishDate = establishDate;
        }

        public string Name { get; set; }

        private string inn;
        public string Inn 
        {
            get => inn;
            set
            {
                if (value.Length != 10 || !value.All(char.IsDigit))
                    throw new ArgumentException();
                inn = value;
            }
        }
        
        public DateTime EstablishDate { get; set; }

        public TimeSpan ActiveTimeSpan => DateTime.Now - EstablishDate;

        public double GetTotalTransactionsAmount()
        {
            DataBase.OpenConnection();
            return DataBase.Transactions()
                .Where(z => z.EnterpriseGuid == guid)
                .Sum(t => t.Amount);
        }
    }
}