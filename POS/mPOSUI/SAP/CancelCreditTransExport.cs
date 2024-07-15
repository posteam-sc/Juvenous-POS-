using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Objects;
using System.Net;
using POS.APP_Data;
using System.Configuration;
using System.Runtime;
using System.Runtime.InteropServices;

namespace POS
{
    public partial class CancelCreditTransExport : Form
    {
        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        #region Variables

        POSEntities entity = new POSEntities();
        DateTime todayDate = DateTime.Today;
        bool IsExportSuccess;
        public static bool IsAllExportSuccess { get; set; } = true;
        bool postCancelCreditTransSuccess;
        bool postCancelCreditTransNoExport;
        public static int exportStatus { get; set; } = -1;
        public bool IsBackDateExport;
        public bool IsAutoExport;
        public bool IsNoDataToExport = false;
        
        string s_cancelCreditTransId = "";
        bool cancel = false;
        #endregion

        public CancelCreditTransExport(string exId, bool isCancel)
        {
            InitializeComponent();
            s_cancelCreditTransId = exId;
            cancel = isCancel;
        }

        private void AfterLoading(object sender, EventArgs e)
        {
            this.Activated -= AfterLoading;
            ExportDataToSAP();
        }

        private void CancelCreditTransExport_Load(object sender, EventArgs e)
        {
            this.Text = "Cancel Credit Transaction Data Export";
            this.Activated += AfterLoading;
        }

        public void ExportDataToSAP()
        {
            int backDateDays = SettingController.BackDate_NoOfDays;
            DateTime backDate = todayDate.AddDays(backDateDays).Date;
            string shortCode = SettingController.DefaultShop.ShortCode;
            DateTime startDate;
            DateTime endDate;

            DateTime executeDate = todayDate; // just initialization
            long executeLogID = 0;
            IsAllExportSuccess = true;

            #region Export

            List<string> packagePurchasedInvoiceIdList = new List<string>();
            startDate = backDate;
            endDate = todayDate.AddDays(-1).Date;
            if (cancel)
            {
                packagePurchasedInvoiceIdList = (from p in entity.PackagePurchasedInvoices
                                                 where p.PackagePurchasedInvoiceId == s_cancelCreditTransId
                                                 select p.PackagePurchasedInvoiceId).Distinct().ToList();
            }
            else
            {
                packagePurchasedInvoiceIdList = (from p in entity.Transactions
                                                 where p.Id == s_cancelCreditTransId
                                                 select p.Id).Distinct().ToList();
            }

            if (packagePurchasedInvoiceIdList.Count > 0)
            {
                foreach (string Id in packagePurchasedInvoiceIdList)
                {
                    DateTime dt = DateTime.Now;
                    lblExportDate.Text = "";
                    lblExportDate.Text = dt.Date.ToString("dd-MM-yyyy");
                    APP_Data.ImportExportLog exlog = new APP_Data.ImportExportLog();
                    if (cancel)
                    {
                        exlog = entity.ImportExportLogs.Where(x => x.ExportedId == s_cancelCreditTransId && x.Type == "Export").FirstOrDefault();
                    }
                    else
                    {
                        exlog = entity.ImportExportLogs.Where(x => x.ExportedId == s_cancelCreditTransId && x.ExportedType == "DeleteTransExport").FirstOrDefault();

                    }
                    if (exlog != null)
                    {
                        switch (exlog.Status)
                        {
                            case "Success":
                                IsExportSuccess = true;
                                break;
                            case "Pending":
                                postCancelCreditTransNoExport = true;
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
                                        case "POST_CancelPackage":
                                            postCancelCreditTransNoExport = true;
                                            break;
                                    }
                                }
                                IsExportSuccess = false;
                                executeDate = (DateTime)exlog.ProcessingDateTime;
                                executeLogID = exlog.Id;
                                break;
                        }

                    }
                    else
                    {
                        int resultID = 0;
                        resultID = ImportExportLog.CreateNewExportCancelCreditTrans(dt, s_cancelCreditTransId, cancel);
                        if (resultID == 0)
                        {
                            lblExportProgress.Text = "New Export Log Fails..Data Import Cannot be Started...Please Try Again...!";
                            return;
                        }
                        postCancelCreditTransNoExport = true;
                        IsExportSuccess = false;
                        executeDate = todayDate;
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
                        UpdateCancelCreditTransStatus(s_cancelCreditTransId);
                        ExecuteAPIs(executeDate);
                        UpdateLogStatus(executeLogID);
                    }
                    if (IsExportSuccess)
                    {
                        lblExportProgress.Text = "Data Export For " + dt.Date.ToString("dd-MM-yyyy") + " Completed Successfullly..";
                    }
                    else
                    {
                        IsAllExportSuccess = false;
                        lblExportProgress.Text = "Data Export For " + dt.Date.ToString("dd-MM-yyyy") + " Finished with Error..!";
                    }

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

        public void UpdateCancelCreditTransStatus(string cancelCreditTransid)
        {
            List<PackagePurchasedInvoice> PackagePurchasedInvoiceList = entity.PackagePurchasedInvoices.Where(x => x.PackagePurchasedInvoiceId == cancelCreditTransid && x.IsExported == false).ToList();
            foreach (PackagePurchasedInvoice ppi in PackagePurchasedInvoiceList)
            {
                ppi.IsExported = true;
                entity.Entry(ppi).State = EntityState.Modified;
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
                    case "POST_CancelPackage":
                        log.DetailStatus = postCancelCreditTransSuccess ? "Success" : "Fail";
                        log.ResponseMessageFromSAP = string.Join(";", API_POST.postCancelCreditTransResponseMessage.ToArray());
                        log.PostJson = API_POST.postCancelCreditTransJson;
                        log.ResponseJson = log.ResponseJson == null ? DateTime.Now.ToString() + ";" + API_POST.ResponseCancelCreditTransJson : log.ResponseJson + ";[" + DateTime.Now.ToString() + "];" + API_POST.ResponseCancelCreditTransJson;

                        break;
                    case "POST_DeletePackage":
                        log.DetailStatus = postCancelCreditTransSuccess ? "Success" : "Fail";
                        log.ResponseMessageFromSAP = string.Join(";", API_POST.postCancelCreditTransResponseMessage.ToArray());
                        log.PostJson = API_POST.postCancelCreditTransJson;
                        log.ResponseJson = log.ResponseJson == null ? DateTime.Now.ToString() + ";" + API_POST.ResponseCancelCreditTransJson : log.ResponseJson + ";[" + DateTime.Now.ToString() + "];" + API_POST.ResponseCancelCreditTransJson;

                        break;
                }
            }
            entity.SaveChanges();
        }

        public void ExecuteAPIs(DateTime postDate)
        {
            //  int no0fProgressSteps = 5;
            int no0fProgressSteps = 4;
            int i = 1;
            ExportProgressBar.Value = 0; // reset progressbar
            ExportProgressBar.Maximum = no0fProgressSteps;
            postDate = postDate.Date;
            IsExportSuccess = true;
            postCancelCreditTransSuccess = true;

            ShowStatus("Step 1 of 4: Getting Access Token...", ++i, true);
            API_Token.Get_AccessToken();
            if (string.IsNullOrEmpty(API_Token.AccessToken) || string.IsNullOrWhiteSpace(API_Token.AccessToken))
            {
                ShowStatus(" Step 1 of 4: Getting Access Token Fails!..Cannot Export Data...", i, false);
                IsExportSuccess = false;
                postCancelCreditTransSuccess = false;
                API_POST.postCancelCreditTransResponseMessage.Add("No Access Token");
                return; // exit import and update log table status to 'fail'
            }

            ShowStatus("Step 1 of 3: Token Received Successfully!", i, true);
            if (postCancelCreditTransNoExport)
            {
                ShowStatus("Step 2 of 3: Sending New Cancelled Credit Transaction..Please Wait...", ++i, true);
                API_POST.POST_CancelCreditTrans(s_cancelCreditTransId, cancel);
                if (!API_POST.postCancelCreditTransSuccess)
                {
                    if (API_POST.response != null)
                    {
                        if (API_POST.response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            MessageBox.Show("Bad Request! Please contact administrators", "BadRequest Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ShowStatus("Step 2 of 3: Sending New Cancelled Credit Transaction Fail...", i, false);
                            IsExportSuccess = false;
                            postCancelCreditTransSuccess = false;
                        }
                        if (API_POST.response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            MessageBox.Show("Unauthorized Access! Please Refresh Access Token", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ShowStatus("Step 2 of 3: Sending New Cancelled Credit Transaction Fail...", i, false);
                            IsExportSuccess = false;
                            postCancelCreditTransSuccess = false;
                        }
                        if (API_POST.response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            MessageBox.Show("Internal Server Error! Please contact administrators", "InternalServerError Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ShowStatus("Step 2 of 3: Sending New Cancelled Credit Transaction Fail...", i, false);
                            IsExportSuccess = false;
                            postCancelCreditTransSuccess = false;
                        }
                    }

                    else
                    {
                        ShowStatus("Step 2 of 3: Sending New Cancelled Credit Transaction Fail...", i, false);
                        IsExportSuccess = false;
                        postCancelCreditTransSuccess = false;
                    }

                }
                else
                {
                    if (API_POST.postCancelCreditTransResponseMessage != null && API_POST.postCancelCreditTransResponseMessage.Contains("No Data to Export"))
                    {
                        ShowStatus("Step 2 of 3: No New Cancelled Credit Transaction to Export..Skip Sending", i, true);
                    }
                    else
                    {
                        ShowStatus("Step 2 of 3: New Cancelled Credit Transaction Exported Successfully..", i, true);
                    }
                }
            }
            else
            {
                ShowStatus("Step 2 of 3: New Cancelled Credit Transaction Already Exported...Skip Sendig...!", i, true);
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
