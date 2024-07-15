using POS.APP_Data;
using POS.mPOSUI.CustomerPurchaseInvoice;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace POS
{
    public partial class PurchasedInvoiceSharers : Form
    {
        #region Variables

        POSEntities posEntity = new POSEntities();
        private ToolTip tp = new ToolTip();
        public string PurchasedInvoiceId { get; set; }
        public string PackageName { get; set; }
        public string OwnerName { get; set; }
        public int OwnerId { get; set; }
        private int CurrentCustomerId = 0;
        public int SharedPackageId = 0;
        bool success = false;
        #endregion

        #region Event
        public PurchasedInvoiceSharers()
        {
            InitializeComponent();
        }

        private void PurchasedInvoiceShares_Load(object sender, EventArgs e)
        {
            CurrentCustomerId = OwnerId;
            Localization.Localize_FormControls(this);
            dgvInvoiceSharers.AutoGenerateColumns = false;
            lblPackageName.Text = PackageName;
            lblPackageOwner.Text = OwnerName;
            BindCustomer();
            BindDataGridView();
         }

        private void BindDataGridView()
        {

            dgvInvoiceSharers.DataSource = posEntity.PurchasedPackageSharers.Where(b=>b.PackageInvoiceId == PurchasedInvoiceId && b.IsDeleted == false).OrderByDescending(x=>x.Id).Select(x=> new SharedInvoiceComboSet(){ CustomerName=x.Customer1.Name, SharedDateTime=x.SharedDateTime, Id=x.Id }).ToList();

        }
        
        private void BindCustomer()
        {
            //Add Customer List with default option
            List<APP_Data.Customer> customerList = (from c in posEntity.Customers orderby c.Name select c).ToList();
            cboCustmer.DataSource = customerList;
            cboCustmer.DisplayMember = "Name";
            cboCustmer.ValueMember = "Id";
            cboCustmer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboCustmer.AutoCompleteSource = AutoCompleteSource.ListItems;
        }
        private void Save()
        {
            tp.RemoveAll();
            tp = new ToolTip();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            if (!string.IsNullOrEmpty(cboCustmer.Text))
            {
                int customerId = 0;
                int.TryParse(cboCustmer.SelectedValue.ToString(), out customerId);
                APP_Data.PurchasedPackageSharer bObj = (from b in posEntity.PurchasedPackageSharers where b.PackageInvoiceId == PurchasedInvoiceId && b.IsDeleted == false && (b.PackageOwnerId==customerId || b.SharedCustomerId==customerId) select b).FirstOrDefault();
                if (bObj == null)
                {
                    PurchasedPackageSharer newsharer = new PurchasedPackageSharer()
                    {
                        PackageInvoiceId = PurchasedInvoiceId,
                        PackageOwnerId = OwnerId,
                        SharedCustomerId = customerId,
                        SharedDateTime = DateTime.Now,
                        IsDeleted = false,
                        IsExported = false
                    };
                    CurrentCustomerId = customerId;
                    posEntity.PurchasedPackageSharers.Add(newsharer);
                    posEntity.SaveChanges();
                    BindDataGridView();

                    DialogResult result = MessageBox.Show("Successfully Saved and Ready to Export!", "mPOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        SharerExport exportSharer = new SharerExport(newsharer.Id.ToString());
                        exportSharer.Text = "Export Sharer";
                        exportSharer.ShowDialog();
                    }
                }
                else
                {
                    tp.SetToolTip(cboCustmer, "Error");
                    tp.Show("This customer is current package owner or sharer!", cboCustmer);
                }
            }
            else
            {
                tp.SetToolTip(cboCustmer, "Error");
                tp.Show("Please choose a customer!", cboCustmer);
            }
            cboCustmer.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Save();
            
        }

        private void AdjustmentType_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(cboCustmer);
        }







        #endregion

        private void PurchasedInvoiceSharers_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (System.Windows.Forms.Application.OpenForms["UsePackageByCustomer"] != null)
            {
                UsePackageByCustomer newForm = (UsePackageByCustomer)System.Windows.Forms.Application.OpenForms["UsePackageByCustomer"];
                newForm.bindPackageSharedUser();
                newForm.SetCurrentUser(CurrentCustomerId);
            }
        }

        private void dgvInvoiceSharers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                POSEntities entity = new POSEntities();
                int currentId = Convert.ToInt32(dgvInvoiceSharers.Rows[e.RowIndex].Cells[3].Value.ToString());
                if(e.ColumnIndex == 2)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        SharedPackageId = currentId;
                        MessageShow();
                        if (success)
                        {
                            PurchasedPackageSharer sharer = entity.PurchasedPackageSharers.Where(x => x.Id == currentId).SingleOrDefault();
                            sharer.IsDeleted = true;
                            entity.Entry(sharer).State = EntityState.Modified;
                            entity.SaveChanges();
                            BindCustomer();
                            BindDataGridView();
                        }
                    }
                }
            }
        }

        private DialogResult MessageShow()
        {
            DialogResult result = new DialogResult();
            string transId = string.Empty;

            result = MessageBox.Show("Prepare to delete and wait to export transaction", "mPOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (result.Equals(DialogResult.OK))
            {
                PaidByCash2 sForm = (PaidByCash2)this.ActiveMdiChild;
                SharerDeleteExport exportForm = new SharerDeleteExport(SharedPackageId);
                exportForm.Text = "Export Sharer Package Delete";
                exportForm.ShowDialog();
                success = exportForm.IsExportSuccess;
            }
            return result;
        }

    }
    class SharedInvoiceComboSet
    {
        public string CustomerName { get; set; }
        public DateTime SharedDateTime { get; set; }
        public int Id { get; set; }
    }
}
