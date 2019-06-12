using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Discount
{
    /// <summary>
    /// Class which handles all database connection logic
    /// </summary>
    public class ConnectionController
    {
        /// <summary>
        /// Singleton instance of this class
        /// </summary>
        public static ConnectionController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConnectionController();
                }
                return instance;
            }
        }

        /// <summary>
        /// Reference to the database connection
        /// </summary>
        public SqlConnection SqlCon { get; set; }

        private string ConString { get; set; }
        private static ConnectionController instance;

        /// <summary>
        /// Private  singleton constructor
        /// </summary>
        private ConnectionController()
        {
            ConStringBuilder();
            SqlCon = new SqlConnection(ConString);
        }

        /// <summary>
        /// Method to create a connection string
        /// </summary>
        public void ConStringBuilder()
        {
            SqlConnectionStringBuilder SqlStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = "",
                UserID = "",
                Password = "",
                InitialCatalog = ""
            };

            ConString = SqlStringBuilder.ConnectionString;
        }

        /// <summary>
        /// Method used to open the database connection
        /// </summary>
        public void OpenSqlConnection()
        {
            try
            {
                if (SqlCon.State == ConnectionState.Closed)
                {
                    SqlCon.Open();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Method used to close the database connection
        /// </summary>
        public void CloseSqlConnection()
        {
            SqlCon.Close();
        }
    }

}
