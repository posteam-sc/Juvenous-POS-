using POS.APP_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Runtime.InteropServices;
using System.Net;
using System.Windows.Forms;

namespace POS
{
    public partial class TransactionExport : Form
    {
        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        #region Variables
        POSEntities entity = new POSEntities();
        DateTime todayDate = DateTime.Today;
        bool IsExportSuccess;
        public static bool IsAllExportSuccess { get; set; } = true;
        bool postTransExport;
        bool postRedeemExport;
        bool postPackageExport;
        public int? UsedPoints = 0;

        bool postTransSuccess;
        bool postRedeemSuccess;
        bool postPackageSuccess;
        public static int exportStatus { get; set; } = -1;
        public bool IsBackDateExport;
        public bool IsAutoExport;
        public bool IsNoDataToExport = false;

        bool IsPackage = false;
        decimal receiveAmt = 0;

        string phoneNo = string.Empty;

        Sales saleForm;

        string TransId = string.Empty;

        int customerId = 0;

        bool? getPoint = false;

        #endregion

        public TransactionExport(string TransactionId, int CustomerId)
        {
            InitializeComponent();
            TransId = TransactionId;
            customerId = CustomerId;
        }

        private void TransactionExport_Load(object sender, EventArgs e)
        {
            this.Text = "Sales Transaction Export";
            this.Activated += AfterLoading;
        }

        private void AfterLoading(object sender, EventArgs e)
        {
            this.Activated -= AfterLoading;
            ExportDataToAPP();
        }

        public void ExportDataToAPP()
        {
            DateTime executeDate = todayDate; // just initialization
            long executeLogID = 0;
            IsAllExportSuccess = true;
            #region todayExport

            this.Text = "Data Export";
            Transaction exportList = new Transaction();
            Customer customer = new Customer();
            exportList = entity.Transactions.Where(t => t.Id == TransId && (t.IsDeleted == false || t.IsDeleted == null)).FirstOrDefault();
            customer = entity.Customers.Where(t => t.Id == customerId).FirstOrDefault(); //Scanned customer 
            if(customer != null)
            {
                phoneNo = customer.PhoneNumber;
            }

            var packageTrans = (from p in entity.Products
                                join td in entity.TransactionDetails on p.Id equals td.ProductId
                                where td.TransactionId == TransId
                                && p.IsPackage == true
                                select p).ToList();
           IsPackage = packageTrans.Count > 0 ? true : false;
        
            if (exportList != null )
            {

                receiveAmt = exportList.RecieveAmount != null ? Convert.ToDecimal(exportList.RecieveAmount) : 0;
                getPoint = exportList.IsGetPoint;

                DateTime dt = DateTime.Now;
                lblExportDate.Text = dt.ToString("dd/MM/yyyy");
                UsedPoints = (from p in entity.Transactions where p.Id == TransId select p.UsedPoints).FirstOrDefault();

                APP_Data.ImportExportLog exlog = new APP_Data.ImportExportLog();
                exlog = entity.ImportExportLogs.Where(x => x.ExportedId == TransId && x.Type == "Export").FirstOrDefault();
                
                if (exlog != null)
                {
                    switch (exlog.Status)
                    {
                        case "Success":
                            IsExportSuccess = true;
                            break;
                        case "Pending":
                            postTransExport = postRedeemExport = postPackageExport =  true;
                            IsExportSuccess = false;
                            executeDate = (DateTime)exlog.ProcessingDateTime;
                            executeLogID = exlog.Id;
                            break;
                        case "Fail":
                            List<ImportExportLogDetail> ExlogDetails = new List<ImportExportLogDetail>();
                            ExlogDetails = entity.ImportExportLogDetails.Where(x => x.ProcessingBatchID == exlog.Id && x.DetailStatus == "Fail").ToList();
                            foreach (ImportExportLogDetail exDetails in ExlogDetails)
                            {
                                switch (exDetails.ProcessName)
                                {
                                    case "Points_Reward":
                                        postTransExport = true;
                                        break;
                                    case "Points_Redeem":
                                        postRedeemExport = true;
                                        break;
                                    case "Create_Package":
                                        postPackageExport = true;
                                        break;
                                }
                            }
                            IsExportSuccess = false;
                            executeDate = (DateTime)(exlog.ProcessingDateTime);
                            executeLogID = exlog.Id;
                            break;
                    }
                }
                else
                {
                    int resultID = 0;
                    resultID = ImportExportLog.CreateNewExportTrans(dt, TransId, UsedPoints, IsPackage, receiveAmt, customerId);
                    if (resultID == 0)
                    {
                        lblExportProgress.Text = "New Export Log Fails..Data Export Cannot be Started...Please Try Again...!";
                        return;
                    }
                    postTransExport = postRedeemExport = postPackageExport = true;
                    IsExportSuccess = false;
                    executeDate = dt;
                    executeLogID = resultID;
                }
                if (!IsExportSuccess)
                {
                    bool IsConnected = Utility.CheckInternetAndServerConnection();
                    if (!IsConnected)
                    {
                        if (IsAutoExport)
                        {
                            Login.IsBackDateExportSuccess = false;
                        }
                        else
                        {
                            IsAllExportSuccess = false;
                        }

                        this.Dispose();
                        return;
                    }
                    UpdateTransactionsStatus(TransId);
                    ExecuteAPIs(TransId);
                    UpdateLogStatus(executeLogID);
                }
                if (IsExportSuccess)
                {
                    lblExportProgress.Text = "Data Export For " + dt.Date.ToString("dd/MM/yyyy") + " Completed Successfullly..";
                }
                else
                {
                    IsAllExportSuccess = false;
                    lblExportProgress.Text = "Data Export For " + dt.Date.ToString("dd/MM/yyyy") + " Finished with Error..!";
                }
                // }

                if (IsAllExportSuccess)
                {
                    lblExportProgress.Text = "Data Export Completed Successfully";
                    MessageBox.Show("Data Export Completed Successfully", "Data Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Dispose();

                }
                else
                {
                    lblExportProgress.Text = "Data Export Completed with Failures..!";
                    MessageBox.Show("Data Export Completed with Failures..!", "Data Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Dispose();
                    // Try Export Again 
                }
            }
            else
            {

                lblExportProgress.Text = "No Data to Export";
                IsNoDataToExport = true;
                MessageBox.Show("No data to export..!", "Data Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Dispose();
            }

            #endregion
        }
        public void UpdateTransactionsStatus(string TransId)
        {
            List<Transaction> tList = entity.Transactions.Where(x => x.Id == TransId && x.IsDeleted == false && x.IsExported == false).ToList();
            foreach (Transaction tran in tList)
            {
                tran.IsExported = true;
                entity.Entry(tran).State = EntityState.Modified;
            }
            entity.SaveChanges();
        }

        public void UpdateLogStatus(long LogID)
        {
            POSEntities entity = new POSEntities();

            APP_Data.ImportExportLog exportLog = entity.ImportExportLogs.Where(x => x.Id == LogID).FirstOrDefault();
            exportLog.LastProcessingDateTime = DateTime.Now;
            exportLog.Status = IsExportSuccess ? "Success" : "Fail";

            entity.Entry(exportLog).State = EntityState.Modified;

            List<ImportExportLogDetail> importLogDetail = entity.ImportExportLogDetails.Where(x => x.ProcessingBatchID == LogID && x.DetailStatus != "Success").ToList();
            foreach (ImportExportLogDetail log in importLogDetail)
            {
                switch (log.ProcessName)
                {
                    case "Points_Reward":
                        log.DetailStatus = postTransSuccess ? "Success" : "Fail";
                        log.ResponseMessageFromSAP = string.Join(";", API_POST.postTransResponseMessage.ToArray());
                        log.PostJson = API_POST.postTransJson;
                        log.ResponseJson = log.ResponseJson == null ? DateTime.Now.ToString() + ";" + API_POST.ResponseTransJson : log.ResponseJson + ";[" + DateTime.Now.ToString() + "];" + API_POST.ResponseTransJson;
                        
                        break;
                    case "Points_Redeem":
                        log.DetailStatus = postRedeemSuccess ? "Success" : "Fail";
                        log.ResponseMessageFromSAP = string.Join(";", API_POST.postRedeemResponseMessage.ToArray());
                        log.PostJson = API_POST.postRedeemJson;
                        log.ResponseJson = log.ResponseJson == null ? DateTime.Now.ToString() + ";" + API_POST.ResponseRedeemJson : log.ResponseJson + ";[" + DateTime.Now.ToString() + "];" + API_POST.ResponseRedeemJson;

                        break;
                    case "Create_Package":
                        log.DetailStatus = postPackageSuccess ? "Success" : "Fail";
                        log.ResponseMessageFromSAP = string.Join(";", API_POST.postPackageResponseMessage.ToArray());
                        log.PostJson = API_POST.postPackageJson;
                        log.ResponseJson = log.ResponseJson == null ? DateTime.Now.ToString() + ";" + API_POST.ResponsePackageJson : log.ResponseJson + ";[" + DateTime.Now.ToString() + "];" + API_POST.ResponsePackageJson;

                        break;
                }

            }
            entity.SaveChanges();
        }

        public void ExecuteAPIs(string transId)
        {
            int no0fProgressSteps = 5;
            int i = 1;
            ExportProgressBar.Value = 0; // reset progressbar
            ExportProgressBar.Maximum = no0fProgressSteps;
            IsExportSuccess = true;
            postTransSuccess = postRedeemSuccess = postPackageSuccess =  true;

            ShowStatus("Step 1 of 4: Getting Access Token...", ++i, true);
            API_Token.Get_AccessToken();
            if (string.IsNullOrEmpty(API_Token.AccessToken) || string.IsNullOrWhiteSpace(API_Token.AccessToken))
            {
                ShowStatus(" Step 1 of 4: Getting Access Token Fails!..Cannot Export Data...", i, false);
                IsExportSuccess = false;
                postTransSuccess = false;
                API_POST.postTransResponseMessage.Add("No Access Token");
                return; // exit import and update log table status to 'fail'
            }

            ShowStatus("Step 1 of 4: Token Received Successfully!", i, true);
            if(receiveAmt > 0 && Convert.ToBoolean(getPoint))
            {
                if (postTransExport)
                {
                    ShowStatus("Step 2 of 4: Sending Trans Invoices..Please Wait...", ++i, true);
                    API_POST.POST_Trans(transId, phoneNo);
                    if (!API_POST.postTransSuccess)
                    {
                        if (API_POST.response != null && API_POST.response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            MessageBox.Show("Unauthorized Access! Please Refresh Access Token", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ShowStatus("Step 2 of 4: Sending Trans Invoices Fails...", i, false);
                            IsExportSuccess = false;
                            postTransSuccess = false;
                        }

                        else
                        {
                            ShowStatus("Step 2 of 4: Sending Trans Invoices Fails...", i, false);
                            IsExportSuccess = false;
                            postTransSuccess = false;
                        }

                    }
                    else
                    {
                        if (API_POST.postTransResponseMessage != null && API_POST.postTransResponseMessage.Contains("No Data to Export"))
                        {
                            ShowStatus("Step 2 of 4: No Trans Invoices to Export..Skip Sending", i, true);
                        }
                        else
                        {
                            ShowStatus("Step 2 of 4: Trans Invoices Exported Successfully..", i, true);
                        }

                    }

                }
                else
                {
                    ShowStatus("Step 2 of 4: Trans Invoices Already Exported...Skip Sendig...!", i, true);

                }
            }
            if(UsedPoints != null && UsedPoints != 0)
            {
                if (postRedeemExport)
                {
                    ShowStatus("Step 3 of 4: Sending Redeem Invoices..Please Wait...", ++i, true);
                    API_POST.POST_Redeem(transId);
                    if (!API_POST.postRedeemSuccess)
                    {
                        if (API_POST.response != null && API_POST.response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            MessageBox.Show("Unauthorized Access! Please Refresh Access Token", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ShowStatus("Step 3 of 4: Sending Redeem Invoices Fails...", i, false);
                            IsExportSuccess = false;
                            postRedeemSuccess = false;
                        }

                        else
                        {
                            ShowStatus("Step 3 of 4: Sending Redeem Invoices Fails...", i, false);
                            IsExportSuccess = false;
                            postRedeemSuccess = false;
                        }

                    }
                    else
                    {
                        if (API_POST.postRedeemResponseMessage != null && API_POST.postRedeemResponseMessage.Contains("No Data to Export"))
                        {
                            ShowStatus("Step 3 of 4: No Redeem Invoices to Export..Skip Sending", i, true);
                        }
                        else
                        {
                            ShowStatus("Step 3 of 4: Redeem Invoices Exported Successfully..", i, true);
                        }

                    }

                }
                else
                {
                    ShowStatus("Step 3 of 4: Redeem Invoices Already Exported...Skip Sendig...!", i, true);

                }
            }
            if (IsPackage && customerId == 0)
            {
                if (postPackageExport)
                {
                    ShowStatus("Step 4 of 4: Sending Package..Please Wait...", ++i, true);
                    API_POST.POST_Package(transId);
                    if (!API_POST.postPackageSuccess)
                    {
                        if (API_POST.response != null && API_POST.response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            MessageBox.Show("Unauthorized Access! Please Refresh Access Token", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ShowStatus("Step 4 of 4: Sending Package Fails...", i, false);
                            IsExportSuccess = false;
                            postPackageSuccess = false;
                        }

                        else
                        {
                            ShowStatus("Step 4 of 4: Sending Package Fails...", i, false);
                            IsExportSuccess = false;
                            postPackageSuccess = false;
                        }

                    }
                    else
                    {
                        if (API_POST.postPackageResponseMessage != null && API_POST.postPackageResponseMessage.Contains("No Data to Export"))
                        {
                            ShowStatus("Step 4 of 4: No Package to Export..Skip Sending", i, true);
                        }
                        else
                        {
                            ShowStatus("Step 4 of 4: Package Exported Successfully..", i, true);
                        }

                    }

                }
                else
                {
                    ShowStatus("Step 4 of 4: Package Already Exported...Skip Sendig...!", i, true);

                }
            }
        }
        public void ShowStatus(string message, int value, bool success)
        {

            this.Invoke((MethodInvoker)delegate
            {

                if (success)
                {
                    lblExportProgress.Text = message;
                    lblExportProgress.Refresh();
                    ExportProgressBar.Value = value;
                    ExportProgressBar.Step = 1;
                    ExportProgressBar.PerformStep();

                }
                else
                {
                    lblExportProgress.Text = message;
                    lblExportProgress.Refresh();
                    int PBM_SETSTATE = 1040; // Code to set the state of progressbar
                    int InProgress = 1; //(Green)
                    int Error = 2; // (Red)
                    int Paused = 3; // (Yellow)
                    SendMessage(ExportProgressBar.Handle, PBM_SETSTATE, Error, InProgress);
                }

            });

        }
    }
}
