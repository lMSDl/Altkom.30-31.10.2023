CREATE OR ALTER PROCEDURE OrderSummary
@id int
AS
BEGIN

    SELECT o.Id, o.DateTime, COUNT(p.Id) AS Count
    FROM [Order] as o
    JOIN Product as p ON o.Id = p.OrderId
    WHERE o.Id = @id
    GROUP BY o.Id, o.[DateTime]    

END