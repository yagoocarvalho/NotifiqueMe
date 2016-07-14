CREATE TABLE LoginCredentials (
id INT NOT NULL IDENTITY,
username CHAR(50) NOT NULL,
passhash CHAR(32) NOT NULL,
email CHAR(100) NOT NULL,
firstname CHAR(25) NOT NULL,
lastname CHAR(25) NOT NULL,
accountType CHAR(10) NOT NULL,
PRIMARY KEY (id))