using Discount.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Discount.Views;

namespace Discount.Controllers
{
    /// <summary>
    /// The Search controller handling all product search functionality
    /// </summary>
    class ProductSearchController
    {
        /// <summary>
        /// Constructor for the class
        /// </summary>
        public ProductSearchController()
        {

        }

        /// <summary>
        /// Method to search after all offer by part of a name string
        /// </summary>
        /// <param name="product">The user input defining search criteria </param>
        public void SearchForOffer(string product)
        {
            List<OfferModel> offers = OfferModel.SearchByName(product, false);            

            // Call the main thread and invoke method to hide loading overlay
            MainWindow.StaticDispatcher.Invoke(() =>
            {
                OfferPage offerPage = new OfferPage();
                offerPage.PopulateWithSpecificOffers(offers);
                SearchPage.searchWindow.NavigationService.Navigate(offerPage);

                MainWindow.HideOverlay();
            });

        }

        /// <summary>
        /// Method to search all shops for certain product by name string and shop integer ID
        /// </summary>
        /// <param name="product"></param>
        /// <param name="shop_id"></param>
        public void SearchForOfferByStore(string product, int shop_id)
        {
            List<OfferModel> offers = OfferModel.SearchByNameFromShopID(product, shop_id, false);

            // Call the main thread and invoke method to hide loading overlay
            MainWindow.StaticDispatcher.Invoke(() =>
            {
                OfferPage offerPage = new OfferPage();
                offerPage.PopulateWithSpecificOffers(offers);
                SearchPage.searchWindow.NavigationService.Navigate(offerPage);

                MainWindow.HideOverlay();
            });

        }
    }
}
