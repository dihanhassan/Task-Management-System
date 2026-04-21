# API Documentation — Task Management System

This document explains every page and action available in the Task Management System.
Written for non-technical readers — no programming knowledge required.

---

## What is this application?

The Task Management System is a website where you can:
- Create an account and sign in securely
- Add tasks you need to complete
- Track which tasks are done, pending, or overdue
- Search and filter your tasks
- Download your tasks as a spreadsheet (CSV)

---

## Pages & Actions

---

### Authentication

#### Sign In Page
- **Address:** `/Account/Login`
- **What it does:** Shows the login form where you enter your email and password.
- **After logging in:** You are taken to your task list automatically.

#### Sign In Action
- **Method:** Form submission (clicking the "Sign In" button)
- **What happens:** The system checks your email and password. If correct, you are logged in. If wrong, you see an error message.

---

#### Register Page
- **Address:** `/Account/Register`
- **What it does:** Shows the registration form to create a new account.
- **Fields required:**
  - Username (e.g., john_doe)
  - Email address (e.g., john@example.com)
  - Password (at least 6 characters, must include a number and uppercase letter)
  - Confirm Password (type the same password again)

#### Register Action
- **Method:** Form submission (clicking "Create Account")
- **What happens:** Your account is created and you are logged in automatically, then taken to your task list.

---

#### Logout
- **Method:** Clicking the "Logout" button in the top navigation bar
- **What happens:** You are signed out and redirected to the login page.

---

### Task Management

> All task pages require you to be signed in. If you are not signed in, you will be redirected to the login page automatically.

---

#### My Tasks (Task List)
- **Address:** `/Task`
- **What it does:** Shows all your tasks in a table, along with a summary at the top.

**Summary cards at the top:**
| Card | What it shows |
|---|---|
| Total Tasks | How many tasks you have in total |
| Completed | How many tasks you have finished |
| Pending | How many tasks are still to do |
| Overdue | How many unfinished tasks are past their due date |

**The task table columns:**
| Column | Meaning |
|---|---|
| # | Task ID number |
| Title | The name of your task |
| Due Date | When the task should be done by |
| Priority | Low, Medium, or High |
| Status | A button to mark the task as done or pending |
| Actions | Edit or Delete the task |

**Highlighted rows:**
- Tasks highlighted in **red** are overdue (past their due date and not completed)
- Completed tasks appear slightly faded

**Search:** Type in the search box to find tasks by title. Results appear automatically as you type (no need to press Enter).

**Filter:** Use the dropdown to show All tasks, only Completed, or only Pending tasks.

**Sort:** Use the sort dropdown to order tasks by due date or priority.

---

#### Create New Task
- **Address:** `/Task/Create`
- **What it does:** Shows a form to add a new task.

**Form fields:**
| Field | Required? | Description |
|---|---|---|
| Title | Yes | Short name for your task (max 100 characters) |
| Description | No | More details about what needs to be done |
| Due Date | No | The deadline for the task |
| Priority | Yes | Low, Medium, or High |
| Mark as Completed | No | Check this if the task is already done |

**After saving:** You are taken back to your task list with a success message.

---

#### Edit Task
- **Address:** `/Task/Edit/{id}` (e.g., `/Task/Edit/5`)
- **What it does:** Opens the edit form for an existing task, pre-filled with the current values.
- **Who can edit:** Only the person who created the task can edit it.

**After saving:** You are taken back to your task list with a success message.

---

#### Delete Task
- **Method:** Clicking the trash icon (delete button) in the Actions column
- **What happens:** A confirmation dialog appears asking "Are you sure you want to delete this task?". If you click Yes, the task is permanently removed. If you click Cancel, nothing happens.
- **Who can delete:** Only the person who created the task can delete it.

---

#### Toggle Task Status (Mark Complete / Pending)
- **Method:** Clicking the checkmark button in the Status column
- **What happens:** The task status switches between "Completed" and "Pending" instantly, without the page reloading. A small notification appears in the top-right corner confirming the change.

---

#### Search Tasks
- **Address:** `/Task/Search?term=your-search-word`
- **What it does:** Returns tasks matching the search word in the title.
- **When it's used:** Automatically called when you type in the search box on the task list page. You don't need to visit this address manually.

---

#### Export to CSV
- **Address:** `/Task/ExportCsv`
- **What it does:** Downloads all your tasks as a CSV file that you can open in Microsoft Excel or Google Sheets.
- **File name format:** `tasks-2026-04-22.csv` (today's date)
- **Columns in the file:** Id, Title, Description, DueDate, Priority, IsCompleted, CreatedAt

---

## Error Pages

If something goes wrong, the application shows a friendly error page instead of a technical error:

| Error Type | When it appears |
|---|---|
| 404 Not Found | You tried to access a task that doesn't exist |
| 403 Forbidden | You tried to edit or delete someone else's task |
| 400 Bad Request | The data submitted was invalid |
| 500 Server Error | An unexpected problem occurred on the server |

---

## Security Notes

- You can only see your own tasks — other users cannot see yours
- All forms are protected against cross-site request forgery (CSRF) attacks
- Passwords are never stored in plain text — they are securely hashed
- Sessions expire after 8 hours of inactivity

---

## Password Requirements

When creating an account or if you need to reset your password, the password must:
- Be at least **6 characters** long
- Contain at least **one uppercase letter** (e.g., A, B, C)
- Contain at least **one number** (e.g., 1, 2, 3)
- Contain at least **one special character** (e.g., @, !, #)
