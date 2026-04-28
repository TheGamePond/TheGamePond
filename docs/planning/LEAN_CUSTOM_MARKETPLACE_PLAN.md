# The Game Pond Lean Custom Marketplace Plan

Date: 2026-04-27
Current direction: ASP.NET Core MVC + C# + self-hosted database + budget VPS

## 1. Direction

Build The Game Pond as a lean custom marketplace website with its own inventory and order handling system. The project should not start by buying into Shopify, Square Retail, Lightspeed, Azure App Service, Azure SQL, or another monthly platform-heavy setup.

Use third parties only where they are strategically necessary:

- Payments: Stripe Checkout first, or Square Payments if the client strongly prefers Square.
- Hosting: budget VPS such as Hetzner, Contabo, or similar.
- Database: self-hosted PostgreSQL by default, or SQL Server Express if the team chooses a Windows-first deployment.
- Email: Brevo, Mailgun, or similar free/low-cost transactional email service.
- SSL: Let's Encrypt.
- Future optional integrations: Square, Shopify, Lightspeed, barcode/scanner tools, or managed database services once the business grows.

The goal is to launch professionally with low fixed monthly cost, while still giving the client a real admin dashboard, inventory management, order handling, and payment workflow.

## 2. Client Positioning

Recommended explanation:

> We can start lean by building your own inventory and order system directly into the website admin dashboard. You will not need to pay Shopify or Square monthly at first. The only third-party service we should use immediately is a secure payment gateway, because we should not process credit cards ourselves. The site can run on a budget VPS with a self-hosted database, keeping the fixed monthly bill around $10-$18 in the likely case. Later, if the store grows, we can upgrade hosting, move the database to a managed service, or connect Square/Shopify/Lightspeed through API.

## 3. Recommended Technical Architecture

### Application Stack

- Framework: ASP.NET Core MVC
- Language: C#
- IDE: Visual Studio 2022 or newer
- Runtime: .NET 8 LTS or current stable LTS
- Database: PostgreSQL on the VPS by default
- Alternate database: SQL Server Express if the team chooses a Windows/SQL Server path
- ORM: Entity Framework Core
- Frontend views: Razor Views
- Styling/UI: HTML, CSS, Bootstrap 5.3.8, and JavaScript where needed
- Admin auth: ASP.NET Core Identity with role-based permissions
- Payments: Stripe Checkout first, or Square Payments if client prefers Square
- Hosting: budget VPS, preferably Linux VPS for cost efficiency
- Web server/reverse proxy: Nginx or Caddy in front of Kestrel
- File storage at launch: local VPS storage with organized uploads folder and backup routine
- Future file storage: S3-compatible object storage, Azure Blob Storage, or similar if media volume grows
- Secrets: environment variables or server-level secret files at launch
- Future secrets: Vault, Azure Key Vault, or equivalent if infrastructure grows
- Logging: ASP.NET Core logging to files plus structured logs
- Future monitoring: Application Insights, Sentry, Better Stack, Grafana/Loki, or similar

### Why This Stack

- ASP.NET Core MVC is stable, professional, and well-suited for a database-backed storefront/admin application.
- C# and Entity Framework Core are strong for business workflows like inventory, orders, trade-ins, roles, and audit logs.
- Razor Views and Bootstrap keep the frontend practical and fast to build.
- PostgreSQL or SQL Server Express avoids managed database cost at launch.
- A budget VPS keeps monthly cost low while leaving room to scale.

## 4. Lean Startup Monthly Cost

Estimated fixed monthly cost:

- VPS hosting: $6-$15
- Domain name annualized: $1-$2/month
- SSL certificate: free with Let's Encrypt
- Database: free when self-hosted PostgreSQL or SQL Server Express
- Email receipts: $0-$10 on a free/low-cost provider
- Backups: $0-$5 if manual/local/included
- Payments: only per sale

Expected fixed monthly range:

- Total lean range: $8-$32/month
- Most likely: $10-$18/month
- Best starter target: about $9-$10/month before sales

Payment processing:

- Usually around 2.9% + $0.30 per online card transaction, depending on country/payment type/provider.
- No sales means no payment processing fees.

Example:

- 20 sales/month
- Average order value: $40
- Revenue: $800
- Fixed hosting stack: about $15
- Processor fixed fees: 20 x $0.30 = $6
- Processor percentage fees: 2.9% of $800 = $23.20
- Approx total processor fees: $29.20
- Approx monthly operating cost with those sales: $44.20

## 5. Core Product Scope

### Public Website

- Home page
- Shop page
- Category pages
- Product detail page
- Cart
- Checkout
- Order confirmation
- Trade-in request page
- Contact page
- Reviews/trust section

### Admin Dashboard

- Staff login
- Role-based permissions
- Sales overview
- Recent orders
- Low-stock alerts
- Product management
- Inventory management
- Order management
- Trade-in management
- Customer/contact messages
- Basic analytics

### Custom Inventory System

- Products
- Product variants
- SKU
- Barcode field
- Category
- Platform/system
- Condition
- Cost
- Sale price
- Quantity on hand
- Quantity reserved
- Quantity sold
- Low-stock threshold
- Supplier/vendor
- Product images
- Internal notes
- Stock adjustment log

### Custom Order System

Order statuses:

- Pending Payment
- Payment Received
- Processing
- Packed
- Shipped
- Delivered
- Cancelled
- Refunded

Core order features:

- Order number
- Customer info
- Shipping/pickup option
- Line items
- Taxes
- Discounts
- Payment status
- Fulfillment status
- Refund status
- Tracking number
- Staff notes
- Status history

## 6. Main Coding Work

### Main Languages

- C#: primary language for controllers, models, services, business logic, payment integration, admin workflows, and tests.
- SQL: database understanding, reporting queries, migrations review, and analytics.
- HTML: Razor view structure.
- CSS: custom styling and The Game Pond brand polish.
- JavaScript: cart interactions, dynamic admin controls, image previews, and small UI behaviors.

### Backend Coding

Used for:

- ASP.NET Core MVC controllers
- Entity Framework Core models and migrations
- Inventory services
- Order services
- Payment services
- Trade-in services
- Authentication and role checks
- Audit logging
- Adapter layer for future integrations

### Frontend Coding

Used for:

- Razor Views
- Bootstrap 5.3.8 components
- Responsive storefront pages
- Admin dashboard pages
- Cart and checkout UI
- Product image upload previews
- Form validation and light interactivity

### Database Coding

Used for:

- Products
- Product variants
- Inventory counts
- Stock adjustments
- Orders
- Order items
- Customers
- Trade-ins
- Store credit
- Admin users
- Audit logs
- Payment events
- Future integration events

### Payment Coding

Used for:

- Creating Stripe Checkout sessions
- Receiving Stripe webhooks
- Verifying webhook signatures
- Marking orders as paid
- Preventing duplicate webhook handling
- Handling cancelled payments
- Preparing refund logic

## 7. Database Ownership

The Game Pond database is the source of truth at launch.

At launch:

- Products live in our database.
- Inventory counts live in our database.
- Orders live in our database.
- Payments are confirmed by Stripe/Square webhook.
- Staff manages fulfillment in our admin dashboard.

Later:

- If needed, Square/Shopify/Lightspeed becomes an external sync target or source.
- Add an adapter layer without rewriting the storefront/admin.

## 8. Upgrade Path

Keep the application modular from day one.

Initial adapter:

- `LocalCommerceAdapter`

Future adapters:

- `SquareCommerceAdapter`
- `ShopifyCommerceAdapter`
- `LightspeedCommerceAdapter`

The first launch should not depend on these future integrations, but the code should keep clean service boundaries so they can be added later.

## 9. Sprint Plan Summary

Detailed sprint plan lives in `SPRINT_PLAN.md`.

### Sprint 0: Project Foundation

- Confirm client decisions.
- Create repo.
- Scaffold ASP.NET Core MVC app.
- Add Bootstrap 5.3.8 and Game Pond brand tokens.
- Configure EF Core and database.
- Configure ASP.NET Core Identity.

### Sprint 1: Product And Inventory Admin

- Product CRUD.
- Image upload.
- Category/platform/condition fields.
- Inventory counts and stock adjustments.
- Low-stock threshold.

### Sprint 2: Storefront And Cart

- Home page.
- Shop page.
- Product detail page.
- Cart flow.
- Stock validation.

### Sprint 3: Checkout And Orders

- Stripe Checkout.
- Payment webhook.
- Order creation.
- Inventory decrement.
- Order confirmation.

### Sprint 4: Admin Order Handling

- Orders table.
- Order detail.
- Status workflow.
- Tracking number.
- Cancel/refund preparation.
- Customer emails.

### Sprint 5: Trade-In Workflow

- Customer trade-in form.
- Image upload.
- Staff review queue.
- Offer workflow.
- Convert accepted trade-in to inventory.

### Sprint 6: QA, Deployment, And Launch

- VPS deployment.
- SSL.
- Backups.
- Smoke tests.
- Security review.
- Staff handoff docs.

## 10. Agent Plan

The agent briefs live in `agents/`.

First wave:

- Product Architecture Agent
- .NET Backend/Admin Agent
- Razor/Bootstrap Storefront Agent
- Database/EF Core Agent
- Payment Agent
- DevOps/VPS Agent
- QA Agent

Second wave:

- Trade-In Agent
- Analytics/Growth Agent
- Integration Adapter Agent
- Content/Catalog Agent

## 11. Questions To Confirm With Client

- Does he need in-person POS on day one?
- Does he already use Square, Clover, Shopify, spreadsheets, or anything else?
- Does he want shipping, local pickup, or both?
- How many products should launch on day one?
- Are used games one quantity under one SKU, or does every used item need a unique SKU?
- Does he need barcode scanning immediately?
- Does he want trade-in payout as store credit only, cash only, or both?
- Who will enter inventory into the system?
- Who will process orders?
- What is the target launch date?
- Does the team prefer PostgreSQL on Linux VPS or SQL Server Express on Windows?

## 12. First Build Definition Of Done

The first complete proof of workflow is done when:

- Admin can log in.
- Admin creates a product.
- Admin sets inventory quantity.
- Product appears on shop page.
- Customer adds product to cart.
- Customer pays in Stripe test mode.
- Stripe webhook marks order as paid.
- Order appears in admin.
- Inventory decreases.
- Staff marks order as processing, packed, and shipped.

