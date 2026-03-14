# Smart Complaint Management System

A full‑stack **Smart Complaint Management System** built with **Blazor WebAssembly** and **ASP.NET Core Web API**.  
The system is designed for municipalities, campuses, housing societies, and service teams to register, track, assign, and resolve complaints in a transparent and structured way.

---

## 1. Project Overview

### Purpose

The Smart Complaint Management System provides a central platform for managing complaints raised by citizens or internal users. It replaces scattered phone calls, emails, and paper forms with a single web interface and a clear workflow.

### Core Objectives

- Allow users to **raise complaints** with complete details (category, description, priority, location, coordinates).
- Enable admins to **monitor and triage** all complaints.
- Enable engineers to **work on assigned complaints**, update status, and provide progress notes.
- Provide **dashboards** and **lists** for tracking workload and resolution performance.

### Target Roles

- **User / Citizen**
  - Registers and logs in.
  - Creates complaints.
  - Tracks status and history of their own complaints.
- **Engineer**
  - Receives complaints assigned by admins or other engineers.
  - Updates complaint status and adds progress notes.
  - Can re‑assign complaints (based on role rules).
- **Admin**
  - Manages categories and staff (e.g., creates engineer accounts).
  - Views all complaints and dashboard statistics.
  - Assigns complaints to engineers.
  - Updates and oversees complaint status.

### High‑Level Workflow

1. **Registration & Login**
   - Users register via the Register page and log in to obtain a JWT token.
2. **Raise Complaint**
   - Authenticated users raise a complaint with category, description, priority, address, and coordinates.
3. **Triage & Assignment**
   - Admin views new complaints and assigns them to appropriate engineers.
4. **Resolution**
   - Engineers work on assigned complaints, update status (Pending, Assigned, In Progress, Resolved), and add notes.
5. **Tracking & Reporting**
   - Dashboard shows counts and recent complaints.
   - Users can view status and full update history for each complaint.

---

## 2. Architecture

This solution follows a **layered, API‑driven architecture** with a **Blazor WebAssembly SPA** front‑end and an **ASP.NET Core Web API** back‑end.

### Logical Layers

- **Presentation Layer (Client)**
  - Project: `ComplaintManagementSystem.Client`
  - Technology: Blazor WebAssembly
  - Responsibilities:
    - Render UI components (`.razor` pages)
    - Handle form validation and navigation
    - Call backend APIs via `HttpClient`
    - Implement client‑side authorization (show/hide pages and actions based on role)

- **Application Layer (API Services)**
  - Project: `ComplaintManagementSystem.API`
  - Responsibilities:
    - Implement business use cases (register/login, create complaint, assign, update status, dashboard)
    - Enforce authorization rules on operations
    - Map between DTOs and data access layer

- **Data Access Layer (Repositories)**
  - Project: `ComplaintManagementSystem.API`
  - Namespaces: `ComplaintManagementSystem.API.Repositories`
  - Technology: Dapper
  - Responsibilities:
    - Execute SQL queries and commands for complaints, categories, users, dashboard data
    - Map result sets to DTOs and entity objects

- **Shared Layer (Contracts & Models)**
  - Project: `ComplaintManagementSystem.Shared`
  - Responsibilities:
    - DTOs shared between client and server (e.g., `CreateComplaintRequest`, `ComplaintDetailsResponse`)
    - Enums and entities used across layers

### Data Flow (Example: Create Complaint)

1. User fills in the form in `CreateComplaint.razor`.
2. Blazor `EditForm` validates the `CreateComplaintRequest` model.
3. `ComplaintService` (client) sends an HTTP `POST` request to `api/complaints`.
4. `ComplaintsController` (API) reads the user ID from JWT and passes the request to `IComplaintService`.
5. `IComplaintService` maps the DTO to an entity and calls `IComplaintRepository.CreateAsync`.
6. `ComplaintRepository` executes SQL `INSERT` using Dapper.
7. API returns `201 Created`; client navigates to the complaints list page.

---

## 3. Main Features

### Authentication & Authorization

- Token‑based authentication using **JWT**.
- Registration and login endpoints in `AuthController`.
- Blazor client stores the token and adds it to every API request.
- Role‑based authorization:
  - `[Authorize]` on complaint endpoints
  - `[Authorize(Roles = "Admin,Engineer")]` for actions like status update and assignment
  - Blazor UI hides admin/engineer actions for unauthorized users.

### Complaint Management

- **Create Complaint**
  - Page: `/create-complaint`
  - Model: `CreateComplaintRequest`
  - Uses data annotations for required fields (category, title, description, address, priority).
  - Extra validation ensures coordinates are numeric and within India’s approximate bounds.

- **Complaint Listing**
  - Page: `/complaints`
  - Uses `ComplaintService.GetComplaintsAsync` with filters:
    - Status, Category, Priority, Search by title
  - Admins and engineers can see more; normal users see primarily their own complaints (logic enforced on the API).

- **Complaint Details**
  - Page: `/complaints/{id:int}`
  - Shows:
    - Core details (title, description, category, priority, created date, status)
    - Timeline of updates (status changes, assignment history)
    - Attachments metadata (if used)
  - Admin/Engineer tools:
    - Change status using `UpdateComplaintStatusRequest`
    - Assign complaint using `AssignComplaintRequest`

- **Assignment Workflow**
  - Admin:
    - Can assign only to Engineers (RoleId = 3), excluding themselves.
  - Engineer:
    - Can assign to Admins or Engineers (RoleId 1 or 3), excluding themselves.
  - Backend persists both current assignment on the complaint and an assignment history record.

### Dashboard

- Page: `/dashboard`
- DTO: `DashboardResponse` (total, pending, in‑progress, resolved, plus recent complaints).
- Admin sees global stats; regular user sees stats for their own complaints.

### Role‑based UI

- **NavMenu**
  - Not logged in:
    - Shows Home, Register, Login.
  - Logged in:
    - Shows Dashboard, Complaints, Complaints Map, Add Complaint, Categories.
    - If Admin: shows Add Engineer.
- **Add Engineer Page**
  - Accessible only to admin (checked both on client and in API).
  - Uses same `RegisterRequest` DTO with `RoleId = 3` for engineers.

---

## 4. Key Projects and Components

### `ComplaintManagementSystem.Client` (Blazor WASM)

**Program**

- `Program.cs`
  - Configures `HttpClient` with the API base address.
  - Registers client services:
    - `AuthService`, `ComplaintService`, `CategoryService`, `UserService`
  - Hooks up `JwtAuthenticationStateProvider` for Blazor auth.

**Layout & Navigation**

- `MainLayout.razor`
  - Wraps the app with header (`NavMenu`) and footer.
  - Footer contains quick links and clickable email/phone using `mailto:` and `tel:` links.
- `NavMenu.razor`
  - Shows navigation links depending on authentication and role.
  - Integrates with `AuthenticationStateProvider` to update state when login/logout occurs.

**Pages (Selected)**

- `Home.razor`
  - Landing/marketing page with CTAs.
  - “Raise a Complaint” calls `OnRaiseComplaintClicked`:
    - Authenticated → `/create-complaint`
    - Not authenticated → `/login`
- `Register.razor`
  - Uses `RegisterRequest` with per‑field validation and calls `AuthService.RegisterAsync`.
- `Login.razor`
  - Uses `LoginRequest`; on success calls `AuthService.LoginAsync` and updates auth state.
  - Provides a direct link to the registration page.
- `CreateComplaint.razor`
  - Auth guard in `OnInitializedAsync`; redirects to `/login` if not authenticated.
  - Validates all fields and India‑specific coordinate range before sending to API.
- `Complaints.razor`
  - Shows filter bar and list of complaints from API.
- `ComplaintDetails.razor`
  - Shows complaint details and history.
  - Provides admin/engineer panels for status update and assignment.
- `AddEngineer.razor`
  - Admin‑only page to register engineer accounts.

**Client Services**

- `AuthService`
  - Wraps `api/auth/register` and `api/auth/login`.
  - Manages JWT token storage and logout.
- `ComplaintService`
  - Wraps:
    - `GET api/complaints`
    - `GET api/complaints/{id}`
    - `POST api/complaints`
    - `PUT api/complaints/{id}/status`
    - `POST api/complaints/{id}/assign`
    - `GET api/dashboard`
- `CategoryService`, `UserService`
  - Simple wrappers around `api/categories` and `api/users` endpoints.

### `ComplaintManagementSystem.API` (ASP.NET Core Web API)

**Program**

- `Program.cs`
  - Registers controllers and OpenAPI.
  - Registers application services and repositories via `AddApplicationServices()`.
  - Configures:
    - JWT Bearer authentication
    - Authorization
    - CORS (to allow the Blazor client origin)

**Controllers (selected)**

- `AuthController`
  - `POST api/auth/register`
  - `POST api/auth/login`
- `ComplaintsController`
  - `GET api/complaints` – filterable complaints list, with user‑based filtering.
  - `GET api/complaints/{id}` – complaint details (allowed anonymously for viewing by ID if needed).
  - `POST api/complaints` – creates complaint for the current user.
  - `PUT api/complaints/{id}/status` – admin/engineer only.
  - `POST api/complaints/{id}/assign` – admin/engineer only.
- `CategoriesController`
  - CRUD endpoints for complaint categories.

**Repositories**

- `ComplaintRepository`
  - `CreateAsync` – insert complaint.
  - `GetAllAsync` – list with optional filters and user filter.
  - `GetByIdAsync` – details plus updates and attachments.
  - `UpdateStatusAsync` – transactional status update and history insert.
  - `AssignAsync` – transactional assignment handling and update logging.
  - `GetDashboardDataAsync` – aggregates for dashboard.

### `ComplaintManagementSystem.Shared` (Shared DTOs & Models)

- `CreateComplaintRequest` – input model for raising complaints.
- `ComplaintListResponse` – summary row for list pages.
- `ComplaintDetailsResponse` – full detail, including updates and attachments.
- `RegisterRequest`, `LoginRequest`, `LoginResponse`.
- `DashboardResponse` with `RecentComplaintDto`.
- `AssignComplaintRequest`, `UpdateComplaintStatusRequest`.
- `CategoryRequest`, `CategoryResponse`, `UserResponse`.
- Enums such as `ComplaintStatusType`.

---

## 5. Technology Stack

- **Language**: C# (targeting .NET 10 / C# 14.0)
- **Client Framework**: Blazor WebAssembly
- **Server Framework**: ASP.NET Core Web API
- **Data Access**: Dapper (`Microsoft.Data.SqlClient` for SQL Server connections)
- **Authentication**: JWT (JSON Web Token), `Microsoft.AspNetCore.Authentication.JwtBearer`
- **UI & Styling**: Bootstrap 5, Font Awesome icons
- **Validation**: Data Annotations + Blazor `EditForm`
- **API Documentation**: ASP.NET Core OpenAPI/Swagger

---

## 6. Intended Use

This project is suitable as:

- A **college-level final year project** demonstrating:
  - Modern .NET full‑stack development
  - Secure JWT authentication
  - Role‑based authorization
  - Layered architecture and DTO‑based API design
- A starting point for production‑grade complaint/ticket management systems with additional features such as notifications, advanced reporting, or mobile apps.

---

If you want, a shorter “Getting Started” section (build/run steps) can be added next, but the database schema and setup scripts can stay in a separate `docs/` file or `.sql` file instead of the main README.
