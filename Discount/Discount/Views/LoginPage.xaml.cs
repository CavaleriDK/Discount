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

namespace Discount
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        /// <summary>
        /// Static instance reference for this class
        /// </summary>
        public static LoginPage LoginWindow { get; set; }

        private LoginController loginController;

        /// <summary>
        /// Constructor for this view controller
        /// </summary>
        public LoginPage()
        {
            InitializeComponent();
            loginController = new LoginController();
            LoginWindow = this;
        }

        /// <summary>
        /// Method to mitigate login attempt to the LoginController
        /// </summary>
        public void LoginAccount()
        {
            OfferPage offerPage = new OfferPage();
            offerPage.PopulateWithAllOffers();
            this.NavigationService.Navigate(offerPage);
            MainWindow.ShowUI();
        }

        /// <summary>
        /// Method to display login error for email
        /// </summary>
        /// <param name="emailError">Whether error should be displayed or not</param>
        public void EmailError(bool emailError)
        {
            if (emailError == true)
            {
                InvalidEmailLabel.Visibility = Visibility.Visible;
            }
            else
                InvalidEmailLabel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Method to display login error for password
        /// </summary>
        /// <param name="passError">Whether the error should be displayed or not</param>
        public void PassError(bool passError)
        {
            if (passError == true)
            {
                InvalidPasswordLabel.Visibility = Visibility.Visible;
            }
            else
                InvalidPasswordLabel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// On Click event delegate for the Login Button
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event reference</param>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string loginEmail = LoginEmailTextbox.Text;
            string loginPassword = LoginPasswordbox.Password;

            // Show loading overlay
            MainWindow.ShowOverlay();

            // Launch login attempt in new thread
            Thread t = new Thread(() => loginController.LoginVerification(loginEmail, loginPassword));
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// On Click event delegate for the New Account button
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event reference</param>
        private void NewAccButton_Click(object sender, RoutedEventArgs e)
        {
            SignupPage signupPage = new SignupPage();
            this.NavigationService.Navigate(signupPage);
        }
        /// <summary>
        /// On KeyUp event delegate for the Login button. Checks against Enter/Return key
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event reference</param>

        private void LoginTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Login_Click(sender, e);
            }
        }
    }
}
