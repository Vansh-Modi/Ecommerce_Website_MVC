using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Admin
{
    public partial class admin_expenses : System.Web.UI.Page
    {
        private string ConnectionString => System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadOrdersDropdownSelector();
                LoadExpensesLedgerHistory();
            }
        }

        private void LoadOrdersDropdownSelector()
        {
            ddlOrdersLink.Items.Clear();
            string query = "SELECT OrderID, UserID, OrderDate FROM [dbo].[Orders] ORDER BY OrderID DESC";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string orderId = reader["OrderID"].ToString();
                                string dateStr = Convert.ToDateTime(reader["OrderDate"]).ToString("dd MMM yyyy");
                                string itemText = $"Order #{orderId} - Placed on {dateStr} (User #{reader["UserID"]})";
                                ddlOrdersLink.Items.Add(new ListItem(itemText, orderId));
                            }
                        }
                    }
                    catch { }
                }
            }
            ddlOrdersLink.Items.Insert(0, new ListItem("-- General Operational Expense (No Order Link) --", ""));
        }

        private void LoadExpensesLedgerHistory()
        {
            string query = "SELECT ExpenseID, Title, Amount, ExpenseType, ExpenseDate, OrderID, Remarks FROM [dbo].[Expenses] ORDER BY ExpenseDate DESC";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    try
                    {
                        conn.Open();
                        da.Fill(dt);

                        rptExpensesLedger.DataSource = dt;
                        rptExpensesLedger.DataBind();

                        decimal totalSum = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            totalSum += Convert.ToDecimal(row["Amount"]);
                        }
                        lblTotalExpensesSum.Text = string.Format("{0:N2}", totalSum);
                    }
                    catch (Exception ex)
                    {
                        DisplayMessage("❌ Ledger Hydration Error: " + ex.Message, false);
                    }
                }
            }
        }

        protected void btnSaveExpense_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            string title = txtTitle.Text.Trim();
            string expenseType = ddlExpenseType.SelectedValue;
            string remarks = txtRemarks.Text.Trim();
            string linkedOrderVal = ddlOrdersLink.SelectedValue;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                DisplayMessage("⚠️ Please assign an expense description title and specify the cost amount.", false);
                return;
            }

            decimal amount = 0;
            if (!decimal.TryParse(txtAmount.Text.Trim(), out amount) || amount <= 0)
            {
                DisplayMessage("❌ Invalid Input: Expense value metrics must map to a positive number.", false);
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string sql = "";
                bool isEditing = !string.IsNullOrEmpty(hfActiveExpenseID.Value);

                if (isEditing)
                {
                    // UPDATE Operation
                    sql = @"UPDATE [dbo].[Expenses] 
                            SET Title = @Title, Amount = @Amount, ExpenseType = @ExpenseType, OrderID = @OrderID, Remarks = @Remarks 
                            WHERE ExpenseID = @ExpenseID";
                }
                else
                {
                    // INSERT Operation
                    sql = @"INSERT INTO [dbo].[Expenses] (Title, Amount, ExpenseType, ExpenseDate, OrderID, Remarks)
                            VALUES (@Title, @Amount, @ExpenseType, @ExpenseDate, @OrderID, @Remarks)";
                }

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@ExpenseType", expenseType);
                    cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks);

                    if (string.IsNullOrEmpty(linkedOrderVal))
                        cmd.Parameters.AddWithValue("@OrderID", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@OrderID", Convert.ToInt32(linkedOrderVal));

                    if (isEditing)
                    {
                        cmd.Parameters.AddWithValue("@ExpenseID", Convert.ToInt32(hfActiveExpenseID.Value));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ExpenseDate", DateTime.Now);
                    }

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        DisplayMessage(isEditing ? "✓ Expense entry updated successfully." : "✓ Financial outflow statement logged successfully.", true);
                        ResetForm();
                        LoadExpensesLedgerHistory();
                    }
                    catch (Exception ex)
                    {
                        DisplayMessage("❌ Query Failure: " + ex.Message, false);
                    }
                }
            }
        }

        protected void rptExpensesLedger_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int expenseId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditExpense")
            {
                PopulateFormForEdit(expenseId);
            }
            else if (e.CommandName == "DeleteExpense")
            {
                DeleteExpenseEntry(expenseId);
            }
        }

        private void PopulateFormForEdit(int expenseId)
        {
            string query = "SELECT Title, Amount, ExpenseType, OrderID, Remarks FROM [dbo].[Expenses] WHERE ExpenseID = @ExpenseID";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ExpenseID", expenseId);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hfActiveExpenseID.Value = expenseId.ToString();
                                txtTitle.Text = reader["Title"].ToString();
                                txtAmount.Text = string.Format("{0:F2}", reader["Amount"]);
                                ddlExpenseType.SelectedValue = reader["ExpenseType"].ToString();

                                string linkedOrder = reader["OrderID"].ToString();
                                ddlOrdersLink.SelectedValue = string.IsNullOrEmpty(linkedOrder) ? "" : linkedOrder;

                                txtRemarks.Text = reader["Remarks"].ToString();

                                // Update UI Elements to reflect Editing state
                                litFormTitle.Text = "Modify Expense Entry #" + expenseId;
                                btnSaveExpense.Text = "Update Expense Entry";
                                btnCancelEdit.Visible = true;
                                lblMessage.Visible = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DisplayMessage("❌ Failed to fetch editing parameters: " + ex.Message, false);
                    }
                }
            }
        }

        private void DeleteExpenseEntry(int expenseId)
        {
            string query = "DELETE FROM [dbo].[Expenses] WHERE ExpenseID = @ExpenseID";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ExpenseID", expenseId);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        DisplayMessage("✓ Expense record deleted permanently from ledger systems.", true);

                        // If deleting the record that is currently being edited, reset the form panel
                        if (hfActiveExpenseID.Value == expenseId.ToString())
                        {
                            ResetForm();
                        }

                        LoadExpensesLedgerHistory();
                    }
                    catch (Exception ex)
                    {
                        DisplayMessage("❌ Error processing record deletion: " + ex.Message, false);
                    }
                }
            }
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            hfActiveExpenseID.Value = "";
            txtTitle.Text = "";
            txtAmount.Text = "";
            txtRemarks.Text = "";
            ddlExpenseType.SelectedIndex = 0;
            ddlOrdersLink.SelectedIndex = 0;

            litFormTitle.Text = "Log Outbound Expense";
            btnSaveExpense.Text = "Log Expense Entry";
            btnCancelEdit.Visible = false;
        }

        private void DisplayMessage(string text, bool isSuccess)
        {
            lblMessage.Text = text;
            lblMessage.CssClass = isSuccess ? "admin-alert-banner alert-success" : "admin-alert-banner alert-error";
            lblMessage.Visible = true;
        }
    }
}