# RepairApp
This application helps the service in managing repairs.

## Table of Contents
* [General info](#general-info)
* [Technologies](#technologies)
* [Setup](#setup)
* [Example of use](example-of-use)

## General info
Assists the service in accepting repairs via the form. The employee can conveniently manage repairs via the administration panel.

## Technologies
RepairApp is created with:
* .NET Core 5
* Entity Framework 
* JavaScript
* HTML 5
* CSS
* Bootstrap
* SQL Server

## Setup
To run this app, install repository.

You need Visual Studio 2019 or older with .NET 5.

In appsetting.json change DefaultConnection:

* Server = your connection from SQL Server.

After these change choose Package Manager Console and run the command below:

$ update-database

After these steps, the application is ready to run. 

Default admin to log in:

Email: admin@admin.pl
Password: Admin1234!

Default user to log in:

Email: user@user.pl
Password: User1234!

## Example of use
After launching the application, you will see the window below.

![Algorithm schema](./wwwroot/img/git/Default.PNG)

You can choose one device for reapir.

![Algorithm schema](./wwwroot/img/git/Repair.PNG)

After logging in user account, you will see your repairs.

![Algorithm schema](./wwwroot/img/git/AllRepairs.PNG)

After logging in admin account, you will see all repairs.

![Algorithm schema](./wwwroot/img/git/AdminRepairs.PNG)

After logging in admin account, you will see all repairs.

![Algorithm schema](./wwwroot/img/git/AdminRepairs.PNG)

In admin account you can manage device and mark.

![Algorithm schema](./wwwroot/img/git/AddDevice.PNG)

![Algorithm schema](./wwwroot/img/git/AddMark.PNG)

If the service technician enters the quote, you can accept or decline.

![Algorithm schema](./wwwroot/img/git/AcceptOrDeny.PNG)

Employee and user receive SMS or e-mail notifications.

![Algorithm schema](./wwwroot/img/git/SMS.PNG)

#### Done, have a nice day!




