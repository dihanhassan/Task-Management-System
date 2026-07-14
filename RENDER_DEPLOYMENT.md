# Render Deployment Guide

This guide walks you through deploying the Task Management System to Render.

## Prerequisites

- GitHub account (code must be pushed)
- Render account (free tier available at render.com)
- Project has been updated for PostgreSQL

## What's Changed

The project has been configured for Render deployment:

1. ✅ **Database Provider**: Switched from SQL Server to PostgreSQL
2. ✅ **Docker Support**: Added Dockerfile for containerization
3. ✅ **Environment Configuration**: Added port handling for Render
4. ✅ **Deployment Config**: Added render.yaml for easy deployment

## Deployment Steps

### Step 1: Push Changes to GitHub

```bash
cd Task-Management-System
git add .
git commit -m "Configure for Render deployment: PostgreSQL + Docker"
git push origin main
```

### Step 2: Create Render Account

1. Go to [render.com](https://render.com)
2. Sign up with GitHub
3. Authorize Render to access your GitHub account

### Step 3: Deploy on Render

**Option A: Using Blueprint (Recommended)**

1. Go to Dashboard → New +
2. Select "Blueprint"
3. Connect your GitHub repo
4. Select `render.yaml` from the root
5. Click "Apply"
6. Render will automatically create:
   - PostgreSQL database
   - Web service (ASP.NET Core app)

**Option B: Manual Setup**

#### Create PostgreSQL Database:
1. Dashboard → New + → PostgreSQL
2. Fill in:
   - Name: `task-management-db`
   - Database: `taskmanagement_db`
   - User: `taskmanager`
   - Plan: Free
3. Click "Create Database"
4. Copy the connection string (you'll need it)

#### Create Web Service:
1. Dashboard → New + → Web Service
2. Connect your GitHub repo
3. Configure:
   - **Name**: `task-management-system`
   - **Runtime**: Docker
   - **Build Command**: (Leave empty - uses Dockerfile)
   - **Start Command**: (Leave empty - uses Dockerfile)
   - **Plan**: Free (or Starter for better performance)

#### Add Environment Variables:
In the Web Service settings, add:

```
ASPNETCORE_ENVIRONMENT=Production
```

Then add the database connection in environment variables:
```
ConnectionStrings__DefaultConnection=<your-postgres-connection-string>
```

Replace `<your-postgres-connection-string>` with the connection string from Step 3.

4. Click "Create Web Service"

### Step 4: Verify Deployment

1. Render will start building and deploying
2. Check the "Logs" tab for build progress
3. Once deployed, you'll get a URL like: `https://task-management-system-xxxx.onrender.com`
4. Visit the URL in your browser

### Step 5: First Time Setup

When you first visit the app:

1. The database will be created automatically (EF Core migrations)
2. Admin user will be seeded with credentials:
   - **Email**: `admin@taskmanager.com`
   - **Password**: `Admin@123`
3. Log in with these credentials

## Troubleshooting

### Deployment Failed

**Check logs in Render Dashboard:**
1. Go to Web Service → Logs
2. Look for error messages
3. Common issues:
   - Connection string not set properly
   - Database not created yet
   - Missing environment variables

### Database Connection Error

Ensure the connection string in environment variables matches exactly:
- Must include the correct hostname, port, database name, username, and password
- Format: `Host=xxx;Port=5432;Database=xxx;Username=xxx;Password=xxx`

### Slow First Load

Free tier Render services spin down after 15 minutes of inactivity. First load may take 30-60 seconds. This is normal.

### Admin User Not Created

If seeding fails:
1. Check logs for errors
2. Manually create admin user via ASP.NET Identity
3. Contact support if persistence

## Scaling Up

When you're ready for production:

1. **Database**: Upgrade to Starter or higher (free tier has limitations)
2. **Web Service**: Change plan to Starter or higher (free tier suspends after 15 min inactivity)
3. **Add custom domain**: In Web Service settings
4. **Enable HTTPS**: Automatic with Render

## Cost

- **Free Tier**: $0/month (15 min inactivity suspension, shared resources)
- **Starter**: ~$7/month web + ~$7/month database (always running, dedicated database)

## Additional Notes

- Database migrations run automatically on startup
- Logs are available in the Render dashboard
- Deployments happen automatically on git push to main (if CI/CD enabled)

## Need Help?

- Render Docs: https://render.com/docs
- Project Issues: Create an issue in the GitHub repo
- Connection String Help: Check Render database details page
