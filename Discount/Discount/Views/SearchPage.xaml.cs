using Discount.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Collections.ObjectModel;
using Discount.Models;
using System.Xaml;
using System.Xml;
using System.Threading;

namespace Discount.Views
{
    /// <summary>
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        /// <summary>
        /// Static reference to this window
        /// </summary>
        public static SearchPage searchWindow { get; set; }

        private ProductSearchController productSearchController;
        private ObservableCollection<ShopModel> shopList;
        private ObservableCollection<ShopModel> ShopList
        {
            get => shopList;
        }

        /// <summary>
        /// Constructor for this page
        /// </summary>
        public SearchPage()
        {
            InitializeComponent();
            productSearchController = new ProductSearchController();
            shopList = new ObservableCollection<ShopModel>( ShopModel.FindAll() );

            ShopMenu.ItemsSource = ShopList;
            searchWindow = this;

        }

        /// <summary>
        /// On Click event delegate for the Search button
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event reference</param>
        private void ProductSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if(ShopMenu.SelectedValue == null)
            {
                string product = ProductSearch.Text;

                // Show loading overlay
                MainWindow.ShowOverlay();

                // Launch search attempt in new thread
                Thread t = new Thread(() => productSearchController.SearchForOffer(product));
                t.IsBackground = true;
                t.Start();
            }
            else
            {
                string product = ProductSearch.Text;
                int selected = (int)ShopMenu.SelectedValue;

                // Show loading overlay
                MainWindow.ShowOverlay();

                // Launch search attempt in new thread
                Thread t = new Thread(() => productSearchController.SearchForOfferByStore(product, selected));
                t.IsBackground = true;
                t.Start();
            }

        }

        /// <summary>
        /// On Click event delegate for the back navigation button
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event reference</param>
        private void NavSearchBackButton_Click(object sender, RoutedEventArgs e)
        {
            OfferPage offerpage = new OfferPage();
            this.NavigationService.Navigate(offerpage);
        }
    }
}
