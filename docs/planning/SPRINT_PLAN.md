# The Game Pond Sprint Plan

Date: 2026-04-27
Stack: ASP.NET Core MVC, C#, Entity Framework Core, PostgreSQL or SQL Server Express, Bootstrap 5.3.8, budget VPS

## Sprint Rules

- Keep each sprint demoable.
- Do not start with Shopify/Square Retail/Lightspeed as the core system.
- Keep the custom database as the launch source of truth.
- Use Stripe Checkout for secure payment handling unless Square Payments is chosen.
- Avoid Azure App Service/Azure SQL for MVP if the goal is lowest fixed monthly cost.
- Prefer PostgreSQL on a Linux VPS for the cheapest production path.

## Sprint 0: Foundation And Decisions

Goal:

- Establish the codebase, technical baseline, and unanswered client decisions.

Build:

- Create GitHub repo.
- Create ASP.NET Core MVC project.
- Add Bootstrap 5.3.8.
- Add Game Pond brand colors and layout shell.
- Configure EF Core.
- Choose database provider:
  - Preferred: PostgreSQL
  - Alternate: SQL Server Express
- Configure local dev database.
- Add ASP.NET Core Identity.
- Create roles:
  - Owner
  - Admin
  - Staff
- Add initial seed data.

Client decisions:

- Shipping, local pickup, or both.
- Launch product count.
- Payment provider.
- Database choice.
- POS needed on day one or not.

Demo:

- App runs locally.
- Admin login page works.
- Home page shell uses Game Pond branding.
- Database migration runs successfully.

Sprint 0 completion status as of 2026-04-28:

- GitHub repo and `sprint-0-foundation` branch are active.
- ASP.NET Core MVC project builds successfully on .NET 8.
- Game Pond logo, color tokens, and branded home shell are in place.
- Entity Framework Core is configured with PostgreSQL.
- ASP.NET Core Identity is wired into the MVC app.
- Roles `Owner`, `Admin`, and `Staff` are seeded in the initial migration.
- Optional first owner seed is available through environment variables.
- `/Account/Login` renders successfully.
- `/Admin` is protected and redirects unauthenticated users to login.
- Initial migration `InitialIdentityFoundation` has been generated.
- Local HTTP smoke test passed for home, login, and admin redirect.

## Sprint 1: Product And Inventory Admin

Goal:

- Staff can enter products and manage inventory.

Build:

- Product model.
- Product category model.
- Product image model.
- Inventory item/stock model.
- Stock adjustment model.
- Product CRUD screens.
- Category/platform/condition fields.
- SKU and barcode fields.
- Cost and sale price fields.
- Quantity on hand.
- Low-stock threshold.
- Image upload to local storage.
- Admin validation.

Demo:

- Admin creates a product.
- Admin uploads an image.
- Admin adjusts stock.
- Low-stock products are visible.

Definition of done:

- Product records persist in the database.
- Inventory changes are logged.
- Product images render in admin.
- Staff-only access enforced.

## Sprint 2: Storefront And Cart

Goal:

- Customers can browse products and build a cart.

Build:

- Home page.
- Shop page.
- Category filter.
- Search/filter basics.
- Product detail page.
- Product image gallery.
- Cart add/remove/update quantity.
- Stock validation.
- Empty cart state.
- Responsive mobile layout.

Demo:

- Customer browses products.
- Customer opens product detail.
- Customer adds item to cart.
- Customer changes quantity.

Definition of done:

- Cart cannot exceed available stock.
- Product pages are mobile friendly.
- Basic SEO titles/meta exist.

## Sprint 3: Checkout, Payment, And Order Creation

Goal:

- Customer can pay in test mode and produce a real order record.

Build:

- Order model.
- Order item model.
- Payment event model.
- Checkout form.
- Stripe Checkout session creation.
- Stripe webhook endpoint.
- Webhook signature verification.
- Idempotency for duplicate webhooks.
- Payment Received status.
- Inventory decrement after confirmed payment.
- Order confirmation page.

Demo:

- Customer checks out with Stripe test card.
- Webhook marks order paid.
- Admin sees new paid order.
- Inventory decreases.

Definition of done:

- No raw card data stored.
- Failed/cancelled checkout does not decrement inventory.
- Duplicate webhook does not duplicate orders or stock changes.

## Sprint 4: Admin Order Handling

Goal:

- Staff can process paid orders from received to shipped.

Build:

- Orders table.
- Order detail page.
- Status transitions:
  - Pending Payment
  - Payment Received
  - Processing
  - Packed
  - Shipped
  - Delivered
  - Cancelled
  - Refunded
- Tracking number field.
- Staff notes.
- Status history.
- Email notification hooks.
- Cancel/refund admin UI shell.

Demo:

- Staff opens order.
- Staff marks order processing, packed, shipped.
- Status history is visible.

Definition of done:

- Invalid status jumps are prevented.
- Status changes are audit logged.
- Staff cannot edit payment status manually except through approved admin actions.

## Sprint 5: Trade-In Workflow

Goal:

- Customers can submit trade-ins and staff can review them.

Build:

- Trade-in request model.
- Trade-in item model.
- Trade-in image upload.
- Customer trade-in form.
- Admin trade-in queue.
- Condition review fields.
- Offer amount.
- Accept/reject workflow.
- Convert accepted trade-in to product/inventory record.

Demo:

- Customer submits trade-in.
- Staff reviews it.
- Staff creates an offer.
- Accepted item becomes inventory.

Definition of done:

- Upload validation exists.
- Staff-only review enforced.
- Trade-in conversion creates traceable inventory.

## Sprint 6: Analytics, Polish, And Admin Usability

Goal:

- Make the admin useful for daily operations.

Build:

- Dashboard cards:
  - Daily orders
  - Daily revenue
  - Low-stock count
  - Pending orders
  - Pending trade-ins
- Top products query.
- Refund rate placeholder/report.
- Customer/contact message handling.
- Admin navigation polish.
- Product import/export planning.

Demo:

- Owner can open dashboard and see store health.

Definition of done:

- Dashboard loads quickly with realistic sample data.
- Metrics are based on database records.
- Admin UI is easy to scan.

## Sprint 7: VPS Deployment And Launch Hardening

Goal:

- Prepare production deployment on the lean hosting plan.

Build:

- VPS setup notes.
- .NET runtime installation.
- PostgreSQL or SQL Server Express install.
- Nginx or Caddy reverse proxy.
- Let's Encrypt SSL.
- Environment variable configuration.
- Uploads folder permissions.
- Backup script.
- Log rotation.
- Basic monitoring/error logging.
- Deployment checklist.

Demo:

- App runs on staging VPS over HTTPS.
- Database persists data.
- Images upload and render.
- Stripe webhook endpoint reachable.

Definition of done:

- Secrets are not in source code.
- Backups have been tested.
- HTTPS works.
- Restart procedure documented.

## Sprint 8: Final QA And Client Handoff

Goal:

- Ship a stable MVP and train staff.

Build:

- End-to-end smoke tests.
- Mobile QA.
- Checkout QA.
- Admin permission QA.
- Order workflow QA.
- Trade-in workflow QA.
- Staff quick-start guide.
- Known limitations list.
- Launch checklist.

Demo:

- Full flow from product creation to paid shipped order.

Definition of done:

- Owner can operate the admin dashboard.
- Team knows how to deploy/update.
- Known post-launch backlog is documented.
