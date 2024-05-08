using System.Data.SqlClient;

namespace WebApplication1.Properties.Services;

public interface IWaerhouseServive
{
    public Task<int?> insert(int IdProduct, int IdWarehouse, int Amount, DateTime CreatedAt);
}



public class WarehouseService:IWaerhouseServive
{

    
    
    public async Task<int?> insert(int IdProduct, int IdWarehouse, int Amount, DateTime CreatedAt)
    {
        await using var con = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=APBD;Trusted_Connection=True;");
        
        await con.OpenAsync();

        await using var cmd = new SqlCommand("Select * from product where IdProduct = @id",con);
        cmd.Parameters.AddWithValue("@id", IdProduct);

        var dr = await cmd.ExecuteReaderAsync();

        if (!dr.HasRows)
        {
            return null;
        }

        await using var cmd2 = new SqlCommand("SELECT * FROM Warehouse where idwarehouse=@id", con);
        cmd2.Parameters.AddWithValue("@id", IdWarehouse);

        var dr2 = await cmd2.ExecuteReaderAsync();

        if (!dr2.HasRows)
        {
            return null;
        }

        var command3 = new SqlCommand("SELECT IdOrder  from Order where IdProduct = @1 and Amount = @2 AND CreatedAt < @3",
            con);
        command3.Parameters.AddWithValue("@1", IdProduct);
        command3.Parameters.AddWithValue("@2", Amount);
        command3.Parameters.AddWithValue("@3", CreatedAt);
        var reader3 = await command3.ExecuteReaderAsync();
        if (!reader3.HasRows)
        {
            return null;
        }

        await reader3.ReadAsync();
        var idOrder = reader3.GetInt32(0);
        await reader3.CloseAsync();

        var command4 = new SqlCommand("SELECT * from Product_Warehouse where IdOrder = @1", con);
        command4.Parameters.AddWithValue("@1", idOrder);
        var reader4 = await command4.ExecuteReaderAsync();
        if (!reader4.HasRows)
        {
            return null;
        }

        var command5 = new SqlCommand("UPDATE [Order] set FullfilledAt = GETDATE() where IdOrder = @1", con);
        command5.Parameters.AddWithValue("@1", idOrder);
        await command5.ExecuteReaderAsync();

        var command6 = new SqlCommand("Select Price from Product where IdProduct = @1", con);
        command6.Parameters.AddWithValue("@1", IdProduct);
        var reader6 = await command6.ExecuteReaderAsync();

        await reader6.ReadAsync();
        var price = reader6.GetDecimal(0);
        await reader6.CloseAsync();

        var finalCommand = new SqlCommand("INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES (@1, @2, @3, @4, @5, GETDATE())", con);
        finalCommand.Parameters.AddWithValue("@1", IdWarehouse);
        finalCommand.Parameters.AddWithValue("@2", IdProduct);
        finalCommand.Parameters.AddWithValue("@3", idOrder);
        finalCommand.Parameters.AddWithValue("@4", Amount);
        finalCommand.Parameters.AddWithValue("@5", Amount + price);

        await finalCommand.ExecuteReaderAsync();

        var acualFinalCommand = new SqlCommand("SELECT IdProcuctWareHouse From Product_Warehouse where IdOrder = @1", con);
        acualFinalCommand.Parameters.AddWithValue("@1", idOrder);
        var reader7 = await acualFinalCommand.ExecuteReaderAsync();

        await reader7.ReadAsync();
        var IdToReturn = reader7.GetInt32(0);
        await reader7.CloseAsync();
        
        return IdToReturn;

    }
}