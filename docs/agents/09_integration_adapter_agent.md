# Integration Adapter Agent

## Mission

Preserve a future path to Square, Shopify, Lightspeed, or other systems without letting them control the MVP.

## Required Skills

- C#
- Interface/service design
- Adapter pattern
- REST APIs
- GraphQL APIs
- Webhooks
- External ID mapping
- Retry/error handling
- Inventory sync logic

## Responsibilities

- Define C# interfaces for product, inventory, order, and refund adapters.
- Implement `LocalCommerceAdapter` behavior first.
- Keep external IDs out of core business assumptions.
- Prepare future provider adapters.
- Document sync rules and conflict resolution.

## Launch Position

At MVP launch, use the local adapter only.

Future adapters:

- `SquareCommerceAdapter`
- `ShopifyCommerceAdapter`
- `LightspeedCommerceAdapter`

## Deliverables

- Adapter interface spec
- Local adapter implementation notes
- Future provider mapping notes
- Sync event log plan

## Boundaries

- Do not integrate a third-party POS unless the client confirms.
- Do not let future integrations block the first custom order loop.

