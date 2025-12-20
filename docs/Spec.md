# Developer Pairing Coordination App

## Summary

This application will be used to allow a team of developers to coordinate together to schedule time to pair program with each other. The devs should be able to show their availability and any other devs should be able to sign up on any of those available slots. The application does not have NOT have the capability to spin up video calls or anything outside of this timme slot coordination.

## Features

- There should a Dev Group at the top level that has it's own identifier. This dev group should be deep linked to and that will serve as the landing page where interested users will go. There should be support for n number of Dev Groups so that the tool is usable by many teams. There should not be support for a root URL that does not contain the group identifier. Examples:
  - Supported: https://theapp.com/XXXX-XXXX-XXXX-XXXX
  - Not supported: https://theapp.com/
- Users do NOT need to authenticate. They should enter their first name and email address when opening the app. When their name is entered, the server should save it to the database and return a UserId.  The frontend should this user information in local storage. If local storage is cleared, it's ok for them to have to re-enter their name and email address. The server should be able to find their email address again and return their user info. 
- Users should be able to show their availability in a calendar which is visible to all users
- Any number of other devs should be able to sign up to pair on available dates
- When a dev signs up to an available slot, the one that is being paired with should get a notification in their browser. 
- There should be a user preference page where they can change settings for the following:
  - Receive notifications when someone joins my slot: enable/disable
  - Receive notifications before a pairing session is scheduled to start: enable/disable
- Deep linking to a single slot should be supported. The slot view will show only the slot and not the rest of the calendar

## Tech Specs

- Backend
  - A .NET 10 API using Minimal APIs
  - SQLite for the database
  - [Ntfy.sh](https://ntfy.sh/) connection for push notifications
- Frontend
  - An Angular 21 application
  - Signal Store for state management
  - Service worker for push notifications
