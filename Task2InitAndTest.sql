-- INIT
DROP TABLE IF EXISTS dbo.ClientPayments;

CREATE TABLE dbo.ClientPayments
(
	Id BIGINT PRIMARY KEY, 	-- id платежа
	Client BIGINT, 			-- id клиента
	Dt DATETIME2(0),		-- дата платежа
	Amount MONEY			-- сумма платежа
);

INSERT INTO dbo.ClientPayments (Id, Client, Dt, Amount) VALUES
(1, 1, '2022-01-03T17:24:00', 100),
(2, 1, '2022-01-05T17:24:14', 200),
(3, 1, '2022-01-05T18:23:34', 250),
(4, 1, '2022-01-07T10:12:38', 50),
(5, 2, '2022-01-05T17:24:14', 278),
(6, 2, '2022-01-10T12:39:29', 300);


-- TEST
SELECT * FROM dbo.GetClientPayments(1, '2022-01-02', '2022-01-07');
SELECT * FROM dbo.GetClientPayments(2, '2022-01-04', '2022-01-11');

