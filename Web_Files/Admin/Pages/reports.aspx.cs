using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace ShowsGarage.Web_Files.Admin
{
    public partial class admin_pl_report : System.Web.UI.Page
    {
        private string ConnectionString => System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        public decimal ValueCarSales { get; set; } = 0;
        public decimal ValueShippingCollected { get; set; } = 0;
        public decimal ValueActualShippingCost { get; set; } = 0;
        public decimal ValueSourcingCost { get; set; } = 0;
        public decimal ValueTotalExpenses { get; set; } = 0;

        public int CountApproved { get; set; }
        public int CountAwaitingVerify { get; set; }
        public int CountAwaitingPay { get; set; }
        public int CountCancelled { get; set; }

        public int CountFirstTimeBuyers { get; set; } = 0;
        public int CountRepeatCollectors { get; set; } = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CalculateGarageProfitLossStatement();
            }
        }

        private void CalculateGarageProfitLossStatement()
        {
            lblMessage.Visible = false;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();

                    // 1. PIPELINE ENGINE ANALYSIS
                    string pipelineCountsSql = @"
                        SELECT 
                            ISNULL(SUM(CASE WHEN Status IN ('Approved','Shipped','Delivered') THEN 1 ELSE 0 END), 0) AS ApprovedCount,
                            ISNULL(SUM(CASE WHEN Status = 'Awaiting Verification' THEN 1 ELSE 0 END), 0) AS VerifyCount,
                            ISNULL(SUM(CASE WHEN Status = 'Awaiting Payment' THEN 1 ELSE 0 END), 0) AS PayCount,
                            ISNULL(SUM(CASE WHEN Status = 'Cancelled' THEN 1 ELSE 0 END), 0) AS CancelledCount,
                            ISNULL(SUM(CASE WHEN Status IN ('Approved','Shipped','Delivered') THEN 1 ELSE 0 END), 0) AS TotalApprovedOrders,
                            ISNULL(SUM(CASE WHEN Status = 'Cancelled' THEN TotalAmount ELSE 0 END), 0) AS RevenueLeakage,
                            ISNULL(SUM(CASE WHEN Status <> 'Cancelled' THEN TotalAmount ELSE 0 END), 0) AS GrossOrderVolume
                        FROM [dbo].[Orders]";

                    using (SqlCommand cmd = new SqlCommand(pipelineCountsSql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                CountApproved = Convert.ToInt32(reader["ApprovedCount"]);
                                CountAwaitingVerify = Convert.ToInt32(reader["VerifyCount"]);
                                CountAwaitingPay = Convert.ToInt32(reader["PayCount"]);
                                CountCancelled = Convert.ToInt32(reader["CancelledCount"]);

                                int approvedOrdersTotalCount = Convert.ToInt32(reader["TotalApprovedOrders"]);
                                litTotalOrdersCount.Text = approvedOrdersTotalCount.ToString();
                                lblRevenueLeakage.Text = string.Format("{0:N2}", Convert.ToDecimal(reader["RevenueLeakage"]));
                                lblGrossOrderVolume.Text = string.Format("{0:N2}", Convert.ToDecimal(reader["GrossOrderVolume"]));
                            }
                        }
                    }

                    // 2. PRODUCT CAR SALES
                    string carRetailSalesSql = @"
                        SELECT ISNULL(SUM(od.Quantity * od.UnitPrice), 0)
                        FROM [dbo].[OrderDetails] od
                        INNER JOIN [dbo].[Orders] o ON od.OrderID = o.OrderID
                        WHERE o.Status IN ('Approved','Shipped','Delivered')";

                    using (SqlCommand cmdCarSales = new SqlCommand(carRetailSalesSql, conn))
                    {
                        ValueCarSales = Convert.ToDecimal(cmdCarSales.ExecuteScalar());
                        lblCarSalesRevenue.Text = string.Format("{0:N2}", ValueCarSales);
                    }

                    // 3. COMBINED INFLOWS & SHIPPING ISOLATION
                    string totalAmountApprovedSql = @"
                        SELECT ISNULL(SUM(TotalAmount), 0) 
                        FROM [dbo].[Orders] 
                        WHERE Status IN ('Approved','Shipped','Delivered')";

                    using (SqlCommand cmdTotalApproved = new SqlCommand(totalAmountApprovedSql, conn))
                    {
                        decimal combinedSalesTotal = Convert.ToDecimal(cmdTotalApproved.ExecuteScalar());
                        litGrossCombinedTotal.Text = string.Format("{0:N2}", combinedSalesTotal);

                        ValueShippingCollected = Math.Max(0, combinedSalesTotal - ValueCarSales);
                        lblShippingCollected.Text = string.Format("{0:N2}", ValueShippingCollected);

                        int approvedCountCheck = Convert.ToInt32(litTotalOrdersCount.Text);
                        decimal avgOrderValue = approvedCountCheck > 0 ? (combinedSalesTotal / approvedCountCheck) : 0.00m;
                        lblAvgOrderValue.Text = string.Format("{0:N2}", avgOrderValue);
                    }

                    // 4. SHIPPING OUTFLOW MATRIX
                    string actualShippingCostSql = "SELECT ISNULL(SUM(Amount), 0) FROM [dbo].[Expenses] WHERE ExpenseType = 'Shipping'";
                    using (SqlCommand cmdActualShipping = new SqlCommand(actualShippingCostSql, conn))
                    {
                        ValueActualShippingCost = Convert.ToDecimal(cmdActualShipping.ExecuteScalar());
                    }

                    // 5. COGS PROCUREMENT
                    string sourcingCostSql = @"
                        SELECT ISNULL(SUM(od.Quantity * p.CostPrice), 0)
                        FROM [dbo].[OrderDetails] od
                        INNER JOIN [dbo].[Products] p ON od.ProductID = p.ProductID
                        INNER JOIN [dbo].[Orders] o ON od.OrderID = o.OrderID
                        WHERE o.Status IN ('Approved','Shipped','Delivered')";

                    using (SqlCommand cmdCost = new SqlCommand(sourcingCostSql, conn))
                    {
                        ValueSourcingCost = Convert.ToDecimal(cmdCost.ExecuteScalar());
                        lblSourcingCost.Text = string.Format("{0:N2}", ValueSourcingCost);
                    }

                    // 6. GENERAL OPERATIONAL EXPENSES OVERHEAD
                    string expensesQuery = "SELECT ISNULL(SUM(Amount), 0) FROM [dbo].[Expenses] WHERE ExpenseType <> 'Damaged Return'";
                    using (SqlCommand cmdExp = new SqlCommand(expensesQuery, conn))
                    {
                        ValueTotalExpenses = Convert.ToDecimal(cmdExp.ExecuteScalar());
                        lblOperatingExpenses.Text = string.Format("{0:N2}", ValueTotalExpenses);
                    }

                    // 7. STOCK VALUATION RUNTIME
                    string stockValuationQuery = "SELECT ISNULL(SUM(StockQuantity * CostPrice), 0) FROM [dbo].[Products]";
                    using (SqlCommand cmdStock = new SqlCommand(stockValuationQuery, conn))
                    {
                        decimal stockValuationValue = Convert.ToDecimal(cmdStock.ExecuteScalar());
                        lblStockValuation.Text = string.Format("{0:N2}", stockValuationValue);
                    }

                    // 8. COHORT GROUP REGISTRY ANALYSIS
                    string cohortSql = @"
                        WITH UserOrderCounts AS (
                            SELECT UserID, COUNT(OrderID) AS OrderCount
                            FROM [dbo].[Orders]
                            WHERE Status IN ('Approved','Shipped','Delivered')
                            GROUP BY UserID
                        )
                        SELECT 
                            ISNULL(SUM(CASE WHEN OrderCount = 1 THEN 1 ELSE 0 END), 0) AS FirstTimeCount,
                            ISNULL(SUM(CASE WHEN OrderCount >= 2 THEN 1 ELSE 0 END), 0) AS RepeatCount
                        FROM UserOrderCounts";

                    using (SqlCommand cmdCohort = new SqlCommand(cohortSql, conn))
                    {
                        using (SqlDataReader cohortReader = cmdCohort.ExecuteReader())
                        {
                            if (cohortReader.Read())
                            {
                                CountFirstTimeBuyers = Convert.ToInt32(cohortReader["FirstTimeCount"]);
                                CountRepeatCollectors = Convert.ToInt32(cohortReader["RepeatCount"]);
                            }
                        }
                    }

                    // 9. BALANCE NET TARGET DIAGNOSTICS
                    decimal totalRevenueStream = ValueCarSales + ValueShippingCollected;
                    decimal totalOutflowStream = ValueSourcingCost + ValueTotalExpenses;
                    decimal netMargin = totalRevenueStream - totalOutflowStream;

                    lblNetMargin.Text = string.Format("{0:N2}", Math.Abs(netMargin));

                    if (netMargin >= 0)
                    {
                        divNetProfitCard.Attributes["class"] = "pl-stat-card state-profit";
                        divNetProfitCard.Style["border-top"] = "3px solid #2ebd59";
                        lblNetStatusIcon.Text = "🟢";
                    }
                    else
                    {
                        divNetProfitCard.Attributes["class"] = "pl-stat-card state-loss";
                        divNetProfitCard.Style["border-top"] = "3px solid #f44336";
                        lblNetStatusIcon.Text = "🔴";
                        lblNetMargin.Text = "-" + lblNetMargin.Text;
                    }

                    // BIND GENERAL OVERHEAD EXPENSE LEDGER BREAKDOWN
                    string breakdownQuery = @"
                        SELECT ExpenseType, SUM(Amount) AS CategoryTotal 
                        FROM [dbo].[Expenses] 
                        WHERE ExpenseType <> 'Damaged Return'
                        GROUP BY ExpenseType 
                        ORDER BY CategoryTotal DESC";

                    using (SqlCommand cmdBreakdown = new SqlCommand(breakdownQuery, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmdBreakdown);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        rptExpenseTypeBreakdown.DataSource = dt;
                        rptExpenseTypeBreakdown.DataBind();
                    }

                    // BIND RECENT AUDITED LOG ROWS
                    string manifestQuery = @"
                        SELECT OrderID, UserID, OrderDate, TotalAmount, Status, PaymentMethod,
                               ISNULL(NULLIF(SUBSTRING(ShippingAddress, CHARINDEX('Name: ', ShippingAddress) + 6, LEN(ShippingAddress)), ''), 'User #' + CAST(UserID AS VARCHAR)) as ShippingName
                        FROM [dbo].[Orders] 
                        WHERE Status IN ('Approved','Shipped','Delivered')
                        ORDER BY OrderDate DESC";

                    using (SqlCommand cmdManifest = new SqlCommand(manifestQuery, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmdManifest);
                        DataTable dtManifest = new DataTable();
                        da.Fill(dtManifest);
                        gvOrderManifest.DataSource = dtManifest;
                        gvOrderManifest.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "❌ Financial Ledger Analytics Compilation Failure: " + ex.Message;
                    lblMessage.Visible = true;
                }
            }
        }
    }
}