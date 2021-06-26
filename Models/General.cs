using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemittanceApp.Models
{
    public class General
    {
        public class Service
        {
            public int country_id { get; set; }
            public string country_name { get; set; }
            public int currency_id { get; set; }
            public string currency_code { get; set; }
            public bool cash_pickup { get; set; }
            public bool bank_transfer { get; set; }
          
        }

        public class TransPurpose
        {
            public int purpose_id { get; set; }
            public string purpose_desc { get; set; }
        }

        public class TransSource
        {
            public int source_id { get; set; }
            public string source_desc { get; set; }
        }

        public class RelationShip
        {
            public int Relation_id { get; set; }
            public string Relation_desc { get; set; }
        }
    }
}
