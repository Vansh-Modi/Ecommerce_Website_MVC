<%@ Page Title="Manage Expenses | Admin" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="expenses.aspx.cs" Inherits="ShowsGarage.Web_Files.Admin.admin_expenses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Manage Expenses | Admin Panel</title>
    <link href="/Web_Files/Admin/Styles/siteSettings.css?" rel="stylesheet" type="text/css" />
    <link href="/Web_Files/Admin/Styles/expenses.css?" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="admin-theme-wrapper">
        <main class="admin-content-container standard-wide-layout">
            
            <div class="admin-controls-bar">
                <h1 class="admin-main-heading">Expense Tracker & Ledger</h1>
                <span class="admin-pill-badge">Financial Management</span>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="admin-alert-banner" Visible="false"></asp:Label>

            <asp:HiddenField ID="hfActiveExpenseID" runat="server" Value="" />

            <div class="expenses-split-grid">
                
                <div class="expenses-form-column">
                    <div class="settings-card-panel">
                        <h2 class="settings-card-title"><asp:Literal ID="litFormTitle" runat="server" Text="Log Outbound Expense"></asp:Literal></h2>
                        
                        <div class="settings-form-grid">
                            <div class="form-group full-width">
                                <label class="settings-label">Expense Title / Description</label>
                                <asp:TextBox ID="txtTitle" runat="server" CssClass="settings-input" placeholder="e.g., Bluedart Courier Charges for Order #105"></asp:TextBox>
                            </div>

                            <div class="form-group half-width">
                                <label class="settings-label">Amount Charged (Rs.)</label>
                                <asp:TextBox ID="txtAmount" runat="server" CssClass="settings-input numeric-emphasis" placeholder="0.00"></asp:TextBox>
                            </div>

                            <div class="form-group half-width">
                                <label class="settings-label">Expense Type</label>
                                <asp:DropDownList ID="ddlExpenseType" runat="server" CssClass="settings-input select-arrow-fix">
                                    <asp:ListItem Value="Shipping Logistics">Shipping & Logistics Courier</asp:ListItem>
                                    <asp:ListItem Value="Packaging Material">Packaging Supplies</asp:ListItem>
                                    <asp:ListItem Value="Damaged Return">Damaged Returns / Order Loss</asp:ListItem>
                                    <asp:ListItem Value="Marketing Cost">Order Specific Marketing/Ads</asp:ListItem>
                                    <asp:ListItem Value="Server & Utils">Server Utilities / Misc</asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <div class="form-group full-width">
                                <label class="settings-label">Link to Order ID (Optional Order Reference)</label>
                                <asp:DropDownList ID="ddlOrdersLink" runat="server" CssClass="settings-input select-arrow-fix"></asp:DropDownList>
                            </div>

                            <div class="form-group full-width">
                                <label class="settings-label">Administrative Remarks</label>
                                <asp:TextBox ID="txtRemarks" runat="server" CssClass="settings-input text-area" TextMode="MultiLine" Rows="3" placeholder="Enter courier AWB number, payment receipt reference, or vendor details..."></asp:TextBox>
                            </div>

                            <div class="summary-actions-block full-width" style="margin-top: 10px; display: flex; gap: 10px;">
                                <asp:Button ID="btnSaveExpense" runat="server" Text="Log Expense Entry" CssClass="btn-settings-save" OnClick="btnSaveExpense_Click" />
                                <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="btn-settings-cancel" OnClick="btnCancelEdit_Click" Visible="false" />
                            </div>
                        </div>

                    </div>
                </div>

                <div class="expenses-ledger-column">
                    <div class="settings-card-panel premium-border">
                        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; border-bottom: 1px solid #222; padding-bottom: 10px;">
                            <h2 class="settings-card-title" style="margin: 0; border: none; padding: 0;">Historical Outflow Ledger</h2>
                            <div class="ledger-grand-total">
                                Total Outflow: <span class="total-red-highlight">Rs.<asp:Label ID="lblTotalExpensesSum" runat="server" Text="0.00"></asp:Label></span>
                            </div>
                        </div>

                        <div class="ledger-table-container">
                            <asp:Repeater ID="rptExpensesLedger" runat="server" OnItemCommand="rptExpensesLedger_ItemCommand">
                                <HeaderTemplate>
                                    <table class="admin-ledger-table">
                                        <thead>
                                            <tr>
                                                <th>Date</th>
                                                <th>Details / Context</th>
                                                <th>Type</th>
                                                <th style="text-align: right;">Cost</th>
                                                <th style="text-align: center; width: 100px;">Actions</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td class="cell-date"><%# Eval("ExpenseDate", "{0:dd MMM yyyy}") %></td>
                                        <td class="cell-title">
                                            <strong><%# Eval("Title") %></strong>
                                            <%# (!string.IsNullOrEmpty(Eval("OrderID").ToString())) ? "<br/><span class='order-link-tag'>Linked to Order: #" + Eval("OrderID") + "</span>" : "" %>
                                            <%# (!string.IsNullOrEmpty(Eval("Remarks").ToString())) ? "<p class='cell-notes'>" + Eval("Remarks") + "</p>" : "" %>
                                        </td>
                                        <td class="cell-category"><span class="cat-pill"><%# Eval("ExpenseType") %></span></td>
                                        <td class="cell-cost">Rs.<%# string.Format("{0:N2}", Eval("Amount")) %></td>
                                        <td class="cell-actions" style="text-align: center; vertical-align: middle;">
                                            <asp:LinkButton ID="lnkEdit" runat="server" CssClass="action-link edit-link" CommandName="EditExpense" CommandArgument='<%# Eval("ExpenseID") %>'>Edit</asp:LinkButton>
                                            <span style="color:#333; margin: 0 4px;">|</span>
                                            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="action-link delete-link" CommandName="DeleteExpense" CommandArgument='<%# Eval("ExpenseID") %>' OnClientClick="return confirm('Are you sure you want to delete this expense entry permanently?');">Delete</asp:LinkButton>
                                        </td>
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

            </div>
        </main>
    </div>
</asp:Content>