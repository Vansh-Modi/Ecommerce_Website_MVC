<%@ Page Title="Submit Payment | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="payment-upload.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.payment_upload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Web_Files/Client/Styles/checkout.css?v=3" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shop-theme-wrapper">
        <main class="shop-content-container payment-upload-layout">
            
            <div class="shop-controls-bar">
                <h1 class="shop-main-heading">Complete Your Order</h1>
            </div>

            <asp:Label ID="lblStatus" runat="server" CssClass="status-msg-error" Visible="false"></asp:Label>

            <!-- Parent Card Framing Block -->
            <div class="checkout-form-card alignment-center-wrapper">
                
                <div class="price-summary-container">
                    <span class="price-summary-label">Total Payment Amount Due:</span>
                    <span class="price-summary-value">Rs. <asp:Literal ID="litPaymentDue" runat="server"></asp:Literal></span>
                </div>

                <!-- Online Payment Matrix Presentation -->
                <asp:Panel ID="pnlOnlinePaymentDetails" runat="server">
                    <h2 class="form-section-title text-center">Scan QR or Transfer Direct</h2>
                    <p class="section-description-text">
                        Please scan the merchant QR code or transfer the exact order amount to the bank account listed below.
                    </p>
                    
                    <asp:Image ID="imgQrCode" runat="server" CssClass="payment-qr-img" AlternateText="Merchant Payment QR Code" />
                    
                    <div class="bank-details-box alignment-left-wrapper">
                        <div class="bank-row-item">
                            <span class="bank-row-label">Merchant UPI ID</span>
                            <span class="bank-row-value values-highlight"><asp:Literal ID="litUpiId" runat="server"></asp:Literal></span>
                        </div>
                        <div class="bank-row-item-divider">
                            <span class="bank-row-label">Direct Bank Wire Credentials</span>
                            <p class="bank-monospace-text"><asp:Literal ID="litBankDetails" runat="server"></asp:Literal></p>
                        </div>
                    </div>

                    <div class="checkout-info-banner alignment-left-wrapper">
                        <span class="info-banner-title">⚠️ Secure Verification Process</span>
                        <p class="info-banner-text">
                            Please double-check that your typed Reference Number matches your bank statement receipt exactly. The owner will cross-verify this reference manually before dispatching your package. False submissions will result in immediate cancellation.
                        </p>
                    </div>
                </asp:Panel>

                <!-- COD Bypass Confirmation Block -->
                <asp:Panel ID="pnlCodConfirmation" runat="server" Visible="false">
                    <div class="cod-confirmation-box">
                        <div class="cod-icon-graphic">📦</div>
                        <h2 class="form-section-title">Cash on Delivery Verification</h2>
                        <p class="cod-meta-description">
                            Thank you for opting for Cash on Delivery. To avoid fraudulent checkouts, your order will remain flagged as <span class="highlight-inline-blue">"Awaiting Verification"</span>.
                        </p>
                    </div>
                </asp:Panel>

                <!-- Universal Payment Proof Submission Data Form -->
                <div class="form-grid alignment-left-wrapper">
                    
                    <asp:Panel ID="pnlOnlineUploadForm" runat="server" Style="display: contents;">
                        <div class="form-group">
                            <label class="form-label">Transaction Reference Number / UTR Id</label>
                            <asp:TextBox ID="txtTxnReference" runat="server" CssClass="form-input" placeholder="Enter 12-Digit UTR or Bank Ref ID"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Upload Transfer Screenshot Receipt <span class="form-label-accent">(Max 2MB)</span></label>
                            <asp:FileUpload ID="fileScreenshot" runat="server" CssClass="form-input custom-file-input" accept=".jpg,.jpeg,.png" />
                        </div>
                    </asp:Panel>

                    <div class="summary-actions-block upload-actions-spacing">
                        <asp:Button ID="btnSubmitProof" runat="server" Text="Submit Reference & Complete Order" CssClass="btn btn-checkout-large" OnClick="btnSubmitProof_Click" />
                    </div>
                </div>

            </div>
        </main>
    </div>
</asp:Content>