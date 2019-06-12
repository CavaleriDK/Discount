using Discount.Models;
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
    /// Interaction logic for SignupPage.xaml
    /// </summary>
    public partial class SignupPage : Page
    {
        /// <summary>
        /// Static instance reference for this class
        /// </summary>
        public static SignupPage signupWindow { get; set; }

        private SignupController signupController;

        /// <summary>
        /// Constructor for the SignupPage
        /// </summary>
        public SignupPage()
        {
            InitializeComponent();
            signupController = new SignupController();
            signupWindow = this;
        }

        /// <summary>
        /// Method to navigate user to login page when account was created
        /// </summary>
        public void UserCreated()
        {
            this.NavigationService.Navigate(LoginPage.LoginWindow);
        }

        /// <summary>
        /// Method to show errors on signup
        /// </summary>
        /// <param name="nameError">Whether error for username should be displayed</param>
        /// <param name="emailError">Whether error for email should be displayed</param>
        /// <param name="passError">Whether error for password should be displayed</param>
        public void SignupError(bool nameError, bool emailError, bool passError)
        {

            if (nameError == true)
            {
                InvalidNameLabel.Visibility = Visibility.Visible;
            }
            else
                InvalidNameLabel.Visibility = Visibility.Hidden;

            if (emailError == true)
            {
                InvalidEmailLabel.Visibility = Visibility.Visible;
            }
            else
                InvalidEmailLabel.Visibility = Visibility.Hidden;

            if (passError == true)
            {
                InvalidPasswordLabel.Visibility = Visibility.Visible;
            }
            else
                InvalidPasswordLabel.Visibility = Visibility.Hidden;

        }

        /// <summary>
        /// On Click event delegate for the Create Account button
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event reference</param>
        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            // Show loading overlay
            MainWindow.ShowOverlay();

            string name = NameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string repeatPassword = RepeatPasswordBox.Password;

            // Launch create account attempt in new thread
            Thread t = new Thread(() => signupController.CreateAcc(name, email, password, repeatPassword));
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// On Click Delegate for the "already have an account" label
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event reference</param>
        private void AlreadyHaveAnAccountButton_Click(object sender, RoutedEventArgs e)
        {
            LoginPage loginPage = new LoginPage();
            this.NavigationService.Navigate(loginPage);
        }

        /// <summary>
        /// On KeyUp event delegate for the create account button. Checks against Enter/Return key
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event reference</param>
        private void SignupTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CreateAccountButton_Click(sender, e);
            }
        }
    }
}
