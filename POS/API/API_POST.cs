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

namespace POS
{
    public static class API_POST
    {
        #region Variables 

        static HttpClient restClient = new HttpClient();
        public static HttpResponseMessage response { get; set; }

        public static string postCustomerJson { get; set; }
        public static bool postCustomerSuccess { get; set; } = false;
        public static List<string> postCustomerResponseMessage;
        public static string ResponseCustomerJson { get; set; }

        public static string postSharerJson { get; set; }
        public static bool postSharerSuccess { get; set; } = false;
        public static List<string> postSharerResponseMessage;
        public static string ResponseSharerJson { get; set; }

        public static string postPackageUsedJson { get; set; }
        public static bool postPackageUsedSuccess { get; set; } = false;
        public static List<string> postPackageUsedResponseMessage;
        public static string ResponsePackageUsedJson { get; set; }

        public static string postNotifyJson { get; set; }
        public static bool postNotifySuccess { get; set; } = false;
        public static List<string> postNotifyResponseMessage;
        public static string ResponseNotifyJson { get; set; }

        public static string postCancelCreditTransJson { get; set; }
        public static bool postCancelCreditTransSuccess { get; set; } = false;
        public static List<string> postCancelCreditTransResponseMessage;
        public static string ResponseCancelCreditTransJson { get; set; }

        public static string QRResponseJson { get; set; }

        public static string postTransJson { get; set; }
        public static bool postTransSuccess { get; set; } = false;
        public static List<string> postTransResponseMessage;
        public static string ResponseTransJson { get; set; }

        public static string postRedeemJson { get; set; }
        public static bool postRedeemSuccess { get; set; } = false;
        public static List<string> postRedeemResponseMessage;
        public static string ResponseRedeemJson { get; set; }

        public static string postPackageJson { get; set; }
        public static bool postPackageSuccess { get; set; } = false;
        public static List<string> postPackageResponseMessage;
        public static string ResponsePackageJson { get; set; }

        public static string postSharerDeleteJson { get; set; }
        public static bool postSharerDeleteSuccess { get; set; } = false;
        public static List<string> postSharerDeleteResponseMessage;
        public static string SharerDeleteJson { get; set; }

        public static int Point { get; set; } = 0;

        public static List<Product> productList = new List<Product>();

        #endregion

        #region Models

        public class Post_Login
        {
            public string Code { get; set; }
            public string Status { get; set; }
            public string Message { get; set; }
            public Data Data { get; set; }
        }

        public class Data
        {
            public string Access_Token { get; set; }
        }

        public class POST_posCustomerList
        {
            public List<POST_posCustomer> data { get; set; }
        }

        public class POST_posCustomer
        {
            public string name { get; set; }
            public string phone_no { get; set; }
            public string date_of_birth { get; set; }
            public string city_id { get; set; }
            public string emergency_contact_phone { get; set; }
            public string emergency_contact_name { get; set; }
        }

        public class POST_CustomerResponse
        {
            public string code { get; set; }
            public string status { get; set; }
            public string message { get; set; }
        }

        public class POST_posSharerList
        {
            public List<POST_posSharer> data { get; set; }
        }

        public class POST_posSharer
        {
            public string share_phone_no { get; set; }
            public string transaction_id { get; set; }
            public string pos_treatment_id { get; set; }
        }

        public class POST_SharerResponse
        {
            public string code { get; set; }
            public string status { get; set; }
            public string message { get; set; }
        }

        public class POST_posPackageUsedList
        {
            public List<POST_posPackageUsed> data { get; set; }
        }

        public class POST_posPackageUsed
        {
            public string phone_no { get; set; }
            public string pos_treatment_id { get; set; }
            public string transaction_id { get; set; }
            public string used_qty { get; set; }
        }

        public class POST_PackageUsedResponse
        {
            public string code { get; set; }
            public string status { get; set; }
            public string message { get; set; }
        }

        public class POST_SharerDeleteResponse
        {
            public string code { get; set; }
            public string status { get; set; }
            public string message { get; set; }
        }

        public class POST_posNotify
        {
        }


        public class POST_NotifyResponse
        {
            public string code { get; set; }
            public string status { get; set; }
            public string message { get; set; }
        }

        public class POST_posCancelCreditTransList
        {
            public List<POST_posCancelCreditTrans> data { get; set; }
        }

        public class POST_posCancelCreditTrans
        {
            public List<int> pos_treatment_id { get; set; }
            public string transaction_id { get; set; }
            public string phone_no { get; set; }
        }

        public class POST_CancelCreditTransResponse
        {
            public string code { get; set; }
            public string status { get; set; }
            public string message { get; set; }
        }

        public class BuyPackages
        {
            public List<Package> buy_packages { get; set; }
        }

        public class Package
        {
            public string phone_no { get; set; }

            public long pos_treatment_id { get; set; }

            public string pos_treatment_name { get; set; }

            public int? total_qty { get; set; }

            public string transaction_id { get; set; }
        }

        public class PostTrans
        {
            public string pos_transaction_id { get; set; }

            public string spend_reward_amount { get; set; }

            public string phone_no { get; set; }
        }

        public class PostRedeem
        {
            public string pos_transaction_id { get; set; }

            public string redeem_point { get; set; }

            public string phone_no { get; set; }
        }

        public class PostTransResponse
        {
            public string message { get; set; }
            public string status { get; set; }
        }

        public class Post_QR
        {
            public string Code { get; set; }

            public string Status { get; set; }

            public string Message { get; set; }

            public QR_Data Data { get; set; }
        }

        public class QR_Data
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public decimal J_Coin { get; set; }

            public int Redeem_Amount { get; set; }

            public string Email { get; set; }

            public string Phone { get; set; }

            public string Date_Of_Birth { get; set; }

            public string Gender { get; set; }
        }

        public class PostSharerDelete
        {
            public string share_phone_no { get; set; }
            public string transaction_id { get; set; }
            public string pos_treatment_id { get; set; }
        }

        public class PostRedeemResponse
        {
            public string message { get; set; }
            public string status { get; set; }
        }

        public class PostPackageResponse
        {
            public string message { get; set; }
            public string status { get; set; }
        }

        #endregion

        #region Methods

        public static POST_posCustomer GetPostCustomerData(int customerId)
        {
            POSEntities entity = new POSEntities();
            List<string> customerTypes = new List<string> { "Patient" };
            POST_posCustomer posCustomer = new POST_posCustomer();

            foreach (string cType in customerTypes)
            {
                int custType = customerTypes.FindIndex(x => x.Contains(cType));
                List<Customer> customerList = new List<Customer>();
                customerList = entity.Customers.Where(x => x.Id == customerId && x.CustomerTypeId == 1).ToList();

                if (customerList.Count > 0)
                {
                    foreach (Customer c in customerList)
                    {
                        posCustomer.name = c.Name;
                        posCustomer.phone_no = c.PhoneNumber;
                        posCustomer.date_of_birth = String.Format("{0:yyyy-MM-dd}", c.Birthday);
                        posCustomer.city_id = c.CityId.ToString();
                        posCustomer.emergency_contact_phone = c.EmergencyContactPhone;
                        posCustomer.emergency_contact_name = c.EmergencyContactName;
                    }
                }
                else
                {
                    break;
                }
            }

            return posCustomer;
        }

        public static POST_posSharer GetPostSharerData(int purchasedPackageSharersId)
        {
            POSEntities entity = new POSEntities();
            POST_posSharer posSharer = new POST_posSharer();

            var sharerList = (from sharer in entity.PurchasedPackageSharers
                              join cust in entity.Customers on sharer.SharedCustomerId equals cust.Id
                              join package in entity.PackagePurchasedInvoices on sharer.PackageInvoiceId equals package.PackagePurchasedInvoiceId
                              join transdetail in entity.TransactionDetails on package.TransactionDetailId equals transdetail.Id
                              where sharer.Id == purchasedPackageSharersId && sharer.IsDeleted == false
                              select new
                              {
                                  cust.PhoneNumber,
                                  transdetail.TransactionId,
                                  transdetail.ProductId
                              }).FirstOrDefault();

            if (!string.IsNullOrEmpty(sharerList.TransactionId))
            {
                posSharer.share_phone_no = sharerList.PhoneNumber;
                posSharer.transaction_id = sharerList.TransactionId;
                posSharer.pos_treatment_id = sharerList.ProductId.ToString();
            }

            return posSharer;
        }

        public static POST_posPackageUsed GetPostPackageUsedData(string packageUsedId)
        {
            POSEntities entity = new POSEntities();
            POST_posPackageUsed posPackageUsed = new POST_posPackageUsed();

            var PackageUsedList = (from used in entity.PackageUsedHistories
                                   join cust in entity.Customers on used.ActualOffsetBy equals cust.Id
                                   join package in entity.PackagePurchasedInvoices on used.PackagePurchasedInvoiceId equals package.PackagePurchasedInvoiceId
                                   join transdetail in entity.TransactionDetails on package.TransactionDetailId equals transdetail.Id
                                   where used.PackageUsedHistoryId == packageUsedId && used.IsDelete == false
                                   select new
                                   {
                                       cust.PhoneNumber,
                                       transdetail.ProductId,
                                       transdetail.TransactionId,
                                       used.UseQty
                                   }).FirstOrDefault();

            if (!string.IsNullOrEmpty(PackageUsedList.TransactionId))
            {
                posPackageUsed.phone_no = PackageUsedList.PhoneNumber;
                posPackageUsed.pos_treatment_id = PackageUsedList.ProductId.ToString();
                posPackageUsed.transaction_id = PackageUsedList.TransactionId;
                posPackageUsed.used_qty = PackageUsedList.UseQty.ToString();
            }

            return posPackageUsed;
        }

        public static POST_posCancelCreditTrans GetPostCancelCreditTransData(string cancelCreditTransId)
        {
            POSEntities entity = new POSEntities();
            List<int> treatmentList = new List<int>();
            POST_posCancelCreditTrans posCancelCreditTrans = new POST_posCancelCreditTrans();

            var CancelCreditTransList = (from ppi in entity.PackagePurchasedInvoices
                                         join cust in entity.Customers on ppi.CustomerId equals cust.Id
                                         join transdetail in entity.TransactionDetails on ppi.TransactionDetailId equals transdetail.Id
                                         where ppi.PackagePurchasedInvoiceId == cancelCreditTransId && ppi.IsDelete == false && ppi.IsCancelled==true
                                         select new
                                         {
                                             transdetail.ProductId,
                                             transdetail.TransactionId,
                                             cust.PhoneNumber
                                         }).FirstOrDefault();

            if (!string.IsNullOrEmpty(CancelCreditTransList.TransactionId))
            {
                treatmentList.Add(Convert.ToInt32(CancelCreditTransList.ProductId));
                posCancelCreditTrans.pos_treatment_id = treatmentList;
                posCancelCreditTrans.transaction_id = CancelCreditTransList.TransactionId;
                posCancelCreditTrans.phone_no = CancelCreditTransList.PhoneNumber;
            }

            return posCancelCreditTrans;
        }

        public static POST_posCancelCreditTrans GetPostDeleteTransData(string transId)
        {
            POSEntities entity = new POSEntities();
            List<int> treatmentList = new List<int>();
            POST_posCancelCreditTrans posCancelCreditTrans = new POST_posCancelCreditTrans();

            var DeleteTransList = (from t in entity.Transactions
                                         join c in entity.Customers on t.CustomerId equals c.Id
                                         join td in entity.TransactionDetails on t.Id equals td.TransactionId
                                         where t.Id == transId
                                         select new
                                         {
                                             td.ProductId,
                                             t.Id,
                                             c.PhoneNumber
                                         }).ToList();
            foreach(var tran in DeleteTransList)
            {
                treatmentList.Add(Convert.ToInt32(tran.ProductId));
                posCancelCreditTrans.pos_treatment_id = treatmentList;
                posCancelCreditTrans.transaction_id = tran.Id;
                posCancelCreditTrans.phone_no = tran.PhoneNumber;
            }
            return posCancelCreditTrans;
        }

        public static PostTrans GetPostTransData(string transId)
        {
            POSEntities entity = new POSEntities();
            PostTrans postTrans = new PostTrans();
            var trans = (from T in entity.Transactions
                         join C in entity.Customers on T.CustomerId equals C.Id
                         where T.Id == transId
                         select new
                         {
                             T.Id,
                             T.RecieveAmount,
                             C.PhoneNumber
                         }).FirstOrDefault();

            if (!string.IsNullOrEmpty(trans.Id))
            {
                postTrans.pos_transaction_id = trans.Id;
                postTrans.spend_reward_amount = trans.RecieveAmount.ToString();
                postTrans.phone_no = trans.PhoneNumber;
            }
            return postTrans;
        }

        public static PostRedeem GetPostRedeemData(string transId)
        {
            POSEntities entity = new POSEntities();
            PostRedeem postRedeem = new PostRedeem();
            var trans = (from T in entity.Transactions
                         join C in entity.Customers on T.CustomerId equals C.Id
                         where T.Id == transId
                         select new
                         {
                             T.Id,
                             T.UsedPoints,
                             C.PhoneNumber
                         }).FirstOrDefault();

            if (!string.IsNullOrEmpty(trans.Id))
            {
                postRedeem.pos_transaction_id = trans.Id;
                postRedeem.redeem_point = trans.UsedPoints.ToString();
                postRedeem.phone_no = trans.PhoneNumber;
            }
            return postRedeem;
        }

        public static BuyPackages GetPackageData(string transId)
        {
            POSEntities entity = new POSEntities();
            BuyPackages buyPackages = new BuyPackages();
            List<Package> packageList = new List<Package>();
            Package package;

            var detail = (from p in entity.Products
                          join td in entity.TransactionDetails on p.Id equals td.ProductId
                          join t in entity.Transactions on td.TransactionId equals t.Id
                          join c in entity.Customers on t.CustomerId equals c.Id
                          where t.Id == transId && p.IsPackage == true
                          select new
                          {
                              c.PhoneNumber,
                              p.Id,
                              p.Name,
                              p.PackageQty,
                              TransactionId = t.Id,
                              td.Qty
                          }).ToList();

            if (detail.Count > 0)
            {
                foreach (var item in detail)
                {
                    package = new Package();
                    package.phone_no = item.PhoneNumber;
                    package.pos_treatment_id = item.Id;
                    package.pos_treatment_name = item.Name;
                    package.total_qty = item.PackageQty * item.Qty;
                    package.transaction_id = item.TransactionId;
                    packageList.Add(package);
                }
            }
            if (packageList.Count > 0)
            {
                buyPackages.buy_packages = packageList;
            }
            else
            {
                buyPackages = null;
            }

            return buyPackages;
        }

        public static PostSharerDelete GetPostPackageSharerDelete(int packageUsedId)
        {
            POSEntities entity = new POSEntities();
            PostSharerDelete sharerDelete = new PostSharerDelete();

            var PackageSharerDeleteList = (from PS in entity.PurchasedPackageSharers
                                   join PI in entity.PackagePurchasedInvoices on PS.PackageInvoiceId equals PI.PackagePurchasedInvoiceId
                                   join C in entity.Customers on PS.SharedCustomerId equals C.Id 
                                   join TD in entity.TransactionDetails on  PI.TransactionDetailId equals TD.Id 
                                   where PS.Id == packageUsedId 
                                   select new
                                   {
                                       C.PhoneNumber,
                                       TD.TransactionId,
                                       TD.ProductId
                                   }).FirstOrDefault();

            if (!string.IsNullOrEmpty(PackageSharerDeleteList.TransactionId))
            {
                sharerDelete.share_phone_no = PackageSharerDeleteList.PhoneNumber;
                sharerDelete.pos_treatment_id = PackageSharerDeleteList.ProductId.ToString();
                sharerDelete.transaction_id = PackageSharerDeleteList.TransactionId;
            }

            return sharerDelete;
        }

        #endregion

        #region API

        public static void POST_Customers(int customerId)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                restClient = new HttpClient();
                postCustomerResponseMessage = new List<string>();
                string Content_Type = "application/json";
                restClient.DefaultRequestHeaders.Accept.Clear();
                restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
                restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);
                POST_posCustomer ContentData = GetPostCustomerData(customerId);
                if (ContentData == null)
                {
                    postCustomerSuccess = true;
                    postCustomerResponseMessage.Add("No Data to Export");
                    return;
                }

                var PostContent_Json = JsonConvert.SerializeObject(ContentData);
                postCustomerJson = PostContent_Json;
                string apiUri = ConfigurationManager.AppSettings["APIServer"];
                var Builder = new UriBuilder($"{apiUri}/api/pos/users");
                HttpContent Content = new StringContent(PostContent_Json, Encoding.UTF8, Content_Type);

                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, Content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ResponseCustomerJson = result;
                    POST_CustomerResponse responseContent = JsonConvert.DeserializeObject<POST_CustomerResponse>(result);

                    if (responseContent != null)
                    {
                        if (!string.IsNullOrEmpty(responseContent.status) && responseContent.status.ToLower() == "success")
                        {
                            responseContent.status = "Success";
                        }

                        postCustomerSuccess = responseContent.status == "Success" ? true : false;
                        postCustomerResponseMessage.Add("OK");
                        postCustomerResponseMessage.Add(responseContent.status);
                    }
                    else
                    {
                        postCustomerSuccess = false;
                        postCustomerResponseMessage.Add("OK");
                        postCustomerResponseMessage.Add("Unrecongnized Response Format");
                    }
                }
                else
                {
                    postCustomerSuccess = false;
                    postCustomerResponseMessage.Add(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                postCustomerSuccess = false;
                postCustomerResponseMessage.Add(ex.ToString());
            }
        }

        public static void POST_Sharers(int sharerId)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                restClient = new HttpClient();
                postSharerResponseMessage = new List<string>();
                string Content_Type = "application/json";
                restClient.DefaultRequestHeaders.Accept.Clear();
                restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
                restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);
                POST_posSharer ContentData = GetPostSharerData(sharerId);
                if (ContentData == null)
                {
                    postSharerSuccess = true;
                    postSharerResponseMessage.Add("No Data to Export");
                    return;
                }

                var PostContent_Json = JsonConvert.SerializeObject(ContentData);
                postSharerJson = PostContent_Json;
                string apiUri = ConfigurationManager.AppSettings["APIServer"];
                var Builder = new UriBuilder($"{apiUri}/api/pos/packages/share");
                HttpContent Content = new StringContent(PostContent_Json, Encoding.UTF8, Content_Type);

                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, Content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ResponseSharerJson = result;
                    POST_SharerResponse responseContent = JsonConvert.DeserializeObject<POST_SharerResponse>(result);

                    if (responseContent != null)
                    {
                        if (!string.IsNullOrEmpty(responseContent.status) && responseContent.status.ToLower() == "success")
                        {
                            responseContent.status = "Success";
                        }

                        postSharerSuccess = responseContent.status == "Success" ? true : false;
                        postSharerResponseMessage.Add("OK");
                        postSharerResponseMessage.Add(responseContent.status);
                    }
                    else
                    {
                        postSharerSuccess = false;
                        postSharerResponseMessage.Add("OK");
                        postSharerResponseMessage.Add("Unrecongnized Response Format");
                    }
                }
                else
                {
                    postSharerSuccess = false;
                    postSharerResponseMessage.Add(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                postSharerSuccess = false;
                postSharerResponseMessage.Add(ex.ToString());

            }
        }

        public static void POST_PackageSharerDelete(int packageSharerId)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                restClient = new HttpClient();
                postSharerDeleteResponseMessage = new List<string>();
                string Content_Type = "application/json";
                restClient.DefaultRequestHeaders.Accept.Clear();
                restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
                restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);
                PostSharerDelete ContentData = GetPostPackageSharerDelete(packageSharerId);
                if (ContentData == null)
                {
                    postSharerDeleteSuccess = true;
                    postSharerDeleteResponseMessage.Add("No Data to Export");
                    return;
                }

                var PostContent_Json = JsonConvert.SerializeObject(ContentData);
                postCustomerJson = PostContent_Json;
                string apiUri = ConfigurationManager.AppSettings["APIServer"];
                var Builder = new UriBuilder($"{apiUri}/api/pos/packages/share/cancel");
                HttpContent Content = new StringContent(PostContent_Json, Encoding.UTF8, Content_Type);

                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, Content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    SharerDeleteJson = result;
                    POST_SharerDeleteResponse responseContent = JsonConvert.DeserializeObject<POST_SharerDeleteResponse>(result);

                    if (responseContent != null)
                    {
                        postSharerDeleteSuccess = responseContent.status == "failed" ? false : true;
                        postSharerDeleteResponseMessage.Add("OK");
                        postSharerDeleteResponseMessage.Add(responseContent.message);
                    }
                    else
                    {
                        postSharerDeleteSuccess = false;
                        postSharerDeleteResponseMessage.Add("OK");
                        postSharerDeleteResponseMessage.Add("Unrecongnized Response Format");
                    }
                }
                else
                {
                    postSharerDeleteSuccess = false;
                    postSharerDeleteResponseMessage.Add(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                postSharerDeleteSuccess = false;
                postSharerDeleteResponseMessage.Add(ex.ToString());
            }
        }

        public static void POST_PackageUsed(string packageUsedId)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                restClient = new HttpClient();
                postPackageUsedResponseMessage = new List<string>();
                //string Content_Type = "application/json";
                string Content_Type = "multipart/form-data";
                restClient.DefaultRequestHeaders.Accept.Clear();
                restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
                restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);
                POST_posPackageUsed ContentData = GetPostPackageUsedData(packageUsedId);
                if (ContentData == null)
                {
                    postPackageUsedSuccess = true;
                    postPackageUsedResponseMessage.Add("No Data to Export");
                    return;
                }

                var formData = new Dictionary<string, string>
                            {
                                { "phone_no", ContentData.phone_no },
                                { "pos_treatment_id", ContentData.pos_treatment_id },
                                {"transaction_id", ContentData.transaction_id },
                                {"used_qty", ContentData.used_qty }
                            };

                // Convert form data to FormUrlEncodedContent
                var formContent = new FormUrlEncodedContent(formData);

                var PostContent_Json = JsonConvert.SerializeObject(ContentData);
                postPackageUsedJson = PostContent_Json;

                string apiUri = ConfigurationManager.AppSettings["APIServer"];
                var Builder = new UriBuilder($"{apiUri}/api/pos/packages/use");

                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, formContent).Result;

                //string apiUri = ConfigurationManager.AppSettings["APIServer"];
                //var Builder = new UriBuilder($"{apiUri}/api/pos/packages/use");
                //HttpContent Content = new StringContent(PostContent_Json, Encoding.UTF8, Content_Type);

                //response = new HttpResponseMessage();
                //response = restClient.PostAsync(Builder.Uri, Content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ResponsePackageUsedJson = result;
                    POST_PackageUsedResponse responseContent = JsonConvert.DeserializeObject<POST_PackageUsedResponse>(result);

                    if (responseContent != null)
                    {
                        if (!string.IsNullOrEmpty(responseContent.status) && responseContent.status.ToLower() == "success")
                        {
                            responseContent.status = "Success";
                        }

                        postPackageUsedSuccess = responseContent.status == "Success" ? true : false;
                        postPackageUsedResponseMessage.Add("OK");
                        postPackageUsedResponseMessage.Add(responseContent.status);
                    }
                    else
                    {
                        postPackageUsedSuccess = false;
                        postPackageUsedResponseMessage.Add("OK");
                        postPackageUsedResponseMessage.Add("Unrecongnized Response Format");
                    }
                }
                else
                {
                    postPackageUsedSuccess = false;
                    postPackageUsedResponseMessage.Add(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                postPackageUsedSuccess = false;
                postPackageUsedResponseMessage.Add(ex.ToString());
            }
        }

        public static void POST_Notify()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                restClient = new HttpClient();
                postNotifyResponseMessage = new List<string>();
                string Content_Type = "application/json";
                restClient.DefaultRequestHeaders.Accept.Clear();
                restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
                restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);

                var formData = new Dictionary<string, string>
                            {
                                {"", ""}
                            };

                // Convert form data to FormUrlEncodedContent
                var formContent = new FormUrlEncodedContent(formData);

                var PostContent_Json = JsonConvert.SerializeObject(formContent);
                postPackageUsedJson = PostContent_Json;

                string apiUri = ConfigurationManager.AppSettings["APIServer"];
                var Builder = new UriBuilder($"{apiUri}/api/pos/users/notify");

                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, formContent).Result;//start here, test Notify API

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ResponseNotifyJson = result;
                    POST_NotifyResponse responseContent = JsonConvert.DeserializeObject<POST_NotifyResponse>(result);

                    if (responseContent != null)
                    {
                        if (!string.IsNullOrEmpty(responseContent.status) && responseContent.status.ToLower() == "success")
                        {
                            responseContent.status = "Success";
                        }

                        postNotifySuccess = responseContent.status == "Success" ? true : false;
                        postNotifyResponseMessage.Add("OK");
                        postNotifyResponseMessage.Add(responseContent.status);
                    }
                    else
                    {
                        postNotifySuccess = false;
                        postNotifyResponseMessage.Add("OK");
                        postNotifyResponseMessage.Add("Unrecongnized Response Format");
                    }
                }
                else
                {
                    postNotifySuccess = false;
                    postNotifyResponseMessage.Add(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                postNotifySuccess = false;
                postNotifyResponseMessage.Add(ex.ToString());
            }
        }

        public static void POST_CancelCreditTrans(string cancelCreditTransId, bool isCancel)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                restClient = new HttpClient();
                postCancelCreditTransResponseMessage = new List<string>();
                string Content_Type = "application/json";
                restClient.DefaultRequestHeaders.Accept.Clear();
                restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
                restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);
                POST_posCancelCreditTrans ContentData = new POST_posCancelCreditTrans();
                if (isCancel)
                {
                    ContentData = GetPostCancelCreditTransData(cancelCreditTransId);
                }
                else
                {
                    ContentData = GetPostDeleteTransData(cancelCreditTransId);
                }
                if (ContentData == null)
                {
                    postCancelCreditTransSuccess = true;
                    postCancelCreditTransResponseMessage.Add("No Data to Export");
                    return;
                }

                var PostContent_Json = JsonConvert.SerializeObject(ContentData);
                postCancelCreditTransJson = PostContent_Json;
                string apiUri = ConfigurationManager.AppSettings["APIServer"];
                var Builder = new UriBuilder($"{apiUri}/api/pos/packages/cancel");
                HttpContent Content = new StringContent(PostContent_Json, Encoding.UTF8, Content_Type);

                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, Content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ResponseCancelCreditTransJson = result;
                    POST_CancelCreditTransResponse responseContent = JsonConvert.DeserializeObject<POST_CancelCreditTransResponse>(result);

                    if (responseContent != null)
                    {
                        if (!string.IsNullOrEmpty(responseContent.status) && responseContent.status.ToLower() == "success")
                        {
                            responseContent.status = "Success";
                        }

                        postCancelCreditTransSuccess = responseContent.status == "Success" ? true : false;
                        postCancelCreditTransResponseMessage.Add("OK");
                        postCancelCreditTransResponseMessage.Add(responseContent.status);
                    }
                    else
                    {
                        postCancelCreditTransSuccess = false;
                        postCancelCreditTransResponseMessage.Add("OK");
                        postCancelCreditTransResponseMessage.Add("Unrecongnized Response Format");
                    }
                }
                else
                {
                    postCancelCreditTransSuccess = false;
                    postCancelCreditTransResponseMessage.Add(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                postCancelCreditTransSuccess = false;
                postCancelCreditTransResponseMessage.Add(ex.ToString());
            }
        }

        public static void POST_Trans(string transId, string phoneNo)
        {
            try
            {

                restClient = new HttpClient();
                postTransResponseMessage = new List<string>();
                string Content_Type = "multipart/form-data";
                restClient.DefaultRequestHeaders.Accept.Clear();
                restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
                restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);
                PostTrans ContentData = GetPostTransData(transId);

                if (ContentData == null)
                {
                    postTransSuccess = true;
                    postTransResponseMessage.Add("No Data to Export");
                    return;
                }

                ContentData.phone_no = phoneNo == string.Empty ? ContentData.phone_no : phoneNo;
                var PostContent_Json = JsonConvert.SerializeObject(ContentData);
                postTransJson = PostContent_Json;

                var formData = new Dictionary<string, string>
                            {
                                { "phone_no", ContentData.phone_no },
                                { "spend_reward_amount", ContentData.spend_reward_amount },
                                {"pos_transaction_id", ContentData.pos_transaction_id }
                            };

                // Convert form data to FormUrlEncodedContent
                var formContent = new FormUrlEncodedContent(formData);

                string apiUri = ConfigurationManager.AppSettings["APIServer"];
                var Builder = new UriBuilder($"{apiUri}/api/pos/points/reward");

                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, formContent).Result;
                if (response.IsSuccessStatusCode)
                {

                    var result = response.Content.ReadAsStringAsync().Result;
                    ResponseTransJson = result;
                    PostTransResponse responseContent = JsonConvert.DeserializeObject<PostTransResponse>(result);

                    if (responseContent != null)
                    {
                        postTransSuccess = responseContent.status == "failed" ? false : true;
                        postTransResponseMessage.Add("OK");
                        postTransResponseMessage.Add(responseContent.message);
                    }
                    else
                    {
                        postTransSuccess = false;
                        postTransResponseMessage.Add("OK");
                        postTransResponseMessage.Add("Unrecongnized Response Format");
                    }

                }
                else
                {
                    postTransSuccess = false;
                    postTransResponseMessage.Add(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                postTransSuccess = false;
                postTransResponseMessage.Add(ex.ToString());

            }

        }

        public static void POST_Redeem(string transId)
        {
            try
            {

                restClient = new HttpClient();
                postRedeemResponseMessage = new List<string>();
                string Content_Type = "multipart/form-data";
                restClient.DefaultRequestHeaders.Accept.Clear();
                restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
                restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);
                PostRedeem ContentData = GetPostRedeemData(transId);
                var PostContent_Json = JsonConvert.SerializeObject(ContentData);
                postRedeemJson = PostContent_Json;

                if (ContentData == null)
                {
                    postRedeemSuccess = true;
                    postTransResponseMessage.Add("No Data to Export");
                    return;
                }
                var formData = new Dictionary<string, string>
                            {
                                { "phone_no", ContentData.phone_no },
                                { "redeem_point", ContentData.redeem_point },
                                {"pos_transaction_id", ContentData.pos_transaction_id }
                            };

                // Convert form data to FormUrlEncodedContent
                var formContent = new FormUrlEncodedContent(formData);

                string apiUri = ConfigurationManager.AppSettings["APIServer"];
                var Builder = new UriBuilder($"{apiUri}/api/pos/points/redeem");

                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, formContent).Result;
                if (response.IsSuccessStatusCode)
                {

                    var result = response.Content.ReadAsStringAsync().Result;
                    ResponseRedeemJson = result;
                    PostRedeemResponse responseContent = JsonConvert.DeserializeObject<PostRedeemResponse>(result);

                    if (responseContent != null)
                    {
                        postRedeemSuccess = responseContent.status == "failed" ? false : true;
                        postRedeemResponseMessage.Add("OK");
                        postRedeemResponseMessage.Add(responseContent.message);
                    }
                    else
                    {
                        postRedeemSuccess = false;
                        postRedeemResponseMessage.Add("OK");
                        postRedeemResponseMessage.Add("Unrecongnized Response Format");
                    }

                }
                else
                {
                    postRedeemSuccess = false;
                    postRedeemResponseMessage.Add(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                postRedeemSuccess = false;
                postRedeemResponseMessage.Add(ex.ToString());

            }

        }

        public static void POST_Package(string transId)
        {
            try
            {

                restClient = new HttpClient();
                postPackageResponseMessage = new List<string>();
                string Content_Type = "application/json";
                restClient.DefaultRequestHeaders.Accept.Clear();
                restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
                restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);
                BuyPackages ContentData = GetPackageData(transId);
                if (ContentData == null)
                {
                    postPackageSuccess = true;
                    postPackageResponseMessage.Add("No Data to Export");
                    return;
                }

                var PostContent_Json = JsonConvert.SerializeObject(ContentData);
                postPackageJson = PostContent_Json;
                string apiUri = ConfigurationManager.AppSettings["APIServer"];
                var Builder = new UriBuilder($"{apiUri}/api/pos/packages");
                HttpContent Content = new StringContent(PostContent_Json, Encoding.UTF8, Content_Type);

                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, Content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ResponseTransJson = result;
                    PostPackageResponse responseContent = JsonConvert.DeserializeObject<PostPackageResponse>(result);

                    if (responseContent != null)
                    {
                        postPackageSuccess = responseContent.status == "failed" ? false : true;
                        postPackageResponseMessage.Add("OK");
                        postPackageResponseMessage.Add(responseContent.message);
                    }
                    else
                    {
                        postPackageSuccess = false;
                        postPackageResponseMessage.Add("OK");
                        postPackageResponseMessage.Add("Unrecongnized Response Format");
                    }

                }
                else
                {
                    postPackageSuccess = false;
                    postPackageResponseMessage.Add(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                postPackageSuccess = false;
                postPackageResponseMessage.Add(ex.ToString());

            }

        }

        public static Post_QR Get_CustomerPoint(string qr)
        {
            POSEntities entity = new POSEntities();
            Post_QR qrData = new Post_QR();
            API_Token.Get_AccessToken();
            HttpClient restClient = new HttpClient();
            //string Qr = "FUArNzjbYbbDHH8QydEmmUSQ0Jh2NCdxinJiUsJBtd6C2Db+yuIpNvRyeCCvEcxnAyuITbsZWpRAOfbiRln2s0ptuFzy1bl4S9OE9s9IoyY=";
            //string Qr = "gwgeg=";
            string Content_Type = "application/json";
            restClient.DefaultRequestHeaders.Accept.Clear();
            restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
            restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);

            string apiServer = ConfigurationManager.AppSettings["APIServer"];
            var Builder = new UriBuilder($"{apiServer}/api/pos/users/my-qr");

            var formData = new Dictionary<string, string>
                            {
                                { "qr_code", qr}
                            };

            // Convert form data to FormUrlEncodedContent
            var formContent = new FormUrlEncodedContent(formData);

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, formContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    QRResponseJson = result;
                    qrData = JsonConvert.DeserializeObject<Post_QR>(result);
                }
                else
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    qrData = JsonConvert.DeserializeObject<Post_QR>(result);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Getting Point Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return qrData;
        }

        #endregion
    }
}
