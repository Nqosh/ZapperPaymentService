# Solution Overview

This solution implements a payment processing platform integrating with the Acme payment provider.

The application supports:

- Payment creation
- Idempotent request handling
- Acme payment submission
- Asynchronous webhook notifications
- Notification signature validation
- Duplicate notification handling
- Reversal and refund scenarios
- Daily reporting
- Automated testing

The solution follows a Clean Architecture approach and uses PostgreSQL for persistence.

# Design and Key Trade-offs

## Architecture

The solution is structured into four layers:

API
↓
Application
↓
Domain
↑
Infrastructure

### Domain

Contains:

- Payment aggregate
- Payment events
- Domain enums
- State transition rules
- Business logic

### Application

Contains:

- Service interfaces
- Use-case implementations
- DTOs
- Repository abstractions

### Infrastructure

Contains:

- Entity Framework Core
- PostgreSQL persistence
- Repository implementations
- Acme HTTP client
- Webhook signature validation
- Database configuration
- Seed data

### API

Contains:

- Minimal API endpoints
- Dependency injection configuration
- Swagger configuration
- Application composition root

## Clean Architecture without CQRS

CQRS, MediatR, Commands, Queries and Handlers were intentionally not introduced.

For the scope of this assignment a service-based approach provided:

- Less complexity
- Easier debugging
- Faster implementation

## Payment Lifecycle

Payments move through controlled state transitions.

Examples:

Pending → Submitted → Approved

Pending → Submitted → Declined

Additional transitions support:

- Refunded
- Reversed

This prevents invalid state changes caused by duplicate or out-of-order notifications.

## Idempotency

The application uses AppPaymentId as the idempotency key.

If the same request is submitted multiple times:

1. Existing payment is located.
2. Existing payment is returned.
3. No duplicate provider submission occurs.

## Notification Processing

Acme notifications are processed asynchronously and support:

- Duplicate notifications
- Out-of-order delivery
- Unsupported event handling
- Signature validation

## Reporting

Reporting is generated from payment events rather than maintaining a separate reporting table.

Benefits:

- Full audit trail
- Simpler data model
- Single source of truth

Trade-off:

- More complex aggregation queries.

# AWS Deployment Approach

## Compute

AWS ECS Fargate would be my preferred deployment model. The solution consists of a small number of containerised services and does not require the operational complexity of managing EC2 instances or a Kubernetes cluster. Fargate provides a serverless container platform with built-in scaling, high availability, and reduced maintenance overhead, allowing engineering effort to remain focused on payment processing and business functionality rather than infrastructure management. While EKS would offer greater flexibility and portability, it would introduce unnecessary complexity for the scale and requirements of this solution.

Containers:
- zapper-payments-api
- acme-pay-stub

## Database

Amazon RDS PostgreSQL

## Networking

Internet
↓
Application Load Balancer
↓
ECS Services
↓
RDS PostgreSQL

## Secrets Management

AWS Secrets Manager

## Monitoring

- CloudWatch
- AWS X-Ray

## CI/CD

GitHub Actions

Build → Test → Docker Build → Push to ECR → Deploy to ECS

# AI Workflow and Tools

## AI Tools Used

- Microsoft Copilot

## Validation of AI Generated Work

- Manual code reviews
- Swagger testing
- Database validation
- Integration testing against Acme Stub
- Breakpoints and debugging
- SQL query verification

# AI Configuration Files

No custom AI configuration files were required for this implementation.

# AI Artifacts

Suggested structure:

ai/
├── architecture-notes.md
├── prompts.md
├── test-scenarios.md
├── acme-integration-debugging.md
├── reporting-investigation.md
└── review-notes.md

# What I Would Do Differently

- Add integration tests earlier
- Capture AI design decisions more systematically
- Add automated webhook tests
- Add OpenTelemetry tracing

# Conclusion

The final solution focuses on Clean Architecture, maintainability, reliable payment processing, robust webhook handling, idempotent request processing, auditable payment history, and practical use of AI-assisted development.
