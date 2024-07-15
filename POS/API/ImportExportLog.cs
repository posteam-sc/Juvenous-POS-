using System;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using POS.APP_Data;

namespace POS
{
    public static class ImportExportLog
    {
        public static int CreateNewImportBatch()
        {
            POSEntities entity = new POSEntities();
            string type = "Import";
            string shortCode = SettingController.DefaultShop.ShortCode;
            DateTime todayDate = DateTime.Today;
            string[] APINames = { "GET_Customers", "POST_Notify" };

            int LatestID = 0;

            using (var transaction = new TransactionScope())
            {
                try
                {
                    ObjectResult<int?> InsertedID = entity.InsertImportExportLog(DateTime.Now, type, "Pending", shortCode, "", "");
                    foreach (Nullable<int> result in InsertedID)
                    {
                        LatestID = result.Value;
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("BatchID", typeof(int));
                    dt.Columns.Add("ProcessName", typeof(string));
                    dt.Columns.Add("DetailStatus", typeof(string));
                    dt.Columns.Add("ResponseMessageFromSAP", typeof(string));
                    dt.Columns.Add("PostJson", typeof(string));
                    dt.Columns.Add("ResponseJson", typeof(string));

                    dt.Rows.Add(LatestID, APINames[0], "Pending");
                    dt.Rows.Add(LatestID, APINames[1], "Pending");

                    var parameter = new SqlParameter("@ProcessList", SqlDbType.Structured);
                    parameter.Value = dt;
                    parameter.TypeName = "dbo.ProcessList";

                    entity.Database.ExecuteSqlCommand("exec dbo.InsertImportExportLogDetail @ProcessList", parameter);
                    transaction.Complete();
                    return LatestID;
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    return 0;
                }
            }
        }

        public static int CreateNewExportCustomer(DateTime tranDate, string s_customerId)
        {
            POSEntities entity = new POSEntities();
            string type = "Export";
            string shortCode = SettingController.DefaultShop.ShortCode;
            DateTime todayDate = DateTime.Today;
            string[] APINames = { "POST_Customers" };

            int LatestID = 0;

            using (var transaction = new TransactionScope())
            {
                try
                {
                    ObjectResult<int?> InsertedID = entity.InsertImportExportLog(tranDate, type, "Pending", shortCode, s_customerId, "CustomerExport");
                    foreach (Nullable<int> result in InsertedID)
                    {
                        LatestID = result.Value;
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("BatchID", typeof(int));
                    dt.Columns.Add("ProcessName", typeof(string));
                    dt.Columns.Add("DetailStatus", typeof(string));
                    dt.Columns.Add("ResponseMessageFromSAP", typeof(string));
                    dt.Columns.Add("PostJson", typeof(string));
                    dt.Columns.Add("ResponseJson", typeof(string));

                    dt.Rows.Add(LatestID, APINames[0], "Pending");

                    var parameter = new SqlParameter("@ProcessList", SqlDbType.Structured);
                    parameter.Value = dt;
                    parameter.TypeName = "dbo.ProcessList";

                    entity.Database.ExecuteSqlCommand("exec dbo.InsertImportExportLogDetail @ProcessList", parameter);
                    transaction.Complete();
                    return LatestID;

                }
                catch (Exception)
                {
                    transaction.Dispose();
                    return 0;
                }
            }

        }

        public static int CreateNewExportSharer(DateTime tranDate, string s_purchasedPackageSharersId)
        {
            POSEntities entity = new POSEntities();
            string type = "Export";
            string shortCode = SettingController.DefaultShop.ShortCode;
            DateTime todayDate = DateTime.Today;
            string[] APINames = { "POST_Sharers" };

            int LatestID = 0;

            using (var transaction = new TransactionScope())
            {
                try
                {
                    ObjectResult<int?> InsertedID = entity.InsertImportExportLog(tranDate, type, "Pending", shortCode, s_purchasedPackageSharersId, "SharerExport");
                    foreach (Nullable<int> result in InsertedID)
                    {
                        LatestID = result.Value;
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("BatchID", typeof(int));
                    dt.Columns.Add("ProcessName", typeof(string));
                    dt.Columns.Add("DetailStatus", typeof(string));
                    dt.Columns.Add("ResponseMessageFromSAP", typeof(string));
                    dt.Columns.Add("PostJson", typeof(string));
                    dt.Columns.Add("ResponseJson", typeof(string));

                    dt.Rows.Add(LatestID, APINames[0], "Pending");

                    var parameter = new SqlParameter("@ProcessList", SqlDbType.Structured);
                    parameter.Value = dt;
                    parameter.TypeName = "dbo.ProcessList";

                    entity.Database.ExecuteSqlCommand("exec dbo.InsertImportExportLogDetail @ProcessList", parameter);
                    transaction.Complete();
                    return LatestID;

                }
                catch (Exception)
                {
                    transaction.Dispose();
                    return 0;
                }
            }

        }

        public static int CreateNewExportPackageUsed(DateTime tranDate, string s_packageUsedId)
        {
            POSEntities entity = new POSEntities();
            string type = "Export";
            string shortCode = SettingController.DefaultShop.ShortCode;
            DateTime todayDate = DateTime.Today;
            string[] APINames = { "POST_PackageUsed" };

            int LatestID = 0;
            using (var transaction = new TransactionScope())
            {
                try
                {
                    ObjectResult<int?> InsertedID = entity.InsertImportExportLog(tranDate, type, "Pending", shortCode, s_packageUsedId, "PackageUsedExport");
                    foreach (Nullable<int> result in InsertedID)
                    {
                        LatestID = result.Value;
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("BatchID", typeof(int));
                    dt.Columns.Add("ProcessName", typeof(string));
                    dt.Columns.Add("DetailStatus", typeof(string));
                    dt.Columns.Add("ResponseMessageFromSAP", typeof(string));
                    dt.Columns.Add("PostJson", typeof(string));
                    dt.Columns.Add("ResponseJson", typeof(string));

                    dt.Rows.Add(LatestID, APINames[0], "Pending");

                    var parameter = new SqlParameter("@ProcessList", SqlDbType.Structured);
                    parameter.Value = dt;
                    parameter.TypeName = "dbo.ProcessList";

                    entity.Database.ExecuteSqlCommand("exec dbo.InsertImportExportLogDetail @ProcessList", parameter);
                    transaction.Complete();
                    return LatestID;

                }
                catch (Exception)
                {
                    transaction.Dispose();
                    return 0;
                }
            }

        }

        public static int CreateNewExportCancelCreditTrans(DateTime tranDate, string s_cancelCreditTransId, bool isCancel)
        {
            POSEntities entity = new POSEntities();
            string type = "Export";
            string shortCode = SettingController.DefaultShop.ShortCode;
            DateTime todayDate = DateTime.Today;
            string[] APINames = { "POST_CancelPackage", "POST_DeletePackage" };

            int LatestID = 0;

            using (var transaction = new TransactionScope())
            {
                try
                {
                    ObjectResult<int?> InsertedID;
                    if (isCancel)
                    {
                        InsertedID = entity.InsertImportExportLog(tranDate, type, "Pending", shortCode, s_cancelCreditTransId, "CancelCreditTransExport");
                    }
                    else
                    {
                        InsertedID = entity.InsertImportExportLog(tranDate, type, "Pending", shortCode, s_cancelCreditTransId, "DeleteTransExport");
                    }

                    foreach (Nullable<int> result in InsertedID)
                    {
                        LatestID = result.Value;
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("BatchID", typeof(int));
                    dt.Columns.Add("ProcessName", typeof(string));
                    dt.Columns.Add("DetailStatus", typeof(string));
                    dt.Columns.Add("ResponseMessageFromSAP", typeof(string));
                    dt.Columns.Add("PostJson", typeof(string));
                    dt.Columns.Add("ResponseJson", typeof(string));

                    if (isCancel)
                    {
                        dt.Rows.Add(LatestID, APINames[0], "Pending");
                    }
                    else
                    {
                        dt.Rows.Add(LatestID, APINames[1], "Pending");
                    }

                    var parameter = new SqlParameter("@ProcessList", SqlDbType.Structured);
                    parameter.Value = dt;
                    parameter.TypeName = "dbo.ProcessList";

                    entity.Database.ExecuteSqlCommand("exec dbo.InsertImportExportLogDetail @ProcessList", parameter);
                    transaction.Complete();
                    return LatestID;

                }
                catch (Exception)
                {
                    transaction.Dispose();
                    return 0;
                }
            }

        }

        public static int CreateNewExportTrans(DateTime tranDate, string TransId, int? points, bool IsPackage, decimal receiveAmt, int customerId)
        {
            POSEntities entity = new POSEntities();
            string type = "Export";
            DateTime todayDate = DateTime.Today;
            string shortCode = SettingController.DefaultShop.ShortCode;
            string APIName = "Points_Reward";
            int LatestID = 0;
            var tranData = entity.Transactions.Where(t => t.Id == TransId).FirstOrDefault();
            bool getPoint = Convert.ToBoolean(tranData.IsGetPoint) ? true : false;

            using (var transaction = new TransactionScope())
            {
                try
                {
                    ObjectResult<int?> InsertedID = entity.InsertImportExportLog(tranDate, type, "Pending", shortCode, TransId, "TransactionExport");
                    foreach (Nullable<int> result in InsertedID)
                    {
                        LatestID = result.Value;
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("BatchID", typeof(int));
                    dt.Columns.Add("ProcessName", typeof(string));
                    dt.Columns.Add("DetailStatus", typeof(string));
                    dt.Columns.Add("ResponseMessageFromSAP", typeof(string));
                    dt.Columns.Add("PostJson", typeof(string));
                    dt.Columns.Add("ResponseJson", typeof(string));
                    if (IsPackage && customerId == 0)
                    {
                        dt.Rows.Add(LatestID, "Create_Package", "Pending");
                    }
                    if (points != null && points != 0)
                    {
                        dt.Rows.Add(LatestID, "Points_Redeem", "Pending");
                    }
                    if (receiveAmt > 0 && getPoint)
                    {
                        dt.Rows.Add(LatestID, APIName, "Pending");
                    }

                    var parameter = new SqlParameter("@ProcessList", SqlDbType.Structured);
                    parameter.Value = dt;
                    parameter.TypeName = "dbo.ProcessList";

                    entity.Database.ExecuteSqlCommand("exec dbo.InsertImportExportLogDetail @ProcessList", parameter);
                    transaction.Complete();
                    return LatestID;

                }
                catch (Exception)
                {
                    transaction.Dispose();
                    return 0;
                }
            }

        }

        public static int CreateNewSharerCancelTrans(DateTime tranDate, string Id)
        {
            POSEntities entity = new POSEntities();
            string type = "Export";
            string shortCode = SettingController.DefaultShop.ShortCode;
            DateTime todayDate = DateTime.Today;
            string[] APINames = { "CancelSPackage" };

            int LatestID = 0;

            using (var transaction = new TransactionScope())
            {
                try
                {
                    ObjectResult<int?> InsertedID = entity.InsertImportExportLog(tranDate, type, "Pending", shortCode, Id, "CancelSPExport");
                    foreach (Nullable<int> result in InsertedID)
                    {
                        LatestID = result.Value;
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("BatchID", typeof(int));
                    dt.Columns.Add("ProcessName", typeof(string));
                    dt.Columns.Add("DetailStatus", typeof(string));
                    dt.Columns.Add("ResponseMessageFromSAP", typeof(string));
                    dt.Columns.Add("PostJson", typeof(string));
                    dt.Columns.Add("ResponseJson", typeof(string));

                    dt.Rows.Add(LatestID, APINames[0], "Pending");

                    var parameter = new SqlParameter("@ProcessList", SqlDbType.Structured);
                    parameter.Value = dt;
                    parameter.TypeName = "dbo.ProcessList";

                    entity.Database.ExecuteSqlCommand("exec dbo.InsertImportExportLogDetail @ProcessList", parameter);
                    transaction.Complete();
                    return LatestID;

                }
                catch (Exception)
                {
                    transaction.Dispose();
                    return 0;
                }
            }

        }
    }
}
