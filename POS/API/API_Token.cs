using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Data;
using POS.APP_Data;
using System.Configuration;
using POS;
using static POS.API_POST;

namespace POS
{
    class API_Token
    {
        #region Variables               
        static APICredential credential { get; set; }
        public static string AccessToken { get; set; }
        public static string LoginResponseJson { get; set; }
        public static HttpResponseMessage tokenResponse { get; set; }
        public static Post_Login login { get; set; }
        #endregion

        #region Methods        
        public static void Get_AccessToken()
        {
            if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrWhiteSpace(AccessToken))
            {
                POSEntities entity = new POSEntities();
                credential = entity.APICredentials.FirstOrDefault();
                AccessToken = credential.AccessToken;
                if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrWhiteSpace(AccessToken))
                {
                    Get_AccessTokenFromAPPAsync();
                    if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrWhiteSpace(AccessToken))
                    {
                        if (API_Token.tokenResponse.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            MessageBox.Show("Invalid Login Information", "Access Token", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("No Access Token!", "Access Token", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                    else
                    {
                        using (entity = new POSEntities())
                        {

                            if (credential != null)
                            {
                                credential.AccessToken = AccessToken;
                                entity.Entry(credential).State = EntityState.Modified;
                                entity.SaveChanges();

                            }

                        }

                    }
                }
            }


        }
        public static void Get_AccessTokenFromAPPAsync()
        {
            POSEntities entity = new POSEntities();
            credential = entity.APICredentials.FirstOrDefault();
            AccessToken = credential.AccessToken;

            HttpClient restClient = new HttpClient();
            string Content_Type = "application/json";
            restClient.DefaultRequestHeaders.Accept.Clear();
            restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
            restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);

            string apiServer = ConfigurationManager.AppSettings["APIServer"];
            var Builder = new UriBuilder($"{apiServer}/api/pos/login");

            var formData = new Dictionary<string, string>
                            {
                                { "branch_code","J0001" },
                                { "hash_value", "2ee6f6672a28c24c3e98790f469a0bab9c6c0bde9c1fd515fa46d291feaf9ecc" }
                            };

            // Convert form data to FormUrlEncodedContent
            var formContent = new FormUrlEncodedContent(formData);

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                tokenResponse = new HttpResponseMessage();
                tokenResponse = restClient.PostAsync(Builder.Uri, formContent).Result;

                if (tokenResponse.IsSuccessStatusCode)
                {
                    var result = tokenResponse.Content.ReadAsStringAsync().Result;
                    LoginResponseJson = result;
                    login = new Post_Login();
                    login = JsonConvert.DeserializeObject<Post_Login>(result);
                    AccessToken = login.Data.Access_Token;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Token Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

    }
}
