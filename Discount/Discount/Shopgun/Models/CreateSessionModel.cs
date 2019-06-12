using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Discount.Shopgun
{
    /// <summary>
    /// The JSON model for a single Session from the Shopgun sessions endpoint
    /// </summary>
    [DataContract] class CreateSessionModel
    {
        /// <summary>
        /// The token attribute from the JSON object
        /// </summary>
        public string Token { get => token; set => token = value; }


        [DataMember] private string token;
        //    //[DataMember]
        //    //private string reference;
        //    //[DataMember]
        //    //private string expires;
        //    //[DataMember]
        //    //private string user;
        //    //[DataMember]
        //    //private string provider;
        //    //[DataMember]
        //    //private string client_id;
        //    //[DataMember]
        //    //private bool is_admin;
    }
}
