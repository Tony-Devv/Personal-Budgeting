
A modern, user-friendly desktop application for managing personal finances, built with Avalonia UI and C#.



## Features

- **Intuitive Dashboard**: Get a quick overview of your financial status with total balance, income, and expenses at a glance
- **Income Management**: Track all your income sources with dates and categories
- **Expense Tracking**: Log and categorize expenses with detailed information
- **Budget Planning**: Create and monitor budgets for different spending categories
- **Reminder System**: Receive email notifications for upcoming expenses
- **Modern UI**: Clean, responsive interface with dark theme support

## Screenshots
### Welcome Page 
![{0B223D11-52CB-4D85-A836-0FD8724F32F5}](https://github.com/user-attachments/assets/8aa963e7-4c6c-4e8c-b6b7-60b29003b0e3)


### Login Page
![{1E71EDA7-001E-4EA2-B836-44B3B8069E5A}](https://github.com/user-attachments/assets/7629e725-3ac8-48b2-bdb6-a5e30347dd54)


### Sign Up Page 
![{8469D2D8-7AC6-4ED5-8BFF-A473F94E6395}](https://github.com/user-attachments/assets/470d1c85-a501-4a9f-bde3-3f4382dc0df2)


### Dashboard / Home Page
![{34B359AD-0B12-4D8A-AB3C-E050DED198B5}](https://github.com/user-attachments/assets/fdbce362-185c-42e2-bba4-7a4ff6350342)


### Income Management
![{2EAE93D3-881C-4E49-B443-8E6EDEA641E4}](https://github.com/user-attachments/assets/75d82eeb-02c0-470b-86f6-e18c834a296a)


### Expense Tracking
![{2851D7E9-9A7B-453D-B9FC-D972675C62BD}](https://github.com/user-attachments/assets/a6b998b0-2d64-48c3-ba81-4846008e20a3)


### Budget Management
![{F91FDFC1-BE16-42C7-AF91-95BDFD926B0E}](https://github.com/user-attachments/assets/124f4907-fffd-409c-ae79-921f32ccf186)


### Settings
![image](https://github.com/user-attachments/assets/f58c07f9-a7a3-4c89-a122-244c052ccde2)


## Technology Stack

- **Frontend**: Avalonia UI (cross-platform .NET UI framework)
- **Backend**: C# / .NET
- **Database**: Microsoft SQL Server
- **Email Notifications**: SendGrid API integration

## Installation

### Prerequisites
- .NET 6.0 SDK or later
- Microsoft SQL Server (2019 or 2022)
- Visual Studio 2022 or JetBrains Rider (recommended for development)

### Database Setup
There are two ways to set up the database:

#### Option 1: Using the BAK (Backup) File
1. Open SQL Server Management Studio
2. Connect to your SQL Server instance
3. Right-click on "Databases" and select "Restore Database"
4. Choose "Device" and browse to select the backup file
5. Alternatively, you can use the following T-SQL command:
   ```sql
   RESTORE DATABASE PersonalBudgeting
   FROM DISK = "path/to/PersonalBudgeting.bak"
   ```
   Replace `"path/to/PersonalBudgeting.bak"` with the actual path to the backup file.

#### Option 2: Using the MDF File
1. Open SQL Server Management Studio
2. Connect to your SQL Server instance
3. Right-click on "Databases" and select "Attach"
4. Click "Add" and browse to the location of the MDF file
5. Select the "PersonalBudgeting.mdf" file
6. Click "OK" to attach the database


### Application Setup
1. Clone the repository or download the source code
   ```
   git clone https://github.com/yourusername/personal-budgeting.git
   ```
2. Open in Visual Studio:
   - Launch Visual Studio 2022
   - Select "Open a project or solution"
   - Navigate to the solution file (.sln) in the project directory
   - Click "Open"

3. Alternatively, build from command line:
   ```
   dotnet build
   ```
4. Run the application
   ```
   dotnet run --project PersonalBudgeting
   ```

## Usage

### First-time Setup
1. Launch the application
2. Create a new user account by clicking "Sign Up" on the welcome screen
3. Log in with your new credentials
4. Set up your initial budget categories via the Budget page

### Managing Income
1. Navigate to the Income section
2. Click the "Add Income" button
3. Enter source, amount, date, and category details
4. Click "Save" to record the income

### Budget Planning
1. Navigate to the Budget section
2. Click "Create Budget" to add a new budget category
3. Set a budget name / category and budget amount 
4. Monitor your spending progress through the visual indicators

### Managing Expenses
1. Navigate to the Expenses section
2. Click "Add Expense" to record a new expense
3. Choose a budget category / name , Enter expense name, required amount, spent amount , and date of the expense
4. Set a reminder date to receive an email notification (Optionally)
5. Click "Save" to record the expense

## Development

### Project Structure
The application is organized into three main projects:

- **PersonalBudgeting/**: Main application project (UI layer)
  - **Views/**: UI components and XAML layouts
    - **Pages/**: Individual application pages (Home, Budget, Expenses, etc.)
  - **ViewModels/**: Data binding and presentation logic
  - **Converters/**: Value converters for UI display
  - **Styles/**: Application themes and style resources
  - **Assets/**: Images and other static resources
  - **Model/**: Local model references and service implementations
    - **Services/**: Service classes including the reminder service
  - **Controllers/**: Local controller references

- **Model/**: Data access and domain layer
  - **Entities/**: Domain model classes
  - **Repositories/**: Data access components
  - **Interfaces/**: Service and repository contracts
  - **Utilities/**: Helper functions and extensions
  - **Handlers/**: Event and request handlers
  - **ApplicationDbContext.cs**: Entity Framework database context

- **Controller/**: Business logic layer
  - **Validators/**: Input validation logic
  - **UserController.cs**: User management operations
  - **BudgetController.cs**: Budget management operations
  - **ExpenseController.cs**: Expense tracking operations
  - **IncomeController.cs**: Income tracking operations

### Building for Production
```
dotnet publish -c Release
```

## License
This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments
- Avalonia UI for the cross-platform UI framework
- SendGrid for the email notification service
- All contributors who have helped improve this application

## Configuration

### Database Connection
After setting up the database, you'll need to configure the connection string:

1. Open `Model/ApplicationDbContext.cs` to verify the connection string name
2. Check and update the connection string in your application settings if needed

### Email Reminder Feature

The application includes an email reminder system for expenses:

1. **Setting Reminders**: When creating or editing an expense, you can set a reminder date
2. **Email Notifications**: The system will send an email notification on the specified date
3. **Customization**: Emails include:
   - Personalized greeting with your name
   - Expense details (name, category, amounts)
   - Reminder date
   - Application branding

The email service uses SendGrid API to deliver reliable, transactional emails. This helps ensure you never miss an important payment or expense deadline.

### Email Configuration

To set up the email reminder system:

1. Configure your SendGrid API account at [sendgrid.com](https://sendgrid.com/)
2. Create an API key with appropriate permissions
3. Update the reminder service with your API key and sender email

Example configuration:
```csharp
// In UserController.cs
private const string API_KEY = "your-sendgrid-api-key";
private const string SENDER_EMAIL = "your-verified-sender@example.com";
private const string SENDER_NAME = "Personal Budget Manager";
```
