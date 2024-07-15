using POS.APP_Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    public partial class SharerDeleteExport : Form
    {
        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        #region Variables
        POSEntities entity = new POSEntities();
        DateTime todayDate = DateTime.Today;
        public bool IsExportSuccess;
        public static bool IsAllExportSuccess { get; set; } = true;
        bool postSharerDeleteExport;

        bool postSharerDeleteSuccess;
        public static int exportStatus { get; set; } = -1;
        public bool IsBackDateExport;
        public bool IsAutoExport;
        public bool IsNoDataToExport = false;
        
        Sales saleForm;

        int SharePackageId = 0;
        #endregion

        public SharerDeleteExport(int PackageId)
        {
            SharePackageId = PackageId;
            InitializeComponent();
        }

        private void SharerDeleteExport_Load(object sender, EventArgs e)
        {
            this.Text = "Sharer Delete Export";
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
            PurchasedPackageSharer exportList = new PurchasedPackageSharer();
            Customer customer = new Customer();
            exportList = entity.PurchasedPackageSharers.Where(t => t.Id == SharePackageId).FirstOrDefault();
            
            if (exportList != null)
            {
                DateTime dt = DateTime.Now;
                lblExportDate.Text = dt.ToString("dd/MM/yyyy");
                string PackageId = Convert.ToString(SharePackageId);
                APP_Data.ImportExportLog exlog = new APP_Data.ImportExportLog();
                exlog = entity.ImportExportLogs.Where(x => x.ExportedId == PackageId && x.ExportedType == "CancelSPExport").FirstOrDefault();

                if (exlog != null)
                {
                    switch (exlog.Status)
                    {
                        case "Success":
                            IsExportSuccess = true;
                            break;
                        case "Pending":
                            postSharerDeleteExport = true;
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
                                    case "CancelSPackage":
                                        postSharerDeleteExport = true;
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
                    resultID = ImportExportLog.CreateNewSharerCancelTrans(dt, Convert.ToString(SharePackageId));
                    if (resultID == 0)
                    {
                        lblExportProgress.Text = "New Export Log Fails..Data Export Cannot be Started...Please Try Again...!";
                        return;
                    }
                    postSharerDeleteExport = true;
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
                    ExecuteAPIs(SharePackageId);
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
                    case "CancelSPackage":
                        log.DetailStatus = postSharerDeleteSuccess ? "Success" : "Fail";
                        log.ResponseMessageFromSAP = string.Join(";", API_POST.postSharerDeleteResponseMessage.ToArray());
                        log.PostJson = API_POST.postTransJson;
                        log.ResponseJson = log.ResponseJson == null ? DateTime.Now.ToString() + ";" + API_POST.SharerDeleteJson : log.ResponseJson + ";[" + DateTime.Now.ToString() + "];" + API_POST.SharerDeleteJson;

                        break;
                }

            }
            entity.SaveChanges();
        }

        public void ExecuteAPIs(int Id)
        {
            int no0fProgressSteps = 3;
            int i = 1;
            ExportProgressBar.Value = 0; // reset progressbar
            ExportProgressBar.Maximum = no0fProgressSteps;
            IsExportSuccess = true;
            postSharerDeleteSuccess = true;

            ShowStatus("Step 1 of 2: Getting Access Token...", ++i, true);
            API_Token.Get_AccessToken();
            if (string.IsNullOrEmpty(API_Token.AccessToken) || string.IsNullOrWhiteSpace(API_Token.AccessToken))
            {
                ShowStatus(" Step 1 of 2: Getting Access Token Fails!..Cannot Export Data...", i, false);
                IsExportSuccess = false;
                postSharerDeleteSuccess = false;
                API_POST.postSharerDeleteResponseMessage.Add("No Access Token");
                return; // exit import and update log table status to 'fail'
            }

            ShowStatus("Step 1 of 2: Token Received Successfully!", i, true);
            
                if (postSharerDeleteExport)
                {
                    ShowStatus("Step 2 of 2: Sending Cancel Shared Package..Please Wait...", ++i, true);
                    API_POST.POST_PackageSharerDelete(Id);
                    if (!API_POST.postSharerDeleteSuccess)
                    {
                        if (API_POST.response != null && API_POST.response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            MessageBox.Show("Unauthorized Access! Please Refresh Access Token", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ShowStatus("Step 2 of 2: Sending Cancel Shared Package Invoices Fails...", i, false);
                            IsExportSuccess = false;
                            postSharerDeleteSuccess = false;
                        }

                        else
                        {
                            ShowStatus("Step 2 of 2: Sending Cancel Shared Package Invoices Fails...", i, false);
                            IsExportSuccess = false;
                            postSharerDeleteSuccess = false;
                        }

                    }
                    else
                    {
                        if (API_POST.postSharerDeleteResponseMessage != null && API_POST.postSharerDeleteResponseMessage.Contains("No Data to Export"))
                        {
                            ShowStatus("Step 2 of 2: No Cancel Shared Package Invoices to Export..Skip Sending", i, true);
                        }
                        else
                        {
                            ShowStatus("Step 2 of 2: Cancel Shared Package Invoices Exported Successfully..", i, true);
                        }

                    }

                }
                else
                {
                    ShowStatus("Step 2 of 2: Cancel Shared Package Already Exported...Skip Sendig...!", i, true);

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
