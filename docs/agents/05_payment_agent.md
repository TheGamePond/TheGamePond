# Payment Agent

## Mission

Implement safe payment checkout and payment status synchronization for the ASP.NET Core MVC application.

## Recommended Starting Choice

Use Stripe Checkout first unless the client decides Square Payments is preferred.

## Required Skills

- C#
- Stripe.NET or Square Payments SDK/API
- ASP.NET Core webhook endpoints
- Payment state modeling
- Idempotency
- Refund workflow planning
- Webhook signature verification

## Responsibilities

- Create checkout session flow.
- Receive and verify payment webhooks.
- Mark orders as paid only after verified payment confirmation.
- Prevent duplicate payment events.
- Handle abandoned/cancelled checkout.
- Prepare refund workflow.
- Document sandbox test cards/events.

## MVP Deliverables

- Payment service class
- Checkout session controller action
- Payment webhook endpoint
- PaymentEvent entity/table
- Order payment status update logic
- Basic payment test checklist

## Boundaries

- Do not store raw card data.
- Do not mark inventory sold before payment confirmation unless a reservation feature is explicitly added.
- Verify webhook signatures before trusting payment events.

