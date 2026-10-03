<%@ Page Title="Product Details | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="productDetails.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.productDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Web_Files/Client/Styles/shop.css?v=3" rel="stylesheet" type="text/css" />
    <link href="/Web_Files/Client/Styles/productDetails.css?v=5" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shop-theme-wrapper">
        <main class="shop-content-container details-max-width">

            <div class="details-back-container">
                <a href="shop.aspx" class="btn-layout-back">← Back to Collection</a>
            </div>

            <asp:Label ID="lblDetailStatus" runat="server" CssClass="status-msg-error" Visible="false" Style="margin-bottom: 20px; display: block;"></asp:Label>

            <!-- Main Product Structural Presentation Split Container -->
            <div class="product-details-layout-container">

                <div class="details-image-panel">
                    <div class="main-showcase-image-frame">
                        <asp:Image ID="imgProduct" runat="server" CssClass="details-main-img" ClientIDMode="Static" />
                    </div>

                    <div class="gallery-thumbnails-container">
                        <asp:Repeater ID="rptGallery" runat="server">
                            <ItemTemplate>
                                <div class="thumbnail-wrapper" onclick="switchShowcaseImage(this)">
                                    <img src='<%# ResolveUrl(Container.DataItem.ToString()) %>' class="gallery-thumb-item" alt="Product thumbnail replica detail view" />
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="details-content-column">
                    <span class="details-brand-label">
                        <asp:Label ID="lblManufacturer" runat="server"></asp:Label>
                    </span>
                    <h1 class="details-product-title">
                        <asp:Label ID="lblProductName" runat="server"></asp:Label>
                    </h1>

                    <div class="details-inventory-status-row">
                        <span class="details-scale-badge">Scale Ratio: <asp:Label ID="lblScaleDisplay" runat="server"></asp:Label></span>
                        <asp:Label ID="lblStockBadge" runat="server" CssClass="details-stock-badge-indicator"></asp:Label>
                    </div>

                    <div class="details-price-display">
                        Rs.<asp:Label ID="lblPrice" runat="server"></asp:Label>
                    </div>

                    <div class="details-description-text">
                        <h4 class="description-section-heading">Model Specifications & Features</h4>
                        <p class="description-paragraph-content">
                            <asp:Label ID="lblDescription" runat="server"></asp:Label>
                        </p>
                    </div>

                    <div class="details-action-row">
                        <asp:Button ID="btnAddToCart" runat="server" Text="Add to Cart" CssClass="btn-add-to-cart-large" OnClick="btnAddToCart_Click" />
                    </div>
                </div>
            </div>

            <!-- Bottom Sourcing Trust Badges & FAQ Accordion Block Box -->
            <div class="my-faq-container">
                <div class="my-faq-header">
                    <h3 class="my-faq-title">Fulfillment & Shop Information</h3>
                </div>

                <div class="shipping-badges-row">
                    <div class="badge-box-item">
                        <svg viewBox="0 0 24 24">
                            <path d="M20 8l-8-5-8 5v8l8 5 8-5V8zm-8-3l6.3 3.94L12 12.19l-6.3-3.25L12 5zm-7 4.75l6 3.1v6.4l-6-3.1v-6.4zm8 9.5v-6.4l6-3.1v6.4l-6 3.1z" />
                        </svg>
                        <h4>Secure Packing</h4>
                        <p>Heavy-duty bubble layered curation wrapping boxes.</p>
                    </div>
                    <div class="badge-box-item">
                        <svg viewBox="0 0 24 24">
                            <path d="M20 8h-3V4H3c-1.1 0-2 .9-2 2v11h2c0 1.66.9 3 2.5 3s2.5-1.34 2.5-3h6c0 1.66.9 3 2.5 3s2.5-1.34 2.5-3h2v-5l-3-4zM6 18.5c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5 0 1.5 1.5-.67 1.5-1.5 1.5zm11 0c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5 0 1.5 1.5-.67 1.5-1.5 1.5zm2-5.5h-3V9h3v4z" />
                        </svg>
                        <h4>Quick Shipping</h4>
                        <p>Dispatched inside 1-4 business workspace days logs.</p>
                    </div>
                    <div class="badge-box-item">
                        <svg viewBox="0 0 24 24">
                            <path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5s1.12-2.5 2.5-2.5 2.5 1.12 2.5 2.5-1.12 2.5-2.5 2.5z" />
                        </svg>
                        <h4>All India Delivery</h4>
                        <p>Delivery 3-7 days transit duration depending on city location.</p>
                    </div>
                </div>

                <div class="faq-accordion-stack">
                    <div class="faq-accordion-item">
                        <div class="faq-trigger-header" onclick="toggleFaqAccordion(this)">
                            <span class="faq-title-text">
                                <svg viewBox="0 0 24 24">
                                    <path d="M21.41 11.58l-9-9C12.05 2.22 11.55 2 11 2H4c-1.1 0-2 .9-2 2v7c0 .55.22 1.05.59 1.42l9 9c.36.36.86.58 1.41.58.55 0 1.05-.22 1.41-.59l7-7c.37-.36.59-.86.59-1.41 0-.55-.23-1.06-.59-1.42zM5.5 7c-.83 0-1.5-.67-1.5-1.5S4.67 4 5.5 4 7 4.67 7 5.5 6.33 7 5.5 7z" />
                                </svg>
                                Is Cash On Delivery (COD) Available?
                            </span>
                            <span class="faq-arrow-icon">▼</span>
                        </div>
                        <div class="faq-content-panel">
                            <div class="faq-inner-text">
                                Yes, Cash-on-Delivery is supported exclusively for order logs tracking addresses located inside Surat city parameters boundaries. All outstation destination packages require secure online UPI transfer settlements on checking out.
                            </div>
                        </div>
                    </div>

                    <div class="faq-accordion-item">
                        <div class="faq-trigger-header" onclick="toggleFaqAccordion(this)">
                            <span class="faq-title-text">
                                <svg viewBox="0 0 24 24">
                                    <path d="M20 8h-3V4H3c-1.1 0-2 .9-2 2v11h2c0 1.66.9 3 2.5 3s2.5-1.34 2.5-3h6c0 1.66.9 3 2.5 3s2.5-1.34 2.5-3h2v-5l-3-4zm-14 9.5c-.55 0-1-.45-1-1s.45-1 1-1 1 .45 1 1-.45 1-1 1zm11 0c-.55 0-1-.45-1-1s.45-1 1-1 1 .45 1 1-.45 1-1 1z" />
                                </svg>
                                How long will it take to dispatch my order?
                            </span>
                            <span class="faq-arrow-icon">▼</span>
                        </div>
                        <div class="faq-content-panel">
                            <div class="faq-inner-text">
                                We safely process, package, and dispatch regular stock listings out of our main terminal station in 1 to 4 operational company business days following payment confirmation routines audit trail logs.
                            </div>
                        </div>
                    </div>

                    <div class="faq-accordion-item">
                        <div class="faq-trigger-header" onclick="toggleFaqAccordion(this)">
                            <span class="faq-title-text">
                                <svg viewBox="0 0 24 24">
                                    <path d="M21 16v-2l-8-5V3.5c0-.83-.67-1.5-1.5-1.5S10 2.67 10 3.5V9l-8 5v2l8-2.5V19l-2 1.5V22l3.5-1 3.5 1v-1.5L13 19v-5.5l8 2.5z" />
                                </svg>
                                Why do some product titles or images mention "Imported"?
                            </span>
                            <span class="faq-arrow-icon">▼</span>
                        </div>
                        <div class="faq-content-panel">
                            <div class="faq-inner-text">
                                Many high-end collector tier models are sourced globally on a special proxy request contract basis. These exclusive replicas are brought in directly from international distribution warehouses because they have limited local releases.
                            </div>
                        </div>
                    </div>

                    <div class="faq-accordion-item">
                        <div class="faq-trigger-header" onclick="toggleFaqAccordion(this)">
                            <span class="faq-title-text">
                                <svg viewBox="0 0 24 24">
                                    <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 16h-2v-2h2v2zm1.07-7.75l-.9.92C12.45 11.9 12 12.5 12 14h-2v-.5c0-1.1.45-2.1 1.17-2.83l1.24-1.26c.37-.36.59-.86.59-1.41 0-1.1-.9-2-2-2s-2 .9-2 2H7c0-2.76 2.24-5 5-5s5 2.24 5 5c0 1.04-.42 1.99-1.07 2.75z" />
                                </svg>
                                Why are some products priced higher than the MRP printed on the packaging?
                            </span>
                            <span class="faq-arrow-icon">▼</span>
                        </div>
                        <div class="faq-content-panel">
                            <div class="faq-inner-text">
                                Show's Garage works as an independent proxy sourcing agency. The base physical collectible is passed to you at retail cost; any remaining cost markup goes toward international curation logistics, customs clearances, and sourcing fees.
                            </div>
                        </div>
                    </div>

                    <div class="faq-accordion-item">
                        <div class="faq-trigger-header" onclick="toggleFaqAccordion(this)">
                            <span class="faq-title-text">
                                <svg viewBox="0 0 24 24">
                                    <path d="M20 8h-3V4H3c-1.1 0-2 .9-2 2v11h2c0 1.66.9 3 2.5 3s2.5-1.34 2.5-3h6c0 1.66.9 3 2.5 3s2.5-1.34 2.5-3h2v-5l-3-4z" />
                                </svg>
                                Which courier partners do you use?
                            </span>
                            <span class="faq-arrow-icon">▼</span>
                        </div>
                        <div class="faq-content-panel">
                            <div class="faq-inner-text">
                                We work with reliable national logistics carriers including Tirupati, Nandan, India-Post, Delhivery, Blue Dart, DTDC, and India Post to make sure your tracking updates are accurate and your models arrive safely.
                            </div>
                        </div>
                    </div>

                    <div class="faq-accordion-item">
                        <div class="faq-trigger-header" onclick="toggleFaqAccordion(this)">
                            <span class="faq-title-text">
                                <svg viewBox="0 0 24 24">
                                    <path d="M12.5 8c-2.65 0-5.05.99-6.9 2.6L2 7v9h3.6l-2.07-2.07C4.99 12.62 6.61 12 8.5 12c3.42 0 6.31 2.2 7.4 5.26l2.35-.73C16.56 12.36 12.87 8 8.5 8z" />
                                </svg>
                                Can I return a product?
                            </span>
                            <span class="faq-arrow-icon">▼</span>
                        </div>
                        <div class="faq-content-panel">
                            <div class="faq-inner-text">
                                Due to the limited collector nature of diecast models, returns are not accepted.
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </main>
    </div>

    <script type="text/javascript">
        function toggleFaqAccordion(headerElement) {
            const item = headerElement.closest('.faq-accordion-item');
            if (item) {
                const isOpen = item.classList.contains('is-open');

                // Close other open panels for clean collapse mechanics
                document.querySelectorAll('.faq-accordion-item').forEach(el => {
                    el.classList.remove('is-open');
                    const panel = el.querySelector('.faq-content-panel');
                    if (panel) panel.style.maxHeight = null;
                });

                if (!isOpen) {
                    item.classList.add('is-open');
                    const contentPanel = item.querySelector('.faq-content-panel');
                    if (contentPanel) contentPanel.style.maxHeight = contentPanel.scrollHeight + "px";
                }
            }
        }

        // Live image swap thumbnail selection handler
        function switchShowcaseImage(thumbnailElement) {
            const imgEl = thumbnailElement.querySelector('img');
            if (imgEl) {
                const targetSrc = imgEl.src;
                const mainImg = document.getElementById('imgProduct');
                if (mainImg) mainImg.src = targetSrc;

                document.querySelectorAll('.thumbnail-wrapper').forEach(el => el.classList.remove('active-thumb'));
                thumbnailElement.classList.add('active-thumb');
            }
        }
    </script>
</asp:Content>