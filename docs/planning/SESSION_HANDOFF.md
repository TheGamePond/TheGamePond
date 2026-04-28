# The Game Pond Session Handoff

Date: 2026-04-27
Workspace: `c:\Users\silve\Projects\Miguels Project`
Drive root: https://drive.google.com/drive/folders/1Kcx60Sw7aNemvG_JDc9Q2El5H8T0Pzwu

## Current Project Direction

The project is now scoped around a lean custom ASP.NET Core MVC marketplace build.

The agreed direction is:

- Build The Game Pond website as a custom full-stack ASP.NET Core MVC application.
- Build our own inventory and order handling software inside the admin dashboard.
- Use C#, Razor Views, Bootstrap 5.3.8, Entity Framework Core, and ASP.NET Core Identity.
- Use self-hosted PostgreSQL on a budget Linux VPS by default.
- SQL Server Express is acceptable if the team chooses a Windows/SQL Server path.
- Do not start with Shopify, Square Retail, Lightspeed, Azure App Service, or Azure SQL as the core system.
- Use Stripe Checkout first unless the client prefers Square Payments.

## Lean Cost Direction

Expected fixed monthly range:

- VPS hosting: $6-$15
- Domain annualized: $1-$2/month
- SSL: free with Let's Encrypt
- Database: free when self-hosted
- Email receipts: $0-$10
- Backups: $0-$5
- Payments: only per sale

Likely fixed cost:

- Around $10-$18/month before sales.
- Best starter target: around $9-$10/month.

Payment processing:

- Usually around 2.9% + $0.30 per online transaction, depending on provider/country/payment type.

## Why This Direction Was Chosen

The client/developer conversation focused on keeping monthly costs low and avoiding paid commerce platforms too early.

The plan gives the client:

- A real admin dashboard.
- Real inventory management.
- Real order status tracking.
- A secure payment gateway.
- A future path to integrate Square/Shopify/Lightspeed later through adapters.

## Documents Created Or Updated

- `MASTER_PLAN.md`
  - Updated to reflect the current .NET/C#/budget VPS direction.
- `LEAN_CUSTOM_MARKETPLACE_PLAN.md`
  - Current source of truth for the lean custom .NET build.
- `SPRINT_PLAN.md`
  - Sprint-by-sprint execution plan.
- `The_Game_Pond_Lean_Custom_Marketplace_Plan.docx`
  - Older Word export; should be regenerated if the client needs a final Word copy of the new .NET plan.
- `handoff/SESSION_HANDOFF.md`
  - This file.
- `agents/`
  - Updated C#/.NET agent briefs.

## Reviewed Project Assets

Google Drive root contents found:

- `TGP WEB`
  - `css/bootstrap.css`, Bootstrap v5.3.8
  - `js/bootstrap.js`
  - `img/` appears empty
- `The Game Pond Logo and Color Chart`
  - `THE GAME POND.png`
  - `Color Chart.png`
  - `Business card front.jpeg`
  - `Business card back.jpeg`

Local brand asset copies were placed in:

- `.analysis_assets/drive/`

## Brand System

Primary colors:

- Pond Green: `#6CC24A`
- Dark Green: `#2E7D32`
- Pond Blue: `#1F6F8B`
- Dark Blue: `#0F3D5E`
- Clean White: `#F5F7F6`

Accent colors:

- Accent Red: `#E63946`
- Accent Yellow: `#F4D35E`
- Accent Blue: `#3A86FF`
- Accent Green: `#2ECC71`
- Accent Purple: `#9D4EDD`

Brand language:

- `Collect. Play. Repeat.`
- `Enter the Pond`
- Gaming, TCG and collectibles, retro, anime, live streams, drops.

## Recommended Stack

- Framework: ASP.NET Core MVC
- Language: C#
- IDE: Visual Studio 2022 or newer
- Runtime: .NET 8 LTS or current stable LTS
- ORM: Entity Framework Core
- Database: PostgreSQL by default, SQL Server Express as alternate
- Views: Razor Views
- Styling: Bootstrap 5.3.8, HTML, CSS, light JavaScript
- Auth: ASP.NET Core Identity with roles
- Payments: Stripe Checkout first
- Hosting: budget VPS
- Reverse proxy: Nginx or Caddy
- SSL: Let's Encrypt

## First Build Definition Of Done

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

## Sprint Plan

See `SPRINT_PLAN.md`.

Sprint summary:

- Sprint 0: Foundation and client decisions
- Sprint 1: Product and inventory admin
- Sprint 2: Storefront and cart
- Sprint 3: Checkout, payment, and order creation
- Sprint 4: Admin order handling
- Sprint 5: Trade-in workflow
- Sprint 6: Analytics, polish, and admin usability
- Sprint 7: VPS deployment and launch hardening
- Sprint 8: Final QA and client handoff

## Agent Files

Updated agent files:

- `agents/01_product_architecture_agent.md`
- `agents/02_brand_ui_agent.md`
- `agents/03_storefront_agent.md`
- `agents/04_backend_admin_agent.md`
- `agents/05_payment_agent.md`
- `agents/06_devops_qa_agent.md`
- `agents/07_trade_in_agent.md`
- `agents/08_analytics_growth_agent.md`
- `agents/09_integration_adapter_agent.md`
- `agents/10_content_catalog_agent.md`
- `agents/11_database_efcore_agent.md`
- `agents/12_vps_deployment_agent.md`

## Key Client Questions Still Open

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

## Next Developer Actions

1. Review `LEAN_CUSTOM_MARKETPLACE_PLAN.md`.
2. Review `SPRINT_PLAN.md`.
3. Review the agent files in `agents/`.
4. Confirm open client questions.
5. Create or confirm GitHub repo.
6. Scaffold ASP.NET Core MVC app.
7. Build Sprint 0 and Sprint 1.
8. Keep the custom database as source of truth unless client changes direction.

## Google Drive Note

The target Drive folder is:

https://drive.google.com/drive/folders/1Kcx60Sw7aNemvG_JDc9Q2El5H8T0Pzwu

The previous session created Drive-native fallback docs, but the connector did not expose folder creation or arbitrary local file upload into a parent folder. Local zip packages should be regenerated after this .NET pivot if the team wants a fresh upload.

