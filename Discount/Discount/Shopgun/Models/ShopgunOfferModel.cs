using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Discount.Shopgun
{
    /// <summary>
    /// The JSON model for a single Offer from the Shopgun offers endpoint
    /// </summary>
    [DataContract] class ShopgunOfferModel
    {
        /// <summary>
        /// Shopgun Offer ID attribute from JSON object
        /// </summary>
        public string ID { get => id; }

        /// <summary>
        /// Heading attribute from the JSON object
        /// </summary>
        public string Heading { get => heading.Replace("'", @"\u0027"); }

        /// <summary>
        /// Pricing attribute from the JSON object
        /// </summary>
        public ShopgunPriceModel Pricing { get => pricing; }

        /// <summary>
        /// Images attribute from the JSON object
        /// </summary>
        public ShopgunImagesModel Images { get => images; }

        /// <summary>
        /// Offer valid from attribute from the JSON object
        /// </summary>
        public string RunFrom { get => run_from; }

        /// <summary>
        /// Offer valid until attribute from the JSON object
        /// </summary>
        public string RunTo { get => run_till; }

        [DataMember] private string id;
        [DataMember] private string heading;
        [DataMember] private string description;
        [DataMember] private ShopgunPriceModel pricing;
        [DataMember] private ShopgunImagesModel images;
        [DataMember] private string run_from;
        [DataMember] private string run_till;
    }
}
