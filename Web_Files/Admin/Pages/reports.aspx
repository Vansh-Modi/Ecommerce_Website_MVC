<%@ Page Title="P/L Analytics Report | Admin" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="report.aspx.cs" Inherits="ShowsGarage.Web_Files.Admin.admin_pl_report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>P&L Analytics Dashboard | Admin Hub</title>
    <link href="/Web_Files/Admin/Styles/siteSettings.css?v=2" rel="stylesheet" type="text/css" />
    <link href="/Web_Files/Admin/Styles/report.css?v=3" rel="stylesheet" type="text/css" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    
    <style type="text/css">
        @media print {
            body * { visibility: hidden; }
            #printableDashboardArea, #printableDashboardArea * { visibility: visible; }
            #printableDashboardArea { position: absolute; left: 0; top: 0; width: 100%; background: #fff !important; }
            .pl-stat-card, .settings-card-panel { background: #fff !important; border: 1px solid #ccc !important; color: #000 !important; box-shadow: none !important; }
            .admin-main-heading, .chart-card-title, .settings-card-title, span, td, th { color: #000 !important; }
            .admin-btn-export, .admin-pill-badge { display: none !important; }
            .admin-pl-breakdown-table th { background: #f0f0f0 !important; color: #000 !important; border-bottom: 2px solid #000 !important; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="printableDashboardArea" class="admin-theme-wrapper">
        <main class="admin-content-container wide-fluid-layout">
            
            <div class="admin-controls-bar" style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 25px;">
                <div>
                    <h1 class="admin-main-heading" style="margin: 0;">Business Intelligence & P/L Analytics</h1>
                    <span class="admin-pill-badge badge-financial">Comprehensive Store Audit</span>
                </div>
                <button type="button" class="admin-btn-export" onclick="window.print();" style="background: #0076df; color: #fff; border: none; padding: 10px 20px; font-weight: bold; border-radius: 4px; cursor: pointer;">
                    📥 Download Full Report (PDF)
                </button>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="admin-alert-banner alert-error" Visible="false"></asp:Label>

            <div class="pl-overview-cards-grid" style="display: grid; grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); gap: 20px; margin-bottom: 30px; width: 100%;">
                <div class="pl-stat-card" style="background: #111; border: 1px solid #222; border-top: 3px solid #2ebd59; padding: 20px; border-radius: 6px;">
                    <div style="display: flex; justify-content: space-between; align-items: center; color: #888; font-size: 11px; font-weight: 700; text-transform: uppercase;"><span>1) Car Selling Revenue</span><span>🚗</span></div>
                    <div style="font-size: 24px; font-weight: 800; color: #fff; margin: 12px 0 4px 0;">Rs.<asp:Label ID="lblCarSalesRevenue" runat="server" Text="0.00"></asp:Label></div>
                    <div style="color: #666; font-size: 12px;">Model item value (Approved).</div>
                </div>

                <div class="pl-stat-card" style="background: #111; border: 1px solid #222; border-top: 3px solid #0076df; padding: 20px; border-radius: 6px;">
                    <div style="display: flex; justify-content: space-between; align-items: center; color: #888; font-size: 11px; font-weight: 700; text-transform: uppercase;"><span>2) Shipping Fees Collected</span><span>📦</span></div>
                    <div style="font-size: 24px; font-weight: 800; color: #fff; margin: 12px 0 4px 0;">Rs.<asp:Label ID="lblShippingCollected" runat="server" Text="0.00"></asp:Label></div>
                    <div style="color: #666; font-size: 12px;">Delivery fees accumulated.</div>
                </div>

                <div class="pl-stat-card" style="background: #111; border: 1px solid #222; border-top: 3px solid #9c27b0; padding: 20px; border-radius: 6px;">
                    <div style="display: flex; justify-content: space-between; align-items: center; color: #888; font-size: 11px; font-weight: 700; text-transform: uppercase;"><span>3) Stock Valuation</span><span>🏬</span></div>
                    <div style="font-size: 24px; font-weight: 800; color: #fff; margin: 12px 0 4px 0;">Rs.<asp:Label ID="lblStockValuation" runat="server" Text="0.00"></asp:Label></div>
                    <div style="color: #666; font-size: 12px;">Active warehouse stock value.</div>
                </div>

                <div class="pl-stat-card" style="background: #111; border: 1px solid #222; border-top: 3px solid #ff9800; padding: 20px; border-radius: 6px;">
                    <div style="display: flex; justify-content: space-between; align-items: center; color: #888; font-size: 11px; font-weight: 700; text-transform: uppercase;"><span>4) Inventory Purchase Cost</span><span>🛒</span></div>
                    <div style="font-size: 24px; font-weight: 800; color: #ff9800; margin: 12px 0 4px 0;">Rs.<asp:Label ID="lblSourcingCost" runat="server" Text="0.00"></asp:Label></div>
                    <div style="color: #666; font-size: 12px;">Wholesale COGS (Approved items).</div>
                </div>

                <div class="pl-stat-card" style="background: #111; border: 1px solid #222; border-top: 3px solid #f44336; padding: 20px; border-radius: 6px;">
                    <div style="display: flex; justify-content: space-between; align-items: center; color: #888; font-size: 11px; font-weight: 700; text-transform: uppercase;"><span>5) Operating Expenses</span><span>💸</span></div>
                    <div style="font-size: 24px; font-weight: 800; color: #fff; margin: 12px 0 4px 0;">Rs.<asp:Label ID="lblOperatingExpenses" runat="server" Text="0.00"></asp:Label></div>
                    <div style="color: #666; font-size: 12px;">General ledger expenses overhead.</div>
                </div>

                <div id="divNetProfitCard" runat="server" class="pl-stat-card" style="background: #111; border: 1px solid #222; border-top: 3px solid #2ebd59; padding: 20px; border-radius: 6px;">
                    <div style="display: flex; justify-content: space-between; align-items: center; color: #888; font-size: 11px; font-weight: 700; text-transform: uppercase;">
                        <span>6) Net Profit Margin</span>
                        <asp:Label ID="lblNetStatusIcon" runat="server" style="font-size: 16px;" Text="🟢"></asp:Label>
                    </div>
                    <div style="font-size: 24px; font-weight: 800; color: #2ebd59; margin: 12px 0 4px 0;">
                        Rs.<asp:Label ID="lblNetMargin" runat="server" Text="0.00"></asp:Label>
                    </div>
                    <div style="color: #666; font-size: 12px;">Calculated net margins.</div>
                </div>

                <div class="pl-stat-card" style="background: #111; border: 1px solid #222; border-top: 3px solid #e5ba6b; padding: 20px; border-radius: 6px;">
                    <div style="display: flex; justify-content: space-between; align-items: center; color: #888; font-size: 11px; font-weight: 700; text-transform: uppercase;"><span>7) Average Order Value (AOV)</span><span>📊</span></div>
                    <div style="font-size: 24px; font-weight: 800; color: #fff; margin: 12px 0 4px 0;">Rs.<asp:Label ID="lblAvgOrderValue" runat="server" Text="0.00"></asp:Label></div>
                    <div style="color: #666; font-size: 12px;">Average basket spend per checkout.</div>
                </div>

                <div class="pl-stat-card" style="background: #111; border: 1px solid #222; border-top: 3px solid #ff4d4d; padding: 20px; border-radius: 6px;">
                    <div style="display: flex; justify-content: space-between; align-items: center; color: #888; font-size: 11px; font-weight: 700; text-transform: uppercase;"><span>8) Value of Cancelled Orders</span><span>⚠️</span></div>
                    <div style="font-size: 24px; font-weight: 800; color: #ff4d4d; margin: 12px 0 4px 0;">Rs.<asp:Label ID="lblRevenueLeakage" runat="server" Text="0.00"></asp:Label></div>
                    <div style="color: #666; font-size: 12px;">Voided/Returned transaction funds.</div>
                </div>

                <div class="pl-stat-card" style="background: #111; border: 1px solid #222; border-top: 3px solid #00abcd; padding: 20px; border-radius: 6px;">
                    <div style="display: flex; justify-content: space-between; align-items: center; color: #888; font-size: 11px; font-weight: 700; text-transform: uppercase;"><span>9) Gross Pipeline Orders Value</span><span>📋</span></div>
                    <div style="font-size: 24px; font-weight: 800; color: #fff; margin: 12px 0 4px 0;">Rs.<asp:Label ID="lblGrossOrderVolume" runat="server" Text="0.00"></asp:Label></div>
                    <div style="color: #666; font-size: 12px;">Total incoming pipeline value.</div>
                </div>
            </div>

            <div class="operational-summary-strip-row" style="margin-top: 20px; margin-bottom: 30px;">
                <div class="strip-node">Total Approved Orders Volume: <strong><asp:Literal ID="litTotalOrdersCount" runat="server" Text="0"></asp:Literal></strong></div>
                <div class="strip-node">Gross Income Statement (Revenue + Shipping): <strong style="color: #2ebd59;">Rs.<asp:Literal ID="litGrossCombinedTotal" runat="server" Text="0.00"></asp:Literal></strong></div>
            </div>

            <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(450px, 1fr)); gap: 20px; margin-bottom: 30px;">
                <div class="settings-card-panel chart-wrapper-card">
                    <h3 class="chart-card-title">Refined Profitability & Cash Flow Profile</h3>
                    <div class="canvas-chart-container" style="position: relative; height:320px;">
                        <canvas id="financialTrajectoryChart"></canvas>
                    </div>
                </div>

                <div class="settings-card-panel chart-wrapper-card">
                    <h3 class="chart-card-title">Total Outflow Leakage Monitor (Shipping + Expenses)</h3>
                    <div class="canvas-chart-container" style="position: relative; height:320px;">
                        <canvas id="carrierLeakageChart"></canvas>
                    </div>
                </div>
            </div>

            <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(450px, 1fr)); gap: 20px; margin-bottom: 30px;">
                <div class="settings-card-panel chart-wrapper-card">
                    <h3 class="chart-card-title">Fulfillment Pipelines Segmentation</h3>
                    <div class="canvas-chart-container" style="position: relative; height:320px;">
                        <canvas id="orderPipelinePieChart"></canvas>
                    </div>
                </div>

                <div class="settings-card-panel chart-wrapper-card">
                    <h3 class="chart-card-title">Repeat Collector Retention Analytics</h3>
                    <div class="canvas-chart-container" style="position: relative; height:320px;">
                        <canvas id="collectorCohortDoughnutChart"></canvas>
                    </div>
                </div>
            </div>

            <div class="pl-breakdown-container-panel" style="margin-top: 30px;">
                <div class="settings-card-panel premium-border">
                    <h2 class="settings-card-title">Categorized Expense Distribution Ledger</h2>
                    <div class="pl-table-responsive-wrapper">
                        <asp:Repeater ID="rptExpenseTypeBreakdown" runat="server">
                           <HeaderTemplate>
                                <table class="admin-pl-breakdown-table">
                                    <thead>
                                        <tr>
                                            <th>Expense Category Allocation Type</th>
                                            <th style="text-align: right; width: 250px;">Total Outflow Accumulation</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="pl-cell-category"><span class="pl-bullet-accent"></span><%# Eval("ExpenseType") %></td>
                                    <td class="pl-cell-value">Rs.<%# string.Format("{0:N2}", Eval("CategoryTotal")) %></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                    </tbody>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>

            <div class="pl-breakdown-container-panel" style="margin-top: 30px;">
                <div class="settings-card-panel premium-border">
                    <h2 class="settings-card-title">Detailed Order History Manifest (Approved Track)</h2>
                    <div class="pl-table-responsive-wrapper">
                        <asp:GridView ID="gvOrderManifest" runat="server" AutoGenerateColumns="False" CssClass="admin-pl-breakdown-table" GridLines="None" Width="100%">
                            <Columns>
                                <asp:BoundField DataField="OrderID" HeaderText="Order ID" ItemStyle-Font-Bold="true" ItemStyle-ForeColor="#0076df" />
                                <asp:BoundField DataField="ShippingName" HeaderText="Customer / Consignee" />
                                <asp:BoundField DataField="OrderDate" HeaderText="Date Logged" DataFormatString="{0:dd MMM yyyy HH:mm}" />
                                <asp:BoundField DataField="PaymentMethod" HeaderText="Payment Mode" />
                                <asp:BoundField DataField="Status" HeaderText="Fulfillment Status" />
                                <asp:TemplateField HeaderText="Total Received" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <strong style="color: #2ebd59;">Rs.<%# string.Format("{0:N2}", Eval("TotalAmount")) %></strong>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>

        </main>
    </div>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", function () {
            const ctxTrajectory = document.getElementById('financialTrajectoryChart').getContext('2d');
            new Chart(ctxTrajectory, {
                data: {
                    labels: ['Current Cumulative Statement'],
                    datasets: [
                        { type: 'bar', label: 'Car Sales Revenue', data: [<%= ValueCarSales %>], backgroundColor: '#2ebd59' },
                        { type: 'bar', label: 'Shipping Collections', data: [<%= ValueShippingCollected %>], backgroundColor: '#0076df' },
                        { type: 'bar', label: 'Inventory + Total OpEx', data: [<%= ValueSourcingCost + ValueTotalExpenses %>], backgroundColor: '#f44336' },
                        { type: 'line', label: 'Net Profit', data: [<%= (ValueCarSales + ValueShippingCollected) - (ValueSourcingCost + ValueTotalExpenses) %>], borderColor: '#00ff66', backgroundColor: '#00ff66', pointRadius: 5, borderWidth: 3, fill: false }
                    ]
                },
                options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { labels: { color: '#ccc' } } } }
            });

            const ctxLeakage = document.getElementById('carrierLeakageChart').getContext('2d');
            new Chart(ctxLeakage, {
                type: 'bar',
                data: {
                    labels: ['Outflow Reconciliation Framework'],
                    datasets: [
                        { label: 'Shipping Income Collected (Rs.)', data: [<%= ValueShippingCollected %>], backgroundColor: '#0076df' },
                        { label: 'Total Operating Costs (Freight + Expenses)', data: [<%= ValueActualShippingCost + ValueTotalExpenses %>], backgroundColor: '#f44336' }
                    ]
                },
                options: { indexAxis: 'y', responsive: true, maintainAspectRatio: false, plugins: { legend: { labels: { color: '#ccc' } } } }
            });

            const ctxPie = document.getElementById('orderPipelinePieChart').getContext('2d');
            new Chart(ctxPie, {
                type: 'pie',
                data: {
                    labels: ['Approved', 'Awaiting Verification', 'Awaiting Payment', 'Cancelled'],
                    datasets: [{
                        data: [<%= CountApproved %>, <%= CountAwaitingVerify %>, <%= CountAwaitingPay %>, <%= CountCancelled %>],
                        backgroundColor: ['#2ebd59', '#0076df', '#ffc107', '#333333'],
                        borderWidth: 0
                    }]
                },
                options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'right', labels: { color: '#ccc' } } } }
            });

            const ctxCohort = document.getElementById('collectorCohortDoughnutChart').getContext('2d');
            new Chart(ctxCohort, {
                type: 'doughnut',
                data: {
                    labels: ['First-Time Buyers', 'Repeat Collectors (2+ Orders)'],
                    datasets: [{
                        data: [<%= CountFirstTimeBuyers %>, <%= CountRepeatCollectors %>],
                        backgroundColor: ['#e5ba6b', '#9c27b0'],
                        borderWidth: 0
                    }]
                },
                options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'right', labels: { color: '#ccc' } } }, cutout: '70%' }
            });
        });
    </script>
</asp:Content>