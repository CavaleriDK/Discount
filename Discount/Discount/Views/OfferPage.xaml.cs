using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Discount.Models;
using Discount.Views;

namespace Discount
{
    /// <summary>
    /// Interaction logic for DiscountPage.xaml
    /// </summary>
    public partial class OfferPage : Page
    {
        /// <summary>
        /// Static instance reference to this class
        /// </summary>
        public static OfferPage offerWindow { get; set; }

        /// <summary>
        /// Constructor for the DiscountPage view controller
        /// </summary>
        public OfferPage()
        {
            InitializeComponent();
            offerWindow = this;
        }

        /// <summary>
        /// Method used to populate the ListView with all offers
        /// </summary>
        public void PopulateWithAllOffers()
        {
            MainWindow.ShowOverlay();

            // Launch login attempt in new thread
            Thread t = new Thread(() => 
            {
                List<OfferModel> offers = OfferModel.FindAllOffersWithShopJoin(false);

                // Call the main thread and invoke method to hide loading overlay
                MainWindow.StaticDispatcher.Invoke(() =>
                {
                    ListViewOffers.ItemsSource = offers;

                    MainWindow.HideOverlay();
                });
            });
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// Method used to populate the ListView with specific offers from searches
        /// </summary>
        /// <param name="offers">The offers to populate</param>
        public void PopulateWithSpecificOffers(List<OfferModel> offers)
        {
            ListViewOffers.ItemsSource = offers;
        }

        /// <summary>
        /// On Click event delegate for clicking on a product in the grid
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event reference</param>
        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OfferModel model = (OfferModel)((Grid)sender).DataContext;

            MessageBoxResult result = MessageBox.Show("Did you purchase: " + model.Title + " for " + model.PriceOfferFormatted + "?", "Purchase Confirmation", System.Windows.MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                PurchaseController.Instance.AddPurchaseToUser(model);
            }
        }

        /// <summary>
        /// On Click event delegate for the Search navigation button
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event reference</param>
        private void NavSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchPage searchPage = new SearchPage();
            this.NavigationService.Navigate(searchPage);
        }
    }
}
