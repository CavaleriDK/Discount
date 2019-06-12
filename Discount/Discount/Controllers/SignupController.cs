using Discount.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Discount
{
    /// <summary>
    /// The SignupController mitigating all functionality for creating a new user account
    /// </summary>
    class SignupController
    {

        User user = User.Instance;

        /// <summary>
        /// Constructor for the SignupController
        /// </summary>
        public SignupController()
        {

        }

        /// <summary>
        /// Method to verify input and create a new user account
        /// </summary>
        /// <param name="signupName">The users username</param>
        /// <param name="signupEmail">The users email</param>
        /// <param name="signupPassword">The users password as plain text</param>
        /// <param name="signupRepeatPassword">The users password repeated as plain text</param>
        public void CreateAcc(string signupName, string signupEmail, string signupPassword, string signupRepeatPassword)
        {
            bool nameError = true;
            bool emailError = true;
            bool passError = true;
            bool success = false;

            if (signupName.Length >= 1 && Regex.IsMatch(signupName, @"^[a-zA-Z0-9-\.-@]+$"))
            {
                if (signupEmail.Length >= 6 && Regex.IsMatch(signupEmail, @"^[a-zA-Z0-9-\.-@]+$"))
                {
                    user.GetEmail(signupEmail);

                    if (user.Email != signupEmail)
                    {
                        if (signupPassword.Length >= 6 && signupPassword == signupRepeatPassword && Regex.IsMatch(signupPassword, @"^[a-zA-Z0-9-\.-@]+$"))
                        {
                            user.Name = signupName;
                            user.Email = signupEmail;
                            user.Password = Utilities.CalculateSHA256Hash(signupPassword); // Hash the password
                            user.Level = 1;
                            user.Experience = 0;

                            success = user.CreateAccount();

                            passError = false;
                            nameError = false;
                            emailError = false;
                        }
                        emailError = false;
                        nameError = false;
                        user.Email = null;
                    }
                }
                nameError = false;
            }

            // Call the main thread and invoke method to hide loading overlay
            MainWindow.StaticDispatcher.Invoke(() =>
            {
                if (nameError == true || emailError == true || passError == true)
                {
                    SignupPage.signupWindow.SignupError(nameError, emailError, passError);
                }

                MainWindow.HideOverlay();

                if (success)
                {
                    MessageBox.Show("User created!");
                    SignupPage.signupWindow.UserCreated();
                }
            });
        }
    }
}
