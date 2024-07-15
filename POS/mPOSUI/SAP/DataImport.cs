using POS.APP_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Transactions;
using System.Windows.Forms;

namespace POS
{
    public partial class DataImport : Form
    {
        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        #region variables
        POSEntities entity = new POSEntities();
        public static bool IsImportSuccess { get; set; } = true;
        public bool IsAutoImport { get; set; }

        DateTime todayDate = DateTime.Today;
        Sales saleForm;
        public bool IsAllDataImport { get; set; }

        bool postNotifySuccess;

        #endregion

        #region Events

        public DataImport(Sales sForm)
        {
            InitializeComponent();
            saleForm = sForm;
        }

        private void DataImport_Load(object sender, EventArgs e)
        {
            this.Activated += AfterLoading;
        }

        #endregion

        #region Methods

        private void AfterLoading(object sender, EventArgs e)
        {
            this.Activated -= AfterLoading;
            ImportDataFromSAP();
        }
        public void ImportDataFromSAP()
        {
            lblImportDate.Text = todayDate.ToString("dd-MM-yyyy");
            lblImportProgress.Text = "Preparing Data Import From SAP, Please Wait...";

            bool IsImportAlreadySuccess = false;
            List<APP_Data.ImportExportLog> InLogList = entity.ImportExportLogs.Where(x => EntityFunctions.TruncateTime(x.ProcessingDateTime) == todayDate && x.Type == "Import").ToList();
            long ImportBatchID = 0;
            if (InLogList.Count > 0)
            {
                int ImportSuccessCount = InLogList.Where(x => x.Status == "Success").Count();
                if (ImportSuccessCount > 0)
                {
                    IsImportAlreadySuccess = true;
                }

                APP_Data.ImportExportLog InLog = InLogList.OrderBy(x => x.ProcessingDateTime).LastOrDefault();
                switch (InLog.Status)
                {
                    case "Pending":
                        ImportBatchID = InLog.Id;
                        break;
                    case "Success":
                    //ImportBatchID = ImportExportLog.CreateNewImportBatch();
                    //if (ImportBatchID == 0)
                    //{
                    //    lblImportProgress.Text = "New Import Log Fails..Data Import Cannot be Started...Please Try Again...!";
                    //    return;
                    //}
                    //break;
                    case "Fail":
                        ImportBatchID = InLog.Id;
                        break;
                }
            }
            else
            {
                ImportBatchID = ImportExportLog.CreateNewImportBatch();
                IsImportAlreadySuccess = false;
                if (ImportBatchID == 0)
                {
                    lblImportProgress.Text = "New Import Log Fails..Data Import Cannot be Started..Please Try Again...!";
                    return;
                }
            }
            bool IsConnected = Utility.CheckInternetAndServerConnection();
            if (!IsConnected)
            {
                if (!IsImportAlreadySuccess)
                {
                    IsImportSuccess = false;
                }
                else
                {
                    IsImportSuccess = true;
                }
                this.Dispose();
                return;
            }

            ExecuteAPIs();
            UpdateLogStatus(ImportBatchID);

            if (IsImportSuccess)
            {
                if (!IsAutoImport) // to show messagebox if not AutoImport
                {
                    MessageBox.Show("Data Import Completed Successfully", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    saleForm.ReloadCustomerList();
                }

                this.Dispose();
            }
            else
            {
                if (!IsAutoImport)
                {
                    MessageBox.Show("Data Import Failed!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Auto Import Failed! Please try manually", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            if (IsImportAlreadySuccess)
            {
                IsImportSuccess = true;
            }
        }

        private void UpdateLogStatus(long batchID)
        {
            APP_Data.ImportExportLog importLog = entity.ImportExportLogs.Where(x => x.Id == batchID).FirstOrDefault();
            importLog.Status = IsImportSuccess ? "Success" : "Fail";
            importLog.LastProcessingDateTime = DateTime.Now;
            entity.Entry(importLog).State = EntityState.Modified;

            List<ImportExportLogDetail> importLogDetail = entity.ImportExportLogDetails.Where(x => x.ProcessingBatchID == batchID).ToList();
            foreach (ImportExportLogDetail log in importLogDetail)
            {
                log.DetailStatus = IsImportSuccess ? "Success" : "Fail";
                switch (log.ProcessName)
                {
                    case "GET_Customers":
                        log.ResponseMessageFromSAP = API_GET.CustomerResponseMessage;
                        log.ResponseJson = API_GET.CustomersResponseJson;
                        break;

                    case "POST_Notify":
                        log.DetailStatus = postNotifySuccess ? "Success" : "Fail";
                        log.ResponseMessageFromSAP = string.Join(";", API_POST.postNotifyResponseMessage.ToArray());
                        log.PostJson = API_POST.postNotifyJson;
                        log.ResponseJson = log.ResponseJson == null ? DateTime.Now.ToString() + ";" + API_POST.ResponseNotifyJson : log.ResponseJson + ";[" + DateTime.Now.ToString() + "];" + API_POST.ResponseNotifyJson;
                        break;
                }
            }

            entity.SaveChanges();
        }

        private void ExecuteAPIs()
        {
            int i = 1;
            IsImportSuccess = true;
            int no0fProgressSteps = 17;
            ImportProgressBar.Maximum = no0fProgressSteps;
            //check later
            //TransactionOptions options = new TransactionOptions();
            //options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            //options.Timeout = new TimeSpan(1, 00, 0);
            //using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, options))
            //{
            ShowStatus("Step 1 of 4: Getting Access Token...", ++i, true);
            API_Token.Get_AccessToken();

            if (string.IsNullOrEmpty(API_Token.AccessToken) || string.IsNullOrWhiteSpace(API_Token.AccessToken))
            {
                ShowStatus(" Step 1 of 4: Getting Token Fails!..", i, false);
                IsImportSuccess = false;
                API_GET.CustomerResponseMessage = "No Access Token";
                return; // exit import and update log table status to 'fail'
            }

            ShowStatus("Step 1 of 4: Token Received Successfully!", i, true);
            ShowStatus("Step 2 of 4: Getting New Patients....", ++i, true);
            API_GET.GET_CustomerData();
            if (API_GET.Master_CustomerItem == null)
            {
                ShowStatus("Step 2 of 4: Getting New Patients Fails....", i, false);
                if (API_GET.response.StatusCode == HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Bad Request! Please contact administrators", "BadRequest Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (API_GET.response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("Unauthorized Access! Please Refresh Access Token", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (API_GET.response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    MessageBox.Show("Internal Server Error! Please contact administrators", "InternalServerError Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                IsImportSuccess = false;
                return;
            }
            ShowStatus("Step 2 of 4: Checking/Saving New Patients....", i, true);

            API_GET.CheckNewCustomer(API_GET.Master_CustomerItem);
            if (API_GET.NewCustomerSaveSuccess == false)
            {
                ShowStatus("Step 3 of 4: Checking/Saving New Patients Fails....", i, false);
                //transaction.Dispose();
                IsImportSuccess = false;
                return;
            }
            ShowStatus("Step 3 of 4: Checking/Saving New Patients Succefully FINISHED....", i, true);

            //POST Notify API

            API_POST.POST_Notify();
            if(!API_POST.postNotifySuccess)
            {
                postNotifySuccess = false;
            }
            else
            {
                postNotifySuccess = true;
            }

            //}
            Application.DoEvents();
        }

        public void ShowStatus(string message, int value, bool success)
        {

            this.Invoke((MethodInvoker)delegate
            {
                if (success)
                {
                    lblImportProgress.Text = message;
                    lblImportProgress.Refresh();
                    ImportProgressBar.Value = value;
                    ImportProgressBar.Step = 1;
                    ImportProgressBar.PerformStep();
                }
                else
                {
                    lblImportProgress.Text = message;
                    lblImportProgress.Refresh();
                    int PBM_SETSTATE = 1040; // Code to set the state of progressbar
                    int InProgress = 1; //(Green)
                    int Error = 2; // (Red)
                    int Paused = 3; // (Yellow)
                    SendMessage(ImportProgressBar.Handle, PBM_SETSTATE, Error, InProgress);
                }
            });
        }
        #endregion
    }
}
