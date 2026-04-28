# Payment Agent

## Mission

Implement safe payment checkout and payment status synchronization for the ASP.NET Core MVC application.

## Recommended Starting Choice

Use a payment gateway for every online payment. Stripe Checkout is the first temporary adapter, but the provider is expected to change, so design a provider-neutral payment service boundary before adding Stripe-specific code.

## Required Skills

- C#
- Stripe.NET or the chosen payment gateway SDK/API
- ASP.NET Core webhook endpoints
- Payment state modeling
- Idempotency
- Refund workflow planning
- Webhook signature verification

## Responsibilities

- Create checkout session flow behind a provider-neutral service interface.
- Receive and verify payment webhooks.
- Mark orders as paid only after verified payment confirmation.
- Prevent duplicate payment events.
- Handle abandoned/cancelled checkout.
- Prepare refund workflow.
- Document sandbox test cards/events for the active provider.

## MVP Deliverables

- Provider-neutral payment service interface plus active provider implementation
- Checkout session controller action
- Payment webhook endpoint
- PaymentEvent entity/table
- Order payment status update logic
- Basic payment test checklist

## Boundaries

- Do not store raw card data.
- Do not mark inventory sold before payment confirmation unless a reservation feature is explicitly added.
- Verify webhook signatures before trusting payment events.
- Do not place Stripe-specific assumptions in order, inventory, or controller code; isolate them in the Stripe adapter.
