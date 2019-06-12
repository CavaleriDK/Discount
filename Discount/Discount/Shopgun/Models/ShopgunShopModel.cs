using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Discount.Shopgun
{
    /// <summary>
    /// The JSON model for a single dealer from the Shopgun dealers endpoint
    /// </summary>
    [DataContract] class ShopgunShopModel
    {
        /// <summary>
        /// Shopgun Dealer ID attribute from JSON object
        /// </summary>
        public string ID { get => id; }

        /// <summary>
        /// Name attribute from JSON object
        /// </summary>
        public string Name { get => name.Replace("'", @"\u0027"); }

        /// <summary>
        /// Logo Image URL attribute from JSON object
        /// </summary>
        public string Logo { get => logo; }

        [DataMember] private string id;
        [DataMember] private string name;
        [DataMember] private string logo;

    }
}
