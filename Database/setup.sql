-- Wait for SQL Server to start then run this
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PatientManagementDB')
BEGIN
    CREATE DATABASE PatientManagementDB;
END
GO

USE PatientManagementDB;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Patients')
BEGIN
    CREATE TABLE Patients (
        PatientId INT PRIMARY KEY IDENTITY(1,1),
        Name VARCHAR(100) NOT NULL,
        Age INT NOT NULL,
        Gender VARCHAR(10) NOT NULL,
        Phone VARCHAR(15) NOT NULL,
        Address VARCHAR(200)
    );
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Doctors')
BEGIN
    CREATE TABLE Doctors (
        DoctorId INT PRIMARY KEY IDENTITY(1,1),
        Name VARCHAR(100) NOT NULL,
        Specialization VARCHAR(100) NOT NULL,
        Phone VARCHAR(15) NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Appointments')
BEGIN
    CREATE TABLE Appointments (
        AppointmentId INT PRIMARY KEY IDENTITY(1,1),
        PatientId INT FOREIGN KEY REFERENCES Patients(PatientId),
        DoctorId INT FOREIGN KEY REFERENCES Doctors(DoctorId),
        AppointmentDate DATETIME NOT NULL,
        Status VARCHAR(20) DEFAULT 'Scheduled'
    );
END
GO

-- Sample data
INSERT INTO Doctors (Name, Specialization, Phone) VALUES
('Dr. Rajesh Sharma', 'Cardiology', '9876543210'),
('Dr. Priya Verma', 'Neurology', '9876543211'),
('Dr. Anil Gupta', 'Orthopedics', '9876543212');

INSERT INTO Patients (Name, Age, Gender, Phone, Address) VALUES
('Rahul Mehta', 35, 'Male', '9000000001', 'Raipur, CG'),
('Sneha Tiwari', 28, 'Female', '9000000002', 'Bilaspur, CG'),
('Amit Joshi', 45, 'Male', '9000000003', 'Durg, CG');
GO