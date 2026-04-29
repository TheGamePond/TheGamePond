# Architecture Decision: Lean .NET VPS Plan

Date: 2026-04-27
Decision: Use ASP.NET Core MVC + C# + EF Core + self-hosted database on a budget VPS.

## Compared Options

### Option A: Managed Azure .NET Stack

Stack:

- ASP.NET Core MVC
- C#
- Azure SQL Database
- Entity Framework Core
- Razor Views
- ASP.NET Core Identity
- Payment gateway checkout, with Stripe Checkout as the temporary first adapter
- Azure App Service
- Azure Blob Storage
- Azure Key Vault/App Configuration
- Azure Application Insights

Strengths:

- Professional managed infrastructure.
- Easier scaling path.
- Managed database and monitoring.
- Good fit for larger production workloads.

Weaknesses for this project right now:

- Higher fixed monthly cost.
- More Azure service setup.
- Less aligned with the client's current goal of starting as lean as possible.

### Option B: Lean .NET VPS Stack

Stack:

- ASP.NET Core MVC
- C#
- PostgreSQL on Linux VPS by default
- SQL Server Express as alternate
- Entity Framework Core
- Razor Views
- Bootstrap 5.3.8
- ASP.NET Core Identity
- Payment gateway checkout, with Stripe Checkout as the temporary first adapter
- Budget VPS
- Local uploads folder with backups
- Let's Encrypt SSL
- Nginx or Caddy

Strengths:

- Lowest practical fixed monthly cost.
- Keeps the business off Shopify/Square/Lightspeed monthly fees at launch.
- Still uses a professional Microsoft/.NET stack.
- Gives the client a custom admin, inventory, and order system.
- Can upgrade later to managed database, object storage, CDN, monitoring, or POS integrations.

Weaknesses:

- More server maintenance responsibility.
- Backups and updates must be handled carefully.
- Scaling later will require more DevOps work.

## Final Decision

Use Option B for MVP.

The decided launch architecture is:

- ASP.NET Core MVC
- C#
- Entity Framework Core
- PostgreSQL on a budget Linux VPS by default
- SQL Server Express only if the team chooses a Windows-first deployment
- Razor Views with Bootstrap 5.3.8
- ASP.NET Core Identity
- Payment gateway checkout, with Stripe Checkout as the temporary first adapter
- Shipping-only fulfillment at launch
- Let's Encrypt SSL
- Budget VPS hosting

## Cost Target

Expected fixed monthly cost:

- Lean range: $8-$32/month
- Most likely: $10-$18/month
- Best starter target: $9-$10/month before sales

Payment fees are per sale only, usually around 2.9% + $0.30 per online transaction depending on provider/country/payment type.

## Upgrade Path

Upgrade only when the business needs it:

- Better VPS
- Managed database
- Object storage
- CDN
- Premium email
- Monitoring
- Barcode/scanner integrations
- Square/Shopify/Lightspeed API adapter
