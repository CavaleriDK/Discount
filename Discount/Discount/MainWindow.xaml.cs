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
using Discount.Models;

namespace Discount
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Dispatcher used to dispatch methods onto the main UI thread
        /// </summary>
        public static System.Windows.Threading.Dispatcher StaticDispatcher { get => staticDispatcher; }

        private static Grid loadingOverlay;
        private static Grid userInterface;
        private static System.Windows.Threading.Dispatcher staticDispatcher;

        User user = User.Instance;

        /// <summary>
        /// Constructor for this View Controller
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            LoginPage loginPage = new LoginPage();
            MainFrame.NavigationService.Navigate(loginPage);

            // setup the loading image as static property
            loadingOverlay = TheLoadingOverlay;

            // setup the UI as static property
            userInterface = UserInterface;

            // Setup static reference to the dispatcher
            staticDispatcher = Dispatcher;

            Header.DataContext = user;
        }

        /// <summary>
        /// Method used to show the user interface
        /// </summary>
        public static void ShowUI()
        {
            userInterface.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Method used to hide the user interface
        /// </summary>
        public static void HideUI()
        {
            userInterface.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Method used to show the loading overlay
        /// </summary>
        public static void ShowOverlay()
        {
            loadingOverlay.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Method used to hide the loading overlay
        /// </summary>
        public static void HideOverlay()
        {
            loadingOverlay.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// On click event delegate for the Nav Menu button
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event reference</param>
        private void NavMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavMenu.Visibility == Visibility.Collapsed)
            {
                NavMenu.Visibility = Visibility.Visible;
            }
            else
            {
                NavMenu.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// On Click event delegate for the Signout button
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event reference</param>
        private void SignoutButton_Click(object sender, RoutedEventArgs e)
        {
            user.LogoutUser();
            HideUI();
            LoginPage loginPage = new LoginPage();
            MainFrame.NavigationService.Navigate(loginPage);
        }

        /// <summary>
        /// On Click event delegate for the SyncButton button
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event reference</param>
        private void SyncButton_Click(object sender, RoutedEventArgs e)
        {
            // Show loading overlay
            ShowOverlay();

            // Populate with only Lidl offers!
            ShopgunController.Instance.PopulateDatabaseWithListOfDealers();

        }

        private void NavUserButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NavPurchaseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// On Click event delegate for the Offers navigation button
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event reference</param>
        private void NavOfferButton_Click(object sender, RoutedEventArgs e)
        {
            OfferPage offerPage = new OfferPage();
            // Populate Offermodel
            MainFrame.NavigationService.Navigate(offerPage);
        }
    }
}
