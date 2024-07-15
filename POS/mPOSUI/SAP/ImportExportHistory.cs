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
using POS.APP_Data;

namespace POS
{
    public partial class ImportExportHistory : Form
    {
        public ImportExportHistory()
        {
            InitializeComponent();
        }

        private void ImportExportHistory_Load(object sender, EventArgs e)
        {
            BindImportExportHistory();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            BindImportExportHistory();
        }

        public void BindImportExportHistory()
        {
            POSEntities entity = new POSEntities();
            DateTime startDate = StartDatedateTimePicker.Value.Date;
            DateTime endDate = EndDatedateTimePicker.Value.Date;
            List<GetImportExportHistory_Result> ImportExportHistroyList = entity.GetImportExportHistory(startDate, endDate).ToList();

            dgvImportHistory.DataSource = "";
            dgvImportHistory.AutoGenerateColumns = false;
            dgvImportHistory.DataSource = ImportExportHistroyList.Where(x => x.Type == "Import").ToList();

            dgvExportHistory.DataSource = "";
            dgvExportHistory.AutoGenerateColumns = false;
            dgvExportHistory.DataSource = ImportExportHistroyList.Where(x => x.Type == "Export").ToList();

        }

        private void dgvImportHistory_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvImportHistory.Rows)
            {
                // ImportExportLog log = (ImportExportLog)row.DataBoundItem;
                GetImportExportHistory_Result log = (GetImportExportHistory_Result)row.DataBoundItem;
                row.Cells[colBatch.Index].Value = log.ProcessingBatch;
                row.Cells[colDateTime.Index].Value = log.ProcessingDateTime;
                row.Cells[ColLastProcessingDateTime.Index].Value = log.LastProcessingDateTime;
                row.Cells[colType.Index].Value = log.Type;
                row.Cells[colStatus.Index].Value = log.Status;
                row.Cells[colBatchID.Index].Value = log.Id;

            }
        }

        private void dgvExportHistory_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvExportHistory.Rows)
            {
                //ImportExportLog log = (ImportExportLog)row.DataBoundItem;
                GetImportExportHistory_Result log = (GetImportExportHistory_Result)row.DataBoundItem;
                row.Cells[colEBatch.Index].Value = log.ProcessingBatch;
                row.Cells[ColEDate.Index].Value = log.ProcessingDateTime;
                row.Cells[ColELastProcessingDateTime.Index].Value = log.LastProcessingDateTime;
                row.Cells[ColEType.Index].Value = log.Type;
                row.Cells[ColEStatus.Index].Value = log.Status;
                row.Cells[colEBatchID.Index].Value = log.Id;

                POSEntities entity = new POSEntities();
                APP_Data.ImportExportLog exlog = new APP_Data.ImportExportLog();
                long BatchId = 0;
                BatchId = (long)row.Cells[colEBatchID.Index].Value;
                exlog = entity.ImportExportLogs.Where(x => x.Id == BatchId).FirstOrDefault();
                if (exlog != null)
                {
                    row.Cells[colEExportedId.Index].Value = exlog.ExportedId;
                    row.Cells[colEExportedType.Index].Value = exlog.ExportedType;
                }
            }
        }

        private void dgvImportHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if (e.ColumnIndex == colDetails.Index)
                {
                    ImportExportLogDetails logDetailForm = new ImportExportLogDetails();
                    logDetailForm.BatchID = (long)dgvImportHistory.Rows[e.RowIndex].Cells[colBatchID.Index].Value;
                    logDetailForm.BatchNo = dgvImportHistory.Rows[e.RowIndex].Cells[colBatch.Index].Value.ToString();
                    logDetailForm.Type = "Import";
                    logDetailForm.ShowDialog();
                }
            }
        }

        private void dgvExportHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex > -1)
            {
                if (e.ColumnIndex == colEDetails.Index)
                {
                    ImportExportLogDetails logDetailForm = new ImportExportLogDetails();
                    logDetailForm.BatchID = (long)dgvExportHistory.Rows[e.RowIndex].Cells[colEBatchID.Index].Value;
                    logDetailForm.BatchNo = dgvExportHistory.Rows[e.RowIndex].Cells[colEBatch.Index].Value.ToString();
                    logDetailForm.Type = "Export";
                    logDetailForm.ShowDialog();
                }

                if (e.ColumnIndex == colEExport.Index)
                {
                    string status = "";
                    status = dgvExportHistory.Rows[e.RowIndex].Cells[ColEStatus.Index].Value.ToString();
                    if (status == "Success")
                    {
                        DialogResult result = MessageBox.Show("Already exported!", "mPOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string ExportedId = "";
                        string ExportedType = "";
                        ExportedId = (string)dgvExportHistory.Rows[e.RowIndex].Cells[colEExportedId.Index].Value;
                        ExportedType = (string)dgvExportHistory.Rows[e.RowIndex].Cells[colEExportedType.Index].Value;

                        if (ExportedType == "CustomerExport")
                        {
                            CustomerExport exportCustomer = new CustomerExport(ExportedId);
                            exportCustomer.Text = "Export Patient";
                            exportCustomer.ShowDialog();
                            BindImportExportHistory();
                        }
                        else if (ExportedType == "SharerExport")
                        {
                            SharerExport exportSharer = new SharerExport(ExportedId);
                            exportSharer.Text = "Export Package Sharer";
                            exportSharer.ShowDialog();
                            BindImportExportHistory();
                        }
                        else if (ExportedType == "PackageUsedExport")
                        {
                            PackageUsedHistoryExport exportPackageUsed = new PackageUsedHistoryExport(ExportedId);
                            exportPackageUsed.Text = "Export Package Used";
                            exportPackageUsed.ShowDialog();
                            BindImportExportHistory();
                        }
                        else if (ExportedType == "CancelCreditTransExport")
                        {
                            CancelCreditTransExport exportCancelCreditTrans = new CancelCreditTransExport(ExportedId, true);
                            exportCancelCreditTrans.Text = "Export Cancelled Package";
                            exportCancelCreditTrans.ShowDialog();
                            BindImportExportHistory();
                        }
                        else if (ExportedType == "DeleteTransExport")
                        {
                            CancelCreditTransExport exportCancelCreditTrans = new CancelCreditTransExport(ExportedId, false);
                            exportCancelCreditTrans.Text = "Export Deleted Transaction";
                            exportCancelCreditTrans.ShowDialog();
                            BindImportExportHistory();
                        }
                        else if (ExportedType == "TransactionExport")
                        {
                            ImportExportHistory sForm = (ImportExportHistory)this.ActiveMdiChild;
                            TransactionExport exportTransaction = new TransactionExport(ExportedId, 0);
                            exportTransaction.Text = "Export Transaction";
                            exportTransaction.ShowDialog();
                            BindImportExportHistory();
                        }
                    }
                }
            }
        }
    }
}
