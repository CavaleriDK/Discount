using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Discount.Models;
using Discount.Shopgun;

namespace Discount
{
    /// <summary>
    /// The controller used to communicate with the Shopgun API
    /// </summary>
    class ShopgunController
    {
        /// <summary>
        /// Singleton instance of this class
        /// </summary>
        public static ShopgunController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ShopgunController();
                }
                return instance;
            }
        }

        /// <summary>
        /// Determines if the connection was initialized and a session was created
        /// </summary>
        public bool Initialized { get => initialized; }

        private string apiSecret = "00jvpeafrit67mp2ngoes3ks3ovgkge4";
        private string apiKey = "00jvpeafrisdcmlqalvwy7vnyfn8z800";
        private string rootPath = "https://api.etilbudsavis.dk";
        private string sessionsPath = "/v2/sessions";
        private string offersListPath = "/v2/offers";
        private string dealerPath = "/v2/dealers";

        private int apiResultLimit = 24;

        private string tempLidlShopgunID = "71c90";
        private string tempFaktaShopgunID = "101cD";
        private string tempKvicklyShopgunID = "c1edq";

        private List<string> tempListOfDealerIDs;

        private static ShopgunController instance;
        private bool initialized = false;

        /// <summary>
        /// Private singleton constructor for this class
        /// </summary>
        private ShopgunController()
        {
            tempListOfDealerIDs = new List<string>()
            {
                tempLidlShopgunID,
                tempFaktaShopgunID,
                tempKvicklyShopgunID
            };

            // Skal udkommenteres og kaldes et andet sted?
            Initialize();
        }

        /// <summary>
        /// Method used to initialize connection with the Shopgun API. 
        /// Makes sure a session is created and a Token for future calls is prepared.
        /// </summary>
        public void Initialize()
        {
            // Make sure that the API is only initialized once
            if (!initialized)
            {
                // Call the API and get a token
                CreateSessionModel sessionResponse = CreateSessionToken();

                // Gracefully exit method if the API call failed
                if (sessionResponse == default(CreateSessionModel))
                {
                    initialized = false;
                    return;
                }

                // Set the token and signature for the APIConsumer class to be used application wide
                APIConsumer.XToken = sessionResponse.Token;
                APIConsumer.XSignature = SignSession(sessionResponse.Token);

                initialized = true;
            }
        }

        /// <summary>
        /// Method used to start populating the database with Dealers and their offers
        /// </summary>
        public void PopulateDatabaseWithListOfDealers()
        {
            if (initialized)
            {
                // Start recursive thread stuff
                Thread t = new Thread(() => GetNextDealer(tempListOfDealerIDs[0]));
                t.IsBackground = true;
                t.Start();
            }
            else
            {
                MessageBox.Show("No connection was initialized. Please try again later.", "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Method used to call the API and create a session token
        /// </summary>
        /// <returns>A JSON Session Model response</returns>
        private CreateSessionModel CreateSessionToken()
        {
            // https://docs.microsoft.com/en-us/dotnet/framework/network-programming/how-to-send-data-using-the-webrequest-class

            // Bør nok laves til en Request klasse, og API Consumer metoden nedenfor bør kunne tage imod genereiske request beskeder
            string jsonBody = "{\"api_key\": \"" + apiKey + "\"}";
            string fullPath = rootPath + sessionsPath;

            CreateSessionModel sessionResponse = APIConsumer.SendJSONPostRequest<CreateSessionModel>(fullPath, jsonBody);

            return sessionResponse;
        }

        /// <summary>
        /// Method used to sign the token with the secret and create a SHA256 hashed value from these
        /// </summary>
        /// <param name="sessionToken">The session token provided by Shopgun API</param>
        /// <returns>SHA256 value of token + api secret</returns>
        private string SignSession(string sessionToken)
        {
            string t = sessionToken + apiSecret;

            return Utilities.CalculateSHA256Hash(t);
        }

        /// <summary>
        /// Method used to get each page of paginated offers results from the Shopgun API
        /// </summary>
        /// <param name="o">The offset to find results from</param>
        /// <param name="d_id">The Shopgun Dealer ID to find offers from</param>
        /// <param name="last_shop">Boolean whether this is the final shop in the list or not</param>
        private void GetNextPageOfOffers(object o, object d_id, object s_id, object last_shop)
        {
            int offset = (int)o;
            string dealerID = (string)d_id;
            int shopID = (int)s_id;
            bool lastShop = (bool)last_shop;

            string query = "?dealer_ids=" + dealerID + "&limit=" + apiResultLimit + "&offset=" + offset;
            string fullPath = rootPath + offersListPath + query;

            ShopgunOfferModel[] shopgunOfferModels;

            try
            {
                // Call the API with the new request
                shopgunOfferModels = APIConsumer.SendJSONGetRequest<ShopgunOfferModel[]>(fullPath);
            }
            catch (Exception e)
            {
                // Her kan den måske prøve at kalde sig selv igen?
                // Så skal der sættes en form for timeout på - 3 strikes out!
                // Indtil videre kommer der bare fejlbesked.
                MessageBox.Show("Error: " + e.Message, "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (shopgunOfferModels.Length > 0)
            {
                // Recursively call this method in a new thread if the latest call returned any results
                Thread t = new Thread(() => GetNextPageOfOffers(offset + apiResultLimit, dealerID, shopID, lastShop));
                t.IsBackground = true;
                t.Start();

                // Do stuff with the offer models here... ... ..
                Console.WriteLine("Returned " + shopgunOfferModels.Length + " results!");

                // Create list of all IDs retrieved
                List<string> ids = new List<string>();
                foreach (ShopgunOfferModel model in shopgunOfferModels)
                {
                    ids.Add(model.ID);
                }

                // Get all models in our Database with corresponding offer IDs
                List<OfferModel> existingOffers = OfferModel.FindAllExistingWithOfferIds(ids);

                // Iterate over all results
                for (int i = 0; i < shopgunOfferModels.Length; i++)
                {
                    // Cache the iterated model because closures are shite
                    ShopgunOfferModel thisShopgunModel = shopgunOfferModels[i];

                    // Check if the offer already exists
                    OfferModel found = existingOffers.FirstOrDefault((offer) => 
                    {
                        if (offer.ShopgunOfferID == thisShopgunModel.ID)
                        {
                            return true;
                        }
                        return false;
                    });

                    // Insert into database if it did not exist
                    if (found == null)
                    {
                        DateTime offerFrom = DateTime.Parse(thisShopgunModel.RunFrom);
                        DateTime offerTo = DateTime.Parse(thisShopgunModel.RunTo);
                        int prePrice = (int)Math.Floor((thisShopgunModel.Pricing.Pre != null) ? Convert.ToDouble(thisShopgunModel.Pricing.Pre) * 100 : 0);
                        int nowPrice = (int)Math.Floor(thisShopgunModel.Pricing.Price * 100);

                        // Temporarily include Shop ID = 0 until shops are integrated properly in DB
                        OfferModel.New(thisShopgunModel.Heading, prePrice, nowPrice, thisShopgunModel.ID, dealerID, thisShopgunModel.Images.Thumb, shopID, offerFrom, offerTo);
                    }
                }
            }
            else if (lastShop)
            {
                // The endpoint returned nothing, and we have iterated over all offers from this dealer
                Console.WriteLine("FINISHED!");

                // Call the main thread and invoke method to hide loading overlay
                MainWindow.StaticDispatcher.Invoke(() =>
                {
                    MainWindow.HideOverlay();
                });
            }
        }

        /// <summary>
        /// Method used to find a dealer from Shopgun API endpoint be their dealer ID
        /// </summary>
        /// <param name="d_id">The Dealer ID to lookup</param>
        private void GetNextDealer(object d_id)
        {
            string dealerID = (string)d_id;

            string query = "/" + dealerID;
            string fullPath = rootPath + dealerPath + query;

            ShopgunShopModel shopgunShopModel;

            try
            {
                // Call the API with the new request
                shopgunShopModel = APIConsumer.SendJSONGetRequest<ShopgunShopModel>(fullPath);
            }
            catch (Exception e)
            {
                // Indtil videre kommer der bare fejlbesked.
                MessageBox.Show("Error: " + e.Message, "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // If the API endpoint returned a valid result
            if (shopgunShopModel != default(ShopgunShopModel))
            {
                // Get index of next shop in list
                int nextIndex = tempListOfDealerIDs.IndexOf(dealerID) + 1;
                bool lastShop = nextIndex == tempListOfDealerIDs.Count;

                // Check if there are more items in the list
                if (nextIndex < tempListOfDealerIDs.Count)
                {
                    // Start recursive thread stuff
                    Thread st = new Thread(() => GetNextDealer(tempListOfDealerIDs[nextIndex]));
                    st.IsBackground = true;
                    st.Start();
                }

                // Check if the shop exists already
                ShopModel model = ShopModel.FindByShopgunDealerID(shopgunShopModel.ID);

                // Create new shop if it did not exist
                if (model == default(ShopModel))
                {
                    model = ShopModel.New(shopgunShopModel.Name, shopgunShopModel.ID);
                }

                // Spawn population offers start thread stuff
                Thread pt = new Thread(() => GetNextPageOfOffers(0, shopgunShopModel.ID, model.ID, lastShop));
                pt.IsBackground = true;
                pt.Start();
            }
        }
    }
}
