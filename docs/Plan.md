# Developer Pairing Coordination App - Implementation Plan

## Overview

Backend-first implementation of a pairing coordination app using .NET 10 Minimal APIs with Aspire/SQLite and Angular 21 with FullCalendar and Signal Store.

---

## Phase 1: Backend Foundation (Aspire + SQLite)

### 1.1 Restructure to Aspire Solution

**Create new project structure:**
```
src/server/
  DevPairing.AppHost/           # Aspire orchestration
  DevPairing.ServiceDefaults/   # Shared Aspire configuration
  DevPairing.Api/               # Main API (rename existing DevPairing)
  DevPairing.Api.Tests/         # Test project
```

**NuGet Packages for DevPairing.Api:**
- `Microsoft.EntityFrameworkCore.Sqlite`
- `Microsoft.EntityFrameworkCore.Design`
- `Microsoft.AspNetCore.OpenApi`
- `Swashbuckle.AspNetCore`

### 1.2 Database Models

**Create in `DevPairing.Api/Models/`:**

| Model | Key Fields |
|-------|------------|
| `DevGroup` | Id, Name, Identifier (Guid), CreatedAt, IsActive |
| `User` | Id, FirstName, Email, CreatedAt (global, not tied to single group) |
| `UserGroupMembership` | Id, UserId, DevGroupId, JoinedAt (many-to-many join table) |
| `PairingSlot` | Id, DevGroupId, OwnerId, StartTime, EndTime, Title, Description, NtfyTopic |
| `PairingSignup` | Id, SlotId, UserId, SignedUpAt, Status |
| `UserPreferences` | Id, UserId, NotifyOnSlotJoin, NotifyBeforeSessionStart (global per user) |

**Key relationships:**
- User ↔ DevGroup: Many-to-many via `UserGroupMembership`
- User preferences are global (not per-group)
- When user enters a group, check if email exists globally; if so, add membership to that group

**Create in `DevPairing.Api/Data/`:**
- `AppDbContext.cs` - EF Core DbContext
- `SeedData.cs` - Seed initial Dev Groups

---

## Phase 2: API Endpoints

### 2.1 Create DTOs

**In `DevPairing.Api/Dtos/`:** Request/Response DTOs for all entities

### 2.2 Endpoint Implementation

**In `DevPairing.Api/Endpoints/`:**

| File | Endpoints |
|------|-----------|
| `DevGroupEndpoints.cs` | GET `/{identifier}`, POST `/`, GET `/{id}/members` |
| `UserEndpoints.cs` | POST `/` (create/find + join group), GET `/{id}`, GET `/email/{email}`, GET `/{id}/groups` |
| `MembershipEndpoints.cs` | POST `/` (join group), DELETE `/{id}` (leave group) |
| `SlotEndpoints.cs` | CRUD + GET by group/date range |
| `SignupEndpoints.cs` | POST, DELETE, GET by slot/user |
| `PreferencesEndpoints.cs` | GET `/users/{id}/preferences`, PUT `/users/{id}/preferences` (global user prefs) |

**Key files:**
- `src/server/DevPairing.Api/Program.cs` - Register all endpoints
- `src/server/DevPairing.Api/Endpoints/SlotEndpoints.cs` - Core pairing logic

---

## Phase 3: Ntfy.sh Integration

**Create in `DevPairing.Api/Services/`:**
- `INtfyService.cs` - Interface
- `NtfyService.cs` - HTTP client to ntfy.sh
- `NtfyOptions.cs` - Configuration

**Notification triggers:**
1. When user signs up for slot → notify slot owner (if preference enabled)
2. Pre-session reminder → notify all participants (future: background job)

---

## Phase 4: Backend Testing

**Create `DevPairing.Api.Tests/` with:**
- `Integration/` - Endpoint tests with WebApplicationFactory
- `Unit/` - Service unit tests
- `Fixtures/` - Test utilities

**Packages:** xunit, FluentAssertions, Moq, Microsoft.AspNetCore.Mvc.Testing

---

## Phase 5: Frontend Core Infrastructure

### 5.1 Install Dependencies

```bash
cd src/client/dev-pairing
npm install @ngrx/signals @ngrx/operators
npm install @fullcalendar/angular @fullcalendar/core @fullcalendar/daygrid @fullcalendar/timegrid @fullcalendar/interaction
npm install date-fns
```

### 5.2 Create Core Services

**In `src/app/core/services/`:**
- `api.service.ts` - Base HTTP service
- `dev-group.service.ts`
- `user.service.ts`
- `slot.service.ts`
- `signup.service.ts`
- `preferences.service.ts`
- `notification.service.ts` - Ntfy SSE subscription

**In `src/app/core/models/`:** TypeScript interfaces matching backend DTOs

### 5.3 Signal Stores

**In `src/app/store/`:**
- `user.store.ts` - Current user (global) + localStorage persistence + user's group memberships
- `group.store.ts` - Currently active dev group (from route param)
- `slots.store.ts` - Calendar slots for current group
- `signups.store.ts` - User's signups (can span multiple groups)
- `preferences.store.ts` - Global user notification preferences

---

## Phase 6: Frontend Components

### 6.1 Routing

**Update `src/app/app.routes.ts`:**
```typescript
{ path: '', component: NoGroupComponent },
{ path: 'preferences', component: PreferencesComponent },  // Global user preferences (outside groups)
{ path: 'g/:groupId', component: GroupLayoutComponent, children: [
    { path: '', component: CalendarComponent },
    { path: 'slot/:slotId', component: SlotDetailComponent }
]}
```

**Route structure rationale:**
- `/preferences` - Global user settings (notifications apply across all groups)
- `/g/:groupId` - Group-specific views (calendar, slots)
- User can participate in multiple groups with same account

### 6.2 Component Structure

**Create in `src/app/features/`:**

| Feature | Components |
|---------|------------|
| `group/` | `group-layout/`, `no-group/` |
| `calendar/` | `calendar.ts`, `slot-dialog/` |
| `slot-detail/` | Single slot view (deep link target) |
| `preferences/` | Global user notification settings (top-level route) |
| `user-login/` | Name/email entry form (modal/dialog) |

**Create in `src/app/shared/`:**
- `components/header/` - Include link to global preferences
- `components/user-avatar/`, `loading-spinner/`
- `guards/group-exists.guard.ts`, `user-required.guard.ts`, `auth.guard.ts`

**Key files:**
- `src/client/dev-pairing/src/app/features/calendar/calendar.ts` - FullCalendar integration
- `src/client/dev-pairing/src/app/store/slots.store.ts` - Slot state management

---

## Phase 7: Notification Integration

**Browser notifications via ntfy.sh SSE:**
1. User subscribes to slot's ntfy topic
2. Service listens via EventSource
3. Shows browser Notification API alerts

**Create:** `src/app/core/services/notification.service.ts`

---

## Phase 8: Frontend Testing

**In `src/app/testing/`:**
- `test-utils.ts`
- `mock-services.ts`
- `fixtures.ts`

Component tests with Vitest + @testing-library/angular

---

## Phase 9: Integration

1. Configure CORS in .NET API for Angular dev server
2. Create `proxy.conf.json` for Angular to proxy `/api` calls
3. End-to-end testing of full flow

---

## Critical Files Summary

| File | Purpose |
|------|---------|
| `src/server/DevPairing.Api/Program.cs` | API entry, middleware, endpoint registration |
| `src/server/DevPairing.Api/Data/AppDbContext.cs` | Database context and relationships |
| `src/server/DevPairing.Api/Models/UserGroupMembership.cs` | Many-to-many user↔group relationship |
| `src/server/DevPairing.Api/Endpoints/SlotEndpoints.cs` | Core slot CRUD operations |
| `src/server/DevPairing.Api/Endpoints/MembershipEndpoints.cs` | User group membership management |
| `src/client/dev-pairing/src/app/app.routes.ts` | Frontend routing (preferences at root, groups under /g/) |
| `src/client/dev-pairing/src/app/features/calendar/calendar.ts` | FullCalendar integration |
| `src/client/dev-pairing/src/app/store/user.store.ts` | Global user state + memberships + localStorage |
| `src/client/dev-pairing/src/app/features/preferences/preferences.ts` | Global notification settings |

---

## Implementation Order

1. **Phase 1** - Aspire solution structure + database models
2. **Phase 2** - All API endpoints
3. **Phase 3** - Ntfy.sh service
4. **Phase 4** - Backend tests
5. **Phase 5** - Frontend services + stores
6. **Phase 6** - Frontend components + routing
7. **Phase 7** - Frontend notifications
8. **Phase 8** - Frontend tests
9. **Phase 9** - Integration + polish
