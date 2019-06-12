using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Globalization;

namespace Discount.Models
{
    /// <summary>
    /// Model representing a row in the Offer table
    /// </summary>
    public class OfferModel
    {
        /// <summary>
        /// The primary key of this model
        /// </summary>
        public int ID { get => id; }

        /// <summary>
        /// The offer id from the Shopgun API
        /// </summary>
        public string ShopgunOfferID { get => shopgunOfferId; }

        /// <summary>
        /// The dealer ID from the Shopgun API 
        /// </summary>
        public string ShopgunDealerID { get => shopgunDealerId; }

        /// <summary>
        /// The string url referencing the image hosted by Shopgun
        /// </summary>
        public string ImageURL { get => shopgunImageUrl; }

        /// <summary>
        /// The string name representation of this offer
        /// </summary>
        public string Title { get => title.Replace(@"\u0027", "'"); }


        /// <summary>
        /// The id referencing a row in our Shop table
        /// </summary>
        public int ShopID { get => shopId; }

        /// <summary>
        /// The original price before this offer
        /// </summary>
        public int PriceBefore
        {
            get => priceBefore;
            set
            {
                priceBefore = value;
                isUnchanged = false;
            }
        }

        /// <summary>
        /// The offer price for this product
        /// </summary>
        public int PriceOffer
        {
            get => priceOffer;
            set
            {
                priceOffer = value;
                isUnchanged = false;
            }
        }

        /// <summary>
        /// The price formatted properly as DKK for display on views
        /// </summary>
        public string PriceOfferFormatted
        {
            get
            {
                return (priceOffer / 100.0f).ToString("C2",
                  CultureInfo.CreateSpecificCulture("da-DK"));
            }
        }

        /// <summary>
        /// The starting date for this offer
        /// </summary>
        public DateTime OfferFrom
        {
            get => offerFrom;
            set
            {
                offerFrom = value;
                isUnchanged = false;
            }
        }

        /// <summary>
        /// The ending date for this offer
        /// </summary>
        public DateTime OfferTo
        {
            get => offerTo;
            set
            {
                offerTo = value;
                isUnchanged = false;
            }
        }

        /// <summary>
        /// The foreign key ShopModel referenced by this offer
        /// </summary>
        public ShopModel Shop
        {
            get => shop;
        }

        private int id;
        private int priceBefore;
        private int priceOffer;
        private string shopgunOfferId;
        private string shopgunDealerId;
        private string shopgunImageUrl;
        private string title;
        private int shopId;
        private DateTime offerFrom;
        private DateTime offerTo;
        private bool isUnchanged;
        private ShopModel shop;

        /// <summary>
        /// Private constructor to prevent other classes from instantiating this object
        /// </summary>
        /// <param name="isNew">Whether this is a new row or not</param>
        private OfferModel(bool isNew)
        {
            // Make sure this reflects wether or not this is a new model or fetched from database
            this.isUnchanged = !isNew;
        }


        /// <summary>
        /// Method used to find all offers in the database
        /// </summary>
        /// <returns>List of all offers instantiated as OfferModel</returns>
        public static List<OfferModel> FindAllOffers()
        {
            string whereClause = "id > 0";

            return FindList(whereClause);
        }

        /// <summary>
        /// Method used to find all offers joined with Shop table
        /// </summary>
        /// <param name="include_expired">Whether to include expired offers or not</param>
        /// <returns>List of all offers with Foregin Key ShopModel attached</returns>
        public static List<OfferModel> FindAllOffersWithShopJoin(bool include_expired)
        {
            string selectClause = "[Offer].ID, Title, Price_before, Price_offer, Offer_from, Offer_to, Shopgun_image_url, Shop_id, [Shop].ID as shop_real_id, [Shop].Name as Shop_name";
            string joinClause = "[Shop].ID = [Offer].Shop_id";
            string whereClause = (include_expired) ? "[Offer].ID > 0" : "[Offer].ID > 0" + " AND Offer_to >= GetDate()";

            return FindListWithJoinFromShops(selectClause, joinClause, whereClause);
        }

        /// <summary>
        /// Method to search for offers in all shops by part of a name string
        /// </summary>
        /// <param name="query">The case insensitive search query</param>
        /// <param name="include_expired">Whether to include expired offers or not</param>
        /// <returns>List of OfferModel that all contain the query as part of their name</returns>
        public static List<OfferModel> SearchByName(string query, bool include_expired)
        {
            string selectClause = "[Offer].ID, Title, Price_before, Price_offer, Offer_from, Offer_to, Shopgun_image_url, Shop_id, [Shop].ID as shop_real_id, [Shop].Name as Shop_name";
            string joinClause = "[Shop].ID = [Offer].Shop_id";
            string whereClause = (include_expired) ? "LOWER(Title) LIKE LOWER('%" + query + "%')" : "LOWER(Title) LIKE LOWER('%" + query + "%')" + " AND Offer_to >= GetDate()";

            return FindListWithJoinFromShops(selectClause, joinClause, whereClause);
        }

        /// <summary>
        /// Method used to find all offers in specific shop matching the search query
        /// </summary>
        /// <param name="query">The search query</param>
        /// <param name="s_id">The primary ID of the shop to search in</param>
        /// <param name="include_expired">Whether to include expired offers or not</param>
        /// <returns>List of OfferModel that all contain the query as part of their name</returns>
        public static List<OfferModel> SearchByNameFromShopID(string query, int s_id, bool include_expired)
        {
            string selectClause = "[Offer].ID, Title, Price_before, Price_offer, Offer_from, Offer_to, Shopgun_image_url, Shop_id, [Shop].ID as shop_real_id, [Shop].Name as Shop_name";
            string joinClause = "[Shop].ID = [Offer].Shop_id";
            string whereClause = (include_expired) ? "LOWER(Title) LIKE LOWER('%" + query + "%') AND Shop_id = " + s_id : "LOWER(Title) LIKE LOWER('%" + query + "%') AND Shop_id = " + s_id + " AND Offer_to >= GetDate()";

            return FindListWithJoinFromShops(selectClause, joinClause, whereClause);
        }

        /// <summary>
        /// Method used to save a models changes to the database. Nullifies Product_id if this was excluded
        /// </summary>
        /// <returns>True if the model was succesfully saved, false otherwise</returns>
        public bool Save()
        {
            if (!isUnchanged)
            {
                StringBuilder stringBuilder = new StringBuilder();

                // Check if this model has no ID and doesn't exist in the database
                if (this.id == default(int))
                {
                    // Prepare statement to create a new Offer row
                    stringBuilder.Append("INSERT INTO [Offer](Title, Price_before, Price_offer, Offer_from, Offer_to, Shopgun_offer_id, Shopgun_dealer_id," +
                        " Shopgun_image_url, Shop_id) OUTPUT INSERTED.ID VALUES ("); // Make sure the statement returns the ID of this row
                    stringBuilder.Append("'" + this.title + "', ");
                    stringBuilder.Append(this.priceBefore + ", ");
                    stringBuilder.Append(this.priceOffer + ", ");
                    stringBuilder.Append("'" + this.offerFrom.ToString("s") + "', "); // Convert to ISO 8601 string
                    stringBuilder.Append("'" + this.offerTo.ToString("s") + "', "); // Convert to ISO 8601 string
                    stringBuilder.Append("'" + this.shopgunOfferId + "', ");
                    stringBuilder.Append("'" + this.shopgunDealerId + "', ");
                    stringBuilder.Append("'" + this.shopgunImageUrl + "', ");
                    stringBuilder.Append(this.shopId + ")");
                }
                else
                {
                    // Prepare statement to update an existing Offer row where id = this.id
                    stringBuilder.Append("UPDATE [Offer] SET ");
                    stringBuilder.Append("Title = '" + this.title + "', ");
                    stringBuilder.Append("Price_before = " + this.priceBefore + ", ");
                    stringBuilder.Append("Price_offer = " + this.priceOffer + ", ");
                    stringBuilder.Append("Offer_from = '" + this.offerFrom.ToString("s") + "', "); // Convert to ISO 8601 string
                    stringBuilder.Append("Offer_to = '" + this.offerTo.ToString("s") + "', "); // Convert to ISO 8601 string
                    stringBuilder.Append("Shopgun_offer_id = '" + this.shopgunOfferId + "', ");
                    stringBuilder.Append("Shopgun_dealer_id = '" + this.shopgunDealerId + "', ");
                    stringBuilder.Append("Shopgun_image_url = '" + this.shopgunImageUrl + "', ");
                    stringBuilder.Append("Shop_id = " + this.shopId + " ");
                    stringBuilder.Append("WHERE id = " + this.id);
                }

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
        /// <param name="title">The heading/title of this offer</param>
        /// <param name="price_before">Price before offer</param>
        /// <param name="price_offer">Offer price</param>
        /// <param name="shopgun_offer_id">Shopgun ID for this offer</param>
        /// <param name="shopgun_dealer_id">Shopgun ID for the shop containing this offer</param>
        /// <param name="shopgun_image_url">The URL for the image of this offer</param>
        /// <param name="shop_id">ID of the shop containing this offer in our database</param>
        /// <param name="offer_from">Date and time of the beginning of this offer</param>
        /// <param name="offer_to">Date and time of the end of this offer</param>
        /// <returns>The new model saved in database, null if error while saving</returns>
        public static OfferModel New(string title, int price_before, int price_offer, string shopgun_offer_id, string shopgun_dealer_id, string shopgun_image_url, int shop_id, DateTime offer_from, DateTime offer_to)
        {
            // Create a new blank model
            OfferModel model = new OfferModel(true);
            model.title = title;
            model.priceBefore = price_before;
            model.priceOffer = price_offer;
            model.shopgunOfferId = shopgun_offer_id;
            model.shopgunDealerId = shopgun_dealer_id;
            model.shopgunImageUrl = shopgun_image_url;
            model.shopId = shop_id;
            model.offerFrom = offer_from;
            model.offerTo = offer_to;

            // Save the model
            if ( model.Save() )
            {
                return model;
            }

            // Return null if the model was failed to be saved
            return null;
        }

        /// <summary>
        /// Method used to return an OfferModel from the table by its ID
        /// </summary>
        /// <param name="id">The primary ID of the model to find</param>
        /// <returns>An OfferModel representing this row. Null if failed</returns>
        public static OfferModel FindByID(int id)
        {
            return Find("id = " + id);
        }

        /// <summary>
        /// Method used to return an OfferModel from the table by its Shopgun Offer ID
        /// </summary>
        /// <param name="shopgun_offer_id">The Shopgun Offer ID of the model to find</param>
        /// <returns>An OfferModel representing this row. Null if failed</returns>
        public static OfferModel FindByShopgunOfferID(string shopgun_offer_id)
        {
            return Find("Shopgun_offer_id = '" + shopgun_offer_id + "'");
        }

        /// <summary>
        /// Get a list of all offers from shop with Shopgun Dealer ID
        /// </summary>
        /// <param name="dealer_id">Shopgun Dealer ID to search for</param>
        /// <param name="include_expired">Whether or not this query should return expired offers</param>
        /// <returns>A list of OfferModels returned by the query</returns>
        public static List<OfferModel> FindByDealerID(string dealer_id, bool include_expired)
        {
            string whereClause = (include_expired) ? "Shopgun_dealer_id = '" + dealer_id + "'": "Shopgun_dealer_id = '" + dealer_id + "' AND Offer_to >= GetDate()";

            return FindList(whereClause);
        }

        /// <summary>
        /// Get a list of all offers from shop with Shop ID
        /// </summary>
        /// <param name="shop_id">Shop ID to search for</param>
        /// <param name="include_expired">Whether or not this query should return expired offers</param>
        /// <returns>A list of OfferModels returned by the query</returns>
        public static List<OfferModel> FindByShopID(int shop_id, bool include_expired)
        {
            string whereClause = (include_expired) ? "Shop_id = " + shop_id : "Shop_id = " + shop_id + " AND Offer_to >= GetDate()";

            return FindList(whereClause);
        }

        /// <summary>
        /// Get a list of all offers by Product ID
        /// </summary>
        /// <param name="product_id">Product ID to search for</param>
        /// <param name="include_expired">Whether or not this query should return expired offers</param>
        /// <returns>A list of OfferModels returned by the query</returns>
        public static List<OfferModel> FindByProductID(int product_id, bool include_expired)
        {
            string whereClause = (include_expired) ? "Product_id = " + product_id : "Product_id = " + product_id + " AND Offer_to >= GetDate()";

            return FindList(whereClause);
        }

        /// <summary>
        /// Get a list of all offers by a list of Shopgun Offer IDs
        /// </summary>
        /// <param name="offer_ids">List of Shopgun Offer Ids to search for</param>
        /// <returns>A list of OfferModels returned by the query</returns>
        public static List<OfferModel> FindAllExistingWithOfferIds(List<string> offer_ids)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Shopgun_offer_id IN (");
            for (int i = 0; i < offer_ids.Count; i++)
            {
                stringBuilder.Append("'" + offer_ids[i] + "'");
                stringBuilder.Append((i != offer_ids.Count-1) ? ", " : "");
            }
            stringBuilder.Append(")");

            string whereClause = stringBuilder.ToString();

            return FindList(whereClause);
        }

        /// <summary>
        /// Used internally to find row in the table and return as model object
        /// </summary>
        /// <param name="where_clause">There WHERE clause to use as part of the SQL statement</param>
        /// <returns>An OfferModel representing this row. Null if failed</returns>
        private static OfferModel Find(string where_clause)
        {
            // Create blank OfferModel and set it up as a an existing row
            OfferModel model = default(OfferModel);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT * FROM [Offer] WHERE ");
            stringBuilder.Append(where_clause);

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
                    model = new OfferModel(false); ;

                    model.id = reader.GetInt32(0);
                    model.title = reader.GetString(1);
                    model.priceBefore = reader.GetInt32(2);
                    model.priceOffer = reader.GetInt32(3);
                    model.offerFrom = reader.GetDateTime(4);
                    model.offerTo = reader.GetDateTime(5);
                    model.shopgunOfferId = reader.GetString(6);
                    model.shopgunDealerId = reader.GetString(7);
                    model.shopgunImageUrl = reader.GetString(8);
                    model.shopId = reader.GetInt32(9);
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
        /// <param name="where_clause">There WHERE clause to use as part of the SQL statement</param>
        /// <returns>A list of OfferModel representing these rows. Empty list if failed</returns>
        private static List<OfferModel> FindList(string where_clause)
        {
            List<OfferModel> models = new List<OfferModel>();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT * FROM [Offer] WHERE ");
            stringBuilder.Append(where_clause);

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
                    OfferModel model = new OfferModel(false);

                    model.id = reader.GetInt32(0);
                    model.title = reader.GetString(1);
                    model.priceBefore = reader.GetInt32(2);
                    model.priceOffer = reader.GetInt32(3);
                    model.offerFrom = reader.GetDateTime(4);
                    model.offerTo = reader.GetDateTime(5);
                    model.shopgunOfferId = reader.GetString(6);
                    model.shopgunDealerId = reader.GetString(7);
                    model.shopgunImageUrl = reader.GetString(8);
                    model.shopId = reader.GetInt32(9);

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

        /// <summary>
        /// Used internally to find rows in the table joined by Shop table and return as a list of model object
        /// </summary>
        /// <param name="select_clause">The SELECT clause for this SQL statement</param>
        /// <param name="join_clause">The JOIN clause for this SQL statement</param>
        /// <param name="where_clause">The WHERE clause to use as part of the SQL statement</param>
        /// <returns>A list of OfferModel representing these rows. Empty list if failed</returns>
        private static List<OfferModel> FindListWithJoinFromShops(string select_clause, string join_clause, string where_clause)
        {
            List<OfferModel> models = new List<OfferModel>();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT ");
            stringBuilder.Append(select_clause);
            stringBuilder.Append(" FROM [Offer] ");
            stringBuilder.Append("INNER JOIN [Shop] ON");
            stringBuilder.Append(join_clause);
            stringBuilder.Append(" WHERE ");
            stringBuilder.Append(where_clause);

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
                    OfferModel model = new OfferModel(false);

                    model.id = reader.GetInt32(0);
                    model.title = reader.GetString(1);
                    model.priceBefore = reader.GetInt32(2);
                    model.priceOffer = reader.GetInt32(3);
                    model.offerFrom = reader.GetDateTime(4);
                    model.offerTo = reader.GetDateTime(5);
                    model.shopgunImageUrl = reader.GetString(6);
                    model.shopId = reader.GetInt32(7);

                    ShopModel sModel = new ShopModel(reader.GetInt32(8), reader.GetString(9));
                    model.shop = sModel;

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
