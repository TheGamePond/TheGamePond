# The Game Pond Session Handoff

Date: 2026-05-01
Sprint: Sprint 2 - Storefront And Cart
Project path: `C:/Users/andre/source/repos/TheGamePond`
Drive root: https://drive.google.com/drive/folders/1Kcx60Sw7aNemvG_JDc9Q2El5H8T0Pzwu

## Current Project Direction

The Game Pond is still scoped as a lean custom ASP.NET Core MVC marketplace.

The current direction remains:

- Build a custom full-stack ASP.NET Core MVC application.
- Use C#, Razor Views, Bootstrap 5.3.8, Entity Framework Core, and ASP.NET Core Identity.
- Use PostgreSQL as the default local and production database path.
- Keep the custom database as the source of truth for catalog, inventory, cart, checkout, and orders.
- Launch fulfillment remains shipping-only.
- Payment processing will be added later through a gateway adapter, with Stripe Checkout as the first temporary adapter.
- Avoid coupling the app to Shopify, Square Retail, Lightspeed, Azure App Service, or Azure SQL as the MVP core.

## Sprint 2 Goal

Customers can browse products from the database and build a cart.

Sprint 2 scope:

- Public shop page.
- Product listing from database.
- Category filter.
- Search/filter basics.
- Product detail page.
- Product image gallery.
- Cart add/remove/update quantity.
- Stock validation.
- Empty cart state.
- Responsive mobile layout.

## Stage 2 Code Present

The project has been updated with Sprint 2 storefront/cart files, including:

- `TheGamePond/Controllers/ShopController.cs`
- `TheGamePond/Controllers/CartController.cs`
- `TheGamePond/Services/Cart/`
- `TheGamePond/ViewComponents/CartNavViewComponent.cs`
- `TheGamePond/Models/Cart/`
- `TheGamePond/Models/Storefront/`
- `TheGamePond/Views/Shop/`
- `TheGamePond/Views/Cart/`

The Sprint 2 code expects the catalog schema from migration:

- `20260429152532_AddCatalogInventoryFoundation`

That schema uses:

- `Product.Slug`
- `Product.CategoryId`
- `Product.Status`
- `Product.Condition` as `ProductCondition`
- `Product.Franchise`
- `ProductImage.ImagePath`
- `ProductImage.SortOrder`
- `ProductCategory.SortOrder`
- `InventoryItem.LocationCode`
- `StockAdjustment.QuantityDelta`
- `StockAdjustment.Reason` as `StockAdjustmentReason`
- `StockAdjustment.Notes`
- `StockAdjustment.CreatedByUserId`

## Build Issue Found

The build was failing because parts of the entity models and admin product workflow still used older Sprint 1 field names while the Sprint 2 storefront/cart code used the newer catalog model.

Examples of mismatched fields:

- Old: `Product.IsActive`
- New: `Product.Status`
- Old: `Product.ProductCategoryId`
- New: `Product.CategoryId`
- Old: `Product.Condition` as string
- New: `Product.Condition` as `ProductCondition`
- Old: `ProductImage.FilePath`
- New: `ProductImage.ImagePath`
- Old: `StockAdjustment.QuantityChange`
- New: `StockAdjustment.QuantityDelta`
- Old: `StockAdjustment.AdjustedByUserId`
- New: `StockAdjustment.CreatedByUserId`

`ApplicationDbContext.cs` also contained two competing catalog configurations. One block matched the old Sprint 1 shape and another matched the newer Sprint 2 migration shape.

## Debug Fix Completed

Fixed the build by aligning the models, admin controller, admin views, and EF configuration to the existing Sprint 2 migration schema.

Updated:

- `TheGamePond/Models/Catalog/Product.cs`
- `TheGamePond/Models/Catalog/ProductCategory.cs`
- `TheGamePond/Models/Catalog/ProductImage.cs`
- `TheGamePond/Models/Catalog/InventoryItem.cs`
- `TheGamePond/Models/Catalog/StockAdjustment.cs`
- `TheGamePond/Models/Admin/StockAdjustmentViewModel.cs`
- `TheGamePond/Data/ApplicationDbContext.cs`
- `TheGamePond/Controllers/AdminProductsController.cs`
- `TheGamePond/Views/AdminProducts/Details.cshtml`
- `TheGamePond/Views/AdminProducts/Index.cshtml`

Specific fixes:

- Removed duplicate/conflicting catalog model configuration from `ApplicationDbContext`.
- Kept `ApplicationDbContext` aligned with `AddCatalogInventoryFoundation`.
- Updated product creation to generate a unique slug.
- Mapped admin visible/hidden checkbox to `ProductStatus.Active` or `ProductStatus.Draft`.
- Converted admin condition text into `ProductCondition`.
- Updated image handling to save/read `ImagePath`.
- Updated stock adjustment handling to use `QuantityDelta`, enum `Reason`, `Notes`, and `CreatedByUserId`.
- Updated product/category ordering to use category `SortOrder`.
- Updated admin product list/detail views to render `Status`, `ImagePath`, and `QuantityDelta`.
- Removed stale decimal column annotations that caused EF to think a migration was pending.

## Verification Completed

Build command:

```powershell
dotnet build TheGamePond.sln --configfile NuGet.Config
```

Result:

```text
Build succeeded.
0 warnings.
0 errors.
```

EF model check:

```powershell
dotnet ef migrations has-pending-model-changes --project TheGamePond/TheGamePond.csproj --startup-project TheGamePond/TheGamePond.csproj --context ApplicationDbContext
```

Result:

```text
No changes have been made to the model since the last migration.
```

No new migration was needed.

## Current Git Working Tree

Changed files after debug:

```text
TheGamePond/Controllers/AdminProductsController.cs
TheGamePond/Data/ApplicationDbContext.cs
TheGamePond/Models/Admin/StockAdjustmentViewModel.cs
TheGamePond/Models/Catalog/InventoryItem.cs
TheGamePond/Models/Catalog/Product.cs
TheGamePond/Models/Catalog/ProductCategory.cs
TheGamePond/Models/Catalog/ProductImage.cs
TheGamePond/Models/Catalog/StockAdjustment.cs
TheGamePond/Views/AdminProducts/Details.cshtml
TheGamePond/Views/AdminProducts/Index.cshtml
docs/planning/SESSION_HANDOFF_SPRINT_2_STOREFRONT_CART_DEBUG_2026-05-01.md
```

## Sprint 2 Definition Of Done Status

Completed or present:

- Public shop controller exists.
- Product listing uses database-backed products.
- Category filtering exists.
- Search and sorting exist.
- Product detail route exists by slug.
- Product images are wired through `ImagePath`.
- Cart add/update/remove controller exists.
- Cart session service exists.
- Cart validates available stock.
- Empty cart state exists.
- Build is now clean.
- EF model matches the latest migration snapshot.

Still needs browser smoke test:

- `/Shop`
- `/Shop/{slug}`
- Add item to cart.
- Update cart quantity.
- Remove cart item.
- Confirm cart cannot exceed stock.
- Confirm mobile layout.
- Confirm admin-created products appear in shop only when `Status` is `Active`.

## Known Notes

- Existing admin product form still accepts condition as text and converts it to `ProductCondition`. This works, but a future polish pass should replace it with an enum dropdown.
- Existing admin visible/hidden checkbox maps to `Active` and `Draft`. A future admin polish pass can expose full `Draft`, `Active`, and `Archived` status options.
- No new migration was created because the corrected code matches the current migration.
- The temporary scratch migration used during debugging was removed.
- Google Drive upload tooling was not exposed in the current Codex session, so this handoff was written locally and is ready for manual upload if needed.

## Recommended Next Developer Actions

1. Review the changed files.
2. Run the app locally from Visual Studio.
3. Confirm the database is up to date:

```powershell
dotnet ef database update
```

4. Create or edit a product in admin.
5. Make sure product status is active.
6. Open `/Shop`.
7. Open a product detail page.
8. Add the product to cart.
9. Update and remove cart items.
10. Commit the debug fix and Sprint 2 handoff.

Suggested commit message:

```text
Fix Sprint 2 catalog model alignment
```

## Next Sprint Direction

Sprint 3: Checkout, Payment, And Order Creation

Build next:

- Order model.
- Order item model.
- Payment event model.
- Checkout form for customer and shipping details.
- Payment gateway abstraction/service boundary.
- Stripe Checkout session creation as initial adapter.
- Stripe webhook endpoint as initial adapter.
- Webhook signature verification.
- Idempotency for duplicate webhooks.
- Payment Received status.
- Inventory decrement after confirmed payment.
- Order confirmation page.
