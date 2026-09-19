CREATE OR ALTER FUNCTION dbo.GetClientPayments
(
	@ClientId BIGINT,
	@Sd DATE,
	@Ed DATE
)
RETURNS TABLE
AS
RETURN
(
	WITH Dates AS
	(
		SELECT DATEADD(DAY, n.DayNumber, @Sd) AS Dt
		FROM (
			SELECT TOP (
				CASE WHEN @Ed >= @Sd 
					THEN DATEDIFF(DAY, @Sd, @Ed) + 1
					ELSE 0
				END
			)
			ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) - 1 AS DayNumber
			FROM sys.all_objects a
			CROSS JOIN sys.all_objects b
		) n
	)
	SELECT d.Dt, ISNULL(SUM(p.Amount), 0) AS Amount
	FROM Dates d
	LEFT JOIN dbo.ClientPayments p
		ON p.Client = @ClientId
		AND p.Dt >= d.Dt
		AND p.Dt < DATEADD(DAY, 1, d.Dt)
	GROUP BY d.Dt
);

