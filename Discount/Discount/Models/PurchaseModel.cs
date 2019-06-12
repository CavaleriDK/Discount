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
    /// The model representing the Purchase table
    /// </summary>
    class PurchaseModel
    {      
        /// <summary>
        /// The primary key of this model
        /// </summary>
        public int Id { get => id; set => id = value; }

        /// <summary>
        /// The price the product is purchased for
        /// </summary>
        public int PricePurchased { get => pricePurchased; set => pricePurchased = value; }

        /// <summary>
        /// The time the product is purchased
        /// </summary>
        public DateTime PurchaseTime { get => purchaseTime; set => purchaseTime = value; }

        /// <summary>
        /// The id referencing a foreign row in our Shop table
        /// </summary>
        public int ShopID { get => shopID; set => shopID = value; }

        /// <summary>
        /// The id referencing a foreign row in our Offer table
        /// </summary>
        public int OfferID { get => offerID; set => offerID = value; }
        
        /// <summary>
        /// The id referencing a foreign row in our User table
        /// </summary>
        public int UserID { get => userID; set => userID = value; }

        private int id;
        private int pricePurchased;
        private DateTime purchaseTime;
        private int shopID;
        private int offerID;
        private int userID;
        private bool isUnchanged;

        /// <summary>
        /// Private constructor to prevent other classes from instantiating this object
        /// </summary>
        private PurchaseModel(bool isNew)
        {
            this.isUnchanged = !isNew;
        }
        
        /// <summary>
        /// Method used to save a models changes to the database. Only usable for new models as this model cannot change values
        /// </summary>
        /// <returns>True if the model was succesfully saved, false otherwise</returns>
        public bool Save()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (!this.isUnchanged)
            {
                //Prepare statement to create a new offer row    
                stringBuilder.Append("INSERT INTO [Purchase] (Price_purchased, Purchase_time, Shop_id, Offer_id, User_id) OUTPUT INSERTED.ID VALUES (");
                stringBuilder.Append(this.pricePurchased + ", ");
                stringBuilder.Append("'" + this.purchaseTime.ToString("s") + "', ");
                stringBuilder.Append(this.shopID + ", ");
                stringBuilder.Append(this.offerID + ", ");
                stringBuilder.Append(this.userID + ")");


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
        /// Creates and saves a new model of this type to the database
        /// </summary>
        /// <param name="pricePurchased">The price the product was bought for</param>
        /// <param name="purchaseTime">The time the product was bought</param>
        /// <param name="shopID">The ID of the shop the product was bought in</param>
        /// <param name="offerID">The ID of the product the user buys</param>
        /// <param name="userID">The ID of the user buying this product</param>
        /// <returns></returns> 
        public static PurchaseModel New(int pricePurchased, DateTime purchaseTime, int shopID, int offerID, int userID)
        {
            PurchaseModel model = new PurchaseModel(true);
            model.PricePurchased = pricePurchased;
            model.PurchaseTime = purchaseTime;
            model.ShopID = shopID;
            model.OfferID = offerID;
            model.UserID = userID;

            if (model.Save())
            {
                return model;
            }


            return null;
        }

    }
}
