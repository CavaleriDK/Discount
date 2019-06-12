using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discount.Models;

namespace Discount
{
    /// <summary>
    /// Controller handling all purchase logic
    /// </summary>
    class PurchaseController
    {
        /// <summary>
        /// Singleton instance of this class
        /// </summary>
        public static PurchaseController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PurchaseController();
                }
                return instance;
            }
        }

        private static PurchaseController instance;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private PurchaseController()
        {

        }

        /// <summary>
        /// Method used to purchase an offer
        /// </summary>
        /// <param name="offer">Offer that was purchased</param>
        public void AddPurchaseToUser(OfferModel offer)
        {
            int offerId = offer.ID;
            int shopId = offer.ShopID;
            int userId = User.Instance.ID;
            int price = offer.PriceOffer;
            int savedAmount = (offer.PriceBefore > 0) ? offer.PriceBefore - offer.PriceOffer : (int)Math.Floor((offer.PriceOffer * 0.2f));
            int savedPercentage = (int)(((float)savedAmount / (float)offer.PriceOffer) * 100f);
            DateTime purchaseTime = DateTime.Now;

            // Should add way to calculate better. Now it simply gives the saved percentage in xp 
            int experienceEarned = savedPercentage;

            // Save purchase in database
            PurchaseModel.New(price, purchaseTime, shopId, offerId, userId);

            // Add experience to user
            User.Instance.AddNewExperience(experienceEarned);

            // Call the main thread and invoke method to hide loading overlay
            MainWindow.StaticDispatcher.Invoke(() =>
            {
                MainWindow.HideOverlay();

                // Animate the experience increase bar?
            });
        }
    }
}
