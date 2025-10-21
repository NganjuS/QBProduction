# QuickBooks Production Module - Migration Guide
## WPF Desktop to ASP.NET MVC Web Application

This guide explains the migration from the WPF desktop application to an ASP.NET MVC 5 web application targeting .NET Framework 4.8.

## Project Structure

### Original WPF Application
- **Location:** `QBProduction/QBProduction.csproj`
- **Type:** Windows Desktop Application (WPF)
- **UI:** XAML-based user interface
- **Target:** .NET Framework 4.8

### New Web Application
- **Location:** `QBProduction/QBProduction.Web.csproj`
- **Type:** ASP.NET MVC 5 Web Application
- **UI:** Razor views with Bootstrap 5
- **Target:** .NET Framework 4.8

## What Has Been Migrated

### 1. Data Models ✓
All entity models have been migrated:
- `Boms` - Bill of Materials master records
- `BomItems` - Individual items in a BOM
- `BomRun` - Production batch runs
- `BomRunItems` - Items used in production runs
- `BomSettings` - Configuration settings

**Location:** `QBProduction.Web/Models/`

### 2. Data Access Layer ✓
- NHibernate configuration adapted for web context
- Fluent NHibernate mappings preserved
- MySQL database connection maintained

**Location:** `QBProduction.Web/Helpers/NHibernateHelper.cs`

### 3. Business Logic ✓
Core business logic has been migrated to:
- `QBProduction.Web/Controllers/` - MVC Controllers
- `QBProduction.Web/Helpers/` - Utility classes

### 4. User Interface ✓
WPF views converted to Razor views:

| WPF Window | Web View | Controller |
|------------|----------|------------|
| MainWindow.xaml | Home/Index.cshtml | HomeController |
| ConfigureBom.xaml | Bom/Index.cshtml, Bom/Edit.cshtml | BomController |
| Production.xaml | Production/StartProduction.cshtml | ProductionController |
| BatchListing.xaml | Production/BatchListing.cshtml | ProductionController |
| RawMaterialReport.xaml | Reports/RawMaterials.cshtml | ReportsController |
| ProductionSummaryReport.xaml | Reports/ProductionSummary.cshtml | ReportsController |

## Key Differences

### Application Type
- **Desktop:** Single-user, runs locally on Windows
- **Web:** Multi-user, accessible via web browser

### Session Management
- **Desktop:** Single continuous session
- **Web:** Stateless HTTP requests, uses session cookies

### QuickBooks Integration
- **Desktop:** Direct COM integration with QB Desktop
- **Web:** Requires QB Web Connector or service-based approach
  - ⚠️ **Important:** Direct QuickBooks Desktop integration from web requires additional architecture (Windows service or QB Web Connector)

### Database Access
- **Both:** Use NHibernate with MySQL
- **Web:** Connection pooling handled by IIS/ASP.NET

## Setup Instructions

### Prerequisites
1. Visual Studio 2017 or later
2. .NET Framework 4.8 SDK
3. MySQL Server (same as desktop version)
4. IIS or IIS Express for hosting

### Configuration Steps

1. **Database Configuration**
   - Update connection string in `QBProduction.Web/Web.config`
   - Default: `server=localhost;user id=root;password=metacity;database=qbproduction`

2. **NuGet Packages**
   ```bash
   cd QBProduction.Web
   nuget restore
   ```

3. **Build the Solution**
   ```bash
   msbuild QBProduction.sln /p:Configuration=Release
   ```

4. **Run the Web Application**
   - Press F5 in Visual Studio, or
   - Deploy to IIS

### IIS Deployment

1. **Application Pool Settings**
   - .NET CLR Version: v4.0
   - Managed Pipeline Mode: Integrated

2. **Publish Settings**
   - Right-click `QBProduction.Web` → Publish
   - Choose IIS, FTP, or File System
   - Configure connection settings

## Features Comparison

### Implemented ✓
- Bill of Materials management (CRUD operations)
- Production batch creation and tracking
- Batch listing with filters
- Raw materials usage reports
- Production summary reports
- Responsive web design (mobile-friendly)
- Bootstrap 5 UI framework

### Requires Additional Work ⚠️

1. **QuickBooks Integration**
   - Desktop version uses direct COM integration
   - Web version needs QB Web Connector or Windows service
   - Recommendation: Implement QB Web Connector service

2. **Crystal Reports**
   - Desktop version uses Crystal Reports for WPF
   - Web version needs Crystal Reports for .NET (separate install)
   - Alternative: Export to PDF/Excel using libraries like iTextSharp or EPPlus

3. **User Authentication**
   - Desktop: Single user, Windows authentication
   - Web: Needs ASP.NET Identity or similar
   - Consider adding user roles (Admin, Operator, Viewer)

4. **Real-time Updates**
   - Desktop: Direct UI updates
   - Web: Consider SignalR for real-time notifications

## Migration Checklist

- [x] Create ASP.NET MVC project structure
- [x] Migrate data models
- [x] Configure NHibernate for web
- [x] Create MVC controllers
- [x] Convert XAML to Razor views
- [x] Setup Web.config
- [x] Create responsive UI with Bootstrap
- [ ] Implement QuickBooks Web Connector
- [ ] Setup Crystal Reports for web
- [ ] Add user authentication/authorization
- [ ] Implement audit logging
- [ ] Add input validation
- [ ] Setup error handling and logging
- [ ] Performance optimization
- [ ] Security hardening
- [ ] User acceptance testing

## Running the Application

### Development
```bash
# Using Visual Studio
1. Open QBProduction.sln
2. Set QBProduction.Web as startup project
3. Press F5

# Using IIS Express
cd QBProduction.Web
iisexpress /path:$(pwd) /port:5000
```

### Production
1. Build in Release mode
2. Publish to IIS
3. Configure SSL certificate
4. Setup application pool with appropriate identity
5. Configure database connection string

## Important Notes

### Security Considerations
1. **Connection Strings:** Use encrypted connection strings in production
2. **SQL Injection:** All database queries use NHibernate parameterization
3. **XSS Protection:** Razor views auto-encode output
4. **CSRF Protection:** AntiForgeryToken used in forms
5. **Authentication:** Add ASP.NET Identity for production use

### Performance
1. **Database:** Enable NHibernate second-level cache for production
2. **Static Files:** Configure IIS static content compression
3. **Bundling:** CSS/JS bundling enabled via BundleConfig
4. **Connection Pooling:** Enabled by default in ADO.NET

### QuickBooks Integration Notes
The QuickBooks SDK (QBFC13) used in the desktop version requires:
- QuickBooks Desktop to be running
- Direct COM access (not possible from web without a service)

**Recommended Approach for Web:**
1. Create a Windows Service that communicates with QuickBooks
2. Web application calls the Windows Service via WCF or Web API
3. Or use QuickBooks Web Connector with SOAP interface

## Support and Additional Resources

- ASP.NET MVC Documentation: https://docs.microsoft.com/en-us/aspnet/mvc/
- NHibernate: https://nhibernate.info/
- QuickBooks SDK: https://developer.intuit.com/
- Bootstrap 5: https://getbootstrap.com/

## Next Steps

1. **Test the Migration**
   - Verify all data models work correctly
   - Test CRUD operations for BOMs
   - Test production batch creation
   - Verify reports display correctly

2. **Enhance Security**
   - Implement user authentication
   - Add role-based authorization
   - Setup HTTPS/SSL

3. **Add Missing Features**
   - QuickBooks integration via Web Connector
   - Crystal Reports migration or alternative
   - Email notifications
   - Export functionality (Excel, PDF)

4. **Deploy to Production**
   - Setup production database
   - Configure IIS
   - Setup monitoring and logging
   - Create backup procedures

---

**Migration Date:** October 2024
**Framework:** .NET Framework 4.8
**Web Framework:** ASP.NET MVC 5
**Database:** MySQL with NHibernate ORM
