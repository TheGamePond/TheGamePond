# Monday.com Board Structure

Board name:

- The Game Pond Marketplace Build

Purpose:

- Track the ASP.NET Core MVC marketplace build from foundation through launch.

Recommended columns:

- Item
- Group
- Status
- Priority
- Sprint
- Agent/Owner
- Description
- Dependency
- GitHub Link
- Due Date
- Notes

Recommended status labels:

- Not Started
- In Progress
- Blocked
- Review
- Done

Recommended priority labels:

- Critical
- High
- Medium
- Low

Groups:

- Sprint 0 - Foundation And Decisions
- Sprint 1 - Product And Inventory Admin
- Sprint 2 - Storefront And Cart
- Sprint 3 - Checkout Payment And Orders
- Sprint 4 - Admin Order Handling
- Sprint 5 - Trade-In Workflow
- Sprint 6 - Analytics And Polish
- Sprint 7 - VPS Deployment
- Sprint 8 - Final QA And Handoff
- Client Questions
- Backlog

Import file:

- `MONDAY_IMPORT.csv`

Notes:

- Import the CSV into Monday, then map `Group` to Monday groups manually if Monday does not auto-group during import.
- Keep `Agent/Owner` as a text column at first. Replace with real people once the team is assigned.
- Keep due dates blank until the client confirms the target launch date.

