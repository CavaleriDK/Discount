using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Discount.Models
{
    /// <summary>
    /// The User model class with database functionality for MVC pattern
    /// </summary>
    class User : INotifyPropertyChanged
    {
        /// <summary>
        /// Singleton instance of this class
        /// </summary>
        public static User Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new User();
                }
                return instance;
            }
        }

        /// <summary>
        /// The primary key ID
        /// </summary>
        public int ID
        {
            get => id;
            set
            {
                Monitor.Enter(this);
                id = value;
                OnPropertyChanged("ID");

                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// The User account username
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                Monitor.Enter(this);
                name = value;
                OnPropertyChanged("Name");
                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// The users email address
        /// </summary>
        public string Email
        {
            get => email;
            set
            {
                Monitor.Enter(this);
                email = value;
                OnPropertyChanged("Email");

                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// The users password in plain text
        /// </summary>
        public string Password
        {
            get => password;
            set
            {
                Monitor.Enter(this);
                password = value;
                OnPropertyChanged("Password");

                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// The users level
        /// </summary>
        public int Level
        {
            get => level;
            set
            {
                Monitor.Enter(this);
                level = value;
                OnPropertyChanged("Level");
                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// The users total XP
        /// </summary>
        public int Experience
        {
            get => experience;
            set
            {
                Monitor.Enter(this);
                experience = value;
                OnPropertyChanged("Experience");
                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// The amount of experience required for this level
        /// </summary>
        public int ExperienceRequiredForThisLevel
        {
            get => expForThis;
            set
            {
                Monitor.Enter(this);
                expForThis = value;
                OnPropertyChanged("ExperienceRequiredForThisLevel");
                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// The amount of experience required for the next level
        /// </summary>
        public int ExperienceRequiredForNextLevel
        {
            get => expForNext;
            set
            {
                Monitor.Enter(this);
                expForNext = value;
                OnPropertyChanged("ExperienceRequiredForNextLevel");
                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// Eventhandler for whenever a property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private int id;
        private string name;
        private string email;
        private string password;
        private int level;
        private int experience;

        private int expForNext;
        private int expForThis;
        private static User instance;

        /// <summary>
        /// Private constructor for the Singleton instance
        /// </summary>
        private User()
        {

        }

        /// <summary>
        /// Method to logout the current user
        /// </summary>
        public void LogoutUser()
        {
            ID = 0;
            Name = null;
            Email = null;
            Password = null;
            Level = 0;
            Experience = 0;
        }

        /// <summary>
        /// Method to add experience to new user. Also handles level up if enough XP was collected
        /// </summary>
        /// <param name="amount">The amount of experience to give the user</param>
        public void AddNewExperience(int amount)
        {
            int newExp = this.Experience + amount;
            int newLevel = this.Level;

            // Check if user should level up
            if (this.Experience + amount >= this.ExperienceRequiredForNextLevel)
            {
                // Increase level
                newLevel = this.Level + 1;
            }

            // Create the SQL statement
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("UPDATE [User] SET Experience = ");
            stringBuilder.Append(newExp);
            stringBuilder.Append(", Level = ");
            stringBuilder.Append(newLevel);
            stringBuilder.Append(" WHERE id = ");
            stringBuilder.Append(this.ID);

            // Setup appropriate SQL statement to save
            string commandText = stringBuilder.ToString();

            //Lock this protected area with the SQL connection
            Monitor.Enter(ConnectionController.Instance.SqlCon);
            try
            {
                // Open the SQL connection
                ConnectionController.Instance.OpenSqlConnection();

                // Create the SQL Command object and setup the command statement
                SqlCommand cmd = ConnectionController.Instance.SqlCon.CreateCommand();
                cmd.CommandText = commandText;

                // Execute the statement blankly
                cmd.ExecuteNonQuery();

                // Update Experience and Level for this user
                this.Experience = newExp;
                this.Level = newLevel;
            }
            catch (Exception e)
            {
                //Tell if something went wrong
                MessageBox.Show("Error: " + e.Message, "GetUser", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                //Close SQL connection
                ConnectionController.Instance.CloseSqlConnection();

                // Release lock
                Monitor.Exit(ConnectionController.Instance.SqlCon);

                ReCalculateExpRequirements();
            }
        }

        /// <summary>
        /// Method to fetch email from Database
        /// </summary>
        /// <param name="signupEmail">The email to check against</param>
        public void GetEmail(string signupEmail)
        {
            StringBuilder stringBuilder = new StringBuilder();

            //Prepare statement to select the User row with matching Email 
            stringBuilder.Append("SELECT * FROM [User] WHERE Email=");
            stringBuilder.Append("'" + signupEmail.ToString() + "'");

            //Setup SQL statement to save
            string commandText = stringBuilder.ToString();

            //Lock this protected area with the SQL connection
            Monitor.Enter(ConnectionController.Instance.SqlCon);

            try
            {
                // Open SQL connection
                ConnectionController.Instance.OpenSqlConnection();

                //Create SQL Command object and set the command
                SqlCommand cmd = ConnectionController.Instance.SqlCon.CreateCommand();
                cmd.CommandText = commandText;

                // Execute statement
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Email = reader.GetString(2);
                }

                // Close reader
                reader.Close();
            }
            catch (Exception e)
            {
                //Tell if something went wrong
                MessageBox.Show("Error: " + e.Message, "GetUser", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                //Close SQL connection
                ConnectionController.Instance.CloseSqlConnection();

                // Release lock
                Monitor.Exit(ConnectionController.Instance.SqlCon);
            }
        }

        /// <summary>
        /// Method to find a whole user account in the database
        /// </summary>
        /// <param name="loginEmail">Email to check against</param>
        public void GetUser(string loginEmail)
        {
            StringBuilder stringBuilder = new StringBuilder();

            //Prepare statement to select the User row with matching Email 
            stringBuilder.Append("SELECT * FROM [User] WHERE Email=");
            stringBuilder.Append("'" + loginEmail.ToString() + "'");

            //Setup SQL statement to save
            string commandText = stringBuilder.ToString();

            //Lock this protected area with the SQL connection
            Monitor.Enter(ConnectionController.Instance.SqlCon);

            try
            {
                // Open SQL connection
                ConnectionController.Instance.OpenSqlConnection();

                //Create SQL Command object and set the command
                SqlCommand cmd = ConnectionController.Instance.SqlCon.CreateCommand();
                cmd.CommandText = commandText;

                // Execute statement
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ID = reader.GetInt32(0);
                    Name = reader.GetString(1);
                    Email = reader.GetString(2);
                    Password = reader.GetString(3);
                    Level = reader.GetInt32(4);
                    Experience = reader.GetInt32(5);
                }

                // Close reader
                reader.Close();
            }
            catch (Exception e)
            {
                //Tell if something went wrong
                MessageBox.Show("Error: " + e.Message, "GetUser", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                //Close SQL connection
                ConnectionController.Instance.CloseSqlConnection();

                // Release lock
                Monitor.Exit(ConnectionController.Instance.SqlCon);

                ReCalculateExpRequirements();
            }
        }

        /// <summary>
        /// Method to create a new account with the data saved in this class instance
        /// </summary>
        /// <returns>Returns true if the account was created succesfully</returns>
        public bool CreateAccount()
        {
            bool success = false;
            StringBuilder stringBuilder = new StringBuilder();

            //Prepare statement to create a new User row
            stringBuilder.Append("INSERT INTO [User](Name, Email, Password, Level, Experience) VALUES (");
            stringBuilder.Append("'" + this.Name + "', ");
            stringBuilder.Append("'" + this.Email + "', ");
            stringBuilder.Append("'" + this.Password + "', ");
            stringBuilder.Append("'" + this.Level + "', ");
            stringBuilder.Append("'" + this.Experience + "')");

            //Setup SQL statement to save
            string commandText = stringBuilder.ToString();

            //Lock this protected area with the SQL connection
            Monitor.Enter(ConnectionController.Instance.SqlCon);

            try
            {
                //Open SQL connection
                ConnectionController.Instance.OpenSqlConnection();

                //Create SQL Command object and set the command
                SqlCommand cmd = ConnectionController.Instance.SqlCon.CreateCommand();
                cmd.CommandText = commandText;

                cmd.ExecuteNonQuery();

                success = true;
            }
            catch (Exception e)
            {
                //Tell if something went wrong
                MessageBox.Show("Error: " + e.Message, "CreateAccount", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                //Close connection
                ConnectionController.Instance.CloseSqlConnection();

                //Release lock
                Monitor.Exit(ConnectionController.Instance.SqlCon);
            }

            return success;
        }

        /// <summary>
        /// Method used to handle recalculation of the required Experience on level up
        /// </summary>
        private void ReCalculateExpRequirements()
        {
            this.ExperienceRequiredForThisLevel = Utilities.CalculateRequiredExperienceForNextLevel(this.Level);
            this.ExperienceRequiredForNextLevel = Utilities.CalculateRequiredExperienceForNextLevel(this.Level + 1);
        }

        /// <summary>
        /// Event handler for when a property has been changed
        /// </summary>
        /// <param name="AName">The property name that was updated</param>
        protected void OnPropertyChanged(string AName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(AName));
            }
        }
    }
}
