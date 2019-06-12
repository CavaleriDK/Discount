using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Discount.Models;

namespace Discount
{
    /// <summary>
    /// The Login controller mitigating all login functionality
    /// </summary>
    public class LoginController
    {
        User user = User.Instance;

        /// <summary>
        /// Constructor for the LoginController
        /// </summary>
        public LoginController()
        {
            
        }

        /// <summary>
        /// Methof for verifying a login attempt
        /// </summary>
        /// <param name="loginEmail">Users email as a string</param>
        /// <param name="loginPassword">Users password in plain text</param>
        public void LoginVerification(string loginEmail, string loginPassword)
        {
            bool emailError = true;
            bool passError = true;
            bool success = false;

            //if (EmailVar.Length >= 6 && EmailVar.Contains('.') && EmailVar.Contains('@'))

            if (loginEmail.Length >= 6 && Regex.IsMatch(loginEmail, @"^[a-zA-Z0-9-\.-@]+$"))
            {
                user.GetUser(loginEmail);

                if (user.Email == loginEmail)
                {
                    if (loginPassword.Length >= 6 && Regex.IsMatch(loginPassword, @"^[a-zA-Z0-9-\.-@]+$"))
                    {
                        // Hash the password
                        loginPassword = Utilities.CalculateSHA256Hash(loginPassword);

                        if (user.Password == loginPassword)
                        {
                            passError = false;
                            success = true;
                        }
                    }

                    emailError = false;
                }
            }

            // Call the main thread and invoke method to hide loading overlay
            MainWindow.StaticDispatcher.Invoke(() =>
            {
                LoginPage.LoginWindow.PassError(passError);
                LoginPage.LoginWindow.EmailError(emailError);

                MainWindow.HideOverlay();

                if (success)
                {
                    LoginPage.LoginWindow.LoginAccount();
                }
            });
        }
    }
}
