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
using System.Data.Linq;
using System.Data.Objects;
using System.Configuration;
using POS.APP_Data;
using System.Globalization;

namespace POS
{
    public static class API_GET
    {
        #region Variables

        public static apiCustomerList Master_CustomerItem { get; set; }
        public static string CustomersResponseJson { get; set; }
        public static string CustomerResponseMessage { get; set; }
        public static bool NewCustomerSaveSuccess { get; set; } = false;
        public static bool UpdateCustomerSuccess { get; set; }
        static List<APP_Data.Customer> CustomerList { get; set; }

        public static HttpResponseMessage response { get; set; }
        public static string AccessToken { get; set; }

        #endregion

        #region Models

        public class apiCustomerList
        {
            public string code { get; set; }
            public string status { get; set; }
            public string message { get; set; }
            public List<apiCustomer> data { get; set; }
        }
        public class apiCustomer
        {
            public int id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string date_of_birth { get; set; }
            public string gender { get; set; }
        }

        #endregion

        #region Methods

        public static void CheckNewCustomer(apiCustomerList InCustomerdata)
        {
            POSEntities entity = new POSEntities();
            CustomerList = entity.Customers.ToList();
            //List<string> Phones = entity.Customers.Select(c => c.PhoneNumber).ToList()
            List<string> Phones = CustomerList.Select(c => c.PhoneNumber).ToList();
            List<string> IndataPhones = InCustomerdata.data.Where(c => !string.IsNullOrEmpty(c.phone)).Select(c => c.phone).Distinct().ToList();
            List<string> NewPhones = IndataPhones.Except(Phones).ToList();
            NewCustomerSaveSuccess = true;
            if (NewPhones.Count > 0)
            {
                try
                {
                    using (entity = new POSEntities())
                    {
                        foreach (string Phone in NewPhones)
                        {
                            APP_Data.Customer NewCustomer = new APP_Data.Customer();
                            string apiDOB = "";
                            NewCustomer.PhoneNumber = Phone;
                            NewCustomer.Name = InCustomerdata.data.Where(c => c.phone == Phone).Select(c => c.name).FirstOrDefault();
                            NewCustomer.Email = InCustomerdata.data.Where(c => c.phone == Phone).Select(c => c.email).FirstOrDefault();
                            apiDOB = InCustomerdata.data.Where(c => c.phone == Phone).Select(c => c.date_of_birth).FirstOrDefault();
                            if (apiDOB != "" && apiDOB != null)
                            {
                                NewCustomer.Birthday = DateTime.ParseExact(apiDOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                NewCustomer.Birthday = null;
                            }

                            NewCustomer.Gender = InCustomerdata.data.Where(c => c.phone == Phone).Select(c => c.gender).FirstOrDefault();

                            NewCustomer.Title = "Mr";
                            NewCustomer.Address = "";
                            NewCustomer.NRC = "";
                            NewCustomer.CityId = 1;
                            NewCustomer.TownShip = "";
                            NewCustomer.MemberTypeID = null;
                            NewCustomer.VIPMemberId = "";
                            NewCustomer.StartDate = null;
                            NewCustomer.CustomerCode = "";
                            NewCustomer.CustomerTypeId = 1;
                            NewCustomer.PromoteDate = null;
                            NewCustomer.Maritalstatus = "Single";
                            NewCustomer.EmergencyContactPhone = "";
                            NewCustomer.EmergencyContactName = "";
                            NewCustomer.Relationship = "";
                            NewCustomer.MainConcern = "";
                            NewCustomer.MedicalHistory = "";
                            NewCustomer.DrugAllergy = "";
                            NewCustomer.ProfilePath = string.Empty;
                            NewCustomer.Remark = null;
                            NewCustomer.ReferredID = null;
                            NewCustomer.IsExported = true;

                            entity.Customers.Add(NewCustomer);
                        }
                        entity.SaveChanges();//check later
                    }                    
                }
                catch (Exception ex)
                {
                    NewCustomerSaveSuccess = false;
                    CustomerResponseMessage = ex.ToString();
                }
            }
        }

        #endregion

        #region API  

        public static void GET_CustomerData()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpClient restClient = new HttpClient();

            string Content_Type = "application/json";
            restClient.DefaultRequestHeaders.Accept.Clear();
            restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
            restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);

            string apiServer = ConfigurationManager.AppSettings["APIServer"];
            var Builder = new UriBuilder($"{apiServer}/api/pos/users");

            try
            {
                response = new HttpResponseMessage();
                response = restClient.GetAsync(Builder.Uri).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    CustomersResponseJson = result;
                    Master_CustomerItem = new apiCustomerList();
                    Master_CustomerItem = JsonConvert.DeserializeObject<apiCustomerList>(result);

                    CustomerResponseMessage = response.ReasonPhrase;
                }
            }
            catch (Exception ex)
            {
                CustomerList = null;
                CustomerResponseMessage = ex.ToString();//response.ReasonPhrase;  
                // MessageBox.Show(ex.Message, "Uom Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
