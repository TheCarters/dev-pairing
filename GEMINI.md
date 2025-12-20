# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Developer Pairing Coordination App - a full-stack application enabling development teams to coordinate pair programming sessions through time slot scheduling. The app uses deep-linked Dev Groups for team organization and supports browser-based push notifications via ntfy.sh.

## Architecture

**Monorepo Structure:**
- `src/client/dev-pairing/` - Angular 21 frontend application
- `src/server/DevPairing/` - .NET 10 Minimal API backend
- `docs/Spec.md` - Complete feature and technical specifications

**Key Technical Decisions:**
- No traditional authentication: Users identified by first name + email stored in localStorage
- SQLite database with Aspire for backend
- Signal Store for Angular state management
- Service workers for push notifications
- Tailwind CSS v4 for styling (with PostCSS)
- Vitest for testing (not Karma/Jasmine)

## Development Commands

### Frontend (Angular)
Working directory: `src/client/dev-pairing/`

```bash
npm start              # Start dev server on http://localhost:4200
ng build               # Production build
ng build --watch       # Development build with watch mode
ng test                # Run unit tests with Vitest
ng generate component component-name  # Generate new component
```

### Backend (.NET)
Working directory: `src/server/DevPairing/`

```bash
dotnet run             # Start the API server
dotnet build           # Build the project
dotnet test            # Run tests (when implemented)
```

## Important Implementation Details

**Routing & Deep Linking:**
- Root URL without group ID is NOT supported
- All routes require Dev Group identifier: `https://theapp.com/XXXX-XXXX-XXXX-XXXX`
- Deep linking to individual slots must be supported

**User Management:**
- No password-based auth
- User data (first name, email) stored in localStorage
- Server associates email with UserId
- Server should retrieve existing user by email if localStorage cleared

**Notifications:**
- Browser notifications via ntfy.sh integration
- Two user preferences: join notifications and pre-session reminders
- Notifications trigger when other devs join available slots

**State Management:**
- Use Signal Store (not NgRx or other alternatives)
- Leverage Angular 21's signal-based reactivity

**Styling:**
- Tailwind CSS v4 with PostCSS configuration
- Component styles configured in `angular.json`
- Prettier configured with 100 character line width and single quotes

## Code Patterns

**Angular Component Structure:**
- Template files use `.html` extension (e.g., `app.html`)
- Style files use `.css` extension (e.g., `app.css`)
- TypeScript files use `.ts` extension without `.component` suffix (e.g., `app.ts`)
- Components use standalone API with `imports` array

**Backend:**
- .NET 10 with Minimal APIs pattern
- Target framework: `net10.0`
- Nullable reference types enabled
- Implicit usings enabled
