-- Создание базы данных
CREATE DATABASE CitizenAppealsDB
GO

USE CitizenAppealsDB
GO

-- Создание таблицы ролей
CREATE TABLE Role
(
    RoleId INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL
)
GO

-- Создание таблицы пользователей
CREATE TABLE [User]
(
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Login NVARCHAR(50) NOT NULL,
    Password NVARCHAR(50) NOT NULL,
    RoleId INT FOREIGN KEY REFERENCES Role(RoleId),
    LastName NVARCHAR(50) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    MiddleName NVARCHAR(50),
    Phone NVARCHAR(20)
)
GO

-- Создание таблицы статусов обращений
CREATE TABLE AppealStatus
(
    StatusId INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL
)
GO

-- Создание таблицы типов обращений
CREATE TABLE AppealType
(
    TypeId INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(100) NOT NULL
)
GO

-- Создание таблицы обращений
CREATE TABLE Appeal
(
    AppealId INT PRIMARY KEY IDENTITY(1,1),
    RegistrationDate DATETIME NOT NULL DEFAULT GETDATE(),
    CitizenLastName NVARCHAR(50) NOT NULL,
    CitizenFirstName NVARCHAR(50) NOT NULL,
    CitizenMiddleName NVARCHAR(50),
    CitizenAddress NVARCHAR(200) NOT NULL,
    CitizenPhone NVARCHAR(20),
    CitizenEmail NVARCHAR(100),
    AppealText NVARCHAR(MAX) NOT NULL,
    TypeId INT FOREIGN KEY REFERENCES AppealType(TypeId),
    StatusId INT FOREIGN KEY REFERENCES AppealStatus(StatusId),
    RegistratorId INT FOREIGN KEY REFERENCES [User](UserId),
    ExecutorId INT FOREIGN KEY REFERENCES [User](UserId),
    ResponseText NVARCHAR(MAX),
    ResponseDate DATETIME
)
GO

-- Вставка начальных данных
INSERT INTO Role (RoleName) VALUES 
('Администратор'),
('Сотрудник'),
('Регистратор')
GO

INSERT INTO AppealStatus (StatusName) VALUES
('Зарегистрировано'),
('В обработке'),
('Выполнено'),
('Отклонено')
GO

INSERT INTO AppealType (TypeName) VALUES
('Жалоба'),
('Предложение'),
('Заявление'),
('Запрос информации')
GO

-- Создание администратора по умолчанию
INSERT INTO [User] (Login, Password, RoleId, LastName, FirstName, MiddleName, Phone)
VALUES ('admin', 'admin', 1, 'Администратор', 'Системы', NULL, NULL)
GO 