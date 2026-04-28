# The Game Pond Marketplace Master Plan

Date: 2026-04-27
Current stack decision: ASP.NET Core MVC + C# + Entity Framework Core + self-hosted PostgreSQL or SQL Server Express + budget VPS

## 1. Source Inventory

Project materials reviewed:

- `The_Game_Pond_Full_Marketplace_Structure.docx`
- Google Drive folder: `The Game Pond`
- Drive subfolder: `TGP WEB`
- Drive subfolder: `The Game Pond Logo and Color Chart`

Current local workspace status:

- No application code exists yet in `c:\Users\silve\Projects\Miguels Project`.
- Existing Drive web files are only Bootstrap assets:
  - `css/bootstrap.css`, Bootstrap v5.3.8
  - `js/bootstrap.js`
  - `img/` folder appears empty
- Brand assets found:
  - `THE GAME POND.png`
  - `Color Chart.png`
  - `Business card front.jpeg`
  - `Business card back.jpeg`

Brand system from the color chart:

- Primary colors:
  - Pond Green: `#6CC24A`
  - Dark Green: `#2E7D32`
  - Pond Blue: `#1F6F8B`
  - Dark Blue: `#0F3D5E`
  - Clean White: `#F5F7F6`
- Accent colors:
  - Accent Red: `#E63946`
  - Accent Yellow: `#F4D35E`
  - Accent Blue: `#3A86FF`
  - Accent Green: `#2ECC71`
  - Accent Purple: `#9D4EDD`
- Intended color usage:
  - 60% white/light backgrounds
  - 25% blue for structure, sections, and trust
  - 15% green for calls to action, highlights, and key elements
- Brand language:
  - `Collect. Play. Repeat.`
  - `Enter the Pond`
  - Gaming, TCG and collectibles, retro, anime, live streams, drops

## 2. Product Vision

The Game Pond should launch as a polished single-store marketplace for gaming products, collectibles, trade-ins, and local community commerce. The first production version should support online browsing, cart, checkout, order fulfillment, inventory sync, admin operations, and trade-in intake. Multi-seller marketplace support should be treated as a future expansion unless the owner confirms it is required at launch.

Primary customer flows:

- Browse products by games, consoles, controllers, accessories, bundles, specials, retro, TCG, anime, and collectibles.
- View product details with price, condition, SKU, stock, gallery, shipping estimate, reviews, and related items.
- Add to cart and complete guest checkout.
- Receive order confirmation and shipping updates.
- Submit trade-in requests and receive offers/store credit.
- Join newsletter, wishlist items, and recover abandoned carts.

Primary staff flows:

- Manage inventory by SKU, barcode, category, condition, stock count, supplier, cost, price, and location.
- See low-stock alerts and restock logs.
- Process orders through pending payment, paid, processing, packed, shipped, delivered, cancelled, refunded.
- Review trade-ins, generate offers, create store credit, and convert accepted trade-ins into inventory.
- Track sales, profit, top products, conversion, refunds, and growth.

## 3. Recommended System Direction

### MVP Direction

Build a lean custom storefront and admin layer with its own inventory and order handling software. Do not start by paying for Shopify, Square Retail, Lightspeed, Azure App Service, Azure SQL, or another platform-heavy setup. The Game Pond database should be the source of truth at launch, and future Square/Shopify/Lightspeed integrations should be optional adapters added only after the store grows.

Recommended application architecture:

- Framework: ASP.NET Core MVC
- Language: C#
- IDE: Visual Studio 2022 or newer
- Runtime: .NET 8 LTS or current stable LTS
- Database: PostgreSQL on a budget Linux VPS by default
- Alternate database: SQL Server Express if the team chooses a Windows/SQL Server path
- ORM: Entity Framework Core
- Frontend views: Razor Views with HTML, CSS, Bootstrap 5.3.8, and JavaScript where needed
- Admin auth: ASP.NET Core Identity with role-based permissions
- Payments: Stripe Checkout first, or Square Payments if client prefers Square
- Hosting: budget VPS such as Hetzner, Contabo, or similar
- Web server/reverse proxy: Nginx or Caddy in front of Kestrel
- File storage at launch: local VPS uploads directory with backup routine
- Future file storage: S3-compatible storage, Azure Blob Storage, or similar if media volume grows
- Secrets: environment variables/server secret files at launch
- Logging: ASP.NET Core file logging plus future monitoring if needed

Lean monthly cost target:

- Expected fixed range: $8-$32/month
- Most likely: $10-$18/month before sales
- Best starter target: $9-$10/month before sales
- Payment fees: only when sales happen, typically around 2.9% + $0.30 per online card transaction depending on processor and country

### Inventory/Order Software Shortlist

Best first candidates:

1. Square Retail / Square APIs
   - Best if the owner needs POS, payments, catalog, inventory, and orders in one small-business-friendly system.
   - Square has Catalog, Inventory, Orders, Payments, Customers, and Webhooks APIs.
   - Strong candidate for a local game store if they want in-store POS plus web checkout.

2. Shopify + Shopify POS
   - Best if ecommerce is primary and the owner wants a strong admin, product management, checkout ecosystem, and POS option.
   - Shopify Admin GraphQL API exposes products, inventory, orders, fulfillments, customers, and bulk operations.
   - Good if we want to avoid rebuilding too much commerce admin.

3. Lightspeed Retail X-Series
   - Best if the owner prioritizes serious retail POS, barcode workflows, inventory counts, purchase orders, multi-outlet operations, and staff workflows.
   - More powerful operationally, but integration work is heavier.

4. Zoho Inventory
   - Best if the owner wants cost-effective back-office inventory and sales order management without committing to Shopify or Square as the commerce core.
   - Good API coverage for items and sales orders.

5. Cin7 Core / Cin7 Omni
   - Best for more complex supplier, multichannel, warehouse, and purchase-order workflows.
   - Likely overkill for the first version unless the owner already uses it or expects complex operations quickly.

Current recommendation:

- Do not use Square/Shopify/Lightspeed as the launch inventory/order platform.
- Build custom inventory and order handling into the admin dashboard.
- Use Stripe Checkout first unless the client prefers Square Payments.
- Keep Square/Shopify/Lightspeed as future API adapter options only.

## 4. Core Data Model

Minimum entities:

- Product
- ProductVariant
- Category
- Brand/Franchise
- ConditionGrade
- InventoryItem
- InventoryLocation
- StockMovement
- Supplier
- RestockLog
- Cart
- CartItem
- Customer
- Order
- OrderItem
- Payment
- Refund
- Shipment
- Coupon
- Review
- WishlistItem
- TradeInRequest
- TradeInItem
- TradeInOffer
- StoreCredit
- AdminUser
- ActivityLog
- IntegrationEvent

Important product fields:

- Name, slug, description, category, platform, franchise
- SKU, barcode/UPC, external system IDs
- Condition: new, used, retro, refurbished, graded, sealed
- Grade notes and photos for used inventory
- Cost, price, sale price, margin
- Quantity available, committed, incoming, reserved
- Weight/dimensions for shipping
- SEO title/description

## 5. API Integration Pattern

Use an internal commerce API as the website contract, then create adapters for the external system.

Website/admin should call internal endpoints such as:

- `GET /api/products`
- `GET /api/products/:slug`
- `POST /api/cart`
- `POST /api/checkout/session`
- `GET /api/orders/:id`
- `PATCH /api/admin/orders/:id/status`
- `POST /api/admin/inventory/adjust`
- `POST /api/trade-ins`
- `POST /api/webhooks/payment`
- `POST /api/webhooks/inventory`
- `POST /api/webhooks/orders`

Adapter responsibilities:

- Map local SKU/barcode/product IDs to the external software IDs.
- Pull catalog and inventory from the external system.
- Push online orders into the external system.
- Reserve or decrement stock after payment.
- Reconcile refunds/cancellations.
- Store raw webhook payloads for audit/debugging.
- Retry failed sync jobs safely.

## 6. MVP Build Plan

### Phase 0: Discovery and Decisions

- Confirm whether this is single-store ecommerce or true multi-seller marketplace.
- Confirm POS/inventory software preference and budget.
- Confirm payment processor preference.
- Confirm shipping carriers and local pickup needs.
- Confirm tax handling, return policy, trade-in policy, and warranty policy.
- Gather product sample data and product photos.
- Decide launch catalog size.

### Phase 1: Foundation

- Initialize repository and app framework.
- Add brand tokens from the color chart.
- Import logo and brand assets.
- Create responsive layout, nav, footer, category structure, and shared components.
- Create database schema and seed data.
- Define API contracts and integration adapter interface.
- Set up environment variables, secrets handling, linting, tests, and CI.

### Phase 2: Storefront MVP

- Home page with promotions, featured products, best sellers, flash sales, bundles, trade-in promo, reviews, and newsletter signup.
- Shop page with category filters, search, platform, price, condition, and stock filters.
- Product detail page with gallery, price, condition, SKU, stock, cart button, reviews, related items, and shipping estimate.
- Cart page with quantities, coupons, totals, and stock validation.
- Checkout flow with guest checkout, payment session, order confirmation, and transactional email.
- Basic SEO, metadata, sitemap, and performance pass.

### Phase 3: Admin MVP

- Admin dashboard with sales cards, recent orders, low-stock alerts, pending approvals, and customer messages.
- Orders page with status pipeline:
  - Pending Payment
  - Payment Received
  - Processing
  - Packed
  - Shipped
  - Delivered
  - Cancelled
  - Refunded
- Inventory page with add/update/delete, SKU, barcode, category, condition, quantity, supplier, and low-stock threshold.
- Trade-in queue with item review, condition grading, offer generation, approval/rejection, and conversion to inventory.
- Analytics page with daily revenue, orders, profit, monthly growth, yearly totals, top products, conversion rate, and refund rate.

### Phase 4: External Integrations

- Connect selected inventory/order platform.
- Implement product import/sync.
- Implement inventory sync and conflict rules.
- Implement paid order creation in external system.
- Implement order status webhook or polling sync.
- Implement refund and cancellation sync.
- Add shipping label workflow if supported by chosen platform or carrier.
- Add email provider for receipts, status updates, trade-in offers, and abandoned carts.

### Phase 5: Launch Readiness

- QA across desktop/mobile.
- Validate checkout, payment webhooks, refunds, and inventory edge cases.
- Verify security:
  - Admin role permissions
  - Webhook signature verification
  - Rate limiting
  - Audit logs
  - No raw card data stored
- Set up domain, SSL, production hosting, backups, monitoring, and error reporting.
- Staff training docs for order processing, inventory updates, trade-ins, refunds, and common support issues.

### Phase 6: Growth

- Customer accounts.
- Loyalty rewards.
- Wishlist.
- Verified buyer reviews.
- Abandoned cart recovery.
- Referral program.
- Email campaigns.
- AI-assisted trade-in pricing.
- Advanced search.
- Marketplace sellers.
- Mobile app.
- Live stream product drops.

## 7. Agent Roster and Skill Sets

These are the specialized agents/roles that should be created for the project. They do not all need to run at once. The first wave should be architecture, UX/brand, storefront, backend/API, integration, and QA/devops.

### 1. Product Architect Agent

Purpose:

- Own the full product blueprint, requirements, scope, milestones, and dependency map.

Skills:

- Ecommerce architecture
- Marketplace workflows
- POS/OMS/inventory evaluation
- API contract design
- Risk management
- Technical documentation
- Prioritization and acceptance criteria

Outputs:

- Final requirements
- Milestone plan
- Data model
- API map
- Launch checklist

### 2. Brand and UX/UI Agent

Purpose:

- Turn The Game Pond identity into a usable, polished, responsive shopping experience.

Skills:

- Responsive ecommerce UX
- Design systems
- Accessibility
- Bootstrap or component-library theming
- Product card, cart, checkout, and admin UX
- Brand token extraction

Outputs:

- Design tokens
- Page layouts
- Component specs
- Mobile and desktop UX flows
- Accessibility checks

### 3. Razor/Bootstrap Storefront Agent

Purpose:

- Build the public website and customer shopping experience using ASP.NET Core MVC Razor Views.

Skills:

- ASP.NET Core MVC
- Razor Views
- C#
- Bootstrap 5.3.8
- HTML/CSS
- Light JavaScript
- Product listing pages
- Product detail pages
- Cart and checkout UI
- Search/filter UX
- SEO and performance
- Analytics event tracking

Outputs:

- Home, shop, product, cart, checkout, trade-in, reviews, contact pages
- Shared Razor layouts/partials
- Storefront pages
- Cart UI
- Responsive QA notes

### 4. .NET Backend and Admin Agent

Purpose:

- Build the MVC controllers, C# services, database-backed admin tools, auth, business logic, and admin foundations.

Skills:

- ASP.NET Core MVC
- C#
- Entity Framework Core
- PostgreSQL or SQL Server Express schema design
- ASP.NET Core Identity and roles
- Order/payment/inventory state machines
- Webhook handling
- Audit logging

Outputs:

- EF Core models and migrations
- MVC controllers/services
- Admin auth/roles
- Order pipeline
- Inventory services
- Webhook endpoints

### 5. Future Inventory/POS Adapter Agent

Purpose:

- Preserve the future ability to connect Square, Shopify, Lightspeed, or another provider without making them part of MVP.

Skills:

- C# adapter/interface design
- Square, Shopify, Lightspeed, Zoho, or Cin7 APIs
- OAuth/API token setup
- Webhook subscriptions
- SKU/barcode mapping
- LocalCommerceAdapter first
- Future product sync
- Future inventory reconciliation
- Future order push/pull workflows
- Idempotency and retries

Outputs:

- Integration adapter
- Sync jobs
- Webhook handlers
- External ID mapping
- Error/retry dashboard

### 6. Payments, Tax, and Shipping Agent

Purpose:

- Own checkout, payment confirmation, refunds, taxes, shipping options, and labels.

Skills:

- Stripe Checkout or Square Payments
- Payment webhooks
- Refund flows
- Sales tax strategy
- Shipping rate APIs
- Label creation
- Local pickup logic
- PCI-safe architecture

Outputs:

- Checkout session service
- Payment status sync
- Refund workflow
- Shipping/tax integration
- Payment QA checklist

### 7. Admin Operations Agent

Purpose:

- Build practical tools for staff to run the store.

Skills:

- Admin dashboards
- Order management
- Inventory CRUD
- Low-stock alerts
- Trade-in operations
- Supplier/restock logs
- Customer messages

Outputs:

- Dashboard
- Orders admin
- Inventory admin
- Trade-in admin
- Staff activity log

### 8. Trade-In and Used Inventory Agent

Purpose:

- Design and implement trade-in intake, grading, offer generation, and store credit workflows.

Skills:

- Used inventory condition grading
- Offer rule modeling
- Store credit workflows
- Fraud prevention
- Photo upload review
- SKU creation from trade-ins

Outputs:

- Trade-in submission flow
- Condition review tools
- Offer engine
- Store credit records
- Inventory conversion workflow

### 9. Analytics and Growth Agent

Purpose:

- Track business performance and marketing loops.

Skills:

- Ecommerce analytics
- GA4/events
- Sales dashboards
- Cohort and conversion metrics
- Email marketing
- Loyalty/referral systems
- Abandoned cart recovery

Outputs:

- Event taxonomy
- Analytics dashboard
- Email campaign hooks
- Loyalty/referral specs

### 10. QA, Security, and DevOps Agent

Purpose:

- Protect launch quality, production reliability, and operational safety.

Skills:

- Automated testing
- Playwright/browser QA
- API integration tests
- Security review
- OWASP basics
- CI/CD
- Hosting, SSL, monitoring, backups
- Incident runbooks

Outputs:

- Test plan
- CI pipeline
- Production deployment
- Monitoring
- Security checklist
- Staff runbooks

### 11. Content, SEO, and Catalog Agent

Purpose:

- Make the catalog shoppable, searchable, and conversion-friendly.

Skills:

- Product taxonomy
- Ecommerce copywriting
- SEO metadata
- Category landing pages
- Product photo standards
- Review/request flows
- Brand voice

Outputs:

- Category taxonomy
- Product naming rules
- Product copy templates
- SEO plan
- Content upload checklist

## 8. Key Decisions Needed From Owner

- Is this single-store ecommerce at launch, or multi-vendor marketplace at launch?
- Which system does the owner already use, if any: Square, Shopify, Lightspeed, Clover, Zoho, Cin7, QuickBooks, spreadsheets?
- Does the store need in-person POS on day one?
- Will online checkout support shipping, local pickup, or both?
- Which payment processor is preferred?
- Is barcode scanning required at launch?
- Does used inventory need one-off SKUs per item?
- Does trade-in payout use cash, store credit, or both?
- Are TCG/anime/collectibles launch categories confirmed?
- What is the target launch date and initial product count?

## 9. Immediate Next Steps

1. Interview owner and answer the key decisions above.
2. Pick the inventory/order/POS platform.
3. Create the repository and technical foundation.
4. Build the design system from The Game Pond brand assets.
5. Build static storefront/admin shell using seeded sample products.
6. Implement database and internal API.
7. Connect payment sandbox.
8. Connect chosen inventory/order software sandbox.
9. Test product sync, checkout, paid order creation, stock decrement, refund, and cancellation.
10. Prepare launch content, staff workflow docs, and production deployment.

## 10. Reference Docs Checked

- Square Catalog API: https://developer.squareup.com/docs/catalog-api/what-it-does
- Square Orders API: https://developer.squareup.com/docs/orders-api/what-it-does
- Shopify GraphQL Admin API: https://shopify.dev/docs/api/admin-graphql/latest
- Shopify Order API object/query docs: https://shopify.dev/docs/api/admin-graphql/latest/queries/order
- Shopify InventoryItem docs: https://shopify.dev/docs/api/admin-graphql/latest/objects/inventoryitem
- Lightspeed Retail X-Series API hub: https://x-series-api.lightspeedhq.com/
- Lightspeed inventory updates: https://x-series-api.lightspeedhq.com/docs/inventory_updates
- Lightspeed stock orders: https://x-series-api.lightspeedhq.com/docs/inventory_creating_stock_orders
- Zoho Inventory sales orders API: https://www.zoho.com/inventory/api/v1/salesorders/
- Stripe Checkout Sessions API: https://docs.stripe.com/api/checkout/sessions/create
