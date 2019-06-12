using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Discount.Models
{
    /// <summary>
    /// Model representing a row in the Shop table
    /// </summary>
    public class ShopModel
    {
        /// <summary>
        /// The Primary key ID of this model
        /// </summary>
        public int ID { get => id; }

        /// <summary>
        /// The shop Name from Shopgun API
        /// </summary>
        public string Name { get => name.Replace(@"\u0027", "'"); }

        /// <summary>
        /// The Shopgun Dealer ID from the Shopgun API
        /// </summary>
        public string ShopgunDealerID { get => shopgunDealerId; }

        private int id;
        private string name;
        private string shopgunDealerId;

        private bool isUnchanged;
        private bool readOnly;

        /// <summary>
        /// Private constructor for this model
        /// </summary>
        /// <param name="isNew">Whether or not this is a new model</param>
        private ShopModel(bool isNew)
        {
            // Make sure this reflects wether or not this is a new model or fetched from database
            this.isUnchanged = !isNew;
            this.readOnly = false;
        }

        /// <summary>
        /// Public constructor used for instantiating this model as part of foreign key reference on OfferModel
        /// </summary>
        /// <param name="id">The Primary ID of this model</param>
        /// <param name="name">The Name of this shop</param>
        public ShopModel(int id, string name)
        {
            this.id = id;
            this.name = name;
            this.readOnly = true;
        }

        /// <summary>
        /// Method used to save a models changes to the database. Only usable for new models as this model cannot change values
        /// </summary>
        /// <returns>True if the model was succesfully saved, false otherwise</returns>
        public bool Save()
        {
            if (!this.isUnchanged && !this.readOnly)
            {
                StringBuilder stringBuilder = new StringBuilder();

                // Prepare statement to create a new Offer row
                stringBuilder.Append("INSERT INTO [Shop](Name, Shopgun_dealer_ID) OUTPUT INSERTED.ID VALUES (");
                stringBuilder.Append("'" + this.name + "', ");
                stringBuilder.Append("'" + this.shopgunDealerId + "')");

                // Setup appropriate SQL statement to save
                string commandText = stringBuilder.ToString();

                // Lock this protected area with the SQL connection itself
                Monitor.Enter(ConnectionController.Instance.SqlCon);

                try
                {
                    // Open the SQL connection
                    ConnectionController.Instance.OpenSqlConnection();

                    // Create the SQL Command object and setup the command statement
                    SqlCommand cmd = ConnectionController.Instance.SqlCon.CreateCommand();
                    cmd.CommandText = commandText;

                    // Set the ID property if this is an insert statement
                    if (this.id == default(int))
                    {
                        this.id = (int)cmd.ExecuteScalar();
                    }
                    else
                    {
                        // Execute the statement blankly
                        cmd.ExecuteNonQuery();
                    }

                    // Set state as saved
                    this.isUnchanged = true;
                }
                catch (Exception e)
                {
                    // Show error if something went horribly wrong
                    MessageBox.Show("Error: " + e.Message, "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                finally
                {
                    // Close the connection always
                    ConnectionController.Instance.CloseSqlConnection();

                    // Release the lock
                    Monitor.Exit(ConnectionController.Instance.SqlCon);
                }
            }

            return this.isUnchanged;
        }

        /// <summary>
        /// Creates and saves a new model of this type in the database
        /// </summary>
        /// <param name="name">Name of this shop</param>
        /// <param name="shopgun_dealer_id">Shopgun Dealer ID of this shop</param>
        /// <returns>The new model saved in database, null if error while saving</returns>
        public static ShopModel New(string name, string shopgun_dealer_id)
        {
            // Create blank model and set all fields
            ShopModel model = new ShopModel(true);
            model.name = name;
            model.shopgunDealerId = shopgun_dealer_id;

            // Save the model
            if (model.Save())
            {
                return model;
            }

            // Return null if the model was failed to be saved
            return null;
        }

        /// <summary>
        /// Method used to return a ShopModel from the table by its ID
        /// </summary>
        /// <param name="id">The primary ID of the model to find</param>
        /// <returns>A ShopModel representing this row. Null if failed</returns>
        public static ShopModel FindByID(int id)
        {
            return Find("id = " + id);
        }

        /// <summary>
        /// Method used to return a ShopModel from the table by its Shopgun Dealer ID
        /// </summary>
        /// <param name="shopgun_dealer_id">The Shopgun Dealer ID of the model to find</param>
        /// <returns>A ShopModel representing this row. Null if failed</returns>
        public static ShopModel FindByShopgunDealerID(string shopgun_dealer_id)
        {
            return Find("Shopgun_dealer_ID = '" + shopgun_dealer_id + "'");
        }

        /// <summary>
        /// Get a list of all shops
        /// </summary>
        /// <returns>A list of ShopModels with all shops in the Database</returns>
        public static List<ShopModel> FindAll()
        {
            // Fake where clause to make sure all results are returned
            string whereClause = "id > 0";

            return FindList(whereClause);
        }

        /// <summary>
        /// Method to search after all shops by part of a name string
        /// </summary>
        /// <param name="query">The case insensitive search query</param>
        /// <returns>List of ShopModels that all contain the query as part of their name</returns>
        public static List<ShopModel> SearchByName(string query)
        {
            // Make sure the search is case insensitive by changing all characters to lower case
            string whereClause = "LOWER(Name) LIKE LOWER('%" + query + "%')";

            return FindList(whereClause);
        }

        /// <summary>
        /// Used internally to find row in the table and return as model object
        /// </summary>
        /// <param name="where_clause">There WHERE clause to use as part of the SQL statement</param>
        /// <returns>A ShopModel representing this row. Null if failed</returns>
        private static ShopModel Find(string where_clause)
        {
            // Create new model and set it up as existing row
            ShopModel model = default(ShopModel);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT * FROM [Shop] WHERE ");
            stringBuilder.Append(where_clause);

            // Build full SQL statement
            string commandText = stringBuilder.ToString();

            // Enter the protected area to prevent race conditions
            Monitor.Enter(ConnectionController.Instance.SqlCon);

            try
            {
                // Open the SQL connection
                ConnectionController.Instance.OpenSqlConnection();

                // Create the SQL Command object and setup the command statement
                SqlCommand cmd = ConnectionController.Instance.SqlCon.CreateCommand();
                cmd.CommandText = commandText;

                // Execute the statement
                SqlDataReader reader = cmd.ExecuteReader();

                // Parse data as Model
                while (reader.Read())
                {
                    model = new ShopModel(false);

                    model.id = reader.GetInt32(0);
                    model.name = reader.GetString(1);
                    model.shopgunDealerId = reader.GetString(2);
                }

                // Close the reader after being used
                reader.Close();
            }
            catch (Exception e)
            {
                // Show error if something went horribly wrong
                MessageBox.Show("Error: " + e.Message, "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                // Gracefully close the connection
                ConnectionController.Instance.CloseSqlConnection();

                // Exit the protected area
                Monitor.Exit(ConnectionController.Instance.SqlCon);
            }

            // Return the model if anything was succesfully found or null
            return model;
        }

        /// <summary>
        /// Used internally to find rows in the table and return as a list of model object
        /// </summary>
        /// <param name="where_clause">The WHERE clause to use as part of the SQL statement</param>
        /// <returns>A list of ShopModels representing these rows. Empty list if failed</returns>
        private static List<ShopModel> FindList(string where_clause)
        {
            List<ShopModel> models = new List<ShopModel>();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT * FROM [Shop] WHERE ");
            stringBuilder.Append(where_clause);

            // Build SQL statement
            string commandText = stringBuilder.ToString();

            // Enter the protected area to prevent race conditions
            Monitor.Enter(ConnectionController.Instance.SqlCon);

            try
            {
                // Open the SQL connection
                ConnectionController.Instance.OpenSqlConnection();

                // Create the SQL Command object and setup the command statement
                SqlCommand cmd = ConnectionController.Instance.SqlCon.CreateCommand();
                cmd.CommandText = commandText;

                // Execute the statement
                SqlDataReader reader = cmd.ExecuteReader();

                // Parse data as Model and append to the list of models
                while (reader.Read())
                {
                    // Create blank OfferModel and set it up as a an existing row
                    ShopModel model = new ShopModel(false);

                    model.id = reader.GetInt32(0);
                    model.name = reader.GetString(1);
                    model.shopgunDealerId = reader.GetString(2);

                    models.Add(model);
                }

                // Close the reader after being used
                reader.Close();
            }
            catch (Exception e)
            {
                // Show error if something went horribly wrong
                MessageBox.Show("Error: " + e.Message, "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                // Gracefully close the connection
                ConnectionController.Instance.CloseSqlConnection();

                // Exit the protected area
                Monitor.Exit(ConnectionController.Instance.SqlCon);
            }

            return models;
        }
    }
}
